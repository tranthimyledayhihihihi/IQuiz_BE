using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

public class OnlineMatchService : IOnlineMatchService
{
    private readonly IUnitOfWork _unit;
    private readonly IQuizRepository _quiz;

    private const int DEFAULT_QUESTION_COUNT = 10;
    private const int BASE_POINTS = 100;
    private const double MAX_TIME = 15.0;
    private const int BONUS_MAX = 50;

    public OnlineMatchService(IUnitOfWork unit, IQuizRepository quiz)
    {
        _unit = unit;
        _quiz = quiz;
    }

    private string GenerateMatchCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var rnd = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    // ✔ Tạo trận theo MatchCode (Private hoặc Random)
    public async Task<string> CreateMatchAsync(int player1Id, int? player2Id = null)
    {
        string code = GenerateMatchCode();

        var match = new TranDauTrucTiep
        {
            MatchCode = code,
            Player1ID = player1Id,
            Player2ID = player2Id ?? 0,
            TrangThai = player2Id == null ? "ChoNguoiChoi" : "DangChoi"
        };

        _unit.TranDau.Add(match);
        await _unit.CompleteAsync();

        return code;
    }

    public Task<TranDauTrucTiep?> GetMatchByCodeAsync(string matchCode)
    {
        return _unit.TranDau.GetQueryable()
            .Where(m => m.MatchCode == matchCode)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<CauHoiDisplayModel>> GetQuestionsByMatchCodeAsync(string matchCode)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return Enumerable.Empty<CauHoiDisplayModel>();

        var questions = await _quiz.GetRandomQuestionsAsync(DEFAULT_QUESTION_COUNT, null, null);

        return questions.Select((q, i) => new CauHoiDisplayModel
        {
            CauHoiID = q.CauHoiID,
            NoiDung = q.NoiDung,
            CacLuaChon = JsonSerializer.Serialize(new
            {
                A = q.DapAnA,
                B = q.DapAnB,
                C = q.DapAnC,
                D = q.DapAnD
            }),
            ThuTuTrongTranDau = i + 1,
            ThoiGianToiDa = MAX_TIME
        });
    }

    public async Task<bool> SubmitAnswerByMatchCodeAsync(string matchCode, int userId, MatchAnswerModel answer)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return false;
        if (match.Player1ID != userId && match.Player2ID != userId) return false;

        var correct = await _quiz.GetCorrectAnswerAsync(answer.CauHoiID);
        bool isCorrect = correct != null && correct.Equals(answer.DapAnDaChon, StringComparison.OrdinalIgnoreCase);

        int reward = 0;
        if (isCorrect)
        {
            double ratio = (MAX_TIME - answer.ThoiGianTraLoi) / MAX_TIME;
            reward = BASE_POINTS + (int)(BONUS_MAX * ratio);
        }

        if (userId == match.Player1ID) match.DiemPlayer1 += reward;
        else match.DiemPlayer2 += reward;

        match.TrangThai = "DangChoi";

        _unit.TranDau.Update(match);
        await _unit.CompleteAsync();

        return true;
    }

    public async Task<MatchResultModel> EndMatchByCodeAsync(string matchCode)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) throw new Exception("Không tìm thấy trận.");

        string result = "Hoa";
        int? winner = null;

        if (match.DiemPlayer1 > match.DiemPlayer2)
        {
            winner = match.Player1ID;
            result = "Thang";
        }
        else if (match.DiemPlayer2 > match.DiemPlayer1)
        {
            winner = match.Player2ID;
            result = "Thang";
        }

        match.TrangThai = "HoanThanh";
        _unit.TranDau.Update(match);
        await _unit.CompleteAsync();

        return new MatchResultModel
        {
            MatchCode = matchCode,
            KetQua = result,
            WinnerHoTen = winner?.ToString() ?? "Hòa",
            DiemPlayer1 = match.DiemPlayer1,
            DiemPlayer2 = match.DiemPlayer2
        };
    }
}
