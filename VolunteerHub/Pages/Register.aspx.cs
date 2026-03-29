using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;
using VolunteerHub.Models;

namespace VolunteerHub.Pages
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
                Response.Redirect("~/Pages/Login.aspx", true);

            if (!IsPostBack)
            {
                ddlWorkspaceCode.Items.Add(new ListItem("-- Select your workspace --", ""));
                foreach (var ws in WorkspaceDAL.GetAllActive())
                    ddlWorkspaceCode.Items.Add(new ListItem($"{ws.Name}  ({ws.Code})", ws.Code));
            }
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

            // Look up workspace by the selected dropdown code via direct SQL (bypasses C# YESNO conversion issues).
            var code = ddlWorkspaceCode.SelectedValue.Trim();
            var workspace = WorkspaceDAL.GetActiveByCode(code);

            if (workspace == null)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\"><i class=\"bi bi-x-circle-fill\"></i> Workspace not found or inactive. Please check with your administrator.</div>";
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
