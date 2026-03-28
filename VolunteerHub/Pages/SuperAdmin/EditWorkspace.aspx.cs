using System;
using System.Web.UI;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class EditWorkspace : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id;
                if (!int.TryParse(Request.QueryString["id"], out id))
                { Response.Redirect("~/Pages/SuperAdmin/Workspaces.aspx", true); return; }

                var ws = WorkspaceDAL.GetById(id);
                if (ws == null) { Response.Redirect("~/Pages/SuperAdmin/Workspaces.aspx", true); return; }

                hfId.Value       = ws.Id.ToString();
                txtName.Text     = ws.Name;
                txtCode.Text     = ws.Code;
                chkActive.Checked = ws.IsActive;

                if (!string.IsNullOrEmpty(ws.LogoPath))
                    currentLogo.InnerHtml = $"<img src=\"{ResolveUrl("~/" + ws.LogoPath)}\" class=\"vh-img-thumbnail\" alt=\"Current logo\" />";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int id = int.Parse(hfId.Value);
            var ws = WorkspaceDAL.GetById(id);
            if (ws == null) return;

            var code = txtCode.Text.Trim().ToUpperInvariant();
            if (!code.Equals(ws.Code, StringComparison.OrdinalIgnoreCase) && WorkspaceDAL.CodeExists(code))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">This code is already taken.</div>";
                return;
            }

            if (fuLogo.HasFile)
            {
                var path = ImageHelper.SaveUpload(fuLogo.PostedFile, "WorkspaceLogos");
                if (path == null) { litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid image file.</div>"; return; }
                ws.LogoPath = path;
            }

            ws.Name     = txtName.Text.Trim();
            ws.Code     = code;
            ws.IsActive = chkActive.Checked;
            WorkspaceDAL.Update(ws);

            Response.Redirect("~/Pages/SuperAdmin/Workspaces.aspx", true);
        }
    }
}
