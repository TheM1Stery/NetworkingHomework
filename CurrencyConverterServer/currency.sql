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
    [OneCurrencyUnitCost] DECIMAL(10,2) NOT NULL
)
GO

DECLARE @AZN AS INT = (SELECT ID FROM Currencies WHERE Name = N'AZN')
DECLARE @EUR AS INT = (SELECT ID FROM Currencies WHERE Name = N'EUR')
DECLARE @USD AS INT = (SELECT ID FROM Currencies WHERE Name = N'USD')
DECLARE @UAH AS INT = (SELECT ID FROM Currencies WHERE NAME = N'UAH')


INSERT INTO CurrencyConversions VALUES 
(@EUR, @AZN, 1.69),
(@EUR, @USD, 1.00),
(@EUR, @UAH, 36.76),
(@AZN, @EUR, 0.59),
(@AZN, @USD, 0.59),
(@AZN, @UAH, 21.73),
(@USD, @EUR, 1.00),
(@USD, @AZN, 1.70),
(@USD, @UAH, 36.94),
(@UAH, @AZN, 0.046),
(@UAH, @USD, 0.027),
(@UAH, @EUR, 0.027)
GO