<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RemoveProductConfirmation.aspx.cs"
    Inherits="Pages_RemoveProductConfirmation" %>

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
                        Вы уверены, что хотите удалить следующее изделие?
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
            <tr>
                <td>
                </td>
                <td style="padding-left: 15px;">
                    <br />
                    Изделий, использующих данное изделие:<asp:Label ID="ParentProductsLabel" runat="server"
                        Text="" CssClass="bold_text"></asp:Label>
                </td>
            </tr>
        </table>
        <div class="border-400px">
        </div>
    </div>
    <input type="button" class="button yes-button-remove-product-confirmation" value="Да" id="YesButton" onclick="__doPostBack('RemoveProduct',$('#productID').val());"
        />
    <input type="button" class="button no-button-remove-product-confirmation" value="Нет" id="NoButton" onclick="close_dialog('#Confirmation')"
        />
        <asp:HiddenField ID="windowMarker" runat="server" Value="RemoveOK" />
    </form>
</body>
</html>