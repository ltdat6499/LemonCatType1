CREATE DATABASE LemonCat
GO
USE LemonCat
GO

CREATE TABLE TAIKHOAN
(
	MaTK int PRIMARY KEY identity,
	TenTK nvarchar(250),
	MatKhau nvarchar(250),
	RealPass nvarchar(250),
	FirstName nvarchar(250),
	LastName nvarchar(250),
	ChucVu int,
	-- Chucvu = 1: Người bình thường
	-- Chucvu = 2: Nhà báo
	-- Chucvu = 3: Nhà phê bình hàng đầu
	-- Chucvu = 4: Admin
	-- Chucvu = 5: Người Kiểm duyệt - Censorer
	Email nvarchar(250),
	Phone nvarchar(20),
	CMND nvarchar(20),
	DiaChi nvarchar(250),
	AnhDaiDien nvarchar(max),
	KichHoat bit,
	GioiTinh bit,
	Money int
)
GO
CREATE TABLE CHUCVU
(
	MaChucVu int PRIMARY KEY identity,
	TenChucVu nvarchar(250)
)
GO
CREATE TABLE STUDIO
(
	MaStudio int PRIMARY KEY identity,
	TenStudio nvarchar(250)
)
GO
CREATE TABLE PHIM
(
	MaPhim int PRIMARY KEY identity,  
	TenPhim nvarchar(250),
	IsTVShow int, -- 1: là TV SHOW, 0: là Movie
	TomTatNgan ntext,
	TomTat ntext,
	AnhDaiDien nvarchar(max),
	Trailer ntext,
	Studio int, 
	Rating int,
	-- = 1: G – General Audiences: All ages admitted. Nothing that would offend parents for viewing by children.
	-- = 2: PG – Parental Guidance Suggested: Some material may not be suitable for children. Parents urged to give "parental guidance". May contain some material parents might not like for their young children.
	-- = 3: PG-13 – Parents Strongly Cautioned: Some material may be inappropriate for children under 13. Parents are urged to be cautious. Some material may be inappropriate for pre-teenagers.
	-- = 4: R – Restricted: Under 17 requires accompanying parent or adult guardian. Contains some adult material. Parents are urged to learn more about the film before taking their young children with them.
	-- = 5: NC-17 – Adults Only: No One 17 and Under Admitted. Clearly adult. Children are not admitted.
	-- Genre
	-- = 1: Absurdist/surreal/whimsical
	-- = 2: Action
	-- = 3: Adventure
	-- = 4: Comedy
	-- = 5: Crime
	-- = 6: Drama
	-- = 7: Fantasy
	-- = 8:	Historical
	-- = 9:	Historical fiction
	-- = 10: Horror
	-- = 11: Magical realism
	-- = 12: Mystery
	-- = 13: Paranoid fiction
	-- = 14: Philosophical
	-- = 15: Romance
	-- = 16: Political
	-- = 17: Saga
	-- = 18: Satire
	-- = 19: Science fiction
	-- = 20: Social
	-- = 21: Speculative
	-- = 22: Thriller
	-- = 23: Urban
	-- = 24: Western
	Theaters nvarchar(250), --null when in TV show
	OnDVD nvarchar(250),
	Time int, --return to minutes
	LemonCatScore int, --return to persent
	IMDBScore int, --return to IMDB Score
	BoxOffice decimal(18,2),
	FOREIGN KEY (Studio) REFERENCES STUDIO (MaStudio)
)
GO
CREATE TABLE TAGS_MOVIE
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	Tag nvarchar(250)
)
GO
CREATE TABLE SCORE
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	UserScore int,
	CountUser int,
	LemonCatScore int,
	CountLemonCat int,
	MetacriditScore int,
	IMDBScore int,
	TomatoesScore int,
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim)
)
GO

