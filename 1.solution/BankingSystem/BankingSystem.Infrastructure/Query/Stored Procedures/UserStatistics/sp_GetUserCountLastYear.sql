CREATE OR ALTER PROCEDURE sp_GetUserCountLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM AspNetUsers
    WHERE RegistrationDate >= DATEADD(YEAR, -1, GETDATE());
END;