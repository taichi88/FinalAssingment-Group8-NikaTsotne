-- Transaction Fee Income By Currency Stored Procedures
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
