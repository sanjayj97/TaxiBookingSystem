
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    Inherits="TaxiBookingSystem.AdminDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Admin Dashboard</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Admin Dashboard</h2>

        <asp:Label ID="lblWelcome" runat="server" />

        <p>Administrator access granted.</p>

        <asp:Button ID="btnLogout" runat="server"
            Text="Admin Logout"
            OnClick="btnLogout_Click" />
    </form>
</body>
</html>
