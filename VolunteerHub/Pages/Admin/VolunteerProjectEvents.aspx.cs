using System;
using System.Linq;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Admin
{
    public partial class VolunteerProjectEvents : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            int projectId, userId;
            if (!int.TryParse(Request.QueryString["projectId"], out projectId) ||
                !int.TryParse(Request.QueryString["userId"], out userId))
            { Response.Redirect("~/Pages/Admin/Projects.aspx", true); return; }

            var project = ProjectDAL.GetById(projectId);
            if (project == null || project.WorkspaceId != (CurrentWorkspaceId ?? -1))
            { Response.Redirect("~/Pages/Admin/Projects.aspx", true); return; }

            var volunteer = UserDAL.GetById(userId);
            if (volunteer == null || volunteer.WorkspaceId != CurrentWorkspaceId)
            { Response.Redirect($"~/Pages/Admin/ProjectDetail.aspx?id={projectId}", true); return; }

            // Header
            pageTitle.InnerText       = $"{volunteer.FullName} — {project.Title}";
            pageMeta.InnerText        = $"All events logged by {volunteer.FirstName} for this project";
            breadcrumbProject.NavigateUrl = $"~/Pages/Admin/ProjectDetail.aspx?id={projectId}";
            breadcrumbProject.Text        = System.Web.HttpUtility.HtmlEncode(project.Title);
            breadcrumbVolunteer.InnerText = volunteer.FullName;

            // Events
            var events = EventDAL.GetByUserAndProject(userId, projectId);
            decimal totalHrs = events.Sum(ev => ev.HoursLogged);
            int photoCount = 0;
            if (EventImageDAL.TableExists())
            {
                foreach (var ev in events)
                    photoCount += EventImageDAL.GetByEvent(ev.Id).Count;
            }

            statHours.InnerText  = totalHrs.ToString("0.#");
            statCount.InnerText  = events.Count.ToString();
            statPhotos.InnerText = photoCount.ToString();

            gvEvents.DataSource = events;
            gvEvents.DataBind();
        }
    }
}
