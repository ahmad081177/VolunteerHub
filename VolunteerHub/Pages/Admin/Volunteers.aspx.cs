using System;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Admin
{
    /// <summary>
    /// Lists all Volunteer-role users in the admin's workspace.
    /// Admins can activate/deactivate volunteer accounts from here.
    /// </summary>
    public partial class Volunteers : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        /// <summary>Fetches all volunteers scoped to this admin's workspace.</summary>
        private void BindGrid()
        {
            gvVolunteers.DataSource = UserDAL.GetAllByWorkspace(CurrentWorkspaceId ?? 0);
            gvVolunteers.DataBind();
        }

        /// <summary>Handles Activate/Deactivate row command from the GridView.</summary>
        protected void gvVolunteers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Toggle") return;
            var parts    = e.CommandArgument.ToString().Split(',');
            int id       = int.Parse(parts[0]);
            bool current = bool.Parse(parts[1]);
            UserDAL.SetIsActive(id, !current);
            BindGrid();
        }
    }
}
