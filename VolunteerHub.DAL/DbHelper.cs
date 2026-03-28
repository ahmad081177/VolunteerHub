using System.Configuration;
using System.Data.OleDb;

namespace VolunteerHub.DAL
{
    /// <summary>
    /// Provides a single factory method for obtaining an open OleDb connection.
    /// All DAL classes call GetConnection() inside a `using` block — the connection
    /// is opened here and closed automatically when the using block exits.
    /// Connection string name: "VolunteerHubDB" (defined in Web.config).
    /// </summary>
    public static class DbHelper
    {
        private static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["VolunteerHubDB"].ConnectionString;

        /// <summary>Returns an open OleDbConnection. Caller must dispose.</summary>
        public static OleDbConnection GetConnection()
        {
            var conn = new OleDbConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
