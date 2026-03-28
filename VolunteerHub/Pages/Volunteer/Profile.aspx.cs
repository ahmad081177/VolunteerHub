using System;
using VolunteerHub.Base;
using VolunteerHub.DAL;
using VolunteerHub.Helpers;

namespace VolunteerHub.Pages.Volunteer
{
    /// <summary>
    /// Volunteer profile page — view/edit personal info, upload a photo,
    /// and change password. Stats sidebar shows lifetime totals.
    /// </summary>
    public partial class Profile : BasePage
    {
        protected override string RequiredRole => "Volunteer";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindProfile();
        }

        /// <summary>Loads current user data into all form fields and stat widgets.</summary>
        private void BindProfile()
        {
            int uid  = CurrentUserId;
            var user = UserDAL.GetById(uid);
            if (user == null) return;

            // Populate form fields
            txtFirstName.Text = user.FirstName;
            txtLastName.Text  = user.LastName;
            txtEmail.Text     = user.Email;
            txtPhone.Text     = user.Phone ?? "";
            txtAddress.Text   = user.Address ?? "";
            txtDob.Text       = user.DateOfBirth?.ToString("yyyy-MM-dd") ?? "";

            // Avatar — show initials or image if set
            currentAvatarText.InnerText = user.Initials;
            if (!string.IsNullOrEmpty(user.ImageProfilePath))
                currentAvatarImg.InnerHtml =
                    $"<img src=\"{ResolveUrl("~/" + user.ImageProfilePath)}\" class=\"vh-img-thumbnail\" alt=\"Profile\" />";

            // Stats sidebar
            statHours.InnerText    = EventDAL.GetTotalHoursByUser(uid).ToString("0.#");
            statProjects.InnerText = VolunteerProjectDAL.GetByUser(uid).Count.ToString();
            statJoined.InnerText   = user.CreatedAt.ToString("MMMM yyyy");
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int uid  = CurrentUserId;
            var user = UserDAL.GetById(uid);
            if (user == null) return;

            // Handle profile photo upload
            if (fuPhoto.HasFile)
            {
                var path = ImageHelper.SaveUpload(fuPhoto.PostedFile, "ProfileImages");
                if (path == null)
                {
                    litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Invalid photo. Use JPG/PNG/GIF under 2 MB.</div>";
                    return;
                }
                user.ImageProfilePath = path;
            }

            user.FirstName = txtFirstName.Text.Trim();
            user.LastName  = txtLastName.Text.Trim();
            user.Phone     = string.IsNullOrWhiteSpace(txtPhone.Text)   ? null : txtPhone.Text.Trim();
            user.Address   = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim();

            UserDAL.UpdateProfile(user);
            litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> Profile updated successfully.</div>";
            BindProfile(); // refresh to show updated values
        }

        protected void btnChangePwd_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var user = UserDAL.GetById(CurrentUserId);
            if (user == null) return;

            // Verify current password before allowing change
            if (!AuthHelper.VerifyPassword(txtCurrentPwd.Text, user.PasswordHash))
            {
                litAlert.Text = "<div class=\"vh-alert vh-alert-danger\">Current password is incorrect.</div>";
                return;
            }

            var newHash = AuthHelper.HashPassword(txtNewPwd.Text);
            UserDAL.UpdatePassword(CurrentUserId, newHash);

            // Clear the password fields after success
            txtCurrentPwd.Text = "";
            txtNewPwd.Text     = "";
            txtConfirmPwd.Text = "";

            litAlert.Text = "<div class=\"vh-alert vh-alert-success\"><i class=\"bi bi-check-circle\"></i> Password changed successfully.</div>";
        }
    }
}
