using System;
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
