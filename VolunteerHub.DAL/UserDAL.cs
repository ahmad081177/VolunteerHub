using System;
using System.Collections.Generic;
using System.Data.OleDb;
using VolunteerHub.Models;

namespace VolunteerHub.DAL
{
    public static class UserDAL
    {
        private static AppUser MapReader(OleDbDataReader r)
        {
            // Convert a database row into an AppUser object.
            // For nullable columns we check for DBNull first — skipping the check causes a runtime
            // exception when the column contains NULL in Access.
            return new AppUser
            {
                Id                    = Convert.ToInt32(r["Id"]),
                FirstName             = r["FirstName"]?.ToString(),
                LastName              = r["LastName"]?.ToString(),
                Email                 = r["Email"]?.ToString(),
                PasswordHash          = r["PasswordHash"]?.ToString(),
                IsMale                = r["IsMale"] != DBNull.Value && Convert.ToBoolean(r["IsMale"]),
                DateOfBirth           = r["DateOfBirth"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["DateOfBirth"]),
                Phone                 = r["Phone"] == DBNull.Value ? null : r["Phone"]?.ToString(),
                Address               = r["Address"] == DBNull.Value ? null : r["Address"]?.ToString(),
                ImageProfilePath      = r["ImageProfilePath"] == DBNull.Value ? null : r["ImageProfilePath"]?.ToString(),
                Role                  = r["Role"]?.ToString(),
                WorkspaceId           = r["WorkspaceId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["WorkspaceId"]),
                IsActive              = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]),
                CreatedAt             = Convert.ToDateTime(r["CreatedAt"]),
                LastLoginAt           = r["LastLoginAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["LastLoginAt"]),
                RememberMeToken       = r["RememberMeToken"] == DBNull.Value ? null : r["RememberMeToken"]?.ToString(),
                RememberMeTokenExpiry = r["RememberMeTokenExpiry"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["RememberMeTokenExpiry"])
            };
        }

        public static bool EmailExists(string email)
        {
            // Convenience overload — used when creating a brand-new account (no exclusion needed)
            return EmailExists(email, 0);
        }

        public static bool EmailExists(string email, int excludeId)
        {
            // excludeId = 0 means "exclude no one" (Access COUNTER IDs start at 1)
            const string sql = "SELECT COUNT(*) FROM AppUser WHERE Email = ? AND Id <> ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@e",  OleDbType.VarChar).Value  = email;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value  = excludeId;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static int Insert(AppUser u)
        {
            const string sql = @"INSERT INTO AppUser
                (FirstName, LastName, Email, PasswordHash, IsMale, DateOfBirth, Phone, Address,
                 ImageProfilePath, Role, WorkspaceId, IsActive, CreatedAt)
                VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                // Use explicit OleDbType — AddWithValue mis-infers Access YESNO (bool)
                // and DATETIME on some driver versions, causing "Data type mismatch".
                cmd.Parameters.Add("@fn",  OleDbType.VarChar).Value = u.FirstName;
                cmd.Parameters.Add("@ln",  OleDbType.VarChar).Value = u.LastName;
                cmd.Parameters.Add("@em",  OleDbType.VarChar).Value = u.Email;
                cmd.Parameters.Add("@ph",  OleDbType.VarChar).Value = u.PasswordHash;
                cmd.Parameters.Add("@gn",  OleDbType.Boolean).Value = u.IsMale;
                var dbParam = cmd.Parameters.Add("@db", OleDbType.DBDate);
                dbParam.Value = u.DateOfBirth.HasValue ? (object)u.DateOfBirth.Value : DBNull.Value;
                cmd.Parameters.Add("@pn",  OleDbType.VarChar).Value = (object)u.Phone   ?? DBNull.Value;
                cmd.Parameters.Add("@ad",  OleDbType.LongVarChar).Value = (object)u.Address ?? DBNull.Value;
                cmd.Parameters.Add("@ip",  OleDbType.VarChar).Value = (object)u.ImageProfilePath ?? DBNull.Value;
                cmd.Parameters.Add("@rl",  OleDbType.VarChar).Value = u.Role;
                var wiParam = cmd.Parameters.Add("@wi", OleDbType.Integer);
                wiParam.Value = u.WorkspaceId.HasValue ? (object)u.WorkspaceId.Value : DBNull.Value;
                cmd.Parameters.Add("@ac",  OleDbType.Boolean).Value = u.IsActive;
                cmd.Parameters.Add("@ca",  OleDbType.DBDate).Value  = u.CreatedAt;
                cmd.ExecuteNonQuery();
                // @@IDENTITY returns the AutoNumber value Access assigned to the newly inserted row.
                cmd.CommandText = "SELECT @@IDENTITY";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static AppUser GetByEmail(string email)
        {
            const string sql = "SELECT * FROM AppUser WHERE Email = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@e", email);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapReader(r) : null;
            }
        }

