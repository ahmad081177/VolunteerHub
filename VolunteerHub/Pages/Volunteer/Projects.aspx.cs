using System.Collections.Generic;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Browse all projects in the volunteer's workspace.
    /// Volunteers can join projects directly from this page.
    /// Already-enrolled and ended projects show appropriate badges instead of the Join button.
    /// </summary>
    public partial class Projects : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack) BindProjects();
        }

        /// <summary>
        /// Loads workspace projects and enriches each with IsEnrolled = true/false
        /// so the view can show the correct action button.
        /// </summary>
        private void BindProjects()
        {
            int uid  = CurrentUserId;
            int wsId = CurrentWorkspaceId ?? 0;
            var projects = ProjectDAL.GetByWorkspace(wsId);
            var rows     = new List<ProjectCardRow>();

            foreach (var p in projects)
                rows.Add(new ProjectCardRow(p)
                {
                    IsEnrolled = VolunteerProjectDAL.IsEnrolled(uid, p.Id)
                });

            rptProjects.DataSource = rows;
            rptProjects.DataBind();
        }

        /// <summary>Handles the "Join" LinkButton inside the Repeater.</summary>
        protected void rptProjects_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Join") return;

            int projectId = int.Parse(e.CommandArgument.ToString());
            int uid       = CurrentUserId;

            // Double-check not already enrolled (race condition guard)
            if (!VolunteerProjectDAL.IsEnrolled(uid, projectId))
            {
                VolunteerProjectDAL.Enroll(uid, projectId);
                litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> You have successfully joined the project!</div>";
            }

            BindProjects(); // refresh grid to update button states
        }

        /// <summary>
        /// View-model that extends VHProject with the volunteer's enrollment status.
        /// Used so the ASPX data-binding can access IsEnrolled on the same object.
        /// </summary>
        private class ProjectCardRow : VHProject
        {
            public bool IsEnrolled { get; set; }
            public ProjectCardRow(VHProject src)
            {
                Id            = src.Id;
                WorkspaceId   = src.WorkspaceId;
                Title         = src.Title;
                Description   = src.Description;
                Location      = src.Location;
                StartDate     = src.StartDate;
                EndDate       = src.EndDate;
                MaxVolunteers = src.MaxVolunteers;
                HoursRequired = src.HoursRequired;
                CreatedAt     = src.CreatedAt;
            }
        }
    }
}
