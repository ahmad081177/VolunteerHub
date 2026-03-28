using System.Collections.Generic;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Shows only the projects the signed-in volunteer has enrolled in,
    /// with their individual progress (hours logged vs required).
    /// </summary>
    public partial class MyProjects : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack) BindProjects();
        }

        private void BindProjects()
        {
            int uid   = CurrentUserId;
            // Get (projectId, totalHours) map for this volunteer
            var hoursMap = VolunteerProjectDAL.GetProjectsWithHours(uid);
            var rows     = new List<MyProjectRow>();

            foreach (var (projId, hrs) in hoursMap)
            {
                var p = ProjectDAL.GetById(projId);
                if (p == null) continue;

                // Calculate progress percentage toward the required hours goal
                int pct = p.HoursRequired.HasValue && p.HoursRequired > 0
                          ? (int)System.Math.Min(100, System.Math.Round(hrs / p.HoursRequired.Value * 100))
                          : 0;

                rows.Add(new MyProjectRow(p) { HoursLogged = hrs, ProgressPct = pct });
            }
            rptProjects.DataSource = rows;
            rptProjects.DataBind();
        }

        /// <summary>
        /// Extends VHProject with the volunteer's hours and progress percentage.
        /// </summary>
        private class MyProjectRow : VHProject
        {
            public decimal HoursLogged  { get; set; }
            public int     ProgressPct  { get; set; }
            public MyProjectRow(VHProject src)
            {
                Id = src.Id; WorkspaceId = src.WorkspaceId; Title = src.Title;
                Description = src.Description; Location = src.Location;
                StartDate = src.StartDate; EndDate = src.EndDate;
                MaxVolunteers = src.MaxVolunteers; HoursRequired = src.HoursRequired;
                CreatedAt = src.CreatedAt;
            }
        }
    }
}
