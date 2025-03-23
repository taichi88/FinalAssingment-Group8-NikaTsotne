-- User Statistics Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetUserCountLast30Days
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM AspNetUsers
    WHERE RegistrationDate >= DATEADD(DAY, -30, GETDATE());
END;