using System;
using System.Data.OleDb;
using VolunteerHub.Helpers;

namespace VolunteerHub.DAL
{
    /// <summary>
    /// Called once in Application_Start (Global.asax.cs).
    /// Uses OleDb DDL CREATE TABLE statements to create the 5 application tables
    /// inside the pre-existing VolunteerHub.accdb file.
    /// Safe to call repeatedly — each table is only created if it doesn't exist yet.
    /// Also seeds a SuperAdmin account on first run so the app is immediately usable.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    EnsureAppUser(conn);
                    EnsureWorkspaces(conn);
                    EnsureProjects(conn);
                    EnsureVolunteerProject(conn);
                    EnsureEvents(conn);
                    SeedSuperAdmin(conn);
                }
            }
            catch (Exception ex)
            {
                // Log to Application event at startup – non-fatal
                System.Diagnostics.Trace.TraceError("DB Init error: " + ex.Message);
            }
        }

        private static bool TableExists(OleDbConnection conn, string tableName)
        {
            try
            {
                using (var cmd = new OleDbCommand($"SELECT TOP 1 * FROM [{tableName}]", conn))
                    cmd.ExecuteReader().Close();
                return true;
            }
            catch { return false; }
        }

        private static void Execute(OleDbConnection conn, string sql)
        {
            using (var cmd = new OleDbCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private static void EnsureAppUser(OleDbConnection conn)
        {
            if (TableExists(conn, "AppUser")) return;
            Execute(conn, @"
                CREATE TABLE AppUser (
                    Id                     COUNTER CONSTRAINT PK_AppUser PRIMARY KEY,
                    FirstName              TEXT(100) NOT NULL,
                    LastName               TEXT(100) NOT NULL,
                    Email                  TEXT(255) NOT NULL,
                    PasswordHash           TEXT(255) NOT NULL,
                    IsMale                 YESNO NOT NULL,
                    DateOfBirth            DATETIME,
                    Phone                  TEXT(20),
                    Address                MEMO,
                    ImageProfilePath       TEXT(500),
                    Role                   TEXT(20) NOT NULL,
                    WorkspaceId            LONG,
                    IsActive               YESNO NOT NULL,
                    CreatedAt              DATETIME NOT NULL,
                    LastLoginAt            DATETIME,
                    RememberMeToken        TEXT(100),
                    RememberMeTokenExpiry  DATETIME
                )");
            Execute(conn, "CREATE UNIQUE INDEX IX_AppUser_Email ON AppUser (Email)");
            Execute(conn, "CREATE INDEX IX_AppUser_WorkspaceId ON AppUser (WorkspaceId)");
            Execute(conn, "CREATE INDEX IX_AppUser_Token ON AppUser (RememberMeToken)");
        }

        private static void EnsureWorkspaces(OleDbConnection conn)
        {
            if (TableExists(conn, "Workspaces")) return;
            // IsActive: drives workspace visibility for registration + admin UI
            // CreatedAt: used for audit/ordering in SuperAdmin Workspaces list
            Execute(conn, @"
                CREATE TABLE Workspaces (
                    Id        COUNTER CONSTRAINT PK_Workspaces PRIMARY KEY,
                    Name      TEXT(200) NOT NULL,
                    Code      TEXT(50)  NOT NULL,
                    LogoPath  TEXT(500),
                    IsActive  YESNO     NOT NULL,
                    CreatedAt DATETIME  NOT NULL
                )");
            Execute(conn, "CREATE UNIQUE INDEX IX_Workspaces_Code ON Workspaces (Code)");
        }

        private static void EnsureProjects(OleDbConnection conn)
        {
            if (TableExists(conn, "Projects")) return;
            // Column names match WorkspaceDAL.MapReader exactly.
            // Title (not Name), MaxVolunteers, HoursRequired, Location — all used by pages.
            Execute(conn, @"
                CREATE TABLE Projects (
                    Id            COUNTER  CONSTRAINT PK_Projects PRIMARY KEY,
                    WorkspaceId   LONG     NOT NULL,
                    Title         TEXT(200) NOT NULL,
                    Description   MEMO,
                    Location      TEXT(200),
                    StartDate     DATETIME NOT NULL,
                    EndDate       DATETIME NOT NULL,
                    MaxVolunteers LONG,
                    HoursRequired DOUBLE,
                    CreatedAt     DATETIME NOT NULL
                )");
            Execute(conn, "CREATE INDEX IX_Projects_WorkspaceId ON Projects (WorkspaceId)");
        }

        private static void EnsureVolunteerProject(OleDbConnection conn)
        {
            if (TableExists(conn, "VolunteerProject")) return;
            // EnrolledAt (not JoinedAt) — matches VolunteerProjectDAL.MapReader
            Execute(conn, @"
                CREATE TABLE VolunteerProject (
                    Id         COUNTER CONSTRAINT PK_VolunteerProject PRIMARY KEY,
                    UserId     LONG NOT NULL,
                    ProjectId  LONG NOT NULL,
                    EnrolledAt DATETIME NOT NULL
                )");
            Execute(conn, "CREATE INDEX IX_VP_UserId ON VolunteerProject (UserId)");
            Execute(conn, "CREATE INDEX IX_VP_ProjectId ON VolunteerProject (ProjectId)");
        }

        private static void EnsureEvents(OleDbConnection conn)
        {
            if (TableExists(conn, "Events")) return;
            // Schema matches EventDAL.MapReader:
            //   EventDate   – the calendar date of the session (no time component)
            //   StartTime/EndTime – optional HH:MM strings
            //   HoursLogged – decimal hours; the authoritative value for all calculations
            //   Notes       – optional volunteer notes
            //   LoggedAt    – UTC creation timestamp
            Execute(conn, @"
                CREATE TABLE Events (
                    Id          COUNTER  CONSTRAINT PK_Events PRIMARY KEY,
                    UserId      LONG     NOT NULL,
                    ProjectId   LONG     NOT NULL,
                    EventDate   DATETIME NOT NULL,
                    StartTime   TEXT(10),
                    EndTime     TEXT(10),
                    HoursLogged DOUBLE   NOT NULL,
                    Notes       MEMO,
                    LoggedAt    DATETIME NOT NULL
                )");
            Execute(conn, "CREATE INDEX IX_Events_UserId ON Events (UserId)");
            Execute(conn, "CREATE INDEX IX_Events_ProjectId ON Events (ProjectId)");
        }

        private static void SeedSuperAdmin(OleDbConnection conn)
        {
            // Only seed if AppUser table is empty — safe to call on every startup
            using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM AppUser", conn))
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0) return;
            }

            // AuthHelper is in the same assembly (VolunteerHub.Helpers namespace)
            string hash = AuthHelper.HashPassword("Admin@1234");
            const string sql = @"INSERT INTO AppUser
                (FirstName, LastName, Email, PasswordHash, IsMale, Role, IsActive, CreatedAt)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@fn",  "Super");
                cmd.Parameters.AddWithValue("@ln",  "Admin");
                cmd.Parameters.AddWithValue("@em",  "admin@volunteerhub.local");
                cmd.Parameters.AddWithValue("@ph",  hash);
                cmd.Parameters.AddWithValue("@gen", true);
                cmd.Parameters.AddWithValue("@rl",  "SuperAdmin");
                cmd.Parameters.AddWithValue("@ac",  true);
                cmd.Parameters.AddWithValue("@ca",  DateTime.UtcNow);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
