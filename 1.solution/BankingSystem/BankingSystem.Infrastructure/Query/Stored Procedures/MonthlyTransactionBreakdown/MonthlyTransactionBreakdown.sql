CREATE OR ALTER PROCEDURE sp_GetTransactionsByDayLastMonth
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @startDate DATE = DATEADD(MONTH, -1, CAST(GETDATE() AS DATE))
    DECLARE @endDate DATE = CAST(GETDATE() AS DATE)
    
    DECLARE @allDates TABLE (Day DATE)
    
    DECLARE @currentDate DATE = @startDate
    WHILE @currentDate <= @endDate
    BEGIN
        INSERT INTO @allDates (Day) VALUES (@currentDate)
        SET @currentDate = DATEADD(DAY, 1, @currentDate)
    END
    
    SELECT 
        d.Day,
        ISNULL(COUNT(t.TransactionDate), 0) AS Count
    FROM @allDates d
    LEFT JOIN Transactions t ON CAST(t.TransactionDate AS DATE) = d.Day
    GROUP BY d.Day
    ORDER BY d.Day;
END;
