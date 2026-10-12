
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="BookTaxi.aspx.cs"
    Inherits="TaxiBookingSystem.BookTaxi" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Book a Taxi</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Book a Taxi</h2>

        <asp:ValidationSummary
            ID="ValidationSummary1"
            runat="server"
            ForeColor="Red"
            HeaderText="Please correct these errors:" />

        <p>
            Pickup Location:
            <asp:DropDownList ID="ddlPickup" runat="server" />
        </p>

        <p>
            Drop Location:
            <asp:DropDownList ID="ddlDrop" runat="server" />
        </p>

        <asp:CustomValidator
            ID="cvLocations"
            runat="server"
            ErrorMessage="Pickup and drop locations must be different."
            ForeColor="Red"
            OnServerValidate="cvLocations_ServerValidate"
            ValidateEmptyText="true" />

        <p>
            Pickup Date and Time:
            <asp:TextBox ID="txtPickupTime" runat="server"
                TextMode="DateTimeLocal" />
            <asp:RequiredFieldValidator
                ID="rfvPickupTime" runat="server"
                ControlToValidate="txtPickupTime"
                ErrorMessage="Pickup date and time is required."
                ForeColor="Red" />
        </p>

        <p>
            Trip Distance (km):
            <asp:TextBox ID="txtDistance" runat="server" />
            <asp:RequiredFieldValidator
                ID="rfvDistance" runat="server"
                ControlToValidate="txtDistance"
                ErrorMessage="Trip distance is required."
                ForeColor="Red" />
            <asp:RangeValidator
                ID="rvDistance" runat="server"
                ControlToValidate="txtDistance"
                Type="Double"
                MinimumValue="0.1"
                MaximumValue="500"
                ErrorMessage="Distance must be between 0.1 and 500 km."
                ForeColor="Red" />
        </p>

        <asp:Button ID="btnBook" runat="server"
            Text="Find and Book Taxi"
            OnClick="btnBook_Click" />

        <br /><br />
        <asp:Label ID="lblMessage" runat="server" />
    </form>
</body>
</html>