CREATE TABLE GENREPHIM
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	MaGenre int,
	FOREIGN KEY (MaGenre) REFERENCES GENRE (MaGenre),
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim)
)
GO
CREATE TABLE RATING
(
	MaRating int PRIMARY KEY identity,
	TenRating nvarchar(250)
)
GO
CREATE TABLE GENRE
(
	MaGenre int PRIMARY KEY identity,
	TenGenre nvarchar(250)
)
GO
CREATE TABLE TVSHOW
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	SoTap int,
	Network int, 
	PremiereDate nvarchar(250),
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
 	FOREIGN KEY (Network) REFERENCES NETWORK (ID),
)
GO
CREATE TABLE NETWORK
(
	ID int PRIMARY KEY identity,
	Network nvarchar(250), 
)
GO
CREATE TABLE TVSHOWEP
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	Ep int,
	TenTap nvarchar(250),
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim)
)
GO
CREATE TABLE DIENVIEN
(
	MaDienVien int PRIMARY KEY identity,
	NgaySinh date,
	NoiSinh nvarchar(250),
	TieuSu ntext,
	AnhDaiDien nvarchar(max),
	TenDienVien nvarchar(max)
)
GO
CREATE TABLE TAGS_PERSON
(
	ID int PRIMARY KEY identity,
	MaDienVien int,
	Tag nvarchar(250)
)
GO
CREATE TABLE ANHDIENVIEN
(
	ID int PRIMARY KEY identity,
	MaDienVien int,
	Anh nvarchar(max),
	TenAnh nvarchar(250),
	KichThuoc nvarchar(250),
	NgayCapNhap nvarchar(250),
	FOREIGN KEY (MaDienVien) REFERENCES DIENVIEN (MaDienVien),
)
GO
CREATE TABLE DAODIENPHIM
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	MaDienVien int,
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
	FOREIGN KEY (MaDienVien) REFERENCES DIENVIEN (MaDienVien)
)
GO
CREATE TABLE KICHBANPHIM
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	MaDienVien int,
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
	FOREIGN KEY (MaDienVien) REFERENCES DIENVIEN (MaDienVien)
)
GO
CREATE TABLE DIENVIENPHIM
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	MaDienVien int,
	TenNhanVat nvarchar(250),
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
	FOREIGN KEY (MaDienVien) REFERENCES DIENVIEN (MaDienVien)
)
GO
CREATE TABLE ANHPHIM
(
	ID int PRIMARY KEY identity,
	MaPhim int,
	Anh nvarchar(max),
	TenAnh nvarchar(250),
	KichThuoc nvarchar(250),
	NgayCapNhap nvarchar(250),
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
)
GO
CREATE TABLE BAIVIETDANHGIA
(
	MaBaiViet int PRIMARY KEY identity,
	MaPhim int,
	MaTK int,
	Score int,
	NoiDungNgan ntext,
	NoiDungBinhLuan ntext,
	NgayBinhLuan nvarchar(250),
	DiemDanhGia int, --Điểm này đánh giá bình luận nào đc lên top hay xuống dựa theo lượt like, dislike của các user khác
	Status bit,
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK)
)
GO

CREATE TABLE BAIVIET_ROOTCOMMENT
(
	ID int PRIMARY KEY identity,
	MaBaiViet int,
	MaTK int,
	NoiDungBinhLuan text,
	NgayBinhLuan nvarchar(250),
	DiemDanhGiaBinhLuan int,
	Status bit,
	FOREIGN KEY (MaBaiViet) REFERENCES BAIVIETDANHGIA (MaBaiViet),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK)
)
GO

CREATE TABLE BAIVIET_CHILDCOMMENT
(
	ID int PRIMARY KEY identity,
	IDRoot int,
	MaTK int,
	NoiDungBinhLuan text,
	NgayBinhLuan nvarchar(250),
	Status bit,
	FOREIGN KEY (IDRoot) REFERENCES BAIVIET_ROOTCOMMENT (ID),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK)
)
GO

