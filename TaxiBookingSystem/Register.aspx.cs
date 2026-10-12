
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TaxiBookingSystem.Services;

namespace TaxiBookingSystem
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;

            string connectionString =
                ConfigurationManager
                .ConnectionStrings["TaxiBookingDb"]
                .ConnectionString;

            string query = @"
                INSERT INTO dbo.Customers
                    (FullName, Email, Phone, PasswordHash)
                VALUES
                    (@FullName, @Email, @Phone, @PasswordHash);";

            try
            {
                string passwordHash =
                    PasswordHelper.HashPassword(password);

                using (SqlConnection connection =
                       new SqlConnection(connectionString))
                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.Add(
                        "@FullName", SqlDbType.NVarChar, 100
                    ).Value = fullName;

                    command.Parameters.Add(
                        "@Email", SqlDbType.NVarChar, 254
                    ).Value = email;

                    command.Parameters.Add(
                        "@Phone", SqlDbType.NVarChar, 20
                    ).Value = phone;

                    command.Parameters.Add(
                        "@PasswordHash", SqlDbType.NVarChar, 256
                    ).Value = passwordHash;

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                lblMessage.Text =
                    "Registration successful. You can now log in.";
            }
            catch (SqlException ex)
            {
                // SQL Server error 2601 or 2627 indicates
                // a duplicate value violating a unique index/key.
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "That email address or phone number is already registered.";
                }
                else
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Registration could not be completed. Please try again.";
                }
            }
        }
    }
}
