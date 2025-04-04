CREATE OR ALTER PROCEDURE sp_GetTransactionIncomeLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @currencies TABLE (Currency VARCHAR(3), CurrencyId INT)
    INSERT INTO @currencies VALUES ('GEL', 0), ('USD', 1), ('EUR', 2) -- Assuming 0=GEL, 1=USD, 2=EUR
    
    SELECT c.Currency, ISNULL(t.FeeAmount, 0) AS Amount
    FROM @currencies c
    LEFT JOIN (
        SELECT Currency, SUM(TransactionFee) AS FeeAmount
        FROM Transactions
        WHERE TransactionDate >= DATEADD(YEAR, -1, GETDATE())
        GROUP BY Currency
    ) t ON c.CurrencyId = t.Currency; -- Join on integer ID
END;
