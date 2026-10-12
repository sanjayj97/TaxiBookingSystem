
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TaxiBookingSystem.Services;

namespace TaxiBookingSystem
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack &&
                Session["CustomerID"] != null)
            {
                Response.Redirect("CustomerDashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            string connectionString =
                ConfigurationManager
                .ConnectionStrings["TaxiBookingDb"]
                .ConnectionString;

            string query = @"
                SELECT CustomerID, FullName, PasswordHash
                FROM dbo.Customers
                WHERE Email = @Email;";

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            using (SqlCommand command =
                   new SqlCommand(query, connection))
            {
                command.Parameters.Add(
                    "@Email", SqlDbType.NVarChar, 254
                ).Value = email;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int customerID =
                            Convert.ToInt32(reader["CustomerID"]);

                        string fullName =
                            Convert.ToString(reader["FullName"]);

                        string storedHash =
                            Convert.ToString(reader["PasswordHash"]);

                        if (PasswordHelper.VerifyPassword(
                            password, storedHash))
                        {
                            Session.Clear();
                            Session["CustomerID"] = customerID;
                            Session["CustomerName"] = fullName;

                            Session.Timeout = 20;

                            Response.Redirect(
                                "CustomerDashboard.aspx", false);

                            Context.ApplicationInstance
                                .CompleteRequest();

                            return;
                        }
                    }
                }
            }

            // Same message for an unknown email or wrong password.
            lblMessage.Text = "Invalid email or password.";
        }
    }
}
