using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.SuperAdmin
{
    public partial class Admins : BasePage
    {
        protected override string RequiredRole => "SuperAdmin";

        // View model with workspace name merged
        private class AdminRow
        {
            public int       Id            { get; set; }
            public string    FullName      { get; set; }
            public string    Initials      { get; set; }
            public string    Email         { get; set; }
            public bool      IsActive      { get; set; }
            public DateTime? LastLoginAt   { get; set; }
            public string    WorkspaceName { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            var admins     = UserDAL.GetAllAdmins();
            var workspaces = WorkspaceDAL.GetAll();
            var wMap       = new Dictionary<int, string>();
            foreach (var w in workspaces) wMap[w.Id] = w.Name;

            var rows = new List<AdminRow>();
            foreach (var a in admins)
                rows.Add(new AdminRow
                {
                    Id           = a.Id,
                    FullName     = a.FullName,
                    Initials     = a.Initials,
                    Email        = a.Email,
                    IsActive     = a.IsActive,
                    LastLoginAt  = a.LastLoginAt,
                    WorkspaceName = a.WorkspaceId.HasValue && wMap.ContainsKey(a.WorkspaceId.Value)
                                   ? wMap[a.WorkspaceId.Value] : null
                });

            gvAdmins.DataSource = rows;
            gvAdmins.DataBind();
        }

        protected void gvAdmins_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Toggle") return;
            var parts    = e.CommandArgument.ToString().Split(',');
            int id       = int.Parse(parts[0]);
            bool current = bool.Parse(parts[1]);
            UserDAL.SetIsActive(id, !current);
            BindGrid();
        }
    }
}
