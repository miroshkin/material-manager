<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Menu.aspx.cs" Inherits="Pages_Menu" %>
<%@ Register Src="../UserControls/header.ascx" TagName="header" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/footer.ascx" TagName="footer" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../Styles/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/jquery-ui.structure.min.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="/Styles/Main.css" />

     <style type="text/css">
    .menu_items
    {
        font-size:14px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>

   

    <uc1:footer ID="footer1" runat="server" />
        <asp:Menu ID="Menu1" runat="server" BackColor="#507CD1" BorderStyle="None" 
            ForeColor="White">
            <DynamicHoverStyle BackColor="#FFCC00" />
            <Items>
                <asp:MenuItem Text="Менеджер изделий" Value="1" NavigateUrl="StartPage.aspx" ToolTip="Tooltip" ImageUrl = "../Icons/box_add.png"></asp:MenuItem>
                <asp:MenuItem Text="Редактор изделий" Value="2" NavigateUrl="CreateAndEditProduct.aspx"></asp:MenuItem>
                <asp:MenuItem Text="Редактор групп" Value="3" NavigateUrl="CreateAndEditGroup.aspx"></asp:MenuItem>
                <asp:MenuItem Text="Поиск" Value="3"></asp:MenuItem>
            </Items>
            <StaticHoverStyle BackColor="Red" />
            <StaticMenuItemStyle Height="25px" Width="500px" />
        </asp:Menu>
    <uc2:header ID="header" runat="server" />
    </div>
    </form>
</body>
</html>
