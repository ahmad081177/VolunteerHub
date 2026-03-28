using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Volunteer read-only view of a single project.
    /// If not yet enrolled, shows a Join button.
    /// If enrolled, shows the volunteer's personal hour log and progress
    /// bar toward the required hours goal.
    /// </summary>
    public partial class ProjectDetail : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id))
            { Response.Redirect("~/Pages/Volunteer/Projects.aspx", true); return; }

            var p = ProjectDAL.GetById(id);
            // Project must belong to this volunteer's workspace
            if (p == null || p.WorkspaceId != (CurrentWorkspaceId ?? -1))
            { Response.Redirect("~/Pages/Volunteer/Projects.aspx", true); return; }

            int uid = CurrentUserId;

            // --- Page header ---
            pageTitle.InnerText       = p.Title;
            breadcrumbTitle.InnerText = p.Title;
            pageMeta.InnerText        = $"{p.Location ?? "No location"}  •  {p.StartDate:MMM dd, yyyy} – {p.EndDate:MMM dd, yyyy}";
            statusBadge.InnerText     = p.Status;
            statusBadge.Attributes["class"] = "vh-badge " + p.StatusBadgeClass;

            // --- Info panel ---
            infoStart.InnerText = p.StartDate.ToString("MMMM dd, yyyy");
            infoEnd.InnerText   = p.EndDate.ToString("MMMM dd, yyyy");
            if (!string.IsNullOrWhiteSpace(p.Location)) infoLoc.InnerText = p.Location;
            else infoLocRow.Visible = false;
            if (p.HoursRequired.HasValue) infoHrs.InnerText = p.HoursRequired.Value.ToString("0.#") + " hrs";
            else infoHrsRow.Visible = false;
            if (p.MaxVolunteers.HasValue) infoMax.InnerText = p.MaxVolunteers.Value.ToString();
            else infoMaxRow.Visible = false;

            // --- Description ---
            if (!string.IsNullOrWhiteSpace(p.Description)) descText.InnerText = p.Description;
            else descCard.Visible = false;

            // --- Enrollment state ---
            bool enrolled = VolunteerProjectDAL.IsEnrolled(uid, id);
            if (enrolled)
            {
                // Show my hours card
                myHoursCard.Visible = true;
                var myEvents = EventDAL.GetByUserAndProject(uid, id);
                gvMyEvents.DataSource = myEvents;
                gvMyEvents.DataBind();

                // Show progress toward required hours
                if (p.HoursRequired.HasValue)
                {
                    decimal totalHrs = EventDAL.GetTotalHoursByUser(uid);
                    // only this project's hours
                    decimal projHrs  = 0;
                    foreach (var ev in myEvents) projHrs += ev.HoursLogged;
                    int pct = (int)Math.Min(100, Math.Round(projHrs / p.HoursRequired.Value * 100));

                    progressCard.Visible     = true;
                    progressLabel.InnerText  = $"{projHrs:0.#} / {p.HoursRequired:0.#} hrs";
                    progressPct.InnerText    = $"{pct}%";
                    progressBar.Style["width"] = pct + "%";
                }
            }
            else if (p.Status != "Ended")
            {
                // Show join button only for not-yet-ended projects
                btnJoin.Visible = true;
            }
        }

        protected void btnJoin_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id)) return;

            // Guard: still within workspace bounds
            var p = ProjectDAL.GetById(id);
            if (p == null || p.WorkspaceId != (CurrentWorkspaceId ?? -1)) return;

            if (!VolunteerProjectDAL.IsEnrolled(CurrentUserId, id))
            {
                VolunteerProjectDAL.Enroll(CurrentUserId, id);
                litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> You've joined this project!</div>";
            }
            // Refresh page to reflect joined state
            BindData();
        }
    }
}
