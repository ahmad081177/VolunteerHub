using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// Pivot/junction record linking a volunteer (AppUser) to a VHProject.
    /// Created when a volunteer clicks "Join" on a project.
    /// One row per (UserId, ProjectId) pair — re-enrollment is not allowed.
    /// </summary>
    public class VolunteerProject
    {
        public int      Id         { get; set; }
        public int      UserId     { get; set; }
        public int      ProjectId  { get; set; }
        /// <summary>UTC timestamp of when the volunteer joined the project.</summary>
        public DateTime EnrolledAt { get; set; }
    }
}
