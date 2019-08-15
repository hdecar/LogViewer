<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="LogViewer.UI._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Log Viewer</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField runat="server" ID="hfSortByColumn"/>
        <asp:HiddenField runat="server" ID="hfSortDirection"/>
        <div>
            <div>
                Page Size: <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                    <asp:ListItem>10</asp:ListItem>
                    <asp:ListItem Selected="True">15</asp:ListItem>
                    <asp:ListItem>25</asp:ListItem>
                    <asp:ListItem>50</asp:ListItem>
                    <asp:ListItem>100</asp:ListItem>
                    <asp:ListItem>500</asp:ListItem>
                </asp:DropDownList>
            &nbsp;Current Page:
                <asp:Label ID="lblCurrentPage" runat="server"></asp:Label> of <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
            </div>
            <div>
                <asp:Button ID="btnPrev" runat="server" Text="<< Prev" OnClick="btnPrev_Click" />  <asp:Button ID="btnNext" runat="server" Text="Next >>" OnClick="btnNext_Click" />
            </div>
            <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" AllowSorting="True" OnSorting="gvLogs_Sorting">
                <AlternatingRowStyle BackColor="Gainsboro" />
                <Columns>
                    <asp:BoundField DataField="DatabaseLogID" HeaderText="ID" SortExpression="DatabaseLogID" />
                    <asp:BoundField DataField="DatabaseUser" HeaderText="User" SortExpression="DatabaseUser" />
                    <asp:BoundField DataField="Event" HeaderText="Event" SortExpression="Event" />
                    <asp:BoundField DataField="Object" HeaderText="Object" SortExpression="Object" />
                    <asp:BoundField DataField="Schema" HeaderText="Schema" SortExpression="Schema" />
                    <asp:BoundField DataField="TSQL" HeaderText="TSQL" SortExpression="TSQL" />
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
