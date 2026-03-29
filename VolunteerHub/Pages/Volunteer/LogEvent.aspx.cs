using System;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Form to log a volunteering event (date + hours + optional start/end times + notes).
    /// The project dropdown is populated only with projects the volunteer has enrolled in.
    /// If ?projectId= is in the query string, that project is pre-selected.
    /// </summary>
    public partial class LogEvent : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProjects();
                // Pre-fill today's date
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Populates the project dropdown with ALL active/upcoming projects in the volunteer's workspace.
        /// (Enrollment is not required to log hours — volunteers log against any project they worked on.)
        /// Pre-selects the project if ?projectId= is present in the URL.
        /// </summary>
        private void BindProjects()
        {
            int wsId = CurrentWorkspaceId ?? 0;
            var projects = ProjectDAL.GetByWorkspace(wsId);

            ddlProject.Items.Clear();
            ddlProject.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select Project --", ""));

            foreach (var p in projects)
            {
                if (p.Status == "Ended") continue;   // hide fully-ended projects
                ddlProject.Items.Add(new System.Web.UI.WebControls.ListItem(p.Title, p.Id.ToString()));
            }

            if (ddlProject.Items.Count == 1)   // only placeholder
                litAlert.Text = "<div class=\"vh-alert vh-alert-info\"><i class=\"bi bi-info-circle\"></i> No active projects found in your workspace yet.</div>";

            // Pre-select from query string
            var qsId = Request.QueryString["projectId"];
            if (!string.IsNullOrEmpty(qsId))
            {
                var item = ddlProject.Items.FindByValue(qsId);
                if (item != null) item.Selected = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int projectId;
            if (!int.TryParse(ddlProject.SelectedValue, out projectId))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Please select a project.</div>";
                return;
            }

            // Server-side: just verify the project belongs to this volunteer's workspace
            var proj = ProjectDAL.GetById(projectId);
            if (proj == null || proj.WorkspaceId != (CurrentWorkspaceId ?? 0))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid project selection.</div>";
                return;
            }

            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, out date))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid date.</div>";
                return;
            }

            // Future date guard — prevents logging hours for work that hasn't happened yet
            if (date > DateTime.Today)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-warning\">You cannot log hours for a future date.</div>";
                return;
            }

            // Hours are derived from start/end times (not manually entered)
            string startTime = txtStartTime.Text.Trim();
            string endTime   = txtEndTime.Text.Trim();
            decimal hours    = CalculateHours(startTime, endTime);
            if (hours <= 0)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">End time must be after start time.</div>";
                return;
            }

            var ev = new VHEvent
            {
                UserId      = CurrentUserId,
                ProjectId   = projectId,
                EventDate   = date,
                StartTime   = startTime,
                EndTime     = endTime,
                HoursLogged = hours,
                Notes       = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                LoggedAt    = DateTime.UtcNow
            };
            int eventId = EventDAL.Insert(ev);

            // Auto-enroll the volunteer if they haven't formally joined the project yet
            // (so Admin/ProjectDetail shows all participants, not just those who clicked "Join")
            if (!VolunteerProjectDAL.IsEnrolled(CurrentUserId, projectId))
                VolunteerProjectDAL.Enroll(CurrentUserId, projectId);

            // Save uploaded images (up to 5) if the EventImages table exists
            if (EventImageDAL.TableExists())
            {
                var files = Request.Files;
                int saved = 0;
                for (int i = 0; i < files.Count && saved < 5; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength == 0) continue;
                    try
                    {
                        string path = ImageHelper.SaveUpload(file, "EventImages");
                        if (path != null)
                        {
                            EventImageDAL.Insert(eventId, path, saved);
                            saved++;
                        }
                    }
                    catch { /* skip invalid files silently */ }
                }
            }

            Response.Redirect("~/Pages/Volunteer/MyEvents.aspx?success=1", true);
        }

        /// <summary>Calculates decimal hours from two "HH:MM" strings. Returns 0 if invalid or end &lt;= start.</summary>
        private static decimal CalculateHours(string start, string end)
        {
            try
            {
                var sp = start.Split(':');
                var ep = end.Split(':');
                int startMin = int.Parse(sp[0]) * 60 + int.Parse(sp[1]);
                int endMin   = int.Parse(ep[0]) * 60 + int.Parse(ep[1]);
                int diff = endMin - startMin;
                return diff > 0 ? Math.Round((decimal)diff / 60, 2) : 0m;
            }
            catch { return 0m; }
        }
    }
}
