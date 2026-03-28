using System;
using System.Net;
using System.Web;
using System.Web.UI;
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
    }
}
