using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Admin
{
    public partial class VolunteerDetail : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id))
            { Response.Redirect("~/Pages/Admin/Volunteers.aspx", true); return; }

            var user = UserDAL.GetById(id);
            if (user == null || user.WorkspaceId != CurrentWorkspaceId || user.Role != "Volunteer")
            { Response.Redirect("~/Pages/Admin/Volunteers.aspx", true); return; }

            // Profile header
            profileAvatar.InnerText  = user.Initials;
            profileName.InnerText    = user.FullName;
            profileEmail.InnerText   = user.Email;
            breadcrumbName.InnerText = user.FullName;
            profileJoined.InnerText  = $"Joined {user.CreatedAt:MMM dd, yyyy}";
            profileStatus.InnerText  = user.IsActive ? "Active" : "Inactive";
            profileStatus.Attributes["class"] = user.IsActive ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted";
            lnkExportExcel.HRef      = ResolveUrl($"~/Helpers/ExportHandler.ashx?type=volunteer&id={id}");

            // Metrics
            var events      = EventDAL.GetByUser(id);
            var hoursMap    = VolunteerProjectDAL.GetProjectsWithHours(id);
            decimal total   = EventDAL.GetTotalHoursByUser(id);

            // Count completed projects (hours logged >= required)
            int completed = 0;
            foreach (var (projId, hrs) in hoursMap)
            {
                var p = ProjectDAL.GetById(projId);
                if (p?.HoursRequired.HasValue == true && hrs >= p.HoursRequired.Value) completed++;
            }

            statTotalHours.InnerText = total.ToString("0.#");
            statProjects.InnerText   = hoursMap.Count.ToString();
            statEvents.InnerText     = events.Count.ToString();
            statCompleted.InnerText  = completed.ToString();

            // Project progress list
            var projectRows = new List<ProjectProgressRow>();
            foreach (var (projId, hrs) in hoursMap)
            {
                var p = ProjectDAL.GetById(projId);
                if (p == null) continue;
                decimal pct = p.HoursRequired.HasValue && p.HoursRequired > 0
                    ? Math.Min(100, Math.Round(hrs / p.HoursRequired.Value * 100))
                    : 0;
                projectRows.Add(new ProjectProgressRow
                {
                    UserId        = id,
                    ProjectId     = projId,
                    Title         = p.Title,
                    HoursLogged   = hrs,
                    HoursRequired = p.HoursRequired.HasValue ? p.HoursRequired.Value.ToString("0.#") : null,
                    ProgressPct   = (int)pct
                });
            }
            rptProjects.DataSource = projectRows;
            rptProjects.DataBind();

            // Recent events (5) in the dashboard card; link to VolunteerDetail with full list
            gvEvents.DataSource = events.Take(5).ToList();
            gvEvents.DataBind();

            // "View Full History" links to Volunteers.aspx → VolunteerDetail (same page full list)
            // We show full list below a certain count, or via the View All link
            lnkViewAll.Visible = events.Count > 5;
            lnkViewAll.NavigateUrl = $"~/Pages/Admin/VolunteerDetail.aspx?id={id}&full=1";
            if (Request.QueryString["full"] == "1")
            {
                gvEvents.DataSource = events;
                gvEvents.DataBind();
                lnkViewAll.Visible = false;
            }

            // Hours over time chart
            var overtime = EventDAL.GetHoursOverTime(id);
            bool hasData = overtime.Any(x => x.Hours > 0);
            pnlChart.Visible      = hasData;
            pnlChartEmpty.Visible = !hasData;

            var sb = new StringBuilder("<script>\n");
            if (hasData)
            {
                var labels = JsonConvert.SerializeObject(overtime.ConvertAll(x => x.Month));
                var data   = JsonConvert.SerializeObject(overtime.ConvertAll(x => x.Hours));
                sb.AppendLine($"VH.lineChart('chartHoursOverTime', {labels}, {data}, '#6366F1');");
            }

            // Volunteer comparison charts (workspace-wide)
            int wsId = user.WorkspaceId ?? 0;
            var volHrsData  = UserDAL.GetHoursPerVolunteer(wsId);
            var volProjData = UserDAL.GetProjectsPerVolunteer(wsId);

            if (volHrsData.Count > 0)
            {
                pnlVolHrsChart.Visible = true;  pnlVolHrsEmpty.Visible = false;
                var labels = JsonConvert.SerializeObject(volHrsData.ConvertAll(x => x.FullName));
                var data   = JsonConvert.SerializeObject(volHrsData.ConvertAll(x => x.Hours));
                sb.AppendLine($"VH.barChart('chartHoursPerVolunteer', {labels}, {data}, '#10B981');");
            }
            else
            {
                pnlVolHrsChart.Visible = false; pnlVolHrsEmpty.Visible = true;
            }
            if (volProjData.Count > 0)
            {
                pnlVolProjChart.Visible = true;  pnlVolProjEmpty.Visible = false;
                var labels = JsonConvert.SerializeObject(volProjData.ConvertAll(x => x.FullName));
                var data   = JsonConvert.SerializeObject(volProjData.ConvertAll(x => x.Count));
                sb.AppendLine($"VH.barChart('chartProjectsPerVolunteer', {labels}, {data}, '#F59E0B');");
            }
            else
            {
                pnlVolProjChart.Visible = false; pnlVolProjEmpty.Visible = true;
            }

            sb.AppendLine("</script>");
            litChartScript.Text = sb.ToString();
        }

        private class ProjectProgressRow
        {
            public int    UserId        { get; set; }
            public int    ProjectId     { get; set; }
            public string Title         { get; set; }
            public decimal HoursLogged  { get; set; }
            public string HoursRequired { get; set; }  // null = unlimited
            public int    ProgressPct   { get; set; }
        }
    }
}
