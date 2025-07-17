-- 1. Kullanýcýlar
CREATE TABLE dbo.Kullanicilar (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    AdSoyad             NVARCHAR(100)  NOT NULL,
    EPosta              NVARCHAR(255)  NOT NULL UNIQUE,
    SifreHash           NVARCHAR(255)  NOT NULL,
    OlusturmaTarihi     DATETIME2      NOT NULL DEFAULT SYSDATETIME()
);

-- 2. Ürünler
CREATE TABLE dbo.Urunler (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    Ad                  NVARCHAR(150)  NOT NULL,
    Aciklama            NVARCHAR(MAX)  NULL,
    Fiyat               DECIMAL(18,2)  NOT NULL,
    StokAdedi           INT            NOT NULL DEFAULT 0,
    OlusturmaTarihi     DATETIME2      NOT NULL DEFAULT SYSDATETIME()
);

-- 3. Sipariþler
CREATE TABLE dbo.Siparisler (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    KullaniciKimlik     INT            NOT NULL,
    SiparisTarihi       DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    Durum               NVARCHAR(50)   NOT NULL,
    ToplamTutar         DECIMAL(18,2)  NOT NULL,
    CONSTRAINT FK_Siparisler_Kullanicilar
        FOREIGN KEY (KullaniciKimlik) 
        REFERENCES dbo.Kullanicilar(Kimlik)
);

-- 4. Kargolar
CREATE TABLE dbo.Kargolar (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    SiparisKimlik       INT            NOT NULL,
    TakipNumarasi       NVARCHAR(100)  UNIQUE,
    GonderimTarihi      DATETIME2      NULL,
    Durum               NVARCHAR(50)   NULL,
    CONSTRAINT FK_Kargolar_Siparisler
        FOREIGN KEY (SiparisKimlik) 
        REFERENCES dbo.Siparisler(Kimlik)
);

-- 5. Konumlar
CREATE TABLE dbo.Konumlar (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    Ad                  NVARCHAR(100)  NOT NULL,
    Enlem               DECIMAL(9,6)   NOT NULL,
    Boylam              DECIMAL(9,6)   NOT NULL
);

-- 6. Emisyon Faktörleri
CREATE TABLE dbo.EmisyonFaktorleri (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    YakitTipi           NVARCHAR(50)   NOT NULL,
    KgCO2_Per_Litre     DECIMAL(18,6)  NOT NULL
);

-- 7. Araçlar
CREATE TABLE dbo.Araclar (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    Tur                 NVARCHAR(50)   NOT NULL,     -- Örn. "Kamyon", "Elektrikli Van"
    OrtalamaTuketim     DECIMAL(5,2)   NOT NULL,     -- litre/km
    YakitTipi           NVARCHAR(50)   NOT NULL,
    CONSTRAINT FK_Araclar_EmisyonFaktorleri
        FOREIGN KEY (YakitTipi) 
        REFERENCES dbo.EmisyonFaktorleri(YakitTipi)
);

-- 8. Rotalar
CREATE TABLE dbo.Rotalar (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    Ad                  NVARCHAR(100)  NOT NULL,
    AracKimlik          INT            NOT NULL,
    ToplamMesafe        DECIMAL(18,2)  NOT NULL,    -- km
    TahminiEmisyon      DECIMAL(18,2)  NOT NULL,    -- kg CO2
    CONSTRAINT FK_Rotalar_Araclar
        FOREIGN KEY (AracKimlik) 
        REFERENCES dbo.Araclar(Kimlik)
);

-- 9. Rota Bölümleri
CREATE TABLE dbo.RotaBolumleri (
    Kimlik              INT            IDENTITY(1,1) PRIMARY KEY,
    RotaKimlik          INT            NOT NULL,
    BaslangicKonum      INT            NOT NULL,
    BitisKonum          INT            NOT NULL,
    Mesafe              DECIMAL(18,2)  NOT NULL,    -- km
    CONSTRAINT FK_RotaBolumleri_Rotalar
        FOREIGN KEY (RotaKimlik) 
        REFERENCES dbo.Rotalar(Kimlik),
    CONSTRAINT FK_RotaBolumleri_BaslangicKonum
        FOREIGN KEY (BaslangicKonum) 
        REFERENCES dbo.Konumlar(Kimlik),
    CONSTRAINT FK_RotaBolumleri_BitisKonum
        FOREIGN KEY (BitisKonum) 
        REFERENCES dbo.Konumlar(Kimlik)
);

-- 10. Sevkiyat–Rota Ýliþkisi (many-to-many)
CREATE TABLE dbo.SevkiyatRotalari (
    KargoKimlik         INT            NOT NULL,
    RotaKimlik          INT            NOT NULL,
    PRIMARY KEY (KargoKimlik, RotaKimlik),
    CONSTRAINT FK_SevkiyatRotalari_Kargolar
        FOREIGN KEY (KargoKimlik) 
        REFERENCES dbo.Kargolar(Kimlik),
    CONSTRAINT FK_SevkiyatRotalari_Rotalar
        FOREIGN KEY (RotaKimlik) 
        REFERENCES dbo.Rotalar(Kimlik)
);
CREATE TABLE dbo.OrderItems (
    SiparisKimlik INT            NOT NULL,
    UrunKimlik    INT            NOT NULL,
    Adet          INT            NOT NULL,
    Fiyat         DECIMAL(18,2)  NOT NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY (SiparisKimlik, UrunKimlik),
    CONSTRAINT FK_OrderItems_Siparisler
        FOREIGN KEY (SiparisKimlik)
        REFERENCES dbo.Siparisler(Kimlik),
    CONSTRAINT FK_OrderItems_Urunler
        FOREIGN KEY (UrunKimlik)
        REFERENCES dbo.Urunler(Kimlik)
);
ALTER TABLE dbo.EmisyonFaktorleri
ADD CONSTRAINT UQ_EmisyonFaktorleri_YakitTipi UNIQUE(YakitTipi);
INSERT INTO dbo.EmisyonFaktorleri (YakitTipi, KgCO2_Per_Litre)
VALUES
  ('Diesel', 0.265),
  ('Benzin', 0.239),
  ('Elektrik', 0.0);

  USE ECommerceGreen;
GO

INSERT INTO dbo.EmisyonFaktorleri (YakitTipi, KgCO2_Per_Litre)
VALUES
  ('Diesel', 0.265),
  ('Benzin', 0.239),
  ('Elektrik', 0.000);