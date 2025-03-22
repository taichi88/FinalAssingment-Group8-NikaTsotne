-- User Statistics Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetUserCountThisYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM AspNetUsers
    WHERE YEAR(RegistrationDate) = YEAR(GETDATE());
END;
GO
