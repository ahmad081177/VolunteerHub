using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Models;

namespace VolunteerHub.Pages.Volunteer
{
    public partial class EditEvent : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id;
                if (!int.TryParse(Request.QueryString["id"], out id))
                { Response.Redirect("~/Pages/Volunteer/MyEvents.aspx", true); return; }

                var ev = EventDAL.GetById(id);

                // IDOR guard: volunteers may only edit their own events
                if (ev == null || ev.UserId != CurrentUserId)
                { Response.Redirect("~/Pages/Volunteer/MyEvents.aspx", true); return; }

                hfId.Value             = ev.Id.ToString();
                lblProject.InnerText   = ev.ProjectTitle ?? "—";
                txtDate.Text           = ev.EventDate.ToString("yyyy-MM-dd");
                txtStartTime.Text      = ev.StartTime ?? "";
                txtEndTime.Text        = ev.EndTime   ?? "";
                txtHours.Text          = ev.HoursLogged.ToString("0.##");
                txtNotes.Text          = ev.Notes      ?? "";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int id = int.Parse(hfId.Value);

            // Re-load to confirm ownership and get original ProjectId (not submitted by form)
            var original = EventDAL.GetById(id);
            if (original == null || original.UserId != CurrentUserId)
            { Response.Redirect("~/Pages/Volunteer/MyEvents.aspx", true); return; }

            DateTime date;
            if (!DateTime.TryParse(txtDate.Text, out date))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid date.</div>";
                return;
            }

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

            var updated = new VHEvent
            {
                Id          = id,
                UserId      = CurrentUserId,
                ProjectId   = original.ProjectId,   // immutable
                EventDate   = date,
                StartTime   = string.IsNullOrWhiteSpace(txtStartTime.Text) ? null : txtStartTime.Text,
                EndTime     = string.IsNullOrWhiteSpace(txtEndTime.Text)   ? null : txtEndTime.Text,
                HoursLogged = hours,
                Notes       = string.IsNullOrWhiteSpace(txtNotes.Text)     ? null : txtNotes.Text.Trim()
            };
            EventDAL.Update(updated);

            Response.Redirect("~/Pages/Volunteer/MyEvents.aspx?saved=1", true);
        }
    }
}