CREATE TABLE BAIVIET_SCORE
(
	ID int PRIMARY KEY identity,
	MaTK int,
	MaBaiViet int,
	Mark bit,
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK),
	FOREIGN KEY (MaBaiViet) REFERENCES BAIVIETDANHGIA (MaBaiViet)
)
GO

CREATE TABLE BAIVIET_ROOTSCORE
(
	ID int PRIMARY KEY identity,
	MaTK int,
	IDRoot int,
	Mark bit,
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK),
	FOREIGN KEY (IDRoot) REFERENCES BAIVIET_ROOTCOMMENT (ID)
)
GO
---------------------------------------------------------------------------------------------------------------------------
CREATE TABLE NEW
(
	MaNew int PRIMARY KEY identity,
	MaTK int,
	LoaiNew int,
	-- = 1: Phân tích
	-- = 2: Đánh giá
	-- = 3: Review
	-- = 4: Khác
	TuaNews nvarchar(250),
	NoiDungNgan ntext,
	NoiDung ntext,
	DiemDanhGia int, --Điểm này đánh giá bình luận nào đc lên top hay xuống dựa theo lượt like, dislike của các user khác
	NgayDang nvarchar(250),
	Status bit,
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK),
)
GO
CREATE TABLE LOAINEW
(
	MaLoaiNew int PRIMARY KEY identity,
	TenLoaiNew nvarchar(250)
)
GO

CREATE TABLE RootCommentNew
(
	ID int PRIMARY KEY identity,
	MaNew int,
	MaTK int,
	NoiDungBinhLuan ntext,
	NgayBinhLuan nvarchar(250),
	DiemDanhGia int,
	Status bit,
	FOREIGN KEY (MaNew) REFERENCES NEW (MaNew),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK),
)
GO
CREATE TABLE ChildCommentNew
(
	ID int PRIMARY KEY identity,
	RootCommentNewID int,
	MaTK int,
	NoiDungBinhLuan ntext,
	NgayBinhLuan nvarchar(250),
	Status bit,
	FOREIGN KEY (RootCommentNewID) REFERENCES RootCommentNew (ID),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK)
)
GO

CREATE TABLE NewsScore
(
	ID int PRIMARY KEY identity,
	MaTK int,
	NewsID int,
	Score bit,
	FOREIGN KEY (NewsID) REFERENCES NEW (MaNew),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK),
)
GO

CREATE TABLE RootNewScore
(
	ID int PRIMARY KEY identity,
	MaTK int,
	RootCommentNewID int,
	Score bit,
	FOREIGN KEY (RootCommentNewID) REFERENCES RootCommentNew (ID),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK)
)
GO

CREATE TABLE CHITIETPHIMNEW
(
	ID int PRIMARY KEY identity,
	MaNew int,
	MaPhim int,
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
	FOREIGN KEY (MaNew) REFERENCES NEW (MaNew),
)
GO
CREATE TABLE CHITIETDIENVIENNEW
(
	ID int PRIMARY KEY identity,
	MaNew int,
	MaDienVien int,
	FOREIGN KEY (MaDienVien) REFERENCES DIENVIEN (MaDienVien),
	FOREIGN KEY (MaNew) REFERENCES NEW (MaNew),
)
GO
------------------------------------------------------------------------------------------------------------
CREATE TABLE DVD
(
	MaDVD int PRIMARY KEY identity,
	MaPhim int,
	SoDia int,
	Gia int,
	SoLuongTrongKho int,
	NgayCapNhap nvarchar(250),
	Status bit,
	FOREIGN KEY (MaPhim) REFERENCES PHIM (MaPhim),
)
GO

CREATE TABLE VIEWEDPRODUCT
(
	ID int PRIMARY KEY identity,
	MaTK int,
	MaDVD int,
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK),
)
GO
---------------------------------------------------------------------------------------------------------
CREATE TABLE BAG
(
	ID int PRIMARY KEY identity,
	MaTK int,
	MaDVD int,
	SoLuong int,
	Status bit,
	FOREIGN KEY (MaDVD) REFERENCES DVD (MaDVD),
	FOREIGN KEY (MaTK) REFERENCES TAIKHOAN (MaTK)
)
GO
CREATE TABLE HOADON
(
	MaHoaDon int PRIMARY KEY identity,
	MaTK int,
	TongTien int,
	Note ntext,
	VAT int,
	Discount int,
	ThanhTien int,
	Name nvarchar(250),
	Avata nvarchar(max),
	Address nvarchar(max),
	Phone nvarchar(250),
	Status int,
	NgayLapHoaDon nvarchar(250)
)
GO
CREATE TABLE CHITIETHOADON
(
	ID int PRIMARY KEY identity,
	MaHoaDon int,
	MaDVD int,
	NameDVD nvarchar(250),
	Avata nvarchar(max),
	SoLuong int,
	DonGia int,
	ThanhTien int,
	FOREIGN KEY (MaHoaDon) REFERENCES HOADON (MaHoaDon)
)
GO
CREATE TABLE VAT
(
	ID int PRIMARY KEY identity,
	VAT int,
	Discount int
)
GO
----------------------------------------------------------------------------------------------------
CREATE TABLE DATASET
(
	ID int PRIMARY KEY identity,
	MaTK int,
	TheLoai int,
	LuotXem int,
	LemonCat int,
	UserScore int,
	KetQua int,  --MovieID
)
GO
---------------------------------------------------------------------------------------------------------
CREATE TABLE YEARDATA
(
	ID int PRIMARY KEY identity,
	Month int,
	Year int,
	TongMovie int,
	TongTV int,
	TongUser int,
	TongDVD int,
	TongVe int,
	DoanhThuBanDVD int,
	DoanhThuBanVe int,
	TongUserLoaiMot int,
	TongUserDacBiet int,
	TongReview int,
	TongNews int
)
GO
--------------------------------------------------------------------------------------------------------
-- Ticket
CREATE TABLE RAPPHIM
(
	ID int PRIMARY KEY identity,
	TenRap nvarchar(250)
)
GO
CREATE TABLE SUATCHIEU
(
	ID int PRIMARY KEY identity,
	IDRap int,
	IDPhim int,
	Time int,
	DanhSachGheTrong nvarchar(max),
	Price int
)
GO
CREATE TABLE ORDERSEAT
(
	ID int PRIMARY KEY identity,
	MaTK int,
	IDSuatChieu int,
	DanhSachGheDat nvarchar(max),
	QR nvarchar(max),
	TongTien int,
	TapDoan nvarchar(max),
	DiaChiTapDoan nvarchar(max),
	TenKhach nvarchar(max),
	Sdt nvarchar(max),
	Email nvarchar(max),
	NgayTaoHoaDon nvarchar(30),
	Price int,
	FOREIGN KEY (IDSuatChieu) REFERENCES SUATCHIEU (ID)
)
GO
CREATE TABLE HanChieu
(
	ID int PRIMARY KEY identity,
	StartDate nvarchar(max),
	EndDate nvarchar(max),
	MaPhim int
)
GO
---------------------------------------------------------------------------------------------------------------
--Trigger Phase
-- Thêm dòng tương ứng với phim khi thêm mới phim
CREATE TRIGGER UTG_PHIM_DEFAULTVALUES
ON PHIM
FOR INSERT
AS
BEGIN
	DECLARE @id int =
	(
		SELECT MaPhim
		FROM inserted
	)
	INSERT INTO SCORE VALUES (@id,0, 0,0,0,0,0,0);
END
GO

CREATE TRIGGER UTG_BOXOFFICE_SET
ON ORDERSEAT
FOR INSERT
AS
BEGIN
	DECLARE @id int =
	(
		SELECT IDPhim
		FROM inserted, SUATCHIEU
		WHERE inserted.IDSuatChieu = SUATCHIEU.ID
	);
	DECLARE @Box int =
	(
		SELECT TongTien
		FROM inserted
	);
	UPDATE PHIM SET BoxOffice += @Box WHERE MaPhim = @id;	
END
GO

CREATE TRIGGER UTG_LemonCatScore_SET
ON BAIVIETDANHGIA
FOR INSERT
AS
BEGIN
	DECLARE @id int =
	(
		SELECT MaPhim
		FROM inserted
	);
	DECLARE @LemonCat int =
	(
		SELECT LemonCatScore
		FROM SCORE
		WHERE MaPhim = @id
	);
	UPDATE PHIM SET LemonCatScore = @LemonCat WHERE MaPhim = @id;	
END
GO
-----------------------------------------------------------------------------------------------------------------
-- Function Phase
-- Client
-- Đăng nhập Client only
CREATE FUNCTION ClientUserLogin (@username nvarchar(max), @password nvarchar(max))
RETURNS BIT
AS
BEGIN
	DECLARE @res bit;
	DECLARE @count int = 
	(
		SELECT COUNT(*)
		FROM TAIKHOAN
		WHERE TenTK = @username AND MatKhau = @password AND ChucVu < 4 AND KichHoat = 1
	);
	IF (@count > 0)
		SET @res = 1;
	ELSE
		SET @res = 0;
	RETURN @res;
END
GO
--Kiểm tra tài khoản có đang bị lock không (status = 0)
CREATE FUNCTION ClientCheckLock (@username nvarchar(max))
RETURNS BIT
AS
BEGIN
	DECLARE @res bit;
	DECLARE @count int = 
	(
		SELECT COUNT(*)
		FROM TAIKHOAN
		WHERE TenTK = @username AND ChucVu < 4 AND KichHoat = 0
	);
	IF (@count > 0)
		SET @res = 1;
	ELSE
		SET @res = 0;
	RETURN @res;
END
GO
-- Kiểm tra UserName - Email đã tồn tại chưa
CREATE FUNCTION IsExistUser (@username nvarchar(max), @email nvarchar(max), @password nvarchar(max))
RETURNS BIT
AS
BEGIN
	DECLARE @res bit;
	IF (@username = '' OR @email = '' OR @password = '')
		RETURN 1;
	DECLARE @count int = 
	(
		SELECT COUNT(*)
		FROM TAIKHOAN
		WHERE TenTK = @username AND Email = @email
	);
	IF (@count > 0)
		SET @res = 1;
	ELSE
		SET @res = 0;
	RETURN @res;
END
GO
-- Kiểm tra email có tồn tại chưa
CREATE FUNCTION IsExistEmail(@email nvarchar(max), @id int)
RETURNS BIT
AS
BEGIN
	DECLARE @count int = 
	(
		SELECT COUNT(*)
		FROM TAIKHOAN
		WHERE Email = @email AND MaTK != @id
	);
	IF (@count > 0)
		RETURN 1;
	ELSE
		RETURN 0;
	RETURN 1;
END
GO
-- Kiểm tra số điện thoại có tốn tại chưa
CREATE FUNCTION IsExistPhone(@phone nvarchar(max), @id int)
RETURNS BIT
AS
BEGIN
	DECLARE @count int = 
	(
		SELECT COUNT(*)
		FROM TAIKHOAN
		WHERE Phone = @phone AND MaTK != @id
	);
	IF (@count > 0)
		RETURN 1;
	ELSE
		RETURN 0;
	RETURN 1;
END
GO
-- Kiểm tra CMND có tồn tại chưa
CREATE FUNCTION IsExistIDCard(@cmnd nvarchar(max), @id int)
RETURNS BIT
AS
BEGIN
	DECLARE @count int = 
	(
		SELECT COUNT(*)
		FROM TAIKHOAN
		WHERE CMND = @cmnd AND MaTK != @id
	);
	IF (@count > 0)
		RETURN 1;
	ELSE
		RETURN 0;
	RETURN 1;
