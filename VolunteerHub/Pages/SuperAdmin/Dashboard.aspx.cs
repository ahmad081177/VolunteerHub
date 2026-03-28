using System;
using System.Linq;
using System.Web.UI;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class Dashboard : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            var workspaces = WorkspaceDAL.GetAll();
            var admins     = UserDAL.GetAllAdmins();

            statWorkspaces.InnerText = workspaces.Count(w => w.IsActive).ToString();
            statAdmins.InnerText     = admins.Count.ToString();

            int totalVols = 0, totalProjects = 0;
            foreach (var ws in workspaces)
            {
                totalVols     += UserDAL.GetAllByWorkspace(ws.Id).Count;
                totalProjects += ProjectDAL.CountByWorkspace(ws.Id);
            }
            statVolunteers.InnerText = totalVols.ToString();
            statProjects.InnerText   = totalProjects.ToString();

            gvWorkspaces.DataSource = workspaces;
            gvWorkspaces.DataBind();
        }

        protected void gvWorkspaces_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e) { }
    }
}
