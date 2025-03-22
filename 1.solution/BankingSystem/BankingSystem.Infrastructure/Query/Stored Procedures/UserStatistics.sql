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

CREATE OR ALTER PROCEDURE sp_GetUserCountLastYear
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM AspNetUsers
    WHERE RegistrationDate >= DATEADD(YEAR, -1, GETDATE());
END;
GO

CREATE OR ALTER PROCEDURE sp_GetUserCountLast30Days
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*)
    FROM AspNetUsers
    WHERE RegistrationDate >= DATEADD(DAY, -30, GETDATE());
END;
GO