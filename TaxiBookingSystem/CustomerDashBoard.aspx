
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="CustomerDashboard.aspx.cs"
    Inherits="TaxiBookingSystem.CustomerDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Customer Dashboard</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Taxi Booking System</h2>

        <asp:Label ID="lblWelcome" runat="server" />

        <p>Welcome to your customer dashboard.</p>

        <asp:HyperLink ID="lnkBookTaxi" runat="server"
            NavigateUrl="BookTaxi.aspx"
            Text="Book a Taxi" />

        <br /><br />

        <asp:HyperLink ID="lnkHistory" runat="server"
            NavigateUrl="BookingHistory.aspx"
            Text="View Booking History" />

        <br /><br />

        <asp:Button ID="btnLogout" runat="server"
            Text="Logout"
            OnClick="btnLogout_Click" />
    </form>
</body>
</html>
