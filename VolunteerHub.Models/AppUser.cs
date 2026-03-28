using System;

namespace VolunteerHub.Models
{
    /// <summary>
    /// Represents a user account. Role is one of: "SuperAdmin", "Admin", "Volunteer".
    /// SuperAdmin has no WorkspaceId. Admin and Volunteer are always scoped to one workspace.
    /// </summary>
    public class AppUser
    {
        public int      Id                    { get; set; }
        public string   FirstName             { get; set; }
        public string   LastName              { get; set; }
        public string   Email                 { get; set; }
        public string   PasswordHash          { get; set; }
        public bool     IsMale                { get; set; }
        public DateTime? DateOfBirth          { get; set; }
        public string   Phone                 { get; set; }
        public string   Address               { get; set; }
        public string   ImageProfilePath      { get; set; }
        public string   Role                  { get; set; }
        public int?     WorkspaceId           { get; set; }
        public bool     IsActive              { get; set; }
        public DateTime CreatedAt             { get; set; }
        public DateTime? LastLoginAt          { get; set; }
        public string   RememberMeToken       { get; set; }
        public DateTime? RememberMeTokenExpiry { get; set; }

        // Computed
        public string FullName => $"{FirstName} {LastName}";
        public string Initials => (FirstName?.Length > 0 ? FirstName[0].ToString() : "") +
                                  (LastName?.Length  > 0 ? LastName[0].ToString()  : "");
    }
}
