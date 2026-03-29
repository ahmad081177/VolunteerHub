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
            int id = int.Parse(e.CommandArgument.ToString());
            // UserId guard is inside EventDAL.Delete — cannot delete another user's events
            EventDAL.Delete(id, CurrentUserId);
            BindEvents();
        }

        /// <summary>
        /// Returns inline HTML showing the first image thumbnail for an event, plus a "+N" badge if there are more.
        /// Called from the Photos TemplateField in the GridView.
        /// Gracefully returns "—" if the EventImages table doesn't exist yet (pre-migration DBs).
        /// </summary>
        protected string BuildImageThumb(int eventId)
        {
            if (!EventImageDAL.TableExists()) return "<span class=\"text-muted\">—</span>";
            var imgs = EventImageDAL.GetByEvent(eventId);
            if (imgs.Count == 0) return "<span class=\"text-muted\">—</span>";

            var sb = new StringBuilder();
            sb.Append("<div class=\"d-flex align-items-center gap-1\">");
            // Show first thumbnail
            string first = ResolveUrl(imgs[0]);
            sb.Append($"<img src=\"{System.Web.HttpUtility.HtmlAttributeEncode(first)}\" " +
                       "style=\"width:36px;height:36px;object-fit:cover;border-radius:6px;border:1px solid #E2E8F0;\" />");
            if (imgs.Count > 1)
                sb.Append($"<span class=\"vh-badge vh-badge-gray\">+{imgs.Count - 1}</span>");
            sb.Append("</div>");
            return sb.ToString();
        }
    }
}
