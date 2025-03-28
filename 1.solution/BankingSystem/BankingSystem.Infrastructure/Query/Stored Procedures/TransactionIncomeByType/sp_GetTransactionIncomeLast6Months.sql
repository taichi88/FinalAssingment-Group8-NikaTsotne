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
