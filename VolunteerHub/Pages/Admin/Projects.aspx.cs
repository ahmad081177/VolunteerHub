using System;
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

        private void BindGrid()
        {
            gvProjects.DataSource = ProjectDAL.GetByWorkspace(CurrentWorkspaceId ?? 0);
            gvProjects.DataBind();
        }
    }
}
