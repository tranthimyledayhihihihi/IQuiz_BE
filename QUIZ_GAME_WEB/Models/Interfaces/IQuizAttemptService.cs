using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IQuizAttemptService
    {
        Task<int> StartNewQuizAttemptAsync(int userId, GameStartOptions options);

        Task<int> StartDailyQuizAttemptAsync(int userId, int cauHoiId);

        Task<(bool IsCorrect, string CorrectAnswer)> SubmitAnswerAsync(AnswerSubmitModel answer);

        Task<KetQua> EndAttemptAndCalculateResultAsync(int attemptId, int userId);

        Task<CauHoiPlayDto?> GetNextQuestionAsync(int attemptId);
    }

}