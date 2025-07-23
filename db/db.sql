USE ECommerceGreen;
GO

--------------------------------------------------------------------------------
-- 0) Varolan tablolarý ters baðýmlýlýk sýrasýyla sil
--------------------------------------------------------------------------------
IF OBJECT_ID('dbo.RotaBolumleri','U') IS NOT NULL DROP TABLE dbo.RotaBolumleri;
IF OBJECT_ID('dbo.SevkiyatRotalari','U') IS NOT NULL DROP TABLE dbo.SevkiyatRotalari;
IF OBJECT_ID('dbo.Rotalar','U') IS NOT NULL DROP TABLE dbo.Rotalar;
IF OBJECT_ID('dbo.Arcalar','U') IS NOT NULL DROP TABLE dbo.Arcalar;
IF OBJECT_ID('dbo.Konumlar','U') IS NOT NULL DROP TABLE dbo.Konumlar;
IF OBJECT_ID('dbo.Kargolar','U') IS NOT NULL DROP TABLE dbo.Kargolar;
IF OBJECT_ID('dbo.OrderItems','U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Siparisler','U') IS NOT NULL DROP TABLE dbo.Siparisler;
IF OBJECT_ID('dbo.Urunler','U') IS NOT NULL DROP TABLE dbo.Urunler;
IF OBJECT_ID('dbo.Kullanicilar','U') IS NOT NULL DROP TABLE dbo.Kullanicilar;
IF OBJECT_ID('dbo.EmisyonFaktorleri','U') IS NOT NULL DROP TABLE dbo.EmisyonFaktorleri;
GO

--------------------------------------------------------------------------------
-- 1) Emisyon Faktörleri
--------------------------------------------------------------------------------
CREATE TABLE dbo.EmisyonFaktorleri (
    Kimlik           INT            IDENTITY(1,1) PRIMARY KEY,
    YakitTipi        NVARCHAR(50)   NOT NULL UNIQUE,
    KgCO2_Per_Litre  DECIMAL(18,6)  NOT NULL
);
GO

--------------------------------------------------------------------------------
-- 2) Kullanýcýlar
--------------------------------------------------------------------------------
CREATE TABLE dbo.Kullanicilar (
    Kimlik           INT           IDENTITY(1,1) PRIMARY KEY,
    AdSoyad          NVARCHAR(100) NOT NULL,
    EPosta           NVARCHAR(255) NOT NULL UNIQUE,
    SifreHash        NVARCHAR(255) NOT NULL,
    OlusturmaTarihi  DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

--------------------------------------------------------------------------------
-- 3) Ürünler
--------------------------------------------------------------------------------
CREATE TABLE dbo.Urunler (
    Kimlik           INT           IDENTITY(1,1) PRIMARY KEY,
    Ad               NVARCHAR(150) NOT NULL,
    Aciklama         NVARCHAR(MAX) NULL,
    Fiyat            DECIMAL(18,2) NOT NULL,
    StokAdedi        INT           NOT NULL DEFAULT 0,
    OlusturmaTarihi  DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

--------------------------------------------------------------------------------
-- 4) Sipariþler
--------------------------------------------------------------------------------
CREATE TABLE dbo.Siparisler (
    Kimlik           INT           IDENTITY(1,1) PRIMARY KEY,
    KullaniciKimlik  INT           NOT NULL,
    SiparisTarihi    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    Durum            NVARCHAR(50)  NOT NULL,
    ToplamTutar      DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Siparisler_Kullanicilar
        FOREIGN KEY (KullaniciKimlik)
        REFERENCES dbo.Kullanicilar(Kimlik)
);
GO

--------------------------------------------------------------------------------
-- 5) Order Items (Sipariþ-Ürün)
--------------------------------------------------------------------------------
CREATE TABLE dbo.OrderItems (
    SiparisKimlik INT           NOT NULL,
    UrunKimlik    INT           NOT NULL,
    Adet          INT           NOT NULL,
    Fiyat         DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY (SiparisKimlik, UrunKimlik),
    CONSTRAINT FK_OrderItems_Siparisler
        FOREIGN KEY (SiparisKimlik) REFERENCES dbo.Siparisler(Kimlik),
    CONSTRAINT FK_OrderItems_Urunler
        FOREIGN KEY (UrunKimlik)    REFERENCES dbo.Urunler(Kimlik)
);
GO

--------------------------------------------------------------------------------
-- 6) Kargolar
--------------------------------------------------------------------------------
CREATE TABLE dbo.Kargolar (
    Kimlik           INT           IDENTITY(1,1) PRIMARY KEY,
    SiparisKimlik    INT           NOT NULL,
    TakipNumarasi    NVARCHAR(100) NOT NULL UNIQUE,
    GonderimTarihi   DATETIME2     NULL,
    Durum            NVARCHAR(50)  NULL,
    CONSTRAINT FK_Kargolar_Siparisler
        FOREIGN KEY (SiparisKimlik)
        REFERENCES dbo.Siparisler(Kimlik)
);
GO

--------------------------------------------------------------------------------
-- 7) Konumlar
--------------------------------------------------------------------------------
CREATE TABLE dbo.Konumlar (
    Kimlik  INT           IDENTITY(1,1) PRIMARY KEY,
    Ad      NVARCHAR(100) NOT NULL,
    Enlem   DECIMAL(9,6)  NOT NULL,
    Boylam  DECIMAL(9,6)  NOT NULL
);
GO

--------------------------------------------------------------------------------
-- 8) Araçlar (FK -> EmisyonFaktorleri.Kimlik)
--------------------------------------------------------------------------------
CREATE TABLE dbo.Arcalar (
    Kimlik                INT           IDENTITY(1,1) PRIMARY KEY,
    Tur                   NVARCHAR(50)  NOT NULL,
    OrtalamaTuketim       DECIMAL(5,2)  NOT NULL,
    EmisyonFaktoriKimlik  INT           NOT NULL,
    CONSTRAINT FK_Araclar_EmisyonFaktorleri
        FOREIGN KEY (EmisyonFaktoriKimlik)
        REFERENCES dbo.EmisyonFaktorleri(Kimlik)
);
GO

--------------------------------------------------------------------------------
-- 9) Rotalar
--------------------------------------------------------------------------------
CREATE TABLE dbo.Rotalar (
    Kimlik           INT           IDENTITY(1,1) PRIMARY KEY,
    Ad               NVARCHAR(100) NOT NULL,
    AracKimlik       INT           NOT NULL,
    ToplamMesafe     DECIMAL(18,2) NOT NULL,
    TahminiEmisyon   DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Rotalar_Araclar
        FOREIGN KEY (AracKimlik)
        REFERENCES dbo.Arcalar(Kimlik)
);
GO

--------------------------------------------------------------------------------
-- 10) Sevkiyat–Rota Ýliþkisi
--------------------------------------------------------------------------------
CREATE TABLE dbo.SevkiyatRotalari (
    KargoKimlik INT NOT NULL,
    RotaKimlik  INT NOT NULL,
    PRIMARY KEY (KargoKimlik, RotaKimlik),
    CONSTRAINT FK_SevkiyatRotalari_Kargolar
        FOREIGN KEY (KargoKimlik)
        REFERENCES dbo.Kargolar(Kimlik),
    CONSTRAINT FK_SevkiyatRotalari_Rotalar
        FOREIGN KEY (RotaKimlik)
        REFERENCES dbo.Rotalar(Kimlik)
);
GO

--------------------------------------------------------------------------------
-- 11) Rota Bölümleri
--------------------------------------------------------------------------------
CREATE TABLE dbo.RotaBolumleri (
    Kimlik           INT           IDENTITY(1,1) PRIMARY KEY,
    RotaKimlik       INT           NOT NULL,
    BaslangicKonum   INT           NOT NULL,
    BitisKonum       INT           NOT NULL,
    Mesafe           DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_RotaBolumleri_Rotalar
        FOREIGN KEY (RotaKimlik) REFERENCES dbo.Rotalar(Kimlik),
    CONSTRAINT FK_RotaBolumleri_BaslangicKonum
        FOREIGN KEY (BaslangicKonum) REFERENCES dbo.Konumlar(Kimlik),
    CONSTRAINT FK_RotaBolumleri_BitisKonum
        FOREIGN KEY (BitisKonum)      REFERENCES dbo.Konumlar(Kimlik)
);
GO


-- 12) “3. Gün” Optimizasyon Alanlarý

ALTER TABLE dbo.Rotalar
ADD
    OptimumMesafe   DECIMAL(18,2) NULL,
    OptimumEmisyon  DECIMAL(18,2) NULL,
    Mode             NVARCHAR(20) NULL,
    FuelType         NVARCHAR(50) NULL;
GO

--------------------------------------------------------------------------------
-- 13) Örnek Seed Verileri
--------------------------------------------------------------------------------
INSERT INTO dbo.EmisyonFaktorleri (YakitTipi, KgCO2_Per_Litre)
VALUES ('Diesel',0.265), ('Benzin',0.239), ('Elektrik',0.000);
GO

INSERT INTO dbo.Konumlar (Ad,Enlem,Boylam)
VALUES ('Merkez Ofis',41.01,29.02), ('Depo',41.02,29.06), ('Müþteri A',41.03,29.05);
GO

INSERT INTO dbo.Arcalar (Tur,OrtalamaTuketim,EmisyonFaktoriKimlik)
VALUES ('Kamyon',25.0,1), ('Minivan',15.0,2);
GO

INSERT INTO dbo.Kullanicilar (AdSoyad,EPosta,SifreHash)
VALUES ('Halil Turan','halil@example.com','HASHEDPWD'), ('Ayþe Yýlmaz','ayse@example.com','HASHEDPWD');
GO

INSERT INTO dbo.Urunler (Ad,Aciklama,Fiyat,StokAdedi)
VALUES ('Ürün 1','Açýklama 1',100.00,10), ('Ürün 2','Açýklama 2',200.00,5);
GO

INSERT INTO dbo.Siparisler (KullaniciKimlik,Durum,ToplamTutar)
VALUES (1,'Hazýrlanýyor',100.00), (2,'Kargoya Verildi',200.00);
GO

INSERT INTO dbo.Kargolar (SiparisKimlik,TakipNumarasi,Durum)
VALUES (1,'TRK001','Yolda'), (2,'TRK002','Teslim Edildi');
GO

INSERT INTO dbo.Rotalar (Ad,AracKimlik,ToplamMesafe,TahminiEmisyon)
VALUES ('Demo Rota',1,50.00,13.25);
GO

INSERT INTO dbo.SevkiyatRotalari (KargoKimlik,RotaKimlik)
VALUES (1,1);
GO

INSERT INTO dbo.RotaBolumleri (RotaKimlik,BaslangicKonum,BitisKonum,Mesafe)
VALUES (1,1,2,25.00),(1,2,3,25.00);
GO

-- 14 Rozetler Tablosu
CREATE TABLE dbo.Rozetler (
    Id                 INT           IDENTITY(1,1) PRIMARY KEY,
    Ad                 NVARCHAR(100) NOT NULL,
    Aciklama           NVARCHAR(255) NOT NULL,
    IconUrl            NVARCHAR(255) NULL,
    RequiredSavePct    DECIMAL(5,2)  NOT NULL  -- Örn: 10.00 = %10
);

-- 15 KullanýcýRozetleri (Many-to-Many)
CREATE TABLE dbo.KullaniciRozetleri (
    KullaniciKimlik    INT           NOT NULL,
    RozetId            INT           NOT NULL,
    VerilisTarihi      DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY (KullaniciKimlik, RozetId),
    CONSTRAINT FK_KR_Kullanicilar FOREIGN KEY (KullaniciKimlik) REFERENCES dbo.Kullanicilar(Kimlik),
    CONSTRAINT FK_KR_Rozetler      FOREIGN KEY (RozetId)       REFERENCES dbo.Rozetler(Id)
);

INSERT INTO dbo.Rozetler (Ad, Aciklama, IconUrl, RequiredSavePct)
VALUES 
(
  'Green Shipper',
  'Bu rozet %10 veya daha fazla emisyon tasarrufu yapanlara verilir',
  '/images/green_shipper.png',
  10.00
);

ALTER TABLE dbo.Kullanicilar
ADD 
    SifreSalt NVARCHAR(128) NOT NULL DEFAULT '',
    SonGirisTarihi DATETIME2 NULL,
    Aktif BIT NOT NULL DEFAULT 1;
GO

-- Token yönetimi için tablo
CREATE TABLE dbo.KullaniciTokenlari (
    Kimlik INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciKimlik INT NOT NULL,
    Token NVARCHAR(MAX) NOT NULL,
    OlusturulmaTarihi DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    SonKullanmaTarihi DATETIME2 NOT NULL,
    CONSTRAINT FK_KullaniciTokenlari_Kullanicilar 
        FOREIGN KEY (KullaniciKimlik) REFERENCES dbo.Kullanicilar(Kimlik)
);
GO