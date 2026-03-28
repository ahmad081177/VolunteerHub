using System;
using System.Web;
using System.Web.UI;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;
using VolunteerHub.Models;

namespace VolunteerHub.Pages
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["UserId"] != null)
                Response.Redirect("~/Pages/Login.aspx", true);
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var email = txtEmail.Text.Trim().ToLowerInvariant();

            if (UserDAL.EmailExists(email))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\"><i class=\"bi bi-x-circle\"></i> An account with this email already exists.</div>";
                return;
            }

            // Look up the entered code against all active workspaces.
            // We fetch all then compare in C# so the match is always case-insensitive,
            // regardless of the Access database's collation setting.
            var code = txtWorkspaceCode.Text.Trim().ToUpperInvariant();
            var allWs = WorkspaceDAL.GetAll();
            Models.Workspace workspace = null;
            foreach (var w in allWs)
                if (string.Equals(w.Code, code, StringComparison.OrdinalIgnoreCase) && w.IsActive)
                { workspace = w; break; }

            if (workspace == null)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\"><i class=\"bi bi-x-circle\"></i> Workspace code not found or inactive. Please check with your administrator.</div>";
                return;
            }

            var user = new AppUser
            {
                FirstName    = txtFirstName.Text.Trim(),
                LastName     = txtLastName.Text.Trim(),
                Email        = email,
                PasswordHash = AuthHelper.HashPassword(txtPassword.Text),
                IsMale       = ddlGender.SelectedValue == "true",
                DateOfBirth  = DateTime.TryParse(txtDob.Text, out var dob) ? (DateTime?)dob : null,
                Role        = "Volunteer",   // self-registration is always Volunteer; admins are created by SuperAdmin only
                WorkspaceId = workspace.Id,
                IsActive    = true,           // new accounts are active immediately
                CreatedAt   = DateTime.UtcNow
            };

            UserDAL.Insert(user);

            litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> Account created! <a href=\"Login.aspx\">Sign in now</a></div>";
        }
    }
}
