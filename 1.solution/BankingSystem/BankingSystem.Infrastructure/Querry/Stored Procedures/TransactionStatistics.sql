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
