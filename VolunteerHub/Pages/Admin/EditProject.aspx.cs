using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Admin
{
    /// <summary>
    /// Allows workspace admins to edit an existing project.
    /// Security: only the owning workspace's admin can edit (WorkspaceId guard in ProjectDAL.Update).
    /// </summary>
    public partial class EditProject : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadProject();
        }

        /// <summary>Loads project from DB and pre-fills form fields.</summary>
        private void LoadProject()
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id))
            { Response.Redirect("~/Pages/Admin/Projects.aspx", true); return; }

            var p = ProjectDAL.GetById(id);
            // Guard: project must belong to this admin's workspace
            if (p == null || p.WorkspaceId != (CurrentWorkspaceId ?? -1))
            { Response.Redirect("~/Pages/Admin/Projects.aspx", true); return; }

            hfId.Value             = p.Id.ToString();
            txtTitle.Text          = p.Title;
            txtDescription.Text    = p.Description ?? "";
            txtLocation.Text       = p.Location ?? "";
            txtStartDate.Text      = p.StartDate.ToString("yyyy-MM-dd");
            txtEndDate.Text        = p.EndDate.ToString("yyyy-MM-dd");
            txtMaxVols.Text        = p.MaxVolunteers?.ToString() ?? "";
            txtHoursRequired.Text  = p.HoursRequired?.ToString() ?? "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int id = int.Parse(hfId.Value);
            var p  = ProjectDAL.GetById(id);
            if (p == null || p.WorkspaceId != (CurrentWorkspaceId ?? -1)) return;

            DateTime start, end;
            if (!DateTime.TryParse(txtStartDate.Text, out start) || !DateTime.TryParse(txtEndDate.Text, out end))
            { litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid dates.</div>"; return; }

            if (end < start)
            { litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">End date must be after start date.</div>"; return; }

            // Update only the mutable fields; WorkspaceId is locked
            p.Title          = txtTitle.Text.Trim();
            p.Description    = string.IsNullOrWhiteSpace(txtDescription.Text)   ? null : txtDescription.Text.Trim();
            p.Location       = string.IsNullOrWhiteSpace(txtLocation.Text)      ? null : txtLocation.Text.Trim();
            p.StartDate      = start;
            p.EndDate        = end;
            int mv; decimal hr;
            p.MaxVolunteers  = int.TryParse(txtMaxVols.Text,       out mv) ? (int?)mv     : null;
            p.HoursRequired  = decimal.TryParse(txtHoursRequired.Text, out hr) ? (decimal?)hr : null;

            ProjectDAL.Update(p);
            Response.Redirect("~/Pages/Admin/Projects.aspx", true);
        }
    }
}
