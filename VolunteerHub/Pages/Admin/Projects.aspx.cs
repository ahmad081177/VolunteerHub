using System;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Admin
{
    public partial class Projects : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        protected void gvProjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Delete") return;
            int pid = Convert.ToInt32(e.CommandArgument);
            ProjectDAL.Delete(pid, CurrentWorkspaceId ?? 0);
            litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle-fill\"></i> Project deleted successfully.</div>";
            BindGrid();
        }

        private void BindGrid()
        {
            gvProjects.DataSource = ProjectDAL.GetByWorkspace(CurrentWorkspaceId ?? 0);
            gvProjects.DataBind();
        }
    }
}