END
GO
-------------------------------------------------------------------------------------------------------
-- Thủ tục cập nhập User Client only
CREATE PROCEDURE UpdateUser @id int, @FirstName nvarchar(max), @LastName nvarchar(max), @Email nvarchar(max), @Phone nvarchar(max), @CMND nvarchar(max), @DiaChi nvarchar(max)
AS
BEGIN
	DECLARE @B1 varchar(20) = 'Transaction1';
	SAVE TRANSACTION B1;
	BEGIN TRAN @B1; 
	UPDATE TAIKHOAN SET FirstName = @FirstName, LastName = @LastName, DiaChi = @DiaChi WHERE MaTK = @id;
	IF (@FirstName = '' OR @LastName = '' OR @Email = '' OR @Phone = '' OR @CMND = '' OR @DiaChi = '')
		ROLLBACK TRANSACTION B1;
	ELSE
		COMMIT TRANSACTION @B1;

	DECLARE @B2 varchar(20) = 'Transaction2'; 
	SAVE TRANSACTION B2;
	BEGIN TRAN @B2; 
	UPDATE TAIKHOAN SET Email = @Email WHERE MaTK = @id;
	DECLARE @resEmail bit = (SELECT dbo.IsExistEmail(@Email, @id));
	IF (@Email = '' OR @resEmail = 1)
		ROLLBACK TRANSACTION B2;
	ELSE
		COMMIT TRANSACTION @B2;

	DECLARE @B3 varchar(20) = 'Transaction3'; 
	SAVE TRANSACTION B3;
	BEGIN TRAN @B3; 
	UPDATE TAIKHOAN SET Phone = @Phone WHERE MaTK = @id;
	DECLARE @resPhone bit = (SELECT dbo.IsExistPhone(@Phone, @id));
	IF (@Phone = '' OR @resPhone = 1)
		ROLLBACK TRANSACTION B3;
	ELSE
		COMMIT TRANSACTION @B3;

	DECLARE @B4 varchar(20) = 'Transaction4';  
	SAVE TRANSACTION B4;
	BEGIN TRAN @B4; 
	UPDATE TAIKHOAN SET CMND = @CMND WHERE MaTK = @id;
	DECLARE @resCMND bit = (SELECT dbo.IsExistIDCard(@CMND, @id));
	IF (@CMND = '' OR @resCMND = 1)
		ROLLBACK TRANSACTION B4;
	ELSE
		COMMIT TRANSACTION @B4;
END
-- Thủ tục đổi mật khẩu Client only
CREATE PROCEDURE ChangePassword @id int, @OPassword nvarchar(max), @NPassword nvarchar(max), @RNPassword nvarchar(max)
AS
BEGIN
	DECLARE @oldPassword nvarchar(max) =
			(
				SELECT MatKhau
				FROM TAIKHOAN
				WHERE MaTK = @id
			);
	IF (@OPassword != '' AND @NPassword != '' AND @RNPassword != '' AND @id != '')
	BEGIN
		IF (@NPassword = @RNPassword AND @OPassword != @NPassword)
		BEGIN
			IF (@oldPassword = @OPassword)
			BEGIN
				UPDATE TAIKHOAN SET MatKhau = @NPassword WHERE MaTK = @id;
			END
		END
	END
END

