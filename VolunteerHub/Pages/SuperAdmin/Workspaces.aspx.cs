using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class Workspaces : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvWorkspaces.DataSource = WorkspaceDAL.GetAll();
            gvWorkspaces.DataBind();
        }
    }
}
