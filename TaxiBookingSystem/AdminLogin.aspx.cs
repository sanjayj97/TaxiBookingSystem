
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TaxiBookingSystem.Services;

namespace TaxiBookingSystem
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack &&
                Session["AdminID"] != null)
            {
                Response.Redirect("AdminDashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            string cs = ConfigurationManager
                .ConnectionStrings["TaxiBookingDb"]
                .ConnectionString;

            string query = @"
                SELECT AdminID, Username, PasswordHash
                FROM dbo.Admins
                WHERE Username = @Username AND IsActive = 1;";

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@Username", SqlDbType.NVarChar, 50
                ).Value = username;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int adminID =
                            Convert.ToInt32(reader["AdminID"]);

                        string adminName =
                            Convert.ToString(reader["Username"]);

                        string storedHash =
                            Convert.ToString(reader["PasswordHash"]);

                        if (PasswordHelper.VerifyPassword(
                            password, storedHash))
                        {
                            // Remove any previous customer identity.
                            Session.Clear();
                            Session["AdminID"] = adminID;
                            Session["AdminUsername"] = adminName;
                            Session.Timeout = 20;

                            Response.Redirect(
                                "AdminDashboard.aspx", false);

                            Context.ApplicationInstance
                                .CompleteRequest();
                            return;
                        }
                    }
                }
            }

            lblMessage.Text = "Invalid username or password.";
        }
    }
}
