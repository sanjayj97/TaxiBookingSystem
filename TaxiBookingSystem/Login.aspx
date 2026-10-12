
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="TaxiBookingSystem.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Customer Login</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Customer Login</h2>

        <asp:ValidationSummary
            ID="ValidationSummary1"
            runat="server"
            ForeColor="Red" />

        <p>
            Email:
            <asp:TextBox ID="txtEmail" runat="server" />
            <asp:RequiredFieldValidator
                ID="rfvEmail" runat="server"
                ControlToValidate="txtEmail"
                ErrorMessage="Email is required."
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
            Text="Login"
            OnClick="btnLogin_Click" />

        <p>
            <asp:Label ID="lblMessage" runat="server"
                ForeColor="Red" />
        </p>

        <p>
            New customer?
            <asp:HyperLink ID="lnkRegister" runat="server"
                NavigateUrl="Register.aspx"
                Text="Register here" />
        </p>
    </form>
</body>
</html>
