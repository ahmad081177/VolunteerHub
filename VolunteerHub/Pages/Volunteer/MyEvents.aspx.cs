using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using VolunteerHub.Base;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Full history of the volunteer's logged events across all projects.
    /// Volunteers can delete their own events from here.
    /// Shows a 3-stat summary strip: total hours, event count, average.
    /// </summary>
    public partial class MyEvents : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Show success banner after redirect from LogEvent or EditEvent
                if (Request.QueryString["success"] == "1")
                    litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> Hours logged successfully!</div>";
                else if (Request.QueryString["saved"] == "1")
                    litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> Event updated successfully!</div>";

                BindEvents();
            }
        }

        private void BindEvents()
        {
            int uid   = CurrentUserId;
            var events = EventDAL.GetByUser(uid);
            decimal total = EventDAL.GetTotalHoursByUser(uid);
            int count     = events.Count;
            decimal avg   = count > 0 ? Math.Round(total / count, 1) : 0;

            statTotal.InnerText = total.ToString("0.#");
            statCount.InnerText = count.ToString();
            statAvg.InnerText   = avg.ToString("0.#");

            gvEvents.DataSource = events;
            gvEvents.DataBind();
        }

        /// <summary>
        /// Handles the Delete row command — only allows deleting own events
        /// (EventDAL.Delete filters by both Id AND UserId to prevent IDOR).
        /// </summary>
        protected void gvEvents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Delete") return;
            int id;
            if (!int.TryParse(e.CommandArgument?.ToString(), out id) || id <= 0) return;
            // Delete associated images first (Access doesn't enforce FK cascades)
            if (EventImageDAL.TableExists()) EventImageDAL.DeleteByEvent(id);
            // UserId guard is inside EventDAL.Delete — cannot delete another user's events
            EventDAL.Delete(id, CurrentUserId);
            BindEvents();
        }

        // BuildImageThumb is inherited from BasePage
    }
}
