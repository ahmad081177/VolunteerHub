using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using VolunteerHub.DAL;

namespace VolunteerHub.Base
{
    public class BasePage : Page
    {
        // Override in subclasses: "SuperAdmin", "Admin", "Volunteer"
        // null = public (no auth required)
        protected virtual string RequiredRole => null;

        protected int CurrentUserId    => Session["UserId"]    != null ? (int)Session["UserId"]    : 0;
        protected string CurrentRole   => Session["Role"]       as string;
        protected int? CurrentWorkspaceId => Session["WorkspaceId"] as int?;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (RequiredRole == null) return;   // public page — no auth needed

            // Step 1: valid session already exists for the correct role → allow through immediately
            if (Session["UserId"] != null && Session["Role"] as string == RequiredRole)
                return;

            // Step 2: no session — try to restore one from the "vh_remember" persistent cookie
            if (TryRestoreFromCookie()) return;

            // Step 3: no session and no valid cookie — redirect to login,
            // preserving the originally-requested URL so the user lands back here after signing in
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl);
            Response.Redirect($"~/Pages/Login.aspx?returnUrl={returnUrl}", true);
        }

        private bool TryRestoreFromCookie()
        {
            var cookie = Request.Cookies["vh_remember"];
            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value)) return false;

            // Look up the user whose remember-me token matches the cookie value
            var user = UserDAL.GetByRememberMeToken(cookie.Value);
            if (user == null || !user.IsActive) return false;

            // Token has expired — remove it from the DB so the stale cookie can't be reused
            if (user.RememberMeTokenExpiry == null || user.RememberMeTokenExpiry < DateTime.UtcNow)
            {
                UserDAL.ClearRememberMeToken(user.Id);
                return false;
            }

            // Cookie belongs to a user with a different role (e.g. an Admin visiting a SuperAdmin page)
            if (user.Role != RequiredRole) return false;

            // All checks passed — rebuild the session from the database record
            Session["UserId"]      = user.Id;
            Session["Role"]        = user.Role;
            Session["WorkspaceId"] = user.WorkspaceId;
            UserDAL.UpdateLastLogin(user.Id);
            return true;
        }

        protected void ShowError(string controlId, string message)
        {
            var ctrl = FindControl(controlId);
            if (ctrl is System.Web.UI.WebControls.Literal lit)
            {
                lit.Text = $"<div class=\"vh-alert vh-alert-danger\">{HttpUtility.HtmlEncode(message)}</div>";
            }
        }

        protected void ShowSuccess(string controlId, string message)
        {
            var ctrl = FindControl(controlId);
            if (ctrl is System.Web.UI.WebControls.Literal lit)
            {
                lit.Text = $"<div class=\"vh-alert vh-alert-success\">{HttpUtility.HtmlEncode(message)}</div>";
            }
        }

        // Called from .aspx data-binding expressions inside templates where the model no longer carries this.
        protected string GetStatusBadgeClass(string status)
        {
            return Helpers.ProjectHelper.GetStatusBadgeClass(status);
        }

        /// <summary>
        /// Returns inline HTML for event image thumbnails with click-to-open gallery.
        /// The thumbnail is wrapped in a data-images attribute so the JS lightbox can show all photos.
        /// </summary>
        protected string BuildImageThumb(int eventId)
        {
            if (!EventImageDAL.TableExists()) return "<span class=\"text-muted\">\u2014</span>";
            var imgs = EventImageDAL.GetByEvent(eventId);
            if (imgs.Count == 0) return "<span class=\"text-muted\">\u2014</span>";

            var resolved = new List<string>(imgs.Count);
            foreach (var p in imgs) resolved.Add(ResolveUrl(p));

            string json = JsonConvert.SerializeObject(resolved);

            var sb = new StringBuilder();
            sb.Append("<div class=\"vh-photo-thumb d-flex align-items-center gap-1\" data-images='");
            sb.Append(HttpUtility.HtmlAttributeEncode(json));
            sb.Append("' title=\"Click to view photos\">");
            sb.Append($"<img src=\"{HttpUtility.HtmlAttributeEncode(resolved[0])}\" ");
            sb.Append("style=\"width:36px;height:36px;object-fit:cover;border-radius:6px;border:1px solid #E2E8F0;\" />");
            if (imgs.Count > 1)
                sb.Append($"<span class=\"vh-badge vh-badge-gray\">+{imgs.Count - 1}</span>");
            sb.Append("</div>");
            return sb.ToString();
        }
    }
}
