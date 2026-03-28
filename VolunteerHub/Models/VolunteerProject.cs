using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// Pivot/junction record linking a volunteer (AppUser) to a VHProject.
    /// Created when a volunteer clicks Join on a project. One row per enrollment.
    /// </summary>
    public class VolunteerProject
    {
        public int      Id        { get; set; }
        public int      UserId    { get; set; }
        public int      ProjectId { get; set; }
        public DateTime JoinedAt  { get; set; }

        // Joined / computed (not in DB)
        public string ProjectName    { get; set; }
        public string ProjectCode    { get; set; }
        public double TotalHours     { get; set; }
        public double? RequiredHours { get; set; }
        public double RemainingHours => (RequiredHours ?? 0) - TotalHours < 0
                                        ? 0 : (RequiredHours ?? 0) - TotalHours;
        public int ProgressPct       => RequiredHours.HasValue && RequiredHours.Value > 0
                                        ? (int)Math.Min(100, (TotalHours / RequiredHours.Value) * 100)
                                        : 100;
    }
}
