-- Monthly Transaction Breakdown Stored Procedure
CREATE OR ALTER PROCEDURE sp_GetTransactionsByDayLastMonth
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create a table with all dates in the last month
    DECLARE @startDate DATE = DATEADD(MONTH, -1, CAST(GETDATE() AS DATE))
    DECLARE @endDate DATE = CAST(GETDATE() AS DATE)
    
    -- Table to store all dates in range
    DECLARE @allDates TABLE (Day DATE)
    
    -- Populate the dates table
    DECLARE @currentDate DATE = @startDate
    WHILE @currentDate <= @endDate
    BEGIN
        INSERT INTO @allDates (Day) VALUES (@currentDate)
        SET @currentDate = DATEADD(DAY, 1, @currentDate)
    END
    
    -- Join with transactions to get counts, ensuring all days appear
    SELECT 
        d.Day,
        ISNULL(COUNT(t.TransactionDate), 0) AS Count
    FROM @allDates d
    LEFT JOIN Transactions t ON CAST(t.TransactionDate AS DATE) = d.Day
    GROUP BY d.Day
    ORDER BY d.Day;
END;
GO
