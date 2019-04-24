<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Confirmation.aspx.cs" Inherits="Pages_Confirmation" %>

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
                    <div style="padding-left: 15px;">
                        Вы уверены, что хотите исключить следующее изделие?
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="padding-left: 15px;">
                    <br />
                    <asp:Label ID="deletingProductName" runat="server" Text="" CssClass="confirmation_product_name"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div class="border-400px">
    </div>
    <input type="button" class="button yes-button-confirmaion" value="Да" id="YesButton" onclick="ExcludeProduct();"/>
    <input type="button" class="button no-button-confirmaion" value="Нет" id="NoButton" onclick="close_dialog('#Confirmation');"/>
    </form>
</body>
</html>
