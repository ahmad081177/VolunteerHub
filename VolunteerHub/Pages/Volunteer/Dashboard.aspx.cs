using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Volunteer dashboard — shows personal stats, hours-over-time chart,
    /// project list with progress, and recent event history.
    /// </summary>
    public partial class Dashboard : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            int uid = CurrentUserId;
            var user = UserDAL.GetById(uid);
            if (user != null)
                greeting.InnerText = $"Welcome back, {user.FirstName}!";

            // ---- Personal stats ----
            var events       = EventDAL.GetByUser(uid);
            var enrollments  = VolunteerProjectDAL.GetByUser(uid);
            decimal totalHrs = EventDAL.GetTotalHoursByUser(uid);

            statHours.InnerText   = totalHrs.ToString("0.#");
            statEvents.InnerText  = events.Count.ToString();
            statProjects.InnerText = enrollments.Count.ToString();

            // Count projects where the volunteer has logged ≥ the project's required hours
            int completed = 0;
            var hoursMap  = VolunteerProjectDAL.GetProjectsWithHours(uid);
            foreach (var (projId, hrs) in hoursMap)
            {
                var p = ProjectDAL.GetById(projId);
                if (p?.HoursRequired.HasValue == true && hrs >= p.HoursRequired.Value) completed++;
            }
            statCompleted.InnerText = completed.ToString();

            // ---- Projects mini-list (up to 5 most recent enrollments) ----
            var projectRows = new List<ProjectMiniRow>();
            foreach (var (projId, hrs) in hoursMap.Take(5))
            {
                var p = ProjectDAL.GetById(projId);
                if (p == null) continue;
                projectRows.Add(new ProjectMiniRow
                {
                    Title         = p.Title,
                    Status        = p.Status,
                    StatusBadgeClass = p.StatusBadgeClass,
                    HoursLogged   = hrs,
                    HoursRequired = p.HoursRequired
                });
            }
            rptProjects.DataSource = projectRows;
            rptProjects.DataBind();

            // ---- Recent 5 events (full history is on MyEvents page) ----
            gvEvents.DataSource = events.Take(5).ToList();
            gvEvents.DataBind();

            // ---- Chart: hours logged per month for the last 6 months ----
            // GetHoursOverTime returns (Month label, Total hours) pairs pre-filled for all 6 months
            var overtime = EventDAL.GetHoursOverTime(uid);
            var labels   = JsonConvert.SerializeObject(overtime.ConvertAll(x => x.Month));
            var data     = JsonConvert.SerializeObject(overtime.ConvertAll(x => x.Hours));
            litChartScript.Text = $"<script>VH.lineChart('chartHoursOverTime', {labels}, {data}, 'Hours');</script>";
        }

        /// <summary>Flat view-model for the projects mini-list repeater.</summary>
        private class ProjectMiniRow
        {
            public string   Title            { get; set; }
            public string   Status           { get; set; }
            public string   StatusBadgeClass { get; set; }
            public decimal  HoursLogged      { get; set; }
            public decimal? HoursRequired    { get; set; }
        }
    }
}
