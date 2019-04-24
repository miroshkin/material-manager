<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopRightPanelButtons.ascx.cs"
    Inherits="UserControls_TopRightPanelButtons" %>
<div id="TopRightPanel" class="style1">
</div>
<asp:Panel ID="AddButtonsPanel" runat="server">
    <input type="button" class="button" value="Добавить продукцию" onclick="AddProduct('Добавить продукцию','SELL','2');"
        style="background-image: url(../Icons/package_add.png);" />
    <input type="button" class="button" value="Добавить внутреннее изделие" id="Button1"
        onclick="AddProduct('Добавить внутреннее изделие','MAKE')" style="background-image: url(../Icons/box_add.png);" />
    <input type="button" class="button" value="Добавить покупное изделие" id="Button2"
        onclick="AddProduct('Добавить покупное изделие','BUY','72')" style="background-image: url(../Icons/box_closed_add.png);" />
    <div style="height: 30px;">
    </div>
</asp:Panel>
<asp:Panel ID="ExcludeButtonPanel" runat="server"> 
<input type="button" class="button" value="Исключить продукцию" id="Button3" style="background-image: url(../Icons/box_delete.png);"
onclick="OnConfirm()"/>
</asp:Panel>
