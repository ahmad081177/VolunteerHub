using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class EditAdmin : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id;
                if (!int.TryParse(Request.QueryString["id"], out id))
                { Response.Redirect("~/Pages/SuperAdmin/Admins.aspx", true); return; }

                var admin = UserDAL.GetById(id);
                if (admin == null || admin.Role != "Admin")
                { Response.Redirect("~/Pages/SuperAdmin/Admins.aspx", true); return; }

                hfId.Value         = admin.Id.ToString();
                txtFirstName.Text  = admin.FirstName;
                txtLastName.Text   = admin.LastName;
                txtEmail.Text      = admin.Email;
                chkActive.Checked  = admin.IsActive;

                BindWorkspaces();
                if (admin.WorkspaceId.HasValue)
                    ddlWorkspace.SelectedValue = admin.WorkspaceId.Value.ToString();
            }
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

            int id = int.Parse(hfId.Value);

            var email = txtEmail.Text.Trim().ToLowerInvariant();

            // Block if email is taken by a *different* user
            if (UserDAL.EmailExists(email, id))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">This email is already registered to another account.</div>";
                return;
            }

            int wsId;
            if (!int.TryParse(ddlWorkspace.SelectedValue, out wsId))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Please select a workspace.</div>";
                return;
            }

            var updated = new AppUser
            {
                Id          = id,
                FirstName   = txtFirstName.Text.Trim(),
                LastName    = txtLastName.Text.Trim(),
                Email       = email,
                WorkspaceId = wsId,
                IsActive    = chkActive.Checked
            };
            UserDAL.UpdateAdmin(updated);

            Response.Redirect("~/Pages/SuperAdmin/Admins.aspx?saved=1", true);
        }
    }
}
