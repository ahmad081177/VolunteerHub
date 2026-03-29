using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace VolunteerHub.DAL
{
    public static class EventImageDAL
    {
        /// <summary>Inserts one image record for an event. SortOrder determines display order (0-based).</summary>
        public static void Insert(int eventId, string imagePath, int sortOrder)
        {
            const string sql = "INSERT INTO EventImages (EventId, ImagePath, SortOrder, UploadedAt) VALUES (?,?,?,?)";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@eid", OleDbType.Integer).Value  = eventId;
                cmd.Parameters.Add("@ip",  OleDbType.VarChar).Value  = imagePath;
                cmd.Parameters.Add("@so",  OleDbType.Integer).Value  = sortOrder;
                cmd.Parameters.Add("@ua",  OleDbType.DBDate).Value   = DateTime.UtcNow;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Returns all image paths for a given event, ordered by SortOrder.</summary>
        public static List<string> GetByEvent(int eventId)
        {
            const string sql = "SELECT ImagePath FROM EventImages WHERE EventId = ? ORDER BY SortOrder";
            var list = new List<string>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@eid", eventId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(r["ImagePath"].ToString());
            }
            return list;
        }

        /// <summary>Deletes all image records for an event (used before re-saving or on event delete).</summary>
        public static void DeleteByEvent(int eventId)
        {
            const string sql = "DELETE FROM EventImages WHERE EventId = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@eid", OleDbType.Integer).Value = eventId;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Deletes one specific image record for an event by its stored path.</summary>
        public static void DeleteByPath(int eventId, string imagePath)
        {
            const string sql = "DELETE FROM EventImages WHERE EventId = ? AND ImagePath = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@eid", OleDbType.Integer).Value = eventId;
                cmd.Parameters.Add("@ip",  OleDbType.VarChar).Value  = imagePath;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Returns true if the EventImages table exists (used for graceful fallback on unpatched DBs).</summary>
        public static bool TableExists()
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                using (var cmd  = new OleDbCommand("SELECT TOP 1 Id FROM EventImages", conn))
                {
                    cmd.ExecuteScalar();
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
