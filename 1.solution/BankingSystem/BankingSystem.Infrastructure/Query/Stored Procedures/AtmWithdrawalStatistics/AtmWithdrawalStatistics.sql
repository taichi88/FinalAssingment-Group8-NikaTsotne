-- ATM Withdrawal Statistics Stored Procedure
CREATE OR ALTER PROCEDURE sp_GetTotalAtmWithdrawalAmount
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ISNULL(SUM(Amount), 0) AS TotalWithdrawalAmount
    FROM Transactions
    WHERE IsATM = 1;
END;
