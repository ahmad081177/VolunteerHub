using System;
using System.Collections.Generic;
using System.Data.OleDb;
using VolunteerHub.Models;

namespace VolunteerHub.DAL
{
    public static class VolunteerProjectDAL
    {
        private static VolunteerProject MapReader(OleDbDataReader r)
        {
            return new VolunteerProject
            {
                Id           = Convert.ToInt32(r["Id"]),
                UserId       = Convert.ToInt32(r["UserId"]),
                ProjectId    = Convert.ToInt32(r["ProjectId"]),
                EnrolledAt   = Convert.ToDateTime(r["EnrolledAt"])
            };
        }

        public static bool IsEnrolled(int userId, int projectId)
        {
            const string sql = "SELECT COUNT(*) FROM VolunteerProject WHERE UserId=? AND ProjectId=?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@p", projectId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static void Enroll(int userId, int projectId)
        {
            const string sql = "INSERT INTO VolunteerProject (UserId, ProjectId, EnrolledAt) VALUES (?,?,?)";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@u", OleDbType.Integer).Value = userId;
                cmd.Parameters.Add("@p", OleDbType.Integer).Value = projectId;
                cmd.Parameters.Add("@e", OleDbType.DBDate).Value  = DateTime.UtcNow;
                cmd.ExecuteNonQuery();
            }
        }

        public static List<VolunteerProject> GetByUser(int userId)
        {
            const string sql = "SELECT * FROM VolunteerProject WHERE UserId = ? ORDER BY EnrolledAt DESC";
            var list = new List<VolunteerProject>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(MapReader(r));
            }
            return list;
        }

        public static List<VolunteerProject> GetByProject(int projectId)
        {
            const string sql = "SELECT * FROM VolunteerProject WHERE ProjectId = ? ORDER BY EnrolledAt DESC";
            var list = new List<VolunteerProject>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@p", projectId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(MapReader(r));
            }
            return list;
        }

        // Returns enrolled project IDs with total hours logged for a user
        public static List<(int ProjectId, decimal TotalHours)> GetProjectsWithHours(int userId)
        {
            // LEFT JOIN ensures projects where no events have been logged yet still appear
            // with TotalHours = 0, rather than being silently excluded by an INNER JOIN.
            // IIF guards against NULL from an empty SUM (Access returns NULL, not 0, for SUM of no rows).
            const string sql = @"SELECT vp.ProjectId,
                IIF(SUM(e.HoursLogged) IS NULL, 0, SUM(e.HoursLogged)) AS TotalHours
                FROM VolunteerProject vp
                LEFT JOIN Events e ON vp.ProjectId = e.ProjectId AND e.UserId = vp.UserId
                WHERE vp.UserId = ?
                GROUP BY vp.ProjectId";
            var list = new List<(int, decimal)>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add((Convert.ToInt32(r["ProjectId"]), Convert.ToDecimal(r["TotalHours"])));
            }
            return list;
        }

        public static int CountByProject(int projectId)
        {
            const string sql = "SELECT COUNT(*) FROM VolunteerProject WHERE ProjectId = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@p", projectId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
