using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// A workspace represents a school or organisation.
    /// Volunteers and admins are always associated with exactly one workspace.
    /// The workspace Code is the join code volunteers enter during registration.
    /// </summary>
    public class Workspace
    {
        public int      Id        { get; set; }
        public string   Name      { get; set; }
        public string   Code      { get; set; }
        public string   LogoPath  { get; set; }
        /// <summary>False = workspace is hidden from volunteers and cannot be joined.</summary>
        public bool     IsActive  { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
