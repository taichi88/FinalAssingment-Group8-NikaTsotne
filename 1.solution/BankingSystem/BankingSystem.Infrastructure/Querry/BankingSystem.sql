GO
USE BankingSystem;

CREATE TABLE Accounts(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	IBAN NVARCHAR(MAX) NOT NULL,
	Balance decimal(18,2) NOT NULL,
	Currency NVARCHAR(3) NOT NULL,
	PersonId NVARCHAR(450) NOT NULL,

	FOREIGN KEY (PersonId)
	REFERENCES AspNetUsers(Id)
)


CREATE TABLE Cards(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Firstname NVARCHAR(MAX) NOT NULL,
	Lastname NVARCHAR(MAX) NOT NULL,
	CardNumber NVARCHAR(MAX) NOT NULL,
	ExpirationDate DATE NOT NULL,
	CVV NVARCHAR(MAX) NOT NULL,
	PinCode NVARCHAR(MAX) NOT NULL,
	AccountId INT NOT NULL,

	FOREIGN KEY (AccountId)
	REFERENCES Accounts(Id)
)


CREATE TABLE Transactions(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Amount decimal(18,2) NOT NULL,
	Currency NVARCHAR(3) NOT NULL,
	TransactionType NVARCHAR(50) NOT NULL,
	TransactionDate DATE NOT NULL,
	FromAccountId INT NOT NULL,
	ToAccountId INT NOT NULL,
	IsATM BIT NULL,
	TransactionType INT NULL,
	TransactionFee DECIMAL(18,2) NOT NULL DEFAULT 0,
	FOREIGN KEY (FromAccountId)
	REFERENCES Accounts(Id),

	FOREIGN KEY (ToAccountId)
	REFERENCES Accounts(Id)
)

USE [BankingSystem]
GO

-- User Statistics Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetUserCountThisYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM [BankingSystem].[dbo].[AspNetUsers]
    WHERE YEAR(RegistrationDate) = YEAR(GETDATE());
END;
GO

CREATE OR ALTER PROCEDURE sp_GetUserCountLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM [BankingSystem].[dbo].[AspNetUsers]
    WHERE RegistrationDate >= DATEADD(YEAR, -1, GETDATE());
END;
GO

CREATE OR ALTER PROCEDURE sp_GetUserCountLast30Days
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM [BankingSystem].[dbo].[AspNetUsers]
    WHERE RegistrationDate >= DATEADD(DAY, -30, GETDATE());
END;
GO

-- Transaction Statistics Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetTransactionCountLastMonth
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Transactions
    WHERE TransactionDate >= DATEADD(MONTH, -1, GETDATE());
END;
GO

CREATE OR ALTER PROCEDURE sp_GetTransactionCountLast6Months
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Transactions
    WHERE TransactionDate >= DATEADD(MONTH, -6, GETDATE());
END;
GO

CREATE OR ALTER PROCEDURE sp_GetTransactionCountLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Transactions
    WHERE TransactionDate >= DATEADD(YEAR, -1, GETDATE());
END;
GO

-- Transaction Fee Income By Currency Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetTransactionIncomeLastMonth
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @currencies TABLE (Currency VARCHAR(3))
    INSERT INTO @currencies VALUES ('GEL'), ('USD'), ('EUR')
    
    SELECT c.Currency, ISNULL(t.FeeAmount, 0) AS Amount
    FROM @currencies c
    LEFT JOIN (
        SELECT Currency, SUM(TransactionFee) AS FeeAmount
        FROM Transactions
        WHERE TransactionDate >= DATEADD(MONTH, -1, GETDATE())
        GROUP BY Currency
    ) t ON c.Currency = t.Currency;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetTransactionIncomeLast6Months
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @currencies TABLE (Currency VARCHAR(3))
    INSERT INTO @currencies VALUES ('GEL'), ('USD'), ('EUR')
    
    SELECT c.Currency, ISNULL(t.FeeAmount, 0) AS Amount
    FROM @currencies c
    LEFT JOIN (
        SELECT Currency, SUM(TransactionFee) AS FeeAmount
        FROM Transactions
        WHERE TransactionDate >= DATEADD(MONTH, -6, GETDATE())
        GROUP BY Currency
    ) t ON c.Currency = t.Currency;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetTransactionIncomeLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @currencies TABLE (Currency VARCHAR(3))
    INSERT INTO @currencies VALUES ('GEL'), ('USD'), ('EUR')
    
    SELECT c.Currency, ISNULL(t.FeeAmount, 0) AS Amount
    FROM @currencies c
    LEFT JOIN (
        SELECT Currency, SUM(TransactionFee) AS FeeAmount
        FROM Transactions
        WHERE TransactionDate >= DATEADD(YEAR, -1, GETDATE())
        GROUP BY Currency
    ) t ON c.Currency = t.Currency;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetAverageTransactionIncome
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @currencies TABLE (Currency VARCHAR(3))
    INSERT INTO @currencies VALUES ('GEL'), ('USD'), ('EUR')
    
    SELECT c.Currency, ISNULL(t.AvgFeeAmount, 0) AS Amount
    FROM @currencies c
    LEFT JOIN (
        SELECT Currency, AVG(TransactionFee) AS AvgFeeAmount
        FROM Transactions
        GROUP BY Currency
    ) t ON c.Currency = t.Currency;
END;
GO


-- Monthly Transaction Breakdown Stored Procedure
CREATE OR ALTER PROCEDURE sp_GetTransactionsByDayLastMonth
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create a table with all dates in the last month
    DECLARE @startDate DATE = DATEADD(MONTH, -1, CAST(GETDATE() AS DATE))
    DECLARE @endDate DATE = CAST(GETDATE() AS DATE)
    
    -- Table to store all dates in range
    DECLARE @allDates TABLE (Day DATE)
    
    -- Populate the dates table
    DECLARE @currentDate DATE = @startDate
    WHILE @currentDate <= @endDate
    BEGIN
        INSERT INTO @allDates (Day) VALUES (@currentDate)
        SET @currentDate = DATEADD(DAY, 1, @currentDate)
    END
    
    -- Join with transactions to get counts, ensuring all days appear
    SELECT 
        d.Day,
        ISNULL(COUNT(t.TransactionDate), 0) AS Count
    FROM @allDates d
    LEFT JOIN Transactions t ON CAST(t.TransactionDate AS DATE) = d.Day
    GROUP BY d.Day
    ORDER BY d.Day;
END;
GO

-- ATM Withdrawal Statistics Stored Procedure
CREATE OR ALTER PROCEDURE sp_GetTotalAtmWithdrawalAmount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ISNULL(SUM(Amount), 0) AS TotalWithdrawalAmount
    FROM Transactions
    WHERE IsATM = 1;
END;
GO
