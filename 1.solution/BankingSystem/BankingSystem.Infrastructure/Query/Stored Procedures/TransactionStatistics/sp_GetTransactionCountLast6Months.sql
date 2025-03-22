-- Transaction Statistics Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetTransactionCountLast6Months
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM Transactions
    WHERE TransactionDate >= DATEADD(MONTH, -6, GETDATE());
END;
GO
