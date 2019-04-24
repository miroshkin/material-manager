<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditGroupPopup.aspx.cs" Inherits="Pages_EditGroupPopup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        
        function OnConfirm() {
            //Имя удаляемой группы
            var groupName = encodeURIComponent($('#groupName').val());
            //Идентификатор удаляемой группы
            var groupID = $('#groupID').val();

            $('#EditGroupPopUp').dialog('close');
            fade_in();

            $('<div title="Подтверждение" id="Confirmation"/>').appendTo('body').load("/Pages/RemoveGroupConfirmation.aspx?groupName=" + groupName + "&groupID=" + groupID).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: true,
                width: 400,
                height: 240,
                close: function (event, ui) {
                    $('#Confirmation').remove();
                },
                open: function () {
                }
            });
        }
        //Передаем идентификатор группы, в которую будет добавлена/перемещена группа
        function CreateGroupPostBack() {
            var d = document.getElementById("productionArticlesGroups");
            var parentGroupID = d.options[d.selectedIndex].value;
            __doPostBack('CreateGroup', parentGroupID);
        }

        $(function () {
            var user_is_authentificated = getCookie("UserIsAuthentificated");
            if (user_is_authentificated != 'true') {
                $('#SaveChangesBtn').hide();
                $("#prefixLabel").prop("disabled", true);
                $("#productionArticlesGroups").prop("disabled", true);
                $("#groupName").prop("disabled", true);
                $("#groupName").prop("disabled", true);
            }
        })

    </script>

     <style type="text/css">
        .CreateProductTextBox
        {
            width: 419px;
        }
    </style>
</head>
<body onload = "OnEnterPress();">
    <form id="form1" runat="server">
   
    <asp:HiddenField ID="productID" runat="server" />
    <table style="font-family: Verdana; font-size: 13px; text-align: left; top: 50px;
        margin-left: -13px;">
        
        <tr>
            <td class="tableFieldName">
                Название:
            </td>
            <td style="font-size: 13px;">
                <asp:TextBox ID="groupName" runat="server" CssClass="CreateProductTextBox" Style="width: 414px;
                    margin-left: 27px;"></asp:TextBox>
            </td>
            <td style="color: Red;">
                <asp:Label ID="groupNameMessage" runat="server" Text="*"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tableFieldName">
                Префикс:
            </td>
            <td style="font-size: 13px;">
                <asp:TextBox ID="prefixLabel" runat="server" CssClass="CreateProductTextBox" Style="width: 414px;
                    margin-left: 27px;"></asp:TextBox>
            </td>
            <td style="color: Red;">
                <asp:Label ID="prefixMessage" runat="server" Text="**"></asp:Label>
                </td>
        </tr>
    </table>
    
    
    <table style="font-family: Verdana; font-size: 13px; text-align: left; top: 50px;
        margin-left: -13px;">
        <tr>
            <td class="tableFieldName">
                Родительская группа:
            </td>
            <td>
                <asp:DropDownList ID="productionArticlesGroups" runat="server" CssClass="CreateProductTextBox"
                    Style="width: 418px; margin-left: 7px; height: 22px;margin-top: -13px;">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    
    <asp:HiddenField ID="windowMarker" runat="server" />
    <div class="border-550px"></div>
    <div>
        <asp:Panel ID="btnPanel1" runat="server">
            <input type="button" class="button save-button-edit-group-popup" value="Сохранить" id="SaveChangesBtn" onclick="SaveChanges();"/>
            <input type="button" class="button cancel-button-edit-group-popup" value="Отмена" id="NoButton" onclick="close_dialog('#EditGroupPopUp');"/>
            <asp:Panel ID="deleteButtonPanel" runat="server">
                <input type="button" class="button" value="Удалить группу" id="Button2" onclick="OnConfirm();"
                    style="text-align: center; font-size: 13px; position: absolute; bottom: 0px;
                    left: 0px; width: 130px; right: 1066px;" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="btnPanel2" runat="server">
            <input type="button" class="button" value="OK" id="Button1" onclick="CreateGroupPostBack();"
                style="width: 90px; text-align: center; position: absolute; bottom: 0px; right: 217px;
                font-size: 13px;" />
            <div style="color: green; font-size: 13px; margin-left: 4px; margin-top: 10px;">
                <asp:Label ID="SuccessMessage" runat="server" Text="Группа успешно изменена"></asp:Label>
            </div>
        </asp:Panel>
        <asp:HiddenField ID="groupID" runat="server" />
        <div style="color: red; font-size: 13px; margin-left: -9px; margin-top: 10px;">
            <asp:Label ID="ErrorMessage" runat="server" Text=""></asp:Label><br />
        </div>
    </div>
    </form>
</body>
</html>
