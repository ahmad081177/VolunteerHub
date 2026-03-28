using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Admin
{
    public partial class CreateProject : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            DateTime start, end;
            if (!DateTime.TryParse(txtStartDate.Text, out start) || !DateTime.TryParse(txtEndDate.Text, out end))
            { litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid dates.</div>"; return; }

            if (end < start)
            { litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">End date must be after start date.</div>"; return; }

            int?     maxVols = null;
            decimal? hrs     = null;
            int      mv; decimal hr;
            if (int.TryParse(txtMaxVols.Text, out mv))       maxVols = mv;
            if (decimal.TryParse(txtHoursRequired.Text, out hr)) hrs = hr;

            var project = new VHProject
            {
                WorkspaceId   = CurrentWorkspaceId ?? 0,
                Title         = txtTitle.Text.Trim(),
                Description   = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                Location      = string.IsNullOrWhiteSpace(txtLocation.Text)    ? null : txtLocation.Text.Trim(),
                StartDate     = start,
                EndDate       = end,
                MaxVolunteers  = maxVols,
                HoursRequired  = hrs,
                CreatedAt     = DateTime.UtcNow
            };
            ProjectDAL.Insert(project);

            Response.Redirect("~/Pages/Admin/Projects.aspx", true);
        }
    }
}
