
using System;

namespace TaxiBookingSystem
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Authorization must happen on the server.
            if (Session["AdminID"] == null)
            {
                Response.Redirect("AdminLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome, " +
                    Server.HtmlEncode(
                        Convert.ToString(
                            Session["AdminUsername"])) + "!";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("AdminLogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
