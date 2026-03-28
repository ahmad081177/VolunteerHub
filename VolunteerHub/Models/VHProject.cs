using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// A volunteering project belonging to a workspace.
    /// Named VHProject (not Project) to avoid conflict with System.Threading.Tasks.Task etc.
    /// Status is derived at runtime from StartDate/EndDate — not stored in the DB.
    /// </summary>
    public class VHProject
    {
        public int      Id            { get; set; }
        public int      WorkspaceId   { get; set; }
        public string   Name          { get; set; }
        public string   Code          { get; set; }
        public DateTime StartDate     { get; set; }
        public DateTime EndDate       { get; set; }
        public string   Description   { get; set; }
        public double?  RequiredHours { get; set; }
        public DateTime CreatedAt     { get; set; }

        // Computed – never stored
        public string Status
        {
            get
            {
                var today = DateTime.Today;
                if (StartDate > today)  return "Upcoming";
                if (EndDate   < today)  return "Ended";
                return "Active";
            }
        }

        public string StatusBadgeClass
        {
            get
            {
                switch (Status)
                {
                    case "Active":   return "vh-badge-active";
                    case "Upcoming": return "vh-badge-upcoming";
                    default:         return "vh-badge-ended";
                }
            }
        }
    }
}
