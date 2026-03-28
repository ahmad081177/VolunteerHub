using System;
using System.Web;
using System.Web.UI;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;

namespace VolunteerHub.Pages
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Already logged in — redirect to their dashboard
            if (!IsPostBack && Session["UserId"] != null)
                RedirectToDashboard(Session["Role"] as string);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var email    = txtEmail.Text.Trim().ToLowerInvariant();
            var password = txtPassword.Text;

            var user = UserDAL.GetByEmail(email);

            // Single generic message for both "user not found" and "wrong password".
            // This prevents user enumeration — an attacker cannot tell which condition failed.
            if (user == null || !AuthHelper.VerifyPassword(password, user.PasswordHash))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\"><i class=\"bi bi-x-circle\"></i> Invalid email or password. Please try again.</div>";
                return;
            }

            if (!user.IsActive)
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-warning\"><i class=\"bi bi-exclamation-triangle\"></i> Your account has been deactivated. Please contact an administrator.</div>";
                return;
            }

            // Set the three session keys every page relies on, then record the login timestamp
            Session["UserId"]      = user.Id;
            Session["Role"]        = user.Role;
            Session["WorkspaceId"] = user.WorkspaceId;
            UserDAL.UpdateLastLogin(user.Id);

            // Remember me: generate a random token, save it to DB, and write an HttpOnly cookie.
            // BasePage.TryRestoreFromCookie reads this cookie to rebuild the session on return visits.
            if (chkRemember.Checked)
            {
                var token  = AuthHelper.GenerateRememberMeToken();
                var expiry = DateTime.UtcNow.AddDays(30);
                UserDAL.UpdateRememberMeToken(user.Id, token, expiry);

                var cookie = new HttpCookie("vh_remember", token)
                {
                    HttpOnly = true,   // blocks JavaScript from reading the cookie (XSS protection)
                    Secure   = Request.IsSecureConnection,
                    Expires  = expiry,
                    Path     = "/"
                };
                Response.Cookies.Add(cookie);
            }

            // Only redirect to returnUrl when it starts with "/" — guards against open-redirect
            // attacks where an attacker tricks the login page into forwarding to an external site.
            var returnUrl = Request.QueryString["returnUrl"];
            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith("/"))
            {
                Response.Redirect(returnUrl, true);
                return;
            }

            RedirectToDashboard(user.Role);
        }

        private void RedirectToDashboard(string role)
        {
            switch (role)
            {
                case "SuperAdmin": Response.Redirect("~/Pages/SuperAdmin/Dashboard.aspx", true); break;
                case "Admin":      Response.Redirect("~/Pages/Admin/Dashboard.aspx",      true); break;
                default:           Response.Redirect("~/Pages/Volunteer/Dashboard.aspx",  true); break;
            }
        }
    }
}