        public static AppUser GetById(int id)
        {
            const string sql = "SELECT * FROM AppUser WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapReader(r) : null;
            }
        }

        public static AppUser GetByRememberMeToken(string token)
        {
            // Used by BasePage.TryRestoreFromCookie to re-authenticate a returning user
            // without requiring them to log in again. Token expiry is validated in BasePage.
            const string sql = "SELECT * FROM AppUser WHERE RememberMeToken = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@t", token);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapReader(r) : null;
            }
        }

        public static List<AppUser> GetAllAdmins()
        {
            const string sql = "SELECT * FROM AppUser WHERE Role = 'Admin' ORDER BY FirstName, LastName";
            var list = new List<AppUser>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            using (var r    = cmd.ExecuteReader())
                while (r.Read()) list.Add(MapReader(r));
            return list;
        }

        public static List<AppUser> GetAllByWorkspace(int workspaceId)
        {
            const string sql = "SELECT * FROM AppUser WHERE WorkspaceId = ? AND Role = 'Volunteer' ORDER BY FirstName, LastName";
            var list = new List<AppUser>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(MapReader(r));
            }
            return list;
        }

        public static void UpdateLastLogin(int id)
        {
            const string sql = "UPDATE AppUser SET LastLoginAt = ? WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@ll", OleDbType.DBDate).Value  = DateTime.UtcNow;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateRememberMeToken(int id, string token, DateTime expiry)
        {
            const string sql = "UPDATE AppUser SET RememberMeToken = ?, RememberMeTokenExpiry = ? WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@t",  OleDbType.VarChar).Value = token;
                cmd.Parameters.Add("@ex", OleDbType.DBDate).Value  = expiry;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearRememberMeToken(int id)
        {
            // Called on Logout and when an expired token is detected in BasePage.
            // Sets both token columns to NULL so the old cookie value can no longer restore a session.
            const string sql = "UPDATE AppUser SET RememberMeToken = NULL, RememberMeTokenExpiry = NULL WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void SetIsActive(int id, bool active)
        {
            const string sql = "UPDATE AppUser SET IsActive = ? WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@a",  OleDbType.Boolean).Value = active;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            // Hard delete — removes the admin account permanently.
            // WHERE Role='Admin' prevents accidentally deleting SuperAdmin or Volunteer accounts.
            const string sql = "DELETE FROM AppUser WHERE Id=? AND Role='Admin'";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@id", OleDbType.Integer).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateAdmin(AppUser u)
        {
            // Updates only the fields an administrator's profile exposes: name, email, workspace, active flag.
            // PasswordHash and Role are deliberately excluded to prevent privilege escalation.
            const string sql = @"UPDATE AppUser
                SET FirstName=?, LastName=?, Email=?, WorkspaceId=?, IsActive=?
                WHERE Id=? AND Role='Admin'";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@fn", OleDbType.VarChar).Value  = u.FirstName;
                cmd.Parameters.Add("@ln", OleDbType.VarChar).Value  = u.LastName;
                cmd.Parameters.Add("@em", OleDbType.VarChar).Value  = u.Email;
                var wiParam = cmd.Parameters.Add("@wi", OleDbType.Integer);
                wiParam.Value = u.WorkspaceId.HasValue ? (object)u.WorkspaceId.Value : DBNull.Value;
                cmd.Parameters.Add("@ac", OleDbType.Boolean).Value  = u.IsActive;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value  = u.Id;
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateProfile(AppUser u)
        {
            const string sql = @"UPDATE AppUser
                SET FirstName=?, LastName=?, Phone=?, Address=?, ImageProfilePath=?
                WHERE Id=?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@fn", u.FirstName);
                cmd.Parameters.AddWithValue("@ln", u.LastName);
                cmd.Parameters.AddWithValue("@pn", (object)u.Phone   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ad", (object)u.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ip", (object)u.ImageProfilePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", u.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdatePassword(int id, string newHash)
        {
            const string sql = "UPDATE AppUser SET PasswordHash = ? WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@h",  newHash);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Returns list of (VolunteerFullName, TotalHours) for the volunteer bar chart on the admin dashboard
        public static List<(string FullName, decimal Hours)> GetHoursPerVolunteer(int workspaceId)
        {
            // LEFT JOIN keeps volunteers with zero hours in the result; IIF avoids NULL in the sum.
            const string sql = @"SELECT u.FirstName, u.LastName,
                IIF(SUM(e.HoursLogged) IS NULL, 0, SUM(e.HoursLogged)) AS TotalHours
                FROM AppUser u LEFT JOIN Events e ON u.Id = e.UserId
                WHERE u.WorkspaceId = ? AND u.Role = 'Volunteer'
                GROUP BY u.FirstName, u.LastName
                ORDER BY u.FirstName, u.LastName";
            var list = new List<(string, decimal)>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add((r["FirstName"] + " " + r["LastName"], Convert.ToDecimal(r["TotalHours"])));
            }
            return list;
        }

        // Returns list of (VolunteerFullName, EnrolledProjectCount) for the volunteer bar chart on the admin dashboard
        public static List<(string FullName, int Count)> GetProjectsPerVolunteer(int workspaceId)
        {
            const string sql = @"SELECT u.FirstName, u.LastName, COUNT(vp.ProjectId) AS ProjCount
                FROM AppUser u LEFT JOIN VolunteerProject vp ON u.Id = vp.UserId
                WHERE u.WorkspaceId = ? AND u.Role = 'Volunteer'
                GROUP BY u.FirstName, u.LastName
                ORDER BY u.FirstName, u.LastName";
            var list = new List<(string, int)>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add((r["FirstName"] + " " + r["LastName"], Convert.ToInt32(r["ProjCount"])));
            }
            return list;
        }
    }
}
