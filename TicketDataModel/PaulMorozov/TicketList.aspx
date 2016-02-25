<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TicketList.aspx.cs" Inherits="PaulMorozov.TicketList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="TicketForm" runat="server">
    <div>
        <asp:DataGrid runat="server" ID="dgrList">

        </asp:DataGrid>
        <asp:Button ID="CreateNewTicket" runat="server" Onclick="CreateNewTicket_Click" />
    </div>
    </form>
</body>
</html>
