<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProductEditor.aspx.cs" Inherits="Pages2_ProductEditor2" %>

<%@ Register Src="../UserControls/header.ascx" TagName="header" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/footer.ascx" TagName="footer" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/TopRightPanelButtons.ascx" TagName="TopRightPanelButtons"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>МОСКОВСКИЙ ЭНЕРГОМЕХАНИЧЕСКИЙ ЗАВОД</title>
    <link href="../Styles/Main.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="header">
        <asp:PlaceHolder ID="PlaceHolder2" runat="server">
            <uc2:header ID="header1" runat="server" />
        </asp:PlaceHolder>
    </div>
    <div>
        <uc3:TopRightPanelButtons ID="TopRightPanelButtons1" runat="server" />
    </div>
    <div id="footer" class="footer">
        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
            <uc1:footer ID="footer1" runat="server" />
        </asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
