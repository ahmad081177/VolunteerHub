using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class CreateWorkspace : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var code = txtCode.Text.Trim().ToUpperInvariant();
            if (WorkspaceDAL.CodeExists(code))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">This code is already taken. Choose a different one.</div>";
                return;
            }

            string logoPath = null;
            if (fuLogo.HasFile)
            {
                logoPath = ImageHelper.SaveUpload(fuLogo.PostedFile, "WorkspaceLogos");
                if (logoPath == null)
                {
                    litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid image. Use JPG/PNG/GIF under 2 MB.</div>";
                    return;
                }
            }

            var ws = new Workspace
            {
                Name      = txtName.Text.Trim(),
                Code      = code,
                LogoPath  = logoPath,
                IsActive  = chkActive.Checked,
                CreatedAt = DateTime.UtcNow
            };
            WorkspaceDAL.Insert(ws);

            Response.Redirect("~/Pages/SuperAdmin/Workspaces.aspx", true);
        }
    }
}
