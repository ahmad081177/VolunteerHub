using System;
using System.Collections.Generic;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Admin
{
    /// <summary>
    /// Admin-only view of a single project showing enrolled volunteers
    /// with their individual progress (hours logged vs required).
    /// </summary>
    public partial class ProjectDetail : BasePage
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
            { Response.Redirect("~/Pages/Admin/Projects.aspx", true); return; }

            var p = ProjectDAL.GetById(id);
            // Enforce workspace isolation — admin can only see own workspace's projects
            if (p == null || p.WorkspaceId != (CurrentWorkspaceId ?? -1))
            { Response.Redirect("~/Pages/Admin/Projects.aspx", true); return; }

            // --- Page header ---
            pageTitle.InnerText      = p.Title;
            breadcrumbTitle.InnerText = p.Title;
            pageMeta.InnerText       = $"{p.Location ?? "No location"}  •  {p.StartDate:MMM dd, yyyy} – {p.EndDate:MMM dd, yyyy}";
            if (!string.IsNullOrWhiteSpace(p.Description))
                descriptionText.InnerText = p.Description;
            else
                descriptionCard.Visible = false;

            // --- Stats ---
            var enrollments   = VolunteerProjectDAL.GetByProject(id);
            decimal totalHrs  = EventDAL.GetTotalHoursByProject(id);
            int daysLeft      = Math.Max(0, (p.EndDate - DateTime.Today).Days);

            statEnrolled.InnerText    = enrollments.Count.ToString();
            statTotalHours.InnerText  = totalHrs.ToString("0.#");
            statDaysLeft.InnerText    = p.Status == "Ended" ? "Ended" : daysLeft.ToString();

            // --- Build a view model merging VolunteerProject + AppUser + hours ---
            var rows = new List<VolunteerProgressRow>();
            foreach (var vp in enrollments)
            {
                var user = UserDAL.GetById(vp.UserId);
                if (user == null) continue;
                decimal hrs = 0;
                foreach (var ev in EventDAL.GetByUserAndProject(vp.UserId, id))
                    hrs += ev.HoursLogged;

                decimal pct = p.HoursRequired.HasValue && p.HoursRequired > 0
                              ? Math.Min(100, Math.Round(hrs / p.HoursRequired.Value * 100))
                              : 0;

                rows.Add(new VolunteerProgressRow
                {
                    FullName    = user.FullName,
                    Initials    = user.Initials,
                    Email       = user.Email,
                    HoursLogged = hrs,
                    EnrolledAt  = vp.EnrolledAt,
                    ProgressPct = (int)pct
                });
            }
            gvVolunteers.DataSource = rows;
            gvVolunteers.DataBind();
        }

        /// <summary>Flat view-model combining volunteer user info with their project hours.</summary>
        private class VolunteerProgressRow
        {
            public string   FullName    { get; set; }
            public string   Initials    { get; set; }
            public string   Email       { get; set; }
            public decimal  HoursLogged { get; set; }
            public DateTime EnrolledAt  { get; set; }
            public int      ProgressPct { get; set; }
        }
    }
}
