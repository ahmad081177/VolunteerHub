using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// A single volunteering session logged by a volunteer against a project.
    /// Named VHEvent (not Event) to avoid conflict with the C# 'event' keyword context.
    /// StartTime/EndTime are optional HH:MM strings. HoursLogged is the authoritative
    /// decimal value used for all calculations and reports.
    /// </summary>
    public class VHEvent
    {
        public int      Id           { get; set; }
        public int      UserId       { get; set; }
        public int      ProjectId    { get; set; }
        /// <summary>The calendar date on which the volunteer session took place.</summary>
        public DateTime EventDate    { get; set; }
        /// <summary>Optional start time string, e.g. "09:00". Null if not provided.</summary>
        public string   StartTime    { get; set; }
        /// <summary>Optional end time string, e.g. "11:30". Null if not provided.</summary>
        public string   EndTime      { get; set; }
        /// <summary>Decimal hours logged (e.g. 1.5 = 1 hour 30 min). Always > 0.</summary>
        public decimal  HoursLogged  { get; set; }
        public string   Notes        { get; set; }
        /// <summary>UTC timestamp when the event record was created.</summary>
        public DateTime LoggedAt     { get; set; }

        // ── Joined (not stored in DB – populated by DAL JOIN queries) ────────
        /// <summary>Project title from the Projects JOIN. Null when not joined.</summary>
        public string   ProjectTitle { get; set; }

        // ── Computed display helpers ──────────────────────────────────────────
        /// <summary>Formatted time range, e.g. "09:00 – 11:30". Returns "—" if no times were recorded.</summary>
        public string DurationDisplay
        {
            get
            {
                bool hasStart = !string.IsNullOrWhiteSpace(StartTime);
                bool hasEnd   = !string.IsNullOrWhiteSpace(EndTime);
                if (hasStart && hasEnd)  return StartTime + " – " + EndTime;
                if (hasStart)            return StartTime;
                if (hasEnd)              return EndTime;
                return "—";
            }
        }
    }
}
