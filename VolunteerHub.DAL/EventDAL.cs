using System;
using System.Collections.Generic;
using System.Data.OleDb;
using VolunteerHub.Models;

namespace VolunteerHub.DAL
{
    public static class EventDAL
    {
        private static VHEvent MapReader(OleDbDataReader r)
        {
            return new VHEvent
            {
                Id           = Convert.ToInt32(r["Id"]),
                UserId       = Convert.ToInt32(r["UserId"]),
                ProjectId    = Convert.ToInt32(r["ProjectId"]),
                EventDate    = Convert.ToDateTime(r["EventDate"]),
                StartTime    = r["StartTime"]?.ToString(),
                EndTime      = r["EndTime"]?.ToString(),
                HoursLogged  = Convert.ToDecimal(r["HoursLogged"]),
                Notes        = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
                LoggedAt     = Convert.ToDateTime(r["LoggedAt"]),
                ProjectTitle = r.GetSchemaTable() != null && HasColumn(r, "ProjectTitle")
                               ? (r["ProjectTitle"] == DBNull.Value ? null : r["ProjectTitle"]?.ToString())
                               : null
            };
        }

        private static bool HasColumn(OleDbDataReader r, string col)
        {
            // Some queries don't JOIN Projects, so "ProjectTitle" may not be in the result set.
            // Accessing a missing column throws IndexOutOfRangeException, so we probe the schema
            // first in MapReader before trying to read a potentially absent column.
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), col, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static int Insert(VHEvent e)
        {
            const string sql = @"INSERT INTO Events
                (UserId, ProjectId, EventDate, StartTime, EndTime, HoursLogged, Notes, LoggedAt)
                VALUES (?,?,?,?,?,?,?,?)";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@uid", e.UserId);
                cmd.Parameters.AddWithValue("@pid", e.ProjectId);
                cmd.Parameters.AddWithValue("@ed",  e.EventDate);
                cmd.Parameters.AddWithValue("@st",  (object)e.StartTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@et",  (object)e.EndTime   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@hl",  e.HoursLogged);
                cmd.Parameters.AddWithValue("@nt",  (object)e.Notes     ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@la",  e.LoggedAt);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT @@IDENTITY";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static List<VHEvent> GetByUser(int userId)
        {
            const string sql = @"SELECT e.*, p.Title AS ProjectTitle
                FROM Events e INNER JOIN Projects p ON e.ProjectId = p.Id
                WHERE e.UserId = ?
                ORDER BY e.EventDate DESC";
            var list = new List<VHEvent>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(MapReader(r));
            }
            return list;
        }

        public static List<VHEvent> GetByUserAndProject(int userId, int projectId)
        {
            const string sql = @"SELECT e.*, p.Title AS ProjectTitle
                FROM Events e INNER JOIN Projects p ON e.ProjectId = p.Id
                WHERE e.UserId = ? AND e.ProjectId = ?
                ORDER BY e.EventDate DESC";
            var list = new List<VHEvent>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@p", projectId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(MapReader(r));
            }
            return list;
        }

        public static decimal GetTotalHoursByUser(int userId)
        {
            const string sql = "SELECT IIF(SUM(HoursLogged) IS NULL, 0, SUM(HoursLogged)) FROM Events WHERE UserId = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public static decimal GetTotalHoursByProject(int projectId)
        {
            const string sql = "SELECT IIF(SUM(HoursLogged) IS NULL, 0, SUM(HoursLogged)) FROM Events WHERE ProjectId = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@p", projectId);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        // Returns last 6 months of hours per month for a user (for line chart)
        public static List<(string Month, decimal Hours)> GetHoursOverTime(int userId)
        {
            // Access SQL can't GROUP BY a formatted date string, so we fetch individual rows
            // for the last 6 months and aggregate by month key in C# instead.
            const string sql = @"SELECT EventDate, HoursLogged FROM Events
                WHERE UserId = ? AND EventDate >= ?
                ORDER BY EventDate";
            // First day of the month 5 months ago — always gives exactly 6 calendar months
            var cutoff = DateTime.Today.AddMonths(-5).AddDays(1 - DateTime.Today.Day);
            var monthly = new Dictionary<string, decimal>();
            // Pre-fill all 6 month slots with 0 so the chart always shows 6 labelled bars,
            // even when the user has no events in some of those months.
            for (int i = 0; i < 6; i++)
            {
                var m = cutoff.AddMonths(i);
                monthly[m.ToString("MMM yy")] = 0m;
            }
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@d", cutoff);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        var key = Convert.ToDateTime(r["EventDate"]).ToString("MMM yy");
                        if (monthly.ContainsKey(key))
                            monthly[key] += Convert.ToDecimal(r["HoursLogged"]);
                    }
            }
            var list = new List<(string, decimal)>();
            foreach (var kv in monthly) list.Add((kv.Key, kv.Value));
            return list;
        }

        public static void Update(VHEvent e)
        {
            const string sql = @"UPDATE Events
                SET EventDate=?, StartTime=?, EndTime=?, HoursLogged=?, Notes=?
                WHERE Id=? AND UserId=?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ed",  e.EventDate);
                cmd.Parameters.AddWithValue("@st",  (object)e.StartTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@et",  (object)e.EndTime   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@hl",  e.HoursLogged);
                cmd.Parameters.AddWithValue("@nt",  (object)e.Notes     ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id",  e.Id);
                cmd.Parameters.AddWithValue("@uid", e.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id, int userId)
        {
            // userId in the WHERE clause prevents IDOR: a volunteer can only delete their own
            // events, even if they craft a request containing someone else's event ID.
            const string sql = "DELETE FROM Events WHERE Id=? AND UserId=?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id",  id);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static VHEvent GetById(int id)
        {
            const string sql = @"SELECT e.*, p.Title AS ProjectTitle FROM Events e
                INNER JOIN Projects p ON e.ProjectId = p.Id WHERE e.Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapReader(r) : null;
            }
        }
    }
}
