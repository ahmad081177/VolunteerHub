using System;
using System.Web.UI;

namespace VolunteerHub.Pages
{
    public partial class Error : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null && litError != null)
            {
                litError.Text = $"<p class=\"text-muted small\">Error ID: {Guid.NewGuid():N}</p>";
                Server.ClearError();
            }
        }
    }
}
