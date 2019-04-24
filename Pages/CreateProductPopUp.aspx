<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateProductPopUp.aspx.cs"
    Inherits="Pages_CreateProduct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="font-family: Verdana; font-size: 13px; text-align: left; top: 50px;">
                <tr>
                    <td class="tableFieldName">
                        Наименование:
                    </td>
                    <td style="font-size: 12px;">
                        <asp:TextBox ID="productName" runat="server" CssClass="CreateProductTextBox" style="width:273px;"></asp:TextBox>
                    </td>
                    <td style="color: Red;">
                        <asp:Label ID="productNameMessage" runat="server" Text="*"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tableFieldName">
                        Чертёж/ГОСТ:
                    </td>
                    <td style="font-size: 12px;">
                        <asp:TextBox ID="drawing" runat="server" CssClass="CreateProductTextBox" style="width:273px;"></asp:TextBox>
                    </td>
                    <td style="color: Red;">
                    <asp:Label ID="drawingMessage" runat="server" Text="**"></asp:Label>
                      </tr>
                <tr>
                    <td class="tableFieldName">
                        Цена(руб.,коп.):
                    </td>
                    <td style="font-size: 12px;">
                        <asp:TextBox ID="price" runat="server" CssClass="CreateProductTextBox" style="width:273px;"></asp:TextBox>
                    </td>
                    <td style="color: Red;">
                        <asp:Label ID="priceMessage" runat="server" CssClass="CreateProductTextBox" Text="***"></asp:Label>
                    </td>
                </tr>
            </table>
            <div style="height: 2px; width: 450px; left: 0px; bottom: 40px; position: absolute;
                background-color: #507CD1; /*box-shadow: 0px 2px 4px 0 rgba(0,0,0,.2);*/">
            </div>
            <div>
                <asp:Panel ID="btnPanel1" runat="server">
                <input type="button" class="button" value="Создать" id="YesButton" onclick="CreateProduct();"
                    style="width: 70px; bottom: 1px; right: 71px; position: absolute; text-align: center;
                    font-size: 13px;" />
                <input type="button" class="button" value="Отмена" id="NoButton" onclick="close_dialog('#CreateProductPopUp');"
                    style="width: 70px; text-align: center; position: absolute; bottom: 1px; right: -4px;
                    font-size: 13px;" />
                </asp:Panel>
                <asp:Panel ID="btnPanel2" runat="server">
                <input type="button" class="button" value="OK" id="Button1" onclick="__doPostBack('','');"
                    style="width: 80px; text-align: center; position: absolute; bottom: 0px; right: 181px;
                    font-size: 13px;" />

                    <div style="color: green; font-size: 13px; margin-left: 4px; margin-top: 10px;">
                    <asp:Label ID="SuccessMessage" runat="server" Text="Изделие успешно добавлено"></asp:Label>
                </div>

                </asp:Panel>
                
                <div style="color: red; font-size: 13px; margin-left: 4px; margin-top: 10px;">
                    <asp:Label ID="ErrorMessage1" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="ErrorMessage2" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="ErrorMessage3" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="windowMarker" runat="server" />
    </form>
</body>
</html>
