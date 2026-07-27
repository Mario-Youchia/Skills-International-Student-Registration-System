\
IF DB_ID(N'Student') IS NULL
BEGIN
    CREATE DATABASE Student;
END;
GO

USE Student;
GO

IF OBJECT_ID(N'dbo.Logins', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Logins
    (
        username NVARCHAR(50) NOT NULL PRIMARY KEY,
        [password] NVARCHAR(100) NOT NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Logins WHERE username = N'Admin')
BEGIN
    INSERT INTO dbo.Logins (username, [password])
    VALUES (N'Admin', N'Skills@123');
END;
GO

IF OBJECT_ID(N'dbo.Registration', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Registration
    (
        regNo INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        firstName NVARCHAR(50) NOT NULL,
        lastName NVARCHAR(50) NOT NULL,
        dateOfBirth DATE NOT NULL,
        gender NVARCHAR(10) NOT NULL,
        [address] NVARCHAR(250) NOT NULL,
        email NVARCHAR(100) NOT NULL,
        mobilePhone NVARCHAR(20) NULL,
        homePhone NVARCHAR(20) NULL,
        parentName NVARCHAR(100) NOT NULL,
        nic NVARCHAR(50) NOT NULL,
        contactNo NVARCHAR(20) NOT NULL
    );

    CREATE INDEX IX_Registration_LastName
        ON dbo.Registration(lastName, firstName);
END;
GO
