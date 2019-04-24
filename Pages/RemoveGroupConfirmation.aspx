<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RemoveGroupConfirmation.aspx.cs"
    Inherits="Pages_RemoveGroupConfirmation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="message" style="font-size: 14px; margin-top: 10px;">
        <table>
            <tr>
                <td>
                    <img src="../Icons/warning.png" />
                </td>
                <td>
                    <div style="padding-left: 15px;" ID="confirmationMessage">Вы уверены, что хотите исключить следующую группу?</div>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="padding-left: 15px;">
                    <br />
                    <asp:Label ID="deletingGroupName" runat="server" Text="" CssClass="confirmation_product_name"></asp:Label>
                    <asp:HiddenField ID="deletingGroupID" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div style="height: 2px; width: 393px; left: 3px; bottom: 40px; position: absolute;
        background-color: #507CD1; box-shadow: 0px 2px 4px 0 rgba(0,0,0,.2);">
    </div>
    <input type="button" class="button" value="Да" id="YesButton" onclick="__doPostBack('RemoveGroup', $('#deletingGroupID').val());"
        style="width: 70px; bottom: 0; right: 79px; position: absolute; text-align: center;
        font-size: 13px;" />
    <input type="button" class="button" value="Нет" id="NoButton" onclick="close_dialog('#Confirmation')"
        style="width: 70px; text-align: center; position: absolute; bottom: 0px; right: 0px;
        font-size: 13px;" />
        <asp:HiddenField ID="windowMarker" runat="server" />
    </form>
</body>
</html>
