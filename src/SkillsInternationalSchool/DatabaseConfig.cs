\
using System;

namespace finalProject
{
    internal static class DatabaseConfig
    {
        private const string ConnectionEnvironmentVariable = "SKILLS_SCHOOL_DB_CONNECTION";

        public static string ConnectionString
        {
            get
            {
                string? configured = Environment.GetEnvironmentVariable(ConnectionEnvironmentVariable);
                if (!string.IsNullOrWhiteSpace(configured))
                {
                    return configured;
                }

                return @"Server=(localdb)\MSSQLLocalDB;Database=Student;Integrated Security=True;TrustServerCertificate=True;";
            }
        }
    }
}
