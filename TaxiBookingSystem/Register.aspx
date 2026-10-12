
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="TaxiBookingSystem.Register" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Customer Registration</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Customer Registration</h2>

        <asp:ValidationSummary
            ID="ValidationSummary1"
            runat="server"
            ForeColor="Red"
            HeaderText="Please correct these errors:" />

        <p>
            Full Name:
            <asp:TextBox ID="txtFullName" runat="server" />
            <asp:RequiredFieldValidator
                ID="rfvName" runat="server"
                ControlToValidate="txtFullName"
                ErrorMessage="Full name is required."
                ForeColor="Red" />
        </p>

        <p>
            Email:
            <asp:TextBox ID="txtEmail" runat="server" />
            <asp:RequiredFieldValidator
                ID="rfvEmail" runat="server"
                ControlToValidate="txtEmail"
                ErrorMessage="Email is required."
                ForeColor="Red" />
            <asp:RegularExpressionValidator
                ID="revEmail" runat="server"
                ControlToValidate="txtEmail"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                ErrorMessage="Enter a valid email address."
                ForeColor="Red" />
        </p>

        <p>
            Phone:
            <asp:TextBox ID="txtPhone" runat="server" />
            <asp:RequiredFieldValidator
                ID="rfvPhone" runat="server"
                ControlToValidate="txtPhone"
                ErrorMessage="Phone number is required."
                ForeColor="Red" />
            <asp:RegularExpressionValidator
                ID="revPhone" runat="server"
                ControlToValidate="txtPhone"
                ValidationExpression="^[0-9]{10,15}$"
                ErrorMessage="Enter 10 to 15 digits."
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
            <asp:RegularExpressionValidator
                ID="revPassword" runat="server"
                ControlToValidate="txtPassword"
                ValidationExpression="^[\s\S]{8,}$"
                ErrorMessage="Password must contain at least 8 characters."
                ForeColor="Red" />
        </p>

        <p>
            Confirm Password:
            <asp:TextBox ID="txtConfirmPassword" runat="server"
                TextMode="Password" />
            <asp:RequiredFieldValidator
                ID="rfvConfirm" runat="server"
                ControlToValidate="txtConfirmPassword"
                ErrorMessage="Please confirm your password."
                ForeColor="Red" />
            <asp:CompareValidator
                ID="cvPassword" runat="server"
                ControlToValidate="txtConfirmPassword"
                ControlToCompare="txtPassword"
                Operator="Equal"
                Type="String"
                ErrorMessage="Passwords do not match."
                ForeColor="Red" />
        </p>

        <asp:Button ID="btnRegister" runat="server"
            Text="Register"
            OnClick="btnRegister_Click" />

        <br /><br />
        <asp:Label ID="lblMessage" runat="server" />
    </form>
</body>
</html>
