USE master
GO

DROP DATABASE IF EXISTS CurrencyDB;
GO

CREATE DATABASE CurrencyDB;
GO

USE CurrencyDB
GO

CREATE TABLE Currencies
(
    ID INT CONSTRAINT PK_Currencies PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(450) NOT NULL
)
GO

INSERT INTO Currencies VALUES('AZN'), ('USD'), ('EUR'), ('UAH')
GO



CREATE TABLE CurrencyConversions
(
    [From] INT CONSTRAINT FK_CurrencyConversion_Currencies FOREIGN KEY REFERENCES Currencies(ID),
    [To] INT CONSTRAINT FK_CurrencyConversion_Currencies2 FOREIGN KEY REFERENCES Currencies(ID),
    [OneCurrencyUnitCost] DECIMAL(5,4) NOT NULL
)
GO

