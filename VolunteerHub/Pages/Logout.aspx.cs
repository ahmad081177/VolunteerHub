using System;
using System.Web;
using System.Web.UI;
using VolunteerHub.DAL;

namespace VolunteerHub.Pages
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Step 1: invalidate the remember-me token in the DB so the cookie can't be replayed
            var cookie = Request.Cookies["vh_remember"];
            if (cookie != null)
            {
                var userId = Session["UserId"] != null ? (int)Session["UserId"] : 0;
                if (userId > 0) UserDAL.ClearRememberMeToken(userId);

                // Overwrite the browser cookie with an expired one to force immediate deletion
                var expired = new HttpCookie("vh_remember", "") { Expires = DateTime.UtcNow.AddDays(-1), Path = "/" };
                Response.Cookies.Add(expired);
            }

            // Step 2: destroy the server-side session completely
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Pages/Login.aspx", true);
        }
    }
}
