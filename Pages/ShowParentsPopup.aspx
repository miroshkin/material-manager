<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowParentsPopup.aspx.cs" Inherits="Pages_ShowParentsPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="productNameLbl" runat="server" Text="" CssClass="common" Style="font-weight: bold; font-size: 14px; position: absolute; left: 5px;"></asp:Label>
        <br />
        <asp:Label ID="amountLbl" runat="server" Text="" CssClass="common" Style="position: absolute; left: 6px;"></asp:Label>
        <asp:Panel ID="Panel1" runat="server">
            <div style="top: 40px; left: 4px; position: absolute; width: 790px;">
                <div style="width: 32px;">
                </div>
                <div id="drawing_name_header" class="header-column1-show-parents-popup">
                    Чертеж/ГОСТ
                </div>
                <div id="product_name_header" class="header-column2-show-parents-popup">
                    Наименование
                </div>
            </div>


            <div id="gridview" class ="gridview-show-parents-popup">
                <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound" ShowHeader="False" CssClass="common" AutoGenerateColumns="False" GridLines="None" CellPadding="4" ForeColor="Black" OnLoad="GridView1_RowDataBound">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:ImageField DataImageUrlField="Image">
                        </asp:ImageField>
                        <asp:BoundField DataField="Чертеж" />
                        <asp:BoundField DataField="Название" />
                        <asp:BoundField DataField="ID" />
                    </Columns>
                    <EditRowStyle BackColor="#2461BF" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#E8E8E8" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                </asp:GridView>
            </div>
        </asp:Panel>
        <input type="button" class="button" value="Закрыть" id="NoButton" onclick="close_dialog('#ShowParentsPopUp');"
            style="width: 70px; text-align: center; position: absolute; bottom: 1px; right: -4px; font-size: 13px;" />
    </form>
</body>
</html>
