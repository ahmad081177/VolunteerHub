using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;
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
                // txtHours is readonly/calculated — pre-fill from stored value for display
                txtHours.Text          = ev.HoursLogged > 0 ? ev.HoursLogged.ToString("0.##") : "";
                txtNotes.Text          = ev.Notes      ?? "";

                // Load existing photos
                if (EventImageDAL.TableExists())
                {
                    var imgs = EventImageDAL.GetByEvent(id);
                    if (imgs.Count > 0)
                    {
                        rptExistingImages.DataSource = imgs;
                        rptExistingImages.DataBind();
                    }
                    else
                    {
                        existingPhotosSection.Visible = false;
                    }
                }
                else
                {
                    existingPhotosSection.Visible = false;
                }
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

            // Hours are calculated from start/end times (not manually entered)
            string startTime = txtStartTime.Text.Trim();
            string endTime   = txtEndTime.Text.Trim();
            decimal hours    = CalculateHours(startTime, endTime);
            if (hours <= 0)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">End time must be after start time.</div>";
                return;
            }

            var updated = new VHEvent
            {
                Id          = id,
                UserId      = CurrentUserId,
                ProjectId   = original.ProjectId,   // immutable
                EventDate   = date,
                StartTime   = startTime,
                EndTime     = endTime,
                HoursLogged = hours,
                Notes       = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
            };
            EventDAL.Update(updated);

            // Process photo changes if EventImages table exists
            if (EventImageDAL.TableExists())
            {
                // 1. Delete any images the user marked for removal
                string toDelete = hfDeleteImageIds.Value;
                if (!string.IsNullOrWhiteSpace(toDelete))
                {
                    var paths = toDelete.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (string imgPath in paths)
                    {
                        string trimmed = imgPath.Trim();
                        EventImageDAL.DeleteByPath(id, trimmed);
                        // Also delete physical file from disk
                        try
                        {
                            string physical = Server.MapPath(trimmed);
                            if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
                        }
                        catch { /* non-fatal */ }
                    }
                }

                // 2. Append newly uploaded images (up to 5 total)
                int existing = EventImageDAL.GetByEvent(id).Count;
                var files = Request.Files;
                int saved = 0;
                for (int i = 0; i < files.Count && (existing + saved) < 5; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength == 0) continue;
                    try
                    {
                        string path = ImageHelper.SaveUpload(file, "EventImages");
                        if (path != null)
                        {
                            EventImageDAL.Insert(id, path, existing + saved);
                            saved++;
                        }
                    }
                    catch { /* skip invalid files silently */ }
                }
            }

            Response.Redirect("~/Pages/Volunteer/MyEvents.aspx?saved=1", true);
        }

        /// <summary>Calculates decimal hours from two "HH:MM" strings. Returns 0 if invalid or end &lt;= start.</summary>
        private static decimal CalculateHours(string start, string end)
        {
            try
            {
                var sp = start.Split(':');
                var ep = end.Split(':');
                int startMin = int.Parse(sp[0]) * 60 + int.Parse(sp[1]);
                int endMin   = int.Parse(ep[0]) * 60 + int.Parse(ep[1]);
                int diff = endMin - startMin;
                return diff > 0 ? Math.Round((decimal)diff / 60, 2) : 0m;
            }
            catch { return 0m; }
        }
    }
}

