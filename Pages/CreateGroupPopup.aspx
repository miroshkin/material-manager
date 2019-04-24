<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateGroupPopup.aspx.cs"
    Inherits="Pages_AddGroupPopup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="font-family: Verdana; font-size: 13px; text-align: left; top: 50px;margin-left: -14px;">
            <tr>
                <td class="tableFieldName">
                    Название:
                </td>
                <td style="font-size: 12px;">
                    <asp:TextBox ID="groupName" runat="server" CssClass="CreateProductTextBox" style="width:307px;font-size: 13px;"></asp:TextBox>
                </td>
                <td style="color: Red;">
                    <asp:Label ID="groupNameMessage" runat="server" Text="*"></asp:Label>
                </td>
            </tr>
             <tr>
                <td class="tableFieldName">
                    Префикс:
                </td>
                <td style="font-size: 12px;">
                    <asp:TextBox ID="prefix" runat="server" CssClass="CreateProductTextBox" style="width:307px;font-size: 13px;"></asp:TextBox>
                </td>
                <td style="color: Red;">
                    <asp:Label ID="prefixMessage" runat="server" Text="**"></asp:Label>
                </td>
            </tr>
        </table>
        <div style="height: 2px; width: 444px; left: 3px; bottom: 40px; position: absolute;
            background-color: #507CD1; box-shadow: 0px 2px 4px 0 rgba(0,0,0,.2);">
        </div>
        <div>
            <asp:Panel ID="btnPanel1" runat="server">
                <input type="button" class="button" value="Создать" id="YesButton" onclick="CreateGroup();"
                    style="width: 70px; bottom: 0; right: 78px; position: absolute; text-align: center;
                    font-size: 13px;" />
                <input type="button" class="button" value="Отмена" id="NoButton" onclick="close_dialog('#CreateGroupPopUp');"
                    style="width: 70px; text-align: center; position: absolute; bottom: 0px; right: 0px;
                    font-size: 13px;" />
            </asp:Panel>
            <asp:Panel ID="btnPanel2" runat="server">
                <input type="button" class="button" value="OK" id="Button1" onclick="__doPostBack('CreateGroup','');"
                    style="width: 80px; text-align: center; position: absolute; bottom: 0px; right: 181px;
                    font-size: 13px;" />
                <div style="color: green; font-size: 13px; margin-left: -9px; margin-top: 10px;">
                    <asp:Label ID="SuccessMessage" runat="server" Text="Группа успешно добавлена"></asp:Label>
                </div>
            </asp:Panel>
            <div style="color: red; font-size: 13px; margin-left: 4px; margin-top: 10px;">
                <asp:Label ID="ErrorMessage" runat="server" Text=""></asp:Label>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="windowMarker" runat="server" />
    </form>
</body>
</html>