--Bag Phase
-- Thủ tục tạo hóa đơn mua hàng 
CREATE PROCEDURE SubmitBag @id int, @Note nvarchar(max), @NgayLapHoaDon nvarchar(max)
AS
BEGIN
	DECLARE @BagID int;
	
	SELECT @BagID = MIN(ID) FROM BAG WHERE MaTK = @id AND Status = 1;
	DECLARE @B1 varchar(20) = 'Transaction1';
	BEGIN TRAN @B1; 
	SAVE TRANSACTION ProcedureSave;
	DECLARE @Name nvarchar(max) =
			(
				SELECT LastName
				FROM TAIKHOAN 
				WHERE MaTK = @id
			);
	DECLARE @Avata nvarchar(max) =
			(
				SELECT AnhDaiDien
				FROM TAIKHOAN 
				WHERE MaTK = @id
			);
	DECLARE @Address nvarchar(max) =
			(
				SELECT DiaChi
				FROM TAIKHOAN 
				WHERE MaTK = @id
			);
	DECLARE @Phone nvarchar(max) =
			(
				SELECT Phone
				FROM TAIKHOAN 
				WHERE MaTK = @id
			);
	INSERT INTO HOADON(MaTK, TongTien, Note, Name, Avata, Address, Phone, Status, NgayLapHoaDon)
	VALUES (@id, 0, @Note, @Name, @Avata, @Address, @Phone, 2, @NgayLapHoaDon);
	DECLARE @BillID int;
	SELECT @BillID = MAX(MaHoaDon) FROM HOADON WHERE MaTK = @id;
	IF (@BagID is null)
		ROLLBACK TRANSACTION ProcedureSave;
	WHILE @BagID is not null
		BEGIN
			DECLARE @ItemPrice int = (SELECT Gia FROM BAG WHERE ID = @BagID);
			DECLARE @ItemID int = (SELECT MaDVD FROM BAG WHERE ID = @BagID);
			DECLARE @Count int = (SELECT SoLuong FROM BAG WHERE ID = @BagID);
			DECLARE @ItemName nvarchar(max) = (SELECT TenPhim FROM PHIM WHERE MaPhim = (SELECT MaPhim FROM DVD WHERE MaDVD = @ItemID));
			DECLARE @ItemAvata nvarchar(max) = (SELECT AnhDaiDien FROM PHIM WHERE MaPhim = (SELECT MaPhim FROM DVD WHERE MaDVD = @ItemID));
			
			INSERT INTO CHITIETHOADON(MaHoaDon, MaDVD, NameDVD, Avata, SoLuong, DonGia, ThanhTien)
			VALUES (@BillID, @ItemID, @ItemName, @ItemAvata, @Count, @ItemPrice, @ItemPrice * @Count);

			DELETE FROM BAG WHERE ID = @BagID;

			UPDATE HOADON SET TongTien += @ItemPrice * @Count WHERE MaHoaDon = @BillID;
			
			SELECT @BagID = MIN(ID) FROM BAG WHERE MaTK = @id AND Status = 1 AND ID > @BagID;
		END
	
	DECLARE @ChildID int;
	SELECT @ChildID = MIN(ID) FROM CHITIETHOADON WHERE MaHoaDon = @BillID;

	WHILE @ChildID is not null
		BEGIN

			DECLARE @ChildItemID int = (SELECT MaDVD FROM CHITIETHOADON WHERE ID = @ChildID);
			DECLARE @ChildCount int = (SELECT SoLuong FROM CHITIETHOADON WHERE ID = @ChildID);

			DECLARE @DVDItemID int = (SELECT MaDVD FROM DVD WHERE MaDVD = @ChildItemID);
			DECLARE @DVDCount int = (SELECT SoLuongTrongKho FROM DVD WHERE MaDVD = @ChildItemID);

			IF (@DVDCount - @ChildCount < 0)
				ROLLBACK TRANSACTION ProcedureSave;
			UPDATE DVD SET SoLuongTrongKho -= @ChildCount WHERE MaDVD = @ChildItemID;
			SELECT @ChildID = MIN(ID) FROM CHITIETHOADON WHERE MaHoaDon = @BillID AND ID > @ChildID;
		END
	COMMIT TRANSACTION @B1;
	SELECT -1;
END