-- Kiểm tra và tạo database nếu chưa có
USE master;
GO

-- Kiểm tra nếu database tồn tại thì xóa
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QUIZ_GAME_WEB_DB')
BEGIN
    ALTER DATABASE QUIZ_GAME_WEB_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QUIZ_GAME_WEB_DB;
END
GO

-- Tạo database mới
CREATE DATABASE QUIZ_GAME_WEB_DB;
GO

USE QUIZ_GAME_WEB_DB;
GO
-- ChuDe
CREATE TABLE ChuDe (
    ChuDeID     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenChuDe    NVARCHAR(100) NOT NULL,
    MoTa        NVARCHAR(255) NULL,
    TrangThai   BIT NOT NULL
);

-- ClientKeys
CREATE TABLE ClientKeys (
    ClientKeyID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    KeyValue    NVARCHAR(255) NOT NULL,
    TenUngDung  NVARCHAR(100) NOT NULL,
    NgayTao     DATETIME2 NOT NULL,
    NgayHetHan  DATETIME2 NULL,
    IsActive    BIT NOT NULL
);

-- DoKho
CREATE TABLE DoKho (
    DoKhoID     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenDoKho    NVARCHAR(50) NOT NULL,
    DiemThuong  INT NOT NULL
);

-- NguoiDung
CREATE TABLE NguoiDung (
    UserID          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenDangNhap     NVARCHAR(50) NOT NULL,
    MatKhau         NVARCHAR(255) NOT NULL,
    Email           NVARCHAR(100) NULL,
    HoTen           NVARCHAR(100) NULL,
    AnhDaiDien      NVARCHAR(255) NULL,
    NgayDangKy      DATETIME2 NOT NULL,
    LanDangNhapCuoi DATETIME2 NULL,
    TrangThai       BIT NOT NULL
);

-- Quyen
CREATE TABLE Quyen (
    QuyenID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenQuyen NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL
);

-- SystemSettings
CREATE TABLE SystemSettings (
    [Key]   NVARCHAR(100) NOT NULL PRIMARY KEY,
    [Value] NVARCHAR(MAX) NOT NULL,
    MoTa    NVARCHAR(255) NULL
);

-- TroGiup
CREATE TABLE TroGiup (
    TroGiupID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenTroGiup  NVARCHAR(50) NOT NULL,
    MoTa        NVARCHAR(255) NULL
);

