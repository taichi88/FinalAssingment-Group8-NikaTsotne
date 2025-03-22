-- Transaction Statistics Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetTransactionCountLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Transactions
    WHERE TransactionDate >= DATEADD(YEAR, -1, GETDATE());
END;
GO
