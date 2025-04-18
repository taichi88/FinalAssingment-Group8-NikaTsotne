CREATE OR ALTER PROCEDURE sp_GetAverageTransactionIncome
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @currencies TABLE (Currency NVARCHAR(3), CurrencyId INT)
    INSERT INTO @currencies VALUES ('GEL', 0), ('USD', 1), ('EUR', 2)
    
    SELECT c.Currency, ISNULL(t.AvgFeeAmount, 0) AS Amount
    FROM @currencies c
    LEFT JOIN (
        SELECT Currency, AVG(TransactionFee) AS AvgFeeAmount
        FROM Transactions
        GROUP BY Currency
    ) t ON c.CurrencyId = t.Currency;
END;
