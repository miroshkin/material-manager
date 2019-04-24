<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Pages_Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="message" style="font-size: 14px; margin-top: 10px; vertical-align:middle">
    <table>
    <tr>
    <td>
    <img src="../Icons/delete.png" />
    </td>
    <td style="padding-left:15px;">
    <asp:Label ID="messageLabel" runat="server" Text="Label"></asp:Label></td>
    </tr>
    </table>
    </div>
    <input type="button" class="button" value="OK" id="NoButton" onclick="close_dialog('#Error');"
        style="width: 70px; 
        text-align: center; position: absolute; left: 160px;bottom: 1px; font-size:12px;" />
    </form>
</body>
</html>
