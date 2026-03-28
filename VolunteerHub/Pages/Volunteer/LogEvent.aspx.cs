using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
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
        /// Populates the project dropdown with projects the volunteer is enrolled in.
        /// Pre-selects the project if ?projectId= is present in the URL.
        /// </summary>
        private void BindProjects()
        {
            int uid  = CurrentUserId;
            var hoursMap = VolunteerProjectDAL.GetProjectsWithHours(uid);

            ddlProject.Items.Clear();
            ddlProject.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select Project --", ""));

            foreach (var (projId, _) in hoursMap)
            {
                var p = ProjectDAL.GetById(projId);
                if (p == null || p.Status == "Ended") continue;
                ddlProject.Items.Add(new System.Web.UI.WebControls.ListItem(p.Title, p.Id.ToString()));
            }

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

            // Server-side enrollment check — verifies the volunteer is genuinely enrolled
            // in this project even if the browser payload was tampered with (forged POST).
            if (!VolunteerProjectDAL.IsEnrolled(CurrentUserId, projectId))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">You are not enrolled in this project.</div>";
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

            decimal hours;
            if (!decimal.TryParse(txtHours.Text, out hours) || hours <= 0 || hours > 24)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid hours value.</div>";
                return;
            }

            var ev = new VHEvent
            {
                UserId      = CurrentUserId,
                ProjectId   = projectId,
                EventDate   = date,
                StartTime   = string.IsNullOrWhiteSpace(txtStartTime.Text) ? null : txtStartTime.Text,
                EndTime     = string.IsNullOrWhiteSpace(txtEndTime.Text)   ? null : txtEndTime.Text,
                HoursLogged = hours,
                Notes       = string.IsNullOrWhiteSpace(txtNotes.Text)     ? null : txtNotes.Text.Trim(),
                LoggedAt    = DateTime.UtcNow
            };
            EventDAL.Insert(ev);

            Response.Redirect("~/Pages/Volunteer/MyEvents.aspx?success=1", true);
        }
    }
}
