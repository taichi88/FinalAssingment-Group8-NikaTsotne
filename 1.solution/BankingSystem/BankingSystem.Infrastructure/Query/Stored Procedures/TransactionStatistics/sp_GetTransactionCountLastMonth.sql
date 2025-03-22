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
