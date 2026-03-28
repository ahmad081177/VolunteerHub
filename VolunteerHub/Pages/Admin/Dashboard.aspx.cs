using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Admin
{
    public partial class Dashboard : BasePage
    {
        protected override string RequiredRole => "Admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            int wsId = CurrentWorkspaceId ?? 0;
            if (wsId == 0) return;

            var ws         = WorkspaceDAL.GetById(wsId);
            var projects   = ProjectDAL.GetByWorkspace(wsId);
            var volunteers = UserDAL.GetAllByWorkspace(wsId);

            wsSubtitle.InnerText   = ws != null ? $"{ws.Name} workspace" : "";
            statProjects.InnerText = projects.Count.ToString();
            statVolunteers.InnerText = volunteers.Count.ToString();

            // Sum hours across all volunteers by looping individually — Access doesn't support
            // a cross-table workspace-scoped SUM in a single query without a complex subquery.
            decimal totalHours = 0;
            foreach (var v in volunteers) totalHours += EventDAL.GetTotalHoursByUser(v.Id);
            statHours.InnerText  = totalHours.ToString("0.#");
            statActive.InnerText = projects.Count(p => p.Status == "Active").ToString();  // Status is computed by VHProject.Status property

            // Show only the 5 most recent projects on the dashboard; full list is on Projects.aspx
            gvProjects.DataSource = projects.Take(5).ToList();
            gvProjects.DataBind();

            // Build an inline <script> block that calls VH.barChart() from volunteerhub.js.
            // JsonConvert serialises the C# list into a JS array literal: ["Title A","Title B"]
            var hrsData  = ProjectDAL.GetHoursPerProject(wsId);
            var volsData = ProjectDAL.GetVolunteersPerProject(wsId);

            var sb = new StringBuilder("<script>\n");
            if (hrsData.Count > 0)
            {
                var labels = JsonConvert.SerializeObject(hrsData.ConvertAll(x => x.Title));
                var data   = JsonConvert.SerializeObject(hrsData.ConvertAll(x => x.Hours));
                sb.AppendLine($"VH.barChart('chartHoursPerProject', {labels}, {data}, 'Hours');");
            }
            if (volsData.Count > 0)
            {
                var labels = JsonConvert.SerializeObject(volsData.ConvertAll(x => x.Title));
                var data   = JsonConvert.SerializeObject(volsData.ConvertAll(x => x.Count));
                sb.AppendLine($"VH.barChart('chartVolsPerProject', {labels}, {data}, 'Volunteers');");
            }
            sb.AppendLine("</script>");
            litChartScript.Text = sb.ToString();
        }
    }
}
