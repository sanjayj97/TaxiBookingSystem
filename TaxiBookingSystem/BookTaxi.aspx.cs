
using System;
using System.Globalization;
using System.Web.UI.WebControls;
using TaxiBookingSystem.Services;

namespace TaxiBookingSystem
{
    public partial class BookTaxi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CustomerID"] == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadLocations();
            }
        }

        private void LoadLocations()
        {
            string cs = System.Configuration.ConfigurationManager
                .ConnectionStrings["TaxiBookingDb"]
                .ConnectionString;

            using (var con = new System.Data.SqlClient.SqlConnection(cs))
            using (var adapter = new System.Data.SqlClient.SqlDataAdapter(
                @"SELECT LocationID, LocationName
                  FROM dbo.Locations
                  WHERE IsActive = 1
                  ORDER BY LocationName;", con))
            {
                var locations = new System.Data.DataTable();
                adapter.Fill(locations);

                ddlPickup.DataSource = locations;
                ddlPickup.DataTextField = "LocationName";
                ddlPickup.DataValueField = "LocationID";
                ddlPickup.DataBind();

                ddlDrop.DataSource = locations;
                ddlDrop.DataTextField = "LocationName";
                ddlDrop.DataValueField = "LocationID";
                ddlDrop.DataBind();
            }

            ddlPickup.Items.Insert(
                0, new ListItem("-- Select Pickup --", ""));

            ddlDrop.Items.Insert(
                0, new ListItem("-- Select Drop --", ""));
        }

        protected void cvLocations_ServerValidate(
            object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                !string.IsNullOrEmpty(ddlPickup.SelectedValue) &&
                !string.IsNullOrEmpty(ddlDrop.SelectedValue) &&
                ddlPickup.SelectedValue != ddlDrop.SelectedValue;
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int pickupID;
            int dropID;

            if (!int.TryParse(ddlPickup.SelectedValue, out pickupID) ||
                !int.TryParse(ddlDrop.SelectedValue, out dropID))
            {
                lblMessage.Text = "Please select valid locations.";
                return;
            }

            DateTime pickupTime;

            if (!DateTime.TryParseExact(
                txtPickupTime.Text,
                "yyyy-MM-ddTHH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out pickupTime))
            {
                lblMessage.Text = "Please enter a valid pickup date and time.";
                return;
            }

            int customerID = Convert.ToInt32(Session["CustomerID"]);

            try
            {
                BookingService service = new BookingService();

                BookingResult result = service.BookTaxi(
                    customerID, pickupID, dropID, pickupTime);

                if (!result.Success)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;
                    lblMessage.Text = result.Message;
                    return;
                }

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                lblMessage.Text =
                    "Booking successful! Booking ID: " +
                    result.BookingID +
                    " | Taxi ID: " + result.TaxiID +
                    " | Distance: " +
                    result.DistanceKm.ToString("0.00") + " km" +
                    " | Fare: ₹" +
                    result.Fare.ToString("0.00") +
                    " | Estimated drop time: " +
                    result.DropTime.ToString("dd-MMM-yyyy hh:mm tt");
            }
            catch (Exception)
            {
                // Log the exception securely in a production application.
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Booking could not be completed. Please try again.";
            }
        }
    }
}
