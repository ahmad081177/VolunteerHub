using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Admin
{
    /// <summary>
    /// Detailed view of a single volunteer: their profile, total metrics,
    /// and full event log. Admin-only — workspace-scoped access only.
    /// </summary>
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
            // Enforce workspace isolation — cannot view volunteers outside own workspace
            if (user == null || user.WorkspaceId != CurrentWorkspaceId || user.Role != "Volunteer")
            { Response.Redirect("~/Pages/Admin/Volunteers.aspx", true); return; }

            // Profile header
            profileAvatar.InnerText = user.Initials;
            profileName.InnerText   = user.FullName;
            profileEmail.InnerText  = user.Email;
            breadcrumbName.InnerText = user.FullName;
            profileStatus.InnerText = user.IsActive ? "Active" : "Inactive";
            profileStatus.Attributes["class"] = user.IsActive ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted";

            // Metrics
            var events     = EventDAL.GetByUser(id);
            var enrollments = VolunteerProjectDAL.GetByUser(id);
            decimal total  = EventDAL.GetTotalHoursByUser(id);

            statTotalHours.InnerText = total.ToString("0.#");
            statProjects.InnerText   = enrollments.Count.ToString();
            statEvents.InnerText     = events.Count.ToString();

            // Event table — already joined with ProjectTitle by EventDAL
            gvEvents.DataSource = events;
            gvEvents.DataBind();
        }
    }
}
