using System;
using System.Collections.Generic;
using System.Data.OleDb;
using VolunteerHub.Models;

namespace VolunteerHub.DAL
{
    public static class ProjectDAL
    {
        private static VHProject MapReader(OleDbDataReader r)
        {
            return new VHProject
            {
                Id           = Convert.ToInt32(r["Id"]),
                WorkspaceId  = Convert.ToInt32(r["WorkspaceId"]),
                Title        = r["Title"]?.ToString(),
                Description  = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Location     = r["Location"] == DBNull.Value ? null : r["Location"]?.ToString(),
                StartDate    = Convert.ToDateTime(r["StartDate"]),
                EndDate      = Convert.ToDateTime(r["EndDate"]),
                MaxVolunteers = r["MaxVolunteers"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaxVolunteers"]),
                HoursRequired = r["HoursRequired"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["HoursRequired"]),
                CreatedAt    = Convert.ToDateTime(r["CreatedAt"])
            };
        }

        public static int Insert(VHProject p)
        {
            const string sql = @"INSERT INTO Projects
                (WorkspaceId, Title, Description, Location, StartDate, EndDate, MaxVolunteers, HoursRequired, CreatedAt)
                VALUES (?,?,?,?,?,?,?,?,?)";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@wi", OleDbType.Integer).Value    = p.WorkspaceId;
                cmd.Parameters.Add("@ti", OleDbType.VarChar).Value    = p.Title;
                cmd.Parameters.Add("@dc", OleDbType.LongVarChar).Value = (object)p.Description ?? DBNull.Value;
                cmd.Parameters.Add("@lc", OleDbType.VarChar).Value    = (object)p.Location     ?? DBNull.Value;
                cmd.Parameters.Add("@sd", OleDbType.DBDate).Value     = p.StartDate;
                cmd.Parameters.Add("@ed", OleDbType.DBDate).Value     = p.EndDate;
                var mvP = cmd.Parameters.Add("@mv", OleDbType.Integer);
                mvP.Value = p.MaxVolunteers.HasValue ? (object)p.MaxVolunteers.Value : DBNull.Value;
                var hrP = cmd.Parameters.Add("@hr", OleDbType.Double);
                hrP.Value = p.HoursRequired.HasValue ? (object)(double)p.HoursRequired.Value : DBNull.Value;
                cmd.Parameters.Add("@ca", OleDbType.DBDate).Value     = p.CreatedAt;
                cmd.ExecuteNonQuery();
                // @@IDENTITY returns the AutoNumber value Access assigned to the newly inserted row.
                cmd.CommandText = "SELECT @@IDENTITY";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static List<VHProject> GetByWorkspace(int workspaceId)
        {
            const string sql = "SELECT * FROM Projects WHERE WorkspaceId = ? ORDER BY StartDate DESC";
            var list = new List<VHProject>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(MapReader(r));
            }
            return list;
        }

        public static VHProject GetById(int id)
        {
            const string sql = "SELECT * FROM Projects WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapReader(r) : null;
            }
        }

        public static void Update(VHProject p)
        {
            // WorkspaceId is included in the WHERE clause as a safety guard: even if a request
            // is crafted with another workspace's project ID, the UPDATE hits 0 rows because
            // the WorkspaceId will not match the caller's workspace.
            const string sql = @"UPDATE Projects
                SET Title=?, Description=?, Location=?, StartDate=?, EndDate=?, MaxVolunteers=?, HoursRequired=?
                WHERE Id=? AND WorkspaceId=?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@ti", OleDbType.VarChar).Value    = p.Title;
                cmd.Parameters.Add("@dc", OleDbType.LongVarChar).Value = (object)p.Description ?? DBNull.Value;
                cmd.Parameters.Add("@lc", OleDbType.VarChar).Value    = (object)p.Location     ?? DBNull.Value;
                cmd.Parameters.Add("@sd", OleDbType.DBDate).Value     = p.StartDate;
                cmd.Parameters.Add("@ed", OleDbType.DBDate).Value     = p.EndDate;
                var mvPu = cmd.Parameters.Add("@mv", OleDbType.Integer);
                mvPu.Value = p.MaxVolunteers.HasValue ? (object)p.MaxVolunteers.Value : DBNull.Value;
                var hrPu = cmd.Parameters.Add("@hr", OleDbType.Double);
                hrPu.Value = p.HoursRequired.HasValue ? (object)(double)p.HoursRequired.Value : DBNull.Value;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value    = p.Id;
                cmd.Parameters.Add("@wi", OleDbType.Integer).Value    = p.WorkspaceId;
                cmd.ExecuteNonQuery();
            }
        }

        // Returns list of (ProjectTitle, TotalHours) for charts
        public static List<(string Title, decimal Hours)> GetHoursPerProject(int workspaceId)
        {
            // IIF is Access SQL's equivalent of COALESCE: returns 0 when no events exist.
            // LEFT JOIN keeps projects with zero hours in the result set — an INNER JOIN would silently drop them.
            const string sql = @"SELECT p.Title, IIF(SUM(e.HoursLogged) IS NULL, 0, SUM(e.HoursLogged)) AS TotalHours
                FROM Projects p LEFT JOIN Events e ON p.Id = e.ProjectId
                WHERE p.WorkspaceId = ?
                GROUP BY p.Title
                ORDER BY p.Title";
            var list = new List<(string, decimal)>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add((r["Title"].ToString(), Convert.ToDecimal(r["TotalHours"])));
            }
            return list;
        }

        // Returns list of (ProjectTitle, VolunteerCount)
        public static List<(string Title, int Count)> GetVolunteersPerProject(int workspaceId)
        {
            const string sql = @"SELECT p.Title, COUNT(vp.UserId) AS VolCount
                FROM Projects p LEFT JOIN VolunteerProject vp ON p.Id = vp.ProjectId
                WHERE p.WorkspaceId = ?
                GROUP BY p.Title
                ORDER BY p.Title";
            var list = new List<(string, int)>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add((r["Title"].ToString(), Convert.ToInt32(r["VolCount"])));
            }
            return list;
        }

        public static int CountByWorkspace(int workspaceId)
        {
            const string sql = "SELECT COUNT(*) FROM Projects WHERE WorkspaceId = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@w", workspaceId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Deletes a project and all its associated events and volunteer enrollments.
        /// WorkspaceId is included as a security guard to prevent cross-workspace deletion.
        /// </summary>
        public static void Delete(int id, int workspaceId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                // Delete dependent events first
                using (var cmd = new OleDbCommand("DELETE FROM Events WHERE ProjectId = ?", conn))
                {
                    cmd.Parameters.Add("@pid", OleDbType.Integer).Value = id;
                    cmd.ExecuteNonQuery();
                }
                // Delete volunteer enrollment records
                using (var cmd = new OleDbCommand("DELETE FROM VolunteerProject WHERE ProjectId = ?", conn))
                {
                    cmd.Parameters.Add("@pid", OleDbType.Integer).Value = id;
                    cmd.ExecuteNonQuery();
                }
                // Delete the project itself (WorkspaceId guard prevents cross-workspace deletion)
                using (var cmd = new OleDbCommand("DELETE FROM Projects WHERE Id = ? AND WorkspaceId = ?", conn))
                {
                    cmd.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    cmd.Parameters.Add("@wi", OleDbType.Integer).Value = workspaceId;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
