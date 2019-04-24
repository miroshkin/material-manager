<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupTreeViewNodeContent.aspx.cs"
    Inherits="Pages_PopupTreeViewNodeContent" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Styles/Main.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="position: fixed; height: 28px; background-color: #507CD1">
    </div>
    <div class="popup_description" style="font-size: 13px; font-weight: normal; margin-right: 15px;">
        <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound1"
            Width="800px" GridLines="None" CellPadding="4" ForeColor="Black" AutoGenerateColumns="False"
            ShowHeader="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:ImageField DataImageUrlField="Image" NullImageUrl="~/Icons/box_closed_delete.png"
                    ShowHeader="False">
                </asp:ImageField>
                <asp:BoundField DataField="Чертеж" HeaderText="Чертеж">
                    <ItemStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Изделия">
                    <ItemStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ID" />
            </Columns>
            <EditRowStyle BackColor="#7C6F57" />
            <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
            <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#E8E8E8" />
            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F8FAFA" />
            <SortedAscendingHeaderStyle BackColor="#246B61" />
            <SortedDescendingCellStyle BackColor="#D4DFE1" />
            <SortedDescendingHeaderStyle BackColor="#15524A" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
