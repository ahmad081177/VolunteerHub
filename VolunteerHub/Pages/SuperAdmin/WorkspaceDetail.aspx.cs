using System;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class WorkspaceDetail : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        protected void gvAdmins_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Toggle")
            {
                var parts   = e.CommandArgument.ToString().Split(',');
                int uid     = int.Parse(parts[0]);
                bool current = bool.Parse(parts[1]);
                UserDAL.SetIsActive(uid, !current);
                litAlert.Text = $"<div class=\"vh-alert vh-alert-success\">Administrator {(current ? "deactivated" : "activated")} successfully.</div>";
            }
            else if (e.CommandName == "Delete")
            {
                int uid = int.Parse(e.CommandArgument.ToString());
                UserDAL.Delete(uid);
                litAlert.Text = "<div class=\"vh-alert vh-alert-success\">Administrator deleted.</div>";
            }
            BindData();
        }

        private void BindData()
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id))
            { Response.Redirect("~/Pages/SuperAdmin/Workspaces.aspx", true); return; }

            var ws = WorkspaceDAL.GetById(id);
            if (ws == null) { Response.Redirect("~/Pages/SuperAdmin/Workspaces.aspx", true); return; }

            pageTitle.InnerText     = ws.Name;
            breadcrumbName.InnerText = ws.Name;
            pageSubtitle.InnerText  = $"Code: {ws.Code}  •  {(ws.IsActive ? "Active" : "Inactive")}";

            // Admins for this workspace
            var allAdmins = UserDAL.GetAllAdmins();
            var wsAdmins  = allAdmins.FindAll(a => a.WorkspaceId == id);
            gvAdmins.DataSource = wsAdmins;
            gvAdmins.DataBind();

            var volunteers = UserDAL.GetAllByWorkspace(id);
            gvVolunteers.DataSource = volunteers;
            gvVolunteers.DataBind();

            statVolunteers.InnerText = volunteers.Count.ToString();
            statProjects.InnerText   = ProjectDAL.CountByWorkspace(id).ToString();
        }
    }
}
