namespace VolunteerHub.Helpers
{
    /// <summary>
    /// UI helper for project display. Lives in the web layer so that CSS class
    /// names stay out of the domain model (VHProject).
    /// </summary>
    public static class ProjectHelper
    {
        /// <summary>
        /// Returns the CSS badge class for a given project status string.
        /// </summary>
        public static string GetStatusBadgeClass(string status)
        {
            if (status == "Active")
            {
                return "vh-badge-active";
            }
            else if (status == "Upcoming")
            {
                return "vh-badge-upcoming";
            }
            else
            {
                return "vh-badge-ended";
            }
        }
    }
}
