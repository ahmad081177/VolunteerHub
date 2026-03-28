using System;
using System.Collections.Generic;
using System.Data.OleDb;
using VolunteerHub.Models;

namespace VolunteerHub.DAL
{
    public static class WorkspaceDAL
    {
        private static Workspace MapReader(OleDbDataReader r)
        {
            return new Workspace
            {
                Id         = Convert.ToInt32(r["Id"]),
                Name       = r["Name"]?.ToString(),
                Code       = r["Code"]?.ToString(),
                LogoPath   = r["LogoPath"] == DBNull.Value ? null : r["LogoPath"]?.ToString(),
                IsActive   = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]),
                CreatedAt  = Convert.ToDateTime(r["CreatedAt"])
            };
        }

        public static bool CodeExists(string code)
        {
            const string sql = "SELECT COUNT(*) FROM Workspaces WHERE Code = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@c", code);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static int Insert(Workspace w)
        {
            const string sql = "INSERT INTO Workspaces (Name, Code, LogoPath, IsActive, CreatedAt) VALUES (?,?,?,?,?)";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@n",  OleDbType.VarChar).Value = w.Name;
                cmd.Parameters.Add("@c",  OleDbType.VarChar).Value = w.Code;
                cmd.Parameters.Add("@lp", OleDbType.VarChar).Value = (object)w.LogoPath ?? DBNull.Value;
                cmd.Parameters.Add("@a",  OleDbType.Boolean).Value = w.IsActive;
                cmd.Parameters.Add("@ca", OleDbType.DBDate).Value  = w.CreatedAt;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT @@IDENTITY";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static List<Workspace> GetAll()
        {
            const string sql = "SELECT * FROM Workspaces ORDER BY Name";
            var list = new List<Workspace>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            using (var r    = cmd.ExecuteReader())
                while (r.Read()) list.Add(MapReader(r));
            return list;
        }

        public static Workspace GetById(int id)
        {
            const string sql = "SELECT * FROM Workspaces WHERE Id = ?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapReader(r) : null;
            }
        }

        public static void Update(Workspace w)
        {
            const string sql = "UPDATE Workspaces SET Name=?, Code=?, LogoPath=?, IsActive=? WHERE Id=?";
            using (var conn = DbHelper.GetConnection())
            using (var cmd  = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@n",  OleDbType.VarChar).Value  = w.Name;
                cmd.Parameters.Add("@c",  OleDbType.VarChar).Value  = w.Code;
                cmd.Parameters.Add("@lp", OleDbType.VarChar).Value  = (object)w.LogoPath ?? DBNull.Value;
                cmd.Parameters.Add("@a",  OleDbType.Boolean).Value  = w.IsActive;
                cmd.Parameters.Add("@id", OleDbType.Integer).Value  = w.Id;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