-- VaiTro
CREATE TABLE VaiTro (
    VaiTroID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL,
    MoTa NVARCHAR(255) NULL
);
-- BXH
CREATE TABLE BXH (
    BXHID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID     INT NOT NULL,
    DiemTuan   INT NOT NULL,
    DiemThang  INT NOT NULL,
    HangTuan   INT NOT NULL,
    HangThang  INT NOT NULL,

    CONSTRAINT FK_BXH_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- CaiDatNguoiDung
CREATE TABLE CaiDatNguoiDung (
    SettingID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID      INT NOT NULL,
    AmThanh     BIT NOT NULL,
    NhacNen     BIT NOT NULL,
    ThongBao    BIT NOT NULL,
    NgonNgu     NVARCHAR(20) NOT NULL,

    CONSTRAINT FK_CaiDatNguoiDung_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- ChuoiNgay
CREATE TABLE ChuoiNgay (
    ChuoiID         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID          INT NOT NULL,
    SoNgayLienTiep  INT NOT NULL,
    NgayCapNhatCuoi DATETIME2 NOT NULL,

    CONSTRAINT FK_ChuoiNgay_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- Comment
CREATE TABLE Comment (
    CommentID       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID          INT NOT NULL,
    Content         NVARCHAR(MAX) NOT NULL,
    NgayTao         DATETIME2 NOT NULL,
    EntityType      NVARCHAR(MAX) NOT NULL,
    RelatedEntityID INT NOT NULL,

    CONSTRAINT FK_Comment_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- NguoiDungOnline
CREATE TABLE NguoiDungOnline (
    OnlineID        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID          INT NOT NULL,
    TrangThai       NVARCHAR(20) NOT NULL,
    ThoiGianCapNhat DATETIME2 NOT NULL,

    CONSTRAINT FK_NguoiDungOnline_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- PhienDangNhap
CREATE TABLE PhienDangNhap (
    SessionID       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID          INT NOT NULL,
    Token           NVARCHAR(500) NULL,
    ThoiGianDangNhap DATETIME2 NOT NULL,
    ThoiGianHetHan  DATETIME2 NULL,
    TrangThai       BIT NOT NULL,

    CONSTRAINT FK_PhienDangNhap_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- QuizTuyChinh (đã gộp thêm các field UGC)
CREATE TABLE QuizTuyChinh (
    QuizTuyChinhID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID         INT NOT NULL,
    TenQuiz        NVARCHAR(100) NOT NULL,
    MoTa           NVARCHAR(255) NULL,
    NgayTao        DATETIME2 NOT NULL,

    -- Từ migration AddUGCFieldsToQuizTuyChinh:
    AdminDuyetID   INT NULL,
    NgayDuyet      DATETIME2 NULL,
    TrangThai      NVARCHAR(50) NOT NULL DEFAULT('Pending'),

    CONSTRAINT FK_QuizTuyChinh_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- ThanhTuu
CREATE TABLE ThanhTuu (
    ThanhTuuID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenThanhTuu     NVARCHAR(100) NULL,
    MoTa            NVARCHAR(MAX) NULL,
    BieuTuong       NVARCHAR(MAX) NULL,
    DieuKien        NVARCHAR(MAX) NULL,
    NguoiDungID     INT NOT NULL,
    AchievementCode NVARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_ThanhTuu_NguoiDung_NguoiDungID
        FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- ThongKeNguoiDung
CREATE TABLE ThongKeNguoiDung (
    ThongKeID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID         INT NOT NULL,
    Ngay           DATETIME2 NOT NULL,
    SoTran         INT NOT NULL,
    SoCauDung      INT NOT NULL,
    DiemTrungBinh  FLOAT NOT NULL,

    CONSTRAINT FK_ThongKeNguoiDung_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- ThuongNgay
CREATE TABLE ThuongNgay (
    ThuongID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID        INT NOT NULL,
    NgayNhan      DATETIME2 NOT NULL,
    PhanThuong    NVARCHAR(100) NOT NULL,
    DiemThuong    INT NOT NULL,
    TrangThaiNhan BIT NOT NULL,

    CONSTRAINT FK_ThuongNgay_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE
);

-- Admin
CREATE TABLE Admin (
    AdminID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID    INT NOT NULL,
    VaiTroID  INT NOT NULL,
    NgayTao   DATETIME2 NOT NULL,
    TrangThai BIT NOT NULL,

    CONSTRAINT FK_Admin_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE,

    CONSTRAINT FK_Admin_VaiTro_VaiTroID
        FOREIGN KEY (VaiTroID) REFERENCES VaiTro(VaiTroID)
        ON DELETE CASCADE
);

-- Bảng trung gian VaiTro_Quyen
CREATE TABLE VaiTro_Quyen (
    VaiTroID INT NOT NULL,
    QuyenID  INT NOT NULL,

    CONSTRAINT PK_VaiTro_Quyen PRIMARY KEY (VaiTroID, QuyenID),

    CONSTRAINT FK_VaiTro_Quyen_VaiTro_VaiTroID
        FOREIGN KEY (VaiTroID) REFERENCES VaiTro(VaiTroID)
        ON DELETE CASCADE,

    CONSTRAINT FK_VaiTro_Quyen_Quyen_QuyenID
        FOREIGN KEY (QuyenID) REFERENCES Quyen(QuyenID)
        ON DELETE CASCADE
);
-- CauHoi (gồm QuizTuyChinhID nullable từ migration 2)
CREATE TABLE CauHoi (
    CauHoiID        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ChuDeID         INT NOT NULL,
    DoKhoID         INT NOT NULL,
    NoiDung         NVARCHAR(500) NOT NULL,
    DapAnA          NVARCHAR(255) NOT NULL,
    DapAnB          NVARCHAR(255) NOT NULL,
    DapAnC          NVARCHAR(255) NOT NULL,
    DapAnD          NVARCHAR(255) NOT NULL,
    DapAnDung       NVARCHAR(10) NOT NULL,
    HinhAnh         NVARCHAR(255) NULL,
    NgayTao         DATETIME2 NOT NULL,

    -- từ migration AddUGCFieldsToQuizTuyChinh:
    QuizTuyChinhID  INT NULL,

    CONSTRAINT FK_CauHoi_ChuDe_ChuDeID
        FOREIGN KEY (ChuDeID) REFERENCES ChuDe(ChuDeID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CauHoi_DoKho_DoKhoID
        FOREIGN KEY (DoKhoID) REFERENCES DoKho(DoKhoID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CauHoi_QuizTuyChinh_QuizTuyChinhID
        FOREIGN KEY (QuizTuyChinhID) REFERENCES QuizTuyChinh(QuizTuyChinhID)
        ON DELETE NO ACTION
);

-- QuizNgay
CREATE TABLE QuizNgay (
    QuizNgayID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Ngay       DATETIME2 NOT NULL,
    CauHoiID   INT NULL,

    CONSTRAINT FK_QuizNgay_CauHoi_CauHoiID
        FOREIGN KEY (CauHoiID) REFERENCES CauHoi(CauHoiID)
        ON DELETE NO ACTION
);

-- QuizAttempt
CREATE TABLE QuizAttempt (
    QuizAttemptID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID          INT NOT NULL,
    QuizTuyChinhID  INT NOT NULL,
    NgayBatDau      DATETIME2 NOT NULL,
    NgayKetThuc     DATETIME2 NULL,
    SoCauHoiLam     INT NOT NULL,
    SoCauDung       INT NOT NULL,
    Diem            INT NOT NULL,
    TrangThai       NVARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_QuizAttempt_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE NO ACTION,

    CONSTRAINT FK_QuizAttempt_QuizTuyChinh_QuizTuyChinhID
        FOREIGN KEY (QuizTuyChinhID) REFERENCES QuizTuyChinh(QuizTuyChinhID)
        ON DELETE NO ACTION
);

-- QuizChiaSe
CREATE TABLE QuizChiaSe (
    QuizChiaSeID    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    QuizTuyChinhID  INT NOT NULL,
    UserGuiID       INT NOT NULL,
    UserNhanID      INT NULL,
    NgayChiaSe      DATETIME2 NOT NULL,

    CONSTRAINT FK_QuizChiaSe_QuizTuyChinh_QuizTuyChinhID
        FOREIGN KEY (QuizTuyChinhID) REFERENCES QuizTuyChinh(QuizTuyChinhID)
        ON DELETE CASCADE,

    CONSTRAINT FK_QuizChiaSe_NguoiDung_UserGuiID
        FOREIGN KEY (UserGuiID) REFERENCES NguoiDung(UserID)
        ON DELETE NO ACTION,

    CONSTRAINT FK_QuizChiaSe_NguoiDung_UserNhanID
        FOREIGN KEY (UserNhanID) REFERENCES NguoiDung(UserID)
        ON DELETE NO ACTION
);
-- CauSai
CREATE TABLE CauSai (
    CauSaiID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID        INT NOT NULL,
    CauHoiID      INT NOT NULL,
    QuizAttemptID INT NOT NULL,
    NgaySai       DATETIME2 NOT NULL,

    CONSTRAINT FK_CauSai_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CauSai_CauHoi_CauHoiID
        FOREIGN KEY (CauHoiID) REFERENCES CauHoi(CauHoiID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CauSai_QuizAttempt_QuizAttemptID
        FOREIGN KEY (QuizAttemptID) REFERENCES QuizAttempt(QuizAttemptID)
        ON DELETE CASCADE
);

-- KetQua
CREATE TABLE KetQua (
    KetQuaID        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID          INT NOT NULL,
    QuizAttemptID   INT NOT NULL,
    Diem            INT NOT NULL,
    SoCauDung       INT NOT NULL,
    TongCauHoi      INT NOT NULL,
    TrangThaiKetQua NVARCHAR(MAX) NOT NULL,
    ThoiGian        DATETIME2 NOT NULL,

    CONSTRAINT FK_KetQua_NguoiDung_UserID
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE,

    CONSTRAINT FK_KetQua_QuizAttempt_QuizAttemptID
        FOREIGN KEY (QuizAttemptID) REFERENCES QuizAttempt(QuizAttemptID)
        ON DELETE CASCADE
);
-- Admin.UserID unique
CREATE UNIQUE INDEX IX_Admin_UserID
    ON Admin(UserID);

CREATE INDEX IX_Admin_VaiTroID
    ON Admin(VaiTroID);

CREATE INDEX IX_BXH_UserID
    ON BXH(UserID);

CREATE UNIQUE INDEX IX_CaiDatNguoiDung_UserID
    ON CaiDatNguoiDung(UserID);

CREATE INDEX IX_CauHoi_ChuDeID
    ON CauHoi(ChuDeID);

CREATE INDEX IX_CauHoi_DoKhoID
    ON CauHoi(DoKhoID);

CREATE INDEX IX_CauHoi_QuizTuyChinhID
    ON CauHoi(QuizTuyChinhID);

CREATE INDEX IX_CauSai_CauHoiID
    ON CauSai(CauHoiID);

CREATE INDEX IX_CauSai_QuizAttemptID
    ON CauSai(QuizAttemptID);

CREATE INDEX IX_CauSai_UserID
    ON CauSai(UserID);

CREATE INDEX IX_ChuoiNgay_UserID
    ON ChuoiNgay(UserID);

CREATE INDEX IX_Comment_UserID
    ON Comment(UserID);

CREATE UNIQUE INDEX IX_KetQua_QuizAttemptID
    ON KetQua(QuizAttemptID);

CREATE INDEX IX_KetQua_UserID
    ON KetQua(UserID);

-- NguoiDung: Email unique (và filter IS NOT NULL), TenDangNhap unique
CREATE UNIQUE INDEX IX_NguoiDung_Email
    ON NguoiDung(Email)
    WHERE Email IS NOT NULL;

CREATE UNIQUE INDEX IX_NguoiDung_TenDangNhap
    ON NguoiDung(TenDangNhap);

CREATE INDEX IX_NguoiDungOnline_UserID
    ON NguoiDungOnline(UserID);

CREATE INDEX IX_PhienDangNhap_UserID
    ON PhienDangNhap(UserID);

CREATE INDEX IX_QuizAttempt_QuizTuyChinhID
    ON QuizAttempt(QuizTuyChinhID);

CREATE INDEX IX_QuizAttempt_UserID
    ON QuizAttempt(UserID);

CREATE INDEX IX_QuizChiaSe_QuizTuyChinhID
    ON QuizChiaSe(QuizTuyChinhID);

CREATE INDEX IX_QuizChiaSe_UserGuiID
    ON QuizChiaSe(UserGuiID);

CREATE INDEX IX_QuizChiaSe_UserNhanID
    ON QuizChiaSe(UserNhanID);

CREATE INDEX IX_QuizNgay_CauHoiID
    ON QuizNgay(CauHoiID);

CREATE INDEX IX_QuizTuyChinh_UserID
    ON QuizTuyChinh(UserID);

CREATE INDEX IX_ThanhTuu_NguoiDungID
    ON ThanhTuu(NguoiDungID);

CREATE INDEX IX_ThongKeNguoiDung_UserID
    ON ThongKeNguoiDung(UserID);

CREATE INDEX IX_ThuongNgay_UserID
    ON ThuongNgay(UserID);

CREATE INDEX IX_VaiTro_Quyen_QuyenID
    ON VaiTro_Quyen(QuyenID);
/* =========================
   SEED DỮ LIỆU BAN ĐẦU
   ========================= */

------------------------
-- ChuDe
------------------------
SET IDENTITY_INSERT ChuDe ON;

INSERT INTO ChuDe (ChuDeID, MoTa, TenChuDe, TrangThai) VALUES
(1, N'Các sự kiện và nhân vật lịch sử quan trọng.', N'Lịch Sử Việt Nam', 1),
(2, N'Các bài toán đại số và hình học cơ bản.', N'Toán Học Phổ Thông', 1),
(3, N'Kiến thức vật lý, hóa học, sinh học.', N'Khoa Học Tự Nhiên', 1);

SET IDENTITY_INSERT ChuDe OFF;

------------------------
-- DoKho
------------------------
SET IDENTITY_INSERT DoKho ON;

INSERT INTO DoKho (DoKhoID, DiemThuong, TenDoKho) VALUES
(1, 10, N'Dễ'),
(2, 25, N'Trung bình'),
(3, 50, N'Khó');

SET IDENTITY_INSERT DoKho OFF;

------------------------
-- NguoiDung
-- (dùng giá trị DateTime sau khi đã được UpdateData trong AddUGCFieldsToQuizTuyChinh)
------------------------
SET IDENTITY_INSERT NguoiDung ON;

INSERT INTO NguoiDung (
    UserID, AnhDaiDien, Email, HoTen, LanDangNhapCuoi, MatKhau,
    NgayDangKy, TenDangNhap, TrangThai
)
VALUES
(1, NULL, N'superadmin@quiz.com', N'Nguyễn Super Admin', NULL,
 N'hashed_sa_password', '2025-12-01T20:18:53.4960751', N'admin_sa', 1),

(2, NULL, N'player01@quiz.com', N'Trần Văn A', NULL,
 N'hashed_p1_password', '2025-12-01T20:18:53.4960771', N'player01', 1),

(3, NULL, N'player02@quiz.com', N'Lê Thị B', NULL,
 N'hashed_p2_password', '2025-12-01T20:18:53.4960774', N'player02', 1);

SET IDENTITY_INSERT NguoiDung OFF;

------------------------
-- Quyen
------------------------
SET IDENTITY_INSERT Quyen ON;

INSERT INTO Quyen (QuyenID, MoTa, TenQuyen) VALUES
(1, N'Quản lý (Khóa/Mở khóa) tài khoản người dùng.', N'ql_nguoi_dung'),
(2, N'Thêm, sửa, xóa, duyệt câu hỏi.', N'ql_cau_hoi'),
(3, N'Truy cập và tạo báo cáo hệ thống.', N'ql_baocao'),
(4, N'Quản lý vai trò và quyền hạn (Chỉ SuperAdmin).', N'ql_vai_tro');

SET IDENTITY_INSERT Quyen OFF;

------------------------
-- TroGiup
------------------------
SET IDENTITY_INSERT TroGiup ON;

INSERT INTO TroGiup (TroGiupID, MoTa, TenTroGiup) VALUES
(1, N'Loại bỏ hai đáp án sai.', N'50/50'),
(2, N'Tham khảo ý kiến cộng đồng.', N'Hỏi khán giả');

SET IDENTITY_INSERT TroGiup OFF;

------------------------
-- VaiTro
------------------------
SET IDENTITY_INSERT VaiTro ON;

INSERT INTO VaiTro (VaiTroID, MoTa, TenVaiTro) VALUES
(1, N'Quản trị viên cấp cao, toàn quyền hệ thống.', N'SuperAdmin'),
(2, N'Kiểm duyệt viên, quản lý câu hỏi và người dùng.', N'Moderator'),
(3, N'Người dùng/Người chơi thông thường.', N'Player');

SET IDENTITY_INSERT VaiTro OFF;

------------------------
-- Admin (Update NgayTao theo migration 2)
------------------------
SET IDENTITY_INSERT Admin ON;

INSERT INTO Admin (AdminID, NgayTao, TrangThai, UserID, VaiTroID) VALUES
(1, '2025-12-01T20:18:53.4960799', 1, 1, 1);

SET IDENTITY_INSERT Admin OFF;

------------------------
-- BXH
------------------------
SET IDENTITY_INSERT BXH ON;

INSERT INTO BXH (BXHID, DiemThang, DiemTuan, HangThang, HangTuan, UserID) VALUES
(1, 125, 125, 1, 1, 2),
(2, 25,  25,  2, 2, 3);

SET IDENTITY_INSERT BXH OFF;

------------------------
-- CaiDatNguoiDung
------------------------
SET IDENTITY_INSERT CaiDatNguoiDung ON;

INSERT INTO CaiDatNguoiDung (
    SettingID, AmThanh, NgonNgu, NhacNen, ThongBao, UserID
) VALUES
(1, 1, N'vi', 0, 1, 2),
(2, 1, N'vi', 1, 1, 3);

SET IDENTITY_INSERT CaiDatNguoiDung OFF;

------------------------
-- CauHoi (NgayTao + QuizTuyChinhID đã update trong migration 2)
------------------------
SET IDENTITY_INSERT CauHoi ON;

INSERT INTO CauHoi (
    CauHoiID, ChuDeID, DapAnA, DapAnB, DapAnC, DapAnD, DapAnDung,
    DoKhoID, HinhAnh, NgayTao, NoiDung, QuizTuyChinhID
)
VALUES
(1, 1,
 N'Phan Đình Phùng', N'Trần Văn Thời', N'Trương Định', N'Nguyễn Trung Trực',
 N'C', 1, NULL,
 '2025-12-01T20:18:53.4960948',
 N'Ai là người phất cờ khởi nghĩa đầu tiên chống Pháp?',
 NULL),

(2, 1,
 N'1953', N'1954', N'1975', N'1950',
 N'B', 2, NULL,
 '2025-12-01T20:18:53.4960951',
 N'Chiến dịch Điện Biên Phủ diễn ra năm nào?',
 NULL),

(3, 2,
 N'3', N'9', N'3 và -3', N'Không có',
 N'C', 1, NULL,
 '2025-12-01T20:18:53.4960953',
 N'Căn bậc hai của 9 là bao nhiêu?',
 NULL),

(4, 3,
 N'Đồng', N'Vàng', N'Nhựa', N'Bạc',
 N'C', 2, NULL,
 '2025-12-01T20:18:53.4960956',
 N'Chất nào sau đây không dẫn điện?',
 NULL);

SET IDENTITY_INSERT CauHoi OFF;

------------------------
-- ChuoiNgay (ngày đã được UpdateData trong migration 2)
------------------------
SET IDENTITY_INSERT ChuoiNgay ON;

INSERT INTO ChuoiNgay (ChuoiID, NgayCapNhatCuoi, SoNgayLienTiep, UserID) VALUES
(1, '2025-12-01T20:18:53.4961094', 5, 2),
(2, '2025-12-01T20:18:53.4961095', 2, 3);

SET IDENTITY_INSERT ChuoiNgay OFF;

------------------------
-- QuizTuyChinh (đã có thêm AdminDuyetID, NgayDuyet, TrangThai)
------------------------
SET IDENTITY_INSERT QuizTuyChinh ON;

INSERT INTO QuizTuyChinh (
    QuizTuyChinhID, MoTa, NgayTao, TenQuiz, UserID,
    AdminDuyetID, NgayDuyet, TrangThai
)
VALUES
(1,
 N'Các câu hỏi tôi thích nhất.',
 '2025-12-01T20:18:53.4960981',
 N'Quiz Của Tôi',
 2,
 NULL, NULL, N'Pending'
);

SET IDENTITY_INSERT QuizTuyChinh OFF;

------------------------
-- VaiTro_Quyen
------------------------
INSERT INTO VaiTro_Quyen (QuyenID, VaiTroID) VALUES
(1, 1),
(2, 1),
(3, 1),
(4, 1),
(1, 2),
(2, 2),
(3, 2);

------------------------
-- QuizAttempt (ngày giờ đã UpdateData trong migration 2)
------------------------
SET IDENTITY_INSERT QuizAttempt ON;

INSERT INTO QuizAttempt (
    QuizAttemptID, Diem, NgayBatDau, NgayKetThuc,
    QuizTuyChinhID, SoCauDung, SoCauHoiLam, TrangThai, UserID
)
VALUES
(1,
 0,
 '2025-12-01T19:18:53.4961002',
 '2025-12-01T20:18:53.4961008',
 1, 0, 0, N'Hoàn thành', 2),

(2,
 0,
 '2025-12-01T18:18:53.4961014',
 '2025-12-01T20:18:53.4961014',
 1, 0, 0, N'Hoàn thành', 3);

SET IDENTITY_INSERT QuizAttempt OFF;

------------------------
-- QuizChiaSe (ngày giờ đã UpdateData trong migration 2)
------------------------
SET IDENTITY_INSERT QuizChiaSe ON;

INSERT INTO QuizChiaSe (
    QuizChiaSeID, NgayChiaSe, QuizTuyChinhID,
    UserGuiID, UserNhanID
)
VALUES
(1,
 '2025-12-01T20:18:53.4961137',
 1, 2, 3);

SET IDENTITY_INSERT QuizChiaSe OFF;

------------------------
-- QuizNgay
------------------------
SET IDENTITY_INSERT QuizNgay ON;

INSERT INTO QuizNgay (QuizNgayID, CauHoiID, Ngay) VALUES
(1, 1, '2025-12-01T00:00:00.0000000');

SET IDENTITY_INSERT QuizNgay OFF;

------------------------
-- CauSai
------------------------
SET IDENTITY_INSERT CauSai ON;

INSERT INTO CauSai (
    CauSaiID, CauHoiID, NgaySai, QuizAttemptID, UserID
)
VALUES
(1, 2, '2025-12-01T00:00:00.0000000', 2, 3);

SET IDENTITY_INSERT CauSai OFF;

------------------------
-- KetQua (ThoiGian đã được UpdateData trong migration 2)
------------------------
SET IDENTITY_INSERT KetQua ON;

INSERT INTO KetQua (
    KetQuaID, Diem, QuizAttemptID, SoCauDung,
    ThoiGian, TongCauHoi, TrangThaiKetQua, UserID
)
VALUES
(1, 50, 1, 2,
 '2025-12-01T15:18:53.4961035',
 2, N'Hoàn thành', 2),

(2, 25, 2, 1,
 '2025-12-01T19:18:53.4961037',
 2, N'Hoàn thành', 3);

SET IDENTITY_INSERT KetQua OFF;
ALTER TABLE NguoiDung
ADD VaiTroID INT NOT NULL DEFAULT 3;
GO
ALTER TABLE NguoiDung
ADD CONSTRAINT FK_NguoiDung_VaiTro_VaiTroID
    FOREIGN KEY (VaiTroID) REFERENCES VaiTro(VaiTroID);
GO
-- UserID = 1 là SuperAdmin
UPDATE NguoiDung
SET VaiTroID = 1
WHERE UserID = 1;

-- UserID = 2,3 là Player
UPDATE NguoiDung
SET VaiTroID = 3
WHERE UserID IN (2, 3);
GO
-------
USE QUIZ_GAME_WEB_DB;
GO

/* ============================================
   XÓA NẾU ĐÃ TỒN TẠI (CHO CHẮC)
   ============================================ */
IF OBJECT_ID('TraLoiTrucTiep', 'U') IS NOT NULL
    DROP TABLE TraLoiTrucTiep;

IF OBJECT_ID('TranDau_CauHoi', 'U') IS NOT NULL
    DROP TABLE TranDau_CauHoi;

IF OBJECT_ID('TranDauTrucTiep', 'U') IS NOT NULL
    DROP TABLE TranDauTrucTiep;
GO


/* ============================================
   1. BẢNG TRẬN ĐẤU 1v1
   ============================================ */
CREATE TABLE TranDauTrucTiep (
    TranDauID       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Player1ID       INT NOT NULL,          -- Người tạo phòng / mời
    Player2ID       INT NOT NULL,          -- Người được mời
    SoCauHoi        INT NOT NULL,          -- Tổng số câu hỏi trong trận
    DiemPlayer1     INT NOT NULL DEFAULT 0,
    DiemPlayer2     INT NOT NULL DEFAULT 0,
    WinnerUserID    INT NULL,              -- NULL nếu hòa hoặc chưa xong
    TrangThai       NVARCHAR(20) NOT NULL, -- 'DangCho','DangChoi','HoanThanh','Huy'
    ThoiGianBatDau  DATETIME2 NOT NULL,
    ThoiGianKetThuc DATETIME2 NULL,

    -- CHỈ Player1ID cho phép cascade khi xóa user (tránh multiple cascade)
    CONSTRAINT FK_TranDauTrucTiep_Player1
        FOREIGN KEY (Player1ID) REFERENCES NguoiDung(UserID)
        ON DELETE CASCADE,

    -- Player2ID: KHÔNG cascade
    CONSTRAINT FK_TranDauTrucTiep_Player2
        FOREIGN KEY (Player2ID) REFERENCES NguoiDung(UserID),

    -- WinnerUserID: KHÔNG cascade
    CONSTRAINT FK_TranDauTrucTiep_Winner
        FOREIGN KEY (WinnerUserID) REFERENCES NguoiDung(UserID)
);
GO

CREATE INDEX IX_TranDauTrucTiep_Player1ID
    ON TranDauTrucTiep(Player1ID);

CREATE INDEX IX_TranDauTrucTiep_Player2ID
    ON TranDauTrucTiep(Player2ID);
GO


/* ============================================
   2. BẢNG CÂU HỎI TRONG TRẬN
   ============================================ */
CREATE TABLE TranDau_CauHoi (
    TranDauCauHoiID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TranDauID       INT NOT NULL,
    CauHoiID        INT NOT NULL,
    ThuTu           INT NOT NULL,

    CONSTRAINT FK_TranDau_CauHoi_TranDau
        FOREIGN KEY (TranDauID) REFERENCES TranDauTrucTiep(TranDauID)
        ON DELETE CASCADE,

    CONSTRAINT FK_TranDau_CauHoi_CauHoi
        FOREIGN KEY (CauHoiID) REFERENCES CauHoi(CauHoiID)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_TranDau_CauHoi_TranDauID
    ON TranDau_CauHoi(TranDauID);

CREATE INDEX IX_TranDau_CauHoi_CauHoiID
    ON TranDau_CauHoi(CauHoiID);
GO


/* ============================================
   3. BẢNG LOG TRẢ LỜI TRỰC TIẾP
   ============================================ */
CREATE TABLE TraLoiTrucTiep (
    TraLoiID        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TranDauID       INT NOT NULL,
    CauHoiID        INT NOT NULL,
    UserID          INT NOT NULL,
    DapAnNguoiChoi  NVARCHAR(10) NOT NULL,   -- 'A','B','C','D'
    DungHaySai      BIT NOT NULL,            -- 1 = đúng, 0 = sai
    ThoiGianTraLoi  DATETIME2 NOT NULL,
    DiemNhanDuoc    INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_TraLoiTrucTiep_TranDau
        FOREIGN KEY (TranDauID) REFERENCES TranDauTrucTiep(TranDauID)
        ON DELETE CASCADE,

    CONSTRAINT FK_TraLoiTrucTiep_CauHoi
        FOREIGN KEY (CauHoiID) REFERENCES CauHoi(CauHoiID)
        ON DELETE CASCADE,

    -- ❌ KHÔNG CASCADE Ở ĐÂY, để tránh multiple cascade path từ NguoiDung
    CONSTRAINT FK_TraLoiTrucTiep_NguoiDung
        FOREIGN KEY (UserID) REFERENCES NguoiDung(UserID)
);
GO

CREATE INDEX IX_TraLoiTrucTiep_TranDauID
    ON TraLoiTrucTiep(TranDauID);

CREATE INDEX IX_TraLoiTrucTiep_UserID
    ON TraLoiTrucTiep(UserID);

CREATE INDEX IX_TraLoiTrucTiep_TranDau_CauHoi_User
    ON TraLoiTrucTiep(TranDauID, CauHoiID, UserID);
GO
/* =========================
   TRẬN ĐẤU 1v1 MẪU
   ========================= */

-- Giả sử:
-- UserID = 2 (player01), UserID = 3 (player02)
-- Dùng các câu hỏi: CauHoiID = 1,2,3

------------------------
-- 1. Tạo trận đấu
------------------------
SET IDENTITY_INSERT TranDauTrucTiep ON;

INSERT INTO TranDauTrucTiep (
    TranDauID, Player1ID, Player2ID, SoCauHoi,
    DiemPlayer1, DiemPlayer2, WinnerUserID,
    TrangThai, ThoiGianBatDau, ThoiGianKetThuc
)
VALUES
(
    1,
    2,              -- player01
    3,              -- player02
    3,              -- 3 câu hỏi
    60,             -- Điểm Player1
    30,             -- Điểm Player2
    2,              -- Player1 thắng
    N'HoanThanh',
    '2025-12-02T20:00:00',
    '2025-12-02T20:05:00'
);

SET IDENTITY_INSERT TranDauTrucTiep OFF;
GO


------------------------
-- 2. Gán câu hỏi cho trận
------------------------
SET IDENTITY_INSERT TranDau_CauHoi ON;

INSERT INTO TranDau_CauHoi (TranDauCauHoiID, TranDauID, CauHoiID, ThuTu) VALUES
(1, 1, 1, 1),
(2, 1, 2, 2),
(3, 1, 3, 3);

SET IDENTITY_INSERT TranDau_CauHoi OFF;
GO


------------------------
-- 3. Log trả lời của từng người chơi
------------------------
SET IDENTITY_INSERT TraLoiTrucTiep ON;

-- Câu 1: Player1 đúng, Player2 sai
INSERT INTO TraLoiTrucTiep (
    TraLoiID, TranDauID, CauHoiID, UserID,
    DapAnNguoiChoi, DungHaySai, ThoiGianTraLoi, DiemNhanDuoc
)
VALUES
(1, 1, 1, 2, N'C', 1, '2025-12-02T20:00:30', 30),
(2, 1, 1, 3, N'B', 0, '2025-12-02T20:00:35', 0);

-- Câu 2: cả 2 cùng đúng
INSERT INTO TraLoiTrucTiep (
    TraLoiID, TranDauID, CauHoiID, UserID,
    DapAnNguoiChoi, DungHaySai, ThoiGianTraLoi, DiemNhanDuoc
)
VALUES
(3, 1, 2, 2, N'B', 1, '2025-12-02T20:01:10', 30),
(4, 1, 2, 3, N'B', 1, '2025-12-02T20:01:15', 30);

-- Câu 3: cả 2 sai
INSERT INTO TraLoiTrucTiep (
    TraLoiID, TranDauID, CauHoiID, UserID,
    DapAnNguoiChoi, DungHaySai, ThoiGianTraLoi, DiemNhanDuoc
)
VALUES
(5, 1, 3, 2, N'A', 0, '2025-12-02T20:02:00', 0),
(6, 1, 3, 3, N'D', 0, '2025-12-02T20:02:05', 0);

SET IDENTITY_INSERT TraLoiTrucTiep OFF;
GO
ALTER TABLE TraLoiTrucTiep
ADD ThoiGianGiaiQuyet FLOAT NOT NULL DEFAULT 0;
ALTER TABLE TranDauTrucTiep
ADD MatchCode VARCHAR(10) NOT NULL DEFAULT('');
-- Thêm cột TrangThaiDuyet
ALTER TABLE CauHoi
ADD TrangThaiDuyet NVARCHAR(20) NOT NULL 
DEFAULT 'Pending';

-- Thêm cột AdminDuyetID (Khóa ngoại)
ALTER TABLE CauHoi
ADD AdminDuyetID INT NULL; 

-- Thêm Khóa ngoại (Giả định bảng NguoiDung có UserID là khóa chính)
ALTER TABLE CauHoi
ADD CONSTRAINT FK_CauHoi_AdminDuyetID
FOREIGN KEY (AdminDuyetID) REFERENCES NguoiDung(UserID);
-- Thêm một người dùng mới có VaiTroID = 2 (Moderator)
INSERT INTO NguoiDung 
    (TenDangNhap, MatKhau, Email, HoTen, NgayDangKy, TrangThai, VaiTroID)
VALUES 
    ('moder', 'hashed_moderator_password', 'moderator.alpha@quiz.com', 'Admin Kiểm Duyệt', GETDATE(), 1, 2);
-- Bước 1: Đổi tên cột ThoiGianDangNhap thành ThoiGianBatDau
EXEC sp_rename 'PhienDangNhap.ThoiGianDangNhap', 'ThoiGianBatDau', 'COLUMN';

-- Bước 2: Đổi tên cột ThoiGianHetHan thành ThoiGianKetThuc
-- Cột này phải cho phép NULL (NULLABLE)
EXEC sp_rename 'PhienDangNhap.ThoiGianHetHan', 'ThoiGianKetThuc', 'COLUMN';

-- Nếu bạn muốn chắc chắn ThoiGianBatDau là NOT NULL và ThoiGianKetThuc là NULL
-- (Các lệnh này có thể khác tùy thuộc vào phiên bản SQL Server và ràng buộc hiện có)
-- ALTER TABLE PhienDangNhap ALTER COLUMN ThoiGianBatDau DATETIME NOT NULL;
-- ALTER TABLE PhienDangNhap ALTER COLUMN ThoiGianKetThuc DATETIME NULL;
-- Bước 3: Thêm cột IPAddress
ALTER TABLE PhienDangNhap
ADD IPAddress NVARCHAR(45) NULL;

-- Bước 4: Thêm cột DeviceType
ALTER TABLE PhienDangNhap
ADD DeviceType NVARCHAR(50) NULL;
EXEC sp_rename 'DoKho', 'DoKho_Old';
GO
CREATE TABLE DoKho (
    DoKhoID INT IDENTITY(1,1) NOT NULL, -- ✅ ĐÃ THIẾT LẬP IDENTITY
    TenDoKho NVARCHAR(50) NOT NULL,
    DiemThuong INT NOT NULL DEFAULT 0, -- Giả định DiemThuong là INT NOT NULL
    CONSTRAINT PK_DoKho PRIMARY KEY (DoKhoID)
);
GO
-- Đảm bảo IDENTITY_INSERT TẮT
SET IDENTITY_INSERT DoKho OFF;
GO

-- Chuyển dữ liệu sang bảng mới. ID mới sẽ được SQL Server tự tạo.
INSERT INTO DoKho (TenDoKho, DiemThuong)
SELECT TenDoKho, DiemThuong
FROM DoKho_Old;
GO
--
-- Thay thế 'FK_CauHoi_DoKho_DoKhoID' bằng tên khóa ngoại bạn tìm thấy
ALTER TABLE CauHoi
DROP CONSTRAINT FK_CauHoi_DoKho_DoKhoID; 
GO

-- Lặp lại bước này nếu có nhiều hơn một ràng buộc (ví dụ: nếu bảng TranDauCauHois cũng tham chiếu)
-- Xóa bảng cũ sau khi xác nhận dữ liệu ổn định
DROP TABLE DoKho_Old;
GO
--
ALTER TABLE CauHoi
ADD CONSTRAINT FK_CauHoi_DoKho_DoKhoID -- Đặt lại tên FK bạn đã sử dụng
FOREIGN KEY (DoKhoID) 
REFERENCES DoKho (DoKhoID); -- Tham chiếu đến bảng DoKho mới
GO
-- 1.1 TẠO BẢNG ThanhTuuDefinition
CREATE TABLE ThanhTuuDefinition (
    DefinitionID INT IDENTITY(1,1) NOT NULL, -- Khóa chính tự động tăng
    
    -- Thuộc tính định nghĩa
    AchievementCode NVARCHAR(50) NOT NULL UNIQUE, -- Mã code duy nhất (Ví dụ: MASTER_100_QUIZ)
    TenThanhTuu NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(500),
    BieuTuong NVARCHAR(255),
    
    -- Điều kiện và Phần thưởng
    LoaiDieuKien NVARCHAR(50) NOT NULL,
    GiaTriCanThiet INT NOT NULL,
    DiemThuong INT NOT NULL,
    
    -- Admin Tracking (Giả định AdminID trỏ đến bảng Admin)
    AdminID INT NOT NULL,
    
    CONSTRAINT PK_ThanhTuuDefinition PRIMARY KEY (DefinitionID)
);
GO
---
-- 2.1 FIX: XÓA CÁC CỘT TRÙNG LẶP (Tên, Mô tả, Điều kiện)
ALTER TABLE ThanhTuu
DROP COLUMN TenThanhTuu,
            MoTa,
            BieuTuong,
            DieuKien;
GO

-- 2.2 THÊM CỘT KHÓA NGOẠI MỚI DefinitionID
ALTER TABLE ThanhTuu
ADD DefinitionID INT NULL;
GO
-- 3.1 FK từ ThanhTuu đến ThanhTuuDefinition
-- Liên kết bản ghi thành tựu đạt được với định nghĩa của nó.
-- LƯU Ý: Nếu bảng ThanhTuu hiện đang có dữ liệu, hãy chạy UPDATE dữ liệu trước khi chạy ALTER COLUMN NOT NULL (Bước 3.3)
ALTER TABLE ThanhTuu
ADD CONSTRAINT FK_ThanhTuu_Definition
FOREIGN KEY (DefinitionID) REFERENCES ThanhTuuDefinition(DefinitionID)
ON DELETE NO ACTION;
GO

-- 3.2 FK từ ThanhTuuDefinition đến Admin
-- Liên kết định nghĩa với Admin đã tạo/quản lý.
-- Giả định bảng Admin có khóa chính là AdminID.
ALTER TABLE ThanhTuuDefinition
ADD CONSTRAINT FK_ThanhTuuDefinition_Admin
FOREIGN KEY (AdminID) REFERENCES Admin(AdminID)
ON DELETE NO ACTION;
GO

-- 3.3 [TÙY CHỌN] ĐẶT LẠI NOT NULL
-- Nếu bạn đã cập nhật tất cả các bản ghi ThanhTuu cũ với DefinitionID thích hợp, 
-- hãy chạy lệnh này để đảm bảo mọi bản ghi mới đều phải có DefinitionID.
/*
ALTER TABLE ThanhTuu
ALTER COLUMN DefinitionID INT NOT NULL;
GO
*/
--
-- Kiểm tra và thêm dữ liệu nếu bảng trống để tránh trùng lặp
IF NOT EXISTS (SELECT 1 FROM ThanhTuuDefinition WHERE AchievementCode = 'FIRST_QUIZ_COMPLETED')
BEGIN
    INSERT INTO ThanhTuuDefinition (
        AchievementCode, TenThanhTuu, MoTa, BieuTuong, 
        LoaiDieuKien, GiaTriCanThiet, DiemThuong, AdminID
    )
    VALUES 
    (
        'FIRST_QUIZ_COMPLETED', 
        N'Người mới nhập môn', 
        N'Hoàn thành thành công bài Quiz đầu tiên.', 
        N'icon_beginner.png',
        'QuizCompleteCount', 
        1, 
        50, 
        1
    );
END

IF NOT EXISTS (SELECT 1 FROM ThanhTuuDefinition WHERE AchievementCode = '7_DAY_STREAK')
BEGIN
    INSERT INTO ThanhTuuDefinition (
        AchievementCode, TenThanhTuu, MoTa, BieuTuong, 
        LoaiDieuKien, GiaTriCanThiet, DiemThuong, AdminID
    )
    VALUES 
    (
        '7_DAY_STREAK', 
        N'Tuần lễ Vàng',
N'Đăng nhập và hoàn thành Quiz mỗi ngày trong 7 ngày liên tiếp.', 
        N'icon_7days.png',
        'LoginStreak', 
        7, 
        250, 
        1
    );
END
GO
--
-- Thêm cột NgayDatDuoc vào bảng ThanhTuu
ALTER TABLE ThanhTuu
ADD NgayDatDuoc DATETIME NOT NULL DEFAULT GETDATE();
GO

-- Sau khi thêm, kiểm tra cấu trúc bảng:
-- EXEC sp_help 'ThanhTuu'; 
-- HO
-- Lấy DefinitionID từ các mã code vừa thêm
DECLARE @DefinitionID_Quiz INT;
DECLARE @DefinitionID_Streak INT;

SELECT @DefinitionID_Quiz = DefinitionID FROM ThanhTuuDefinition WHERE AchievementCode = 'FIRST_QUIZ_COMPLETED';
SELECT @DefinitionID_Streak = DefinitionID FROM ThanhTuuDefinition WHERE AchievementCode = '7_DAY_STREAK';

-- Thêm bản ghi đã đạt được
IF @DefinitionID_Quiz IS NOT NULL AND @DefinitionID_Streak IS NOT NULL
BEGIN
    
    -- Người dùng ID=2 đạt thành tựu 1 (Quiz đầu tiên)
    INSERT INTO ThanhTuu (
        DefinitionID, NguoiDungID, AchievementCode, NgayDatDuoc
    )
    VALUES 
    (
        @DefinitionID_Quiz, 
        2, -- User ID
        'FIRST_QUIZ_COMPLETED', 
        GETDATE()
    );

    -- Người dùng ID=3 đạt thành tựu 2 (7-Day Streak)
    INSERT INTO ThanhTuu (
        DefinitionID, NguoiDungID, AchievementCode, NgayDatDuoc
    )
    VALUES 
    (
        @DefinitionID_Streak, 
        3, -- User ID
        '7_DAY_STREAK', 
        GETDATE()
    );

END
GO
--
SELECT 
    T.NguoiDungID, 
    D.TenThanhTuu, 
    D.DiemThuong,
    T.NgayDatDuoc
FROM 
    ThanhTuu T
JOIN 
    ThanhTuuDefinition D ON T.DefinitionID = D.DefinitionID;
GO

