using System;
using System.Web.UI;
using VolunteerHub.DAL;

namespace VolunteerHub
{
    public partial class DashboardMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var role      = Session["Role"]    as string;
                var userId    = Session["UserId"]  != null ? (int)Session["UserId"] : 0;

                // Show correct nav group
                if (navSuperAdmin != null) navSuperAdmin.Visible = role == "SuperAdmin";
                if (navAdmin      != null) navAdmin.Visible      = role == "Admin";
                if (navVolunteer  != null) navVolunteer.Visible  = role == "Volunteer";

                // Sidebar role label
                if (sidebarRoleLabel != null)
                    sidebarRoleLabel.InnerText = role == "SuperAdmin" ? "Super Admin"
                                               : role == "Admin"      ? "Administrator"
                                               : "Volunteer";

                // Topbar user info
                if (userId > 0)
                {
                    var user = UserDAL.GetById(userId);
                    if (user != null)
                    {
                        if (topbarName   != null) topbarName.InnerText   = user.FullName;
                        if (topbarRole   != null) topbarRole.InnerText   = sidebarRoleLabel?.InnerText ?? role;
                        if (topbarAvatar != null) topbarAvatar.InnerText = user.Initials;
                    }
                }
            }
        }
    }
}
