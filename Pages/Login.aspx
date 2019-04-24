<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table style="font-family: Verdana; font-size: 12px; position: absolute; left: 0px;">
            <tr>
                <td>
                    <asp:Label ID="Label1" Text="Пользователь:" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="user_name" runat="server" ForeColor="Black" Width="199px">
                        </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="passwordLabel" runat="server" Text="Пароль:"></asp:Label>
                </td>
                <td>
                     <asp:TextBox ID="password" runat="server" ForeColor="Black" TextMode="Password" Font-Size="11px" Width="194px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td style="font-size:11px">
                    <asp:Label ID="Message" runat="server" CssClass="error_message"  ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="LogIn" runat="server" Text="Войти" CssClass="button log_in" />
                </td>
            </tr>
        </table>
        <div></div>
    </form>
</body>
</html>
