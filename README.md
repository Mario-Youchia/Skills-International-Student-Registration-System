# Skills International Student Registration System

A C# Windows Forms application for authenticated, database-backed student registration and record management with Microsoft SQL Server.

[![Watch the student registration system demo](assets/images/demo-preview.png)](https://www.youtube.com/watch?v=REPLACE_WITH_VIDEO_ID)

## Features

- Database-backed login with a seeded demonstration account.
- Create, search, update, clear, and delete student registrations.
- Store personal, contact, and parent details in a relational database.
- Validate required fields, email addresses, phone numbers, gender, and date of birth.
- Use parameterized ADO.NET commands for all database operations.

## Run

Requirements: Windows, Visual Studio 2022 with the .NET desktop workload, .NET 6, and SQL Server LocalDB or SQL Server Express.

1. Run `database/setup.sql` in SQL Server Management Studio or Azure Data Studio.
2. Open `SkillsInternationalSchool.sln` and restore NuGet packages.
3. Start the application.

Demonstration login:

```text
Username: Admin
Password: Skills@123
```

The default connection uses `(localdb)\MSSQLLocalDB`. For another SQL Server instance, set `SKILLS_SCHOOL_DB_CONNECTION` before launching the application.

## Screens

![Login screen](assets/images/login-screen.png)

![Student registration form](assets/images/registration-form.png)

![Database schema](assets/images/database-schema.png)
