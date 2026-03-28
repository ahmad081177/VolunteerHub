using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// A single volunteering session logged by a volunteer against a project.
    /// Named VHEvent (not Event) to avoid conflict with the C# 'event' keyword context.
    /// StartTime/EndTime are stored as HH:MM strings (optional). HoursLogged is the
    /// authoritative duration used for all calculations.
    /// </summary>
    public class VHEvent
    {
        public int      Id              { get; set; }
        public int      UserId          { get; set; }
        public int      ProjectId       { get; set; }
        public DateTime StartDateTime   { get; set; }
        public DateTime EndDateTime     { get; set; }
        public int      DurationMinutes { get; set; }
        public string   Location        { get; set; }
        public string   Description     { get; set; }
        public DateTime CreatedAt       { get; set; }

        // Joined (not in DB)
        public string ProjectName { get; set; }
        public string VolunteerName { get; set; }

        // Computed
        public double DurationHours => Math.Round(DurationMinutes / 60.0, 2);
        public string DurationDisplay
        {
            get
            {
                int h = DurationMinutes / 60;
                int m = DurationMinutes % 60;
                return h > 0 ? $"{h}h {m}m" : $"{m}m";
            }
        }
    }
}
