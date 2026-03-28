using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class CreateAdmin : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindWorkspaces();
        }

        private void BindWorkspaces()
        {
            ddlWorkspace.Items.Clear();
            ddlWorkspace.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select Workspace --", ""));
            foreach (var ws in WorkspaceDAL.GetAll())
                if (ws.IsActive)
                    ddlWorkspace.Items.Add(new System.Web.UI.WebControls.ListItem(ws.Name, ws.Id.ToString()));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var email = txtEmail.Text.Trim().ToLowerInvariant();
            if (UserDAL.EmailExists(email))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Email is already registered.</div>";
                return;
            }

            int wsId;
            if (!int.TryParse(ddlWorkspace.SelectedValue, out wsId))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Please select a workspace.</div>";
                return;
            }

            var admin = new AppUser
            {
                FirstName    = txtFirstName.Text.Trim(),
                LastName     = txtLastName.Text.Trim(),
                Email        = email,
                PasswordHash = AuthHelper.HashPassword(txtPassword.Text),
                IsMale       = true,
                Role         = "Admin",
                WorkspaceId  = wsId,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            };
            UserDAL.Insert(admin);

            Response.Redirect("~/Pages/SuperAdmin/Admins.aspx", true);
        }
    }
}
