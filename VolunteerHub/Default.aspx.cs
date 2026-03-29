using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VolunteerHub
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect already-authenticated users straight to their dashboard
            var role = Session["UserId"] != null ? Session["Role"] as string : null;
            if (role == "SuperAdmin")
                Response.Redirect("~/Pages/SuperAdmin/Dashboard.aspx", true);
            else if (role == "Admin")
                Response.Redirect("~/Pages/Admin/Dashboard.aspx", true);
            else if (role == "Volunteer")
                Response.Redirect("~/Pages/Volunteer/Dashboard.aspx", true);
        }
    }
}