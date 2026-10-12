
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminLogin.aspx.cs"
    Inherits="TaxiBookingSystem.AdminLogin" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Admin Login</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Administrator Login</h2>

        <p>
            Username:
            <asp:TextBox ID="txtUsername" runat="server" />
            <asp:RequiredFieldValidator
                ID="rfvUsername" runat="server"
                ControlToValidate="txtUsername"
                ErrorMessage="Username is required."
                ForeColor="Red" />
        </p>

        <p>
            Password:
            <asp:TextBox ID="txtPassword" runat="server"
                TextMode="Password" />
            <asp:RequiredFieldValidator
                ID="rfvPassword" runat="server"
                ControlToValidate="txtPassword"
                ErrorMessage="Password is required."
                ForeColor="Red" />
        </p>

        <asp:Button ID="btnLogin" runat="server"
            Text="Admin Login"
            OnClick="btnLogin_Click" />

        <br /><br />
        <asp:Label ID="lblMessage" runat="server"
            ForeColor="Red" />
    </form>
</body>
</html>
