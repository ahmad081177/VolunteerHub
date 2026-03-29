using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// A volunteering project belonging to a workspace.
    /// Named VHProject (not Project) to avoid ambiguity with other Project types.
    /// Status is derived at runtime from StartDate/EndDate — never stored in the DB.
    /// </summary>
    public class VHProject
    {
        public int      Id            { get; set; }
        public int      WorkspaceId   { get; set; }
        public string   Title         { get; set; }
        public string   Description   { get; set; }
        public string   Location      { get; set; }
        public DateTime StartDate     { get; set; }
        public DateTime EndDate       { get; set; }
        /// <summary>Optional cap on enrolled volunteers. NULL = no cap.</summary>
        public int?     MaxVolunteers { get; set; }
        /// <summary>Required volunteer hours to "complete" the project. NULL = untracked.</summary>
        public decimal? HoursRequired { get; set; }
        public DateTime CreatedAt     { get; set; }

        // ── Computed ─────────────────────────────
        /// <summary>"Upcoming", "Active", or "Ended" – derived from today's date vs Start/End.</summary>
        public string Status
        {
            get
            {
                var today = DateTime.Today;
                if (StartDate > today) return "Upcoming";
                if (EndDate   < today) return "Ended";
                return "Active";
            }
        }

    }
}
