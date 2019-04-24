<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddProduct.aspx.cs" Inherits="Pages_AddProduct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Styles/Main.css" />
</head>
<body>
    <form id ="form1" runat="server" style="font-family: Verdana;">
    <div id="popupTreeHeader" style="background: #507CD1; color: white; width: 300px;
        position: absolute; height: 20px; left: 0px; font-size: 14px; text-align: center;
        padding-top: 2px; border-style: solid; border-color: #1a3f88; border-width: 1px;top:7px;">
        Группы изделий
    </div>
    <div id="popup_treeview" class="popup_window_treeview" onscroll="document.getElementById('popup_treeview_vertical_scroll').value = this.scrollTop;">
        <asp:TreeView ID="TreeView2" runat="server" NodeWrap="false" CssClass="popupTreeViewPosition">
            <NodeStyle ForeColor="Black" HorizontalPadding="5" VerticalPadding="3" />
            <HoverNodeStyle CssClass="hover_node" ForeColor="White" />
            <SelectedNodeStyle CssClass="SelectedNodeStyle"/>
        </asp:TreeView>
    </div>
    <div id="Div1" style="background: #507CD1; color: white; width: 714px; position: absolute;
        left: 308px; height: 20px; font-size: 14px; text-align: center; padding-top: 2px;
        border-bottom: solid 1px; border-style: solid; border-color: #1a3f88; border-width: 1px;top:7px;">
        Изделие
    </div>
    <div id="popup_description" class="popup_window_productDescription" style="margin-right: 15px;">
    </div>
    <br />
    <div id="Buttons" style="width: 250px; height: 10px;">
        <input type="button" class="cancel_button cancel-button-add-product" value="Отмена" id="CancelButton" onclick="close_dialog('#AddProduct');"/>
    </div>
    <asp:HiddenField ID="popup_treeview_vertical_scroll" runat="server" />
    </form>
</body>
</html>
