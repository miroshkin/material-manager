<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateAndEditProduct.aspx.cs"
    Inherits="Pages_CreateAndEditProduct" %>

<%@ Register Src="../UserControls/header.ascx" TagName="header" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/footer.ascx" TagName="footer" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/TopRightPanelButtons.ascx" TagName="TopRightPanelButtons"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>РЕДАКТОР ИЗДЕЛИЙ</title>
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/MyJavaScript.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../Styles/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/jquery-ui.structure.min.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="/Styles/Main.css" />
    <script type="text/javascript">
        //Обрабатываем нажатие клавиши enter
        function OnEnterPress() {
            $(this).keypress(function (e) {
                //Нажатая клавиша enter
                if (e.which == 13) {
                    switch ($('#windowMarker').val()) {
                        case "Save":
                            SaveChanges();
                            break;
                        case "SaveOK":
                            postBack();
                            break;
                        case "Create":
                            CreateProduct();
                            break;
                        case "CreateOK":
                            __doPostBack('', '');
                            break;
                        case "RemoveOK":
                            __doPostBack('RemoveProduct', $('#productID').val());
                            break;
                    }
                }
            });
        }

        //Закрываем диалоговые окна
        function close_dialog(dialogName) {
            $(dialogName).dialog('close');
        }

        //Затемнение для открытия окна диалога
        function fade_in() {
            if ($("div").is(".overlay") == false) {
                $('<div class="ui-widget-overlay" id="overlay"/>').appendTo('body').show({ effect: 'fade', duration: 50 });
            }
        }

        //Осветление после закрытия диалога 
        function fade_out() {
            if ($("div").is(".overlay") == false) {
                $('#overlay').fadeOut(50, function () { $('#overlay').remove(); });
            }
        }

        function postBack() {
            __doPostBack('', '');
        }

        function __doPostBack(eventTarget, eventArgument) {
            document.Form1.__EVENTTARGET.value = eventTarget;
            document.Form1.__EVENTARGUMENT.value = eventArgument;
            document.Form1.submit();
        }

        //Выводим модальное окно для создания изделия
        function CreateProduct() {
            //Название изделия
            var productName = encodeURIComponent($('#productName').val());

            //Чертеж/ГОСТ изделия
            var drawing = encodeURIComponent($('#drawing').val());

            //Цена изделия
            var price = encodeURIComponent($('#price').val());

            //Идентификатор группы изделия
            var groupID = encodeURIComponent($('#hiddenGroupID').val());

            //Идентификатор типа изделия
            var typeID = encodeURIComponent($('#hiddenTypeID').val());

            $('#CreateProductPopUp').dialog('close');
            fade_in();

            $('<div title="Создание изделия" id="CreateProductPopUp"/>').appendTo('body').load("/Pages/CreateProductPopUp.aspx?productName=" + productName
            + "&drawing=" + drawing + "&price=" + price
            + "&groupID=" + groupID + "&typeID=" + typeID).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: false,
                width: 450,
                height: 235,
                close: function (event, ui) {
                    $('#CreateProductPopUp').remove();
                },
                open: function () {
                }
            });
        }

        //Выводим модальное окно описания продукта с возможностью редактирования
        function EditProduct(ID) {

            $('#EditProductPopUp').dialog('close');
            fade_in();

            $('<div title="Редактирование изделия" id="EditProductPopUp"/>').appendTo('body').load("/Pages/EditProductPopUp.aspx?ID=" + ID + "&action=LoadProductionInfo").dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: false,
                width: 580,
                height: 340,
                close: function (event, ui) {
                    $('#EditProductPopUp').remove();
                },
                open: function () {
                    $('#productID').val(ID);
                }
            });
        }

        

        function ChangeProductionType(ID) {

            //Название изделия
            var productName = encodeURIComponent($('#productName').val());

            //Чертеж/ГОСТ изделия
            var drawing = encodeURIComponent($('#drawing').val());

            //Цена изделия
            var price = encodeURIComponent($('#price').val());

            //Тип изделия
            var e = document.getElementById("productionArticlesTypes");
            var typeID = e.options[e.selectedIndex].value;

            $('#EditProductPopUp').dialog('close');
            fade_in();

            $('<div title="Редактирование изделия" id="EditProductPopUp"/>').appendTo('body').load("/Pages/EditProductPopUp.aspx?ID=" + ID + "&typeID=" + typeID + "&action=ChangeProductionType"
            + "&price=" + price + "&drawing=" + drawing + "&productName=" + productName).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: false,
                width: 580,
                height: 340,
                close: function (event, ui) {
                    $('#EditProductPopUp').remove();
                },
                open: function () {
                    $('#productID').val(ID);
                }
            });
        }


        function SaveChanges() {

            //Идентификатор изменяемого изделия
            var ID = $('#productID').val();

            //Тип изменяемого изделия
            var e = document.getElementById("productionArticlesTypes");
            var typeID = e.options[e.selectedIndex].value;

            //Группа изменяемого изделия
            var d = document.getElementById("productionArticlesGroups");
            var groupID = d.options[d.selectedIndex].value;

            //Наименование изменяемого изделия
            var productName = encodeURIComponent($('#productName').val());

            //Чертеж/ГОСТ изменяемого изделия
            var drawing = encodeURIComponent($('#drawing').val());

            //Цена изменяемого изделия
            var price = encodeURIComponent($('#price').val());

            $('#EditProductPopUp').dialog('close');
            fade_in();

            $('<div title="Редактирование изделия" id="EditProductPopUp"/>').appendTo('body').load("/Pages/EditProductPopUp.aspx?action=SaveChanges" +
             "&ID=" + ID + "&typeID=" + typeID + "&groupID=" + groupID + "&productName=" + productName + "&drawing=" + drawing + "&price=" + price).dialog({
                 beforeClose: function (event, ui) {
                     fade_out();
                 },
                 show: { effect: 'fade', duration: 300 },
                 hide: { effect: 'fade', duration: 300 },
                 closeOnEscape: true,
                 draggable: true,
                 resizable: false,
                 width: 580,
                 height: 340,
                 close: function (event, ui) {
                     $('#EditProductPopUp').remove();
                 },
                 open: function () {
                 }
             });
        }

        //Выводим окно ошибки
        function Error(message) {
            fade_in();
            var encoded = encodeURIComponent(message);
            $('<div title="Ошибка" id="Error"/>').appendTo('body').load("/Pages/Error.aspx?message=" + encoded).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: true,
                width: 400,
                height: 150,
                close: function (event, ui) {
                    $('#Error').remove();
                },
                open: function () {
                }
            });
        }

        function close_error_dialog() {
            $('#Error').dialog('close');
        }

        function DisableMenuItem() {
            $('#ProductEditorMenuItem').css('background-color', '#999999');
            $('#ProductEditorMenuItem').prop("onclick", null);
        }

        $(function () {
            $(".resizable1").resizable(
            {
                autoHide: true,
                handles: 'e',
                resize: function (e, ui) {
                    var parent = ui.element.parent();
                    //Вычисляем ширину элементов левой части страницы
                    var left_side_width = ui.element.width() - 18;
                    var dropdown_width = $('#left_side').width() - 16;
                    //Устанавливаем  ширину элементов левой части страницы
                    document.getElementById('dropdown').style.width = dropdown_width + "px";
                    document.getElementById('tree_container').style.width = left_side_width + "px";

                    //Вычисляем ширину описания продуктов
                    var product_info_width = $('#left_side').parent().width() - $('#left_side').width() - 26;

                    //Устанавливаем ширину
                    document.getElementById('productInfo').style.width = product_info_width + "px";

                    var remainingSpace = parent.width() - ui.element.outerWidth(),
                        divTwo = ui.element.next(),
                        divTwoWidth = (remainingSpace - (divTwo.outerWidth() - divTwo.width())) / parent.width() * 100 + "%";
                    divTwo.width(divTwoWidth);
                },
                stop: function (e, ui) {
                    var parent = ui.element.parent();
                    ui.element.css(
                    {
                        width: ui.element.width() / parent.width() * 100 + "%",
                    });
                    //Сохраняем положение позиции сплиттера
                    setCookie('splitter_position', ui.element.width() / parent.width() * 100, 5);
                }
            });
        });

        function getCookie(cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1);
                if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
            }
            return "";
        }

        function setCookie(cname, cvalue, exdays) {
            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            var expires = "expires=" + d.toUTCString();
            document.cookie = cname + "=" + cvalue + "; " + expires;
        }

        function SetElementsSizes() {
            var splitter_position = getCookie('splitter_position');
            $('#left_side').css('width', splitter_position + "%");
            $('#right_side').css('width', 100 - splitter_position + "%");
            var left_side_width = $('#left_side').width() - 18;
            var dropdown_width = $('#left_side').width() - 16;
            document.getElementById('dropdown').style.width = dropdown_width + "px";
            document.getElementById('tree_container').style.width = left_side_width + "px";

            var product_info_width = $('#left_side').parent().width() - $('#left_side').width() - 26;
            document.getElementById('productInfo').style.width = product_info_width + "px";
        }

        //Обработчик изменения окна браузера
        $(window).resize(function () {
            var left_side_width = $('#left_side').width() - 18;
            var dropdown_width = $('#left_side').width() - 16;
            document.getElementById('dropdown').style.width = dropdown_width + "px";
            document.getElementById('tree_container').style.width = left_side_width + "px";
            var product_info_width = $('#left_side').parent().width() - $('#left_side').width() - 26;
            document.getElementById('productInfo').style.width = product_info_width + "px";

        });

        $(function () {
            var user_is_authentificated = getCookie("UserIsAuthentificated");
            if (user_is_authentificated != 'true') {
                $('#CreateProductBtn').hide();
                $("#tree_container").css("bottom", "7px");
                $("#grid_view_wrapper").css("bottom", "-73px");
                
            }
        })

    </script>

    <style type="text/css">
            .td {
                text-align: left;
            }

            .auto-style1 {
                width: 24px;
                height: 24px;
            }
        </style>
</head>
<body onload="showPageLoadingGif();document.getElementById('tree_container').scrollTop = document.getElementById('treeview_vertical_scroll').value;DisableMenuItem(); OnEnterPress();SetElementsSizes();AddLoadingGif();">
    
    <%-- Показываем gif загрузки страницы через Postback --%>
    <div id="loading" style="position:absolute; width:100%;height:100%; left:50%;top:50%;">
    <img src="../Icons/ajax-loader.gif" alt="Loading..."/></div>
    <script type="text/javascript">
        var ld = (document.all);
        var ns4 = document.layers;
        var ns6 = document.getElementById && !document.all;
        var ie4 = document.all;
        if (ns4)
            ld = document.loading;
        else if (ns6)
            ld = document.getElementById("loading").style;
        else if (ie4)
            ld = document.all.loading.style;
    </script>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div id="header" class="header">
            <uc2:header ID="header1" runat="server" />
        </div>
        <div class="sectionName">
            РЕДАКТОР ИЗДЕЛИЙ
        </div>
        
        <asp:HiddenField ID="productID" runat="server" />
        <asp:HiddenField ID="hiddenGroupID" runat="server" />
        <asp:HiddenField ID="hiddenTypeID" runat="server" />

        <div class="wrap">
            <div id="left_side" class="resizable resizable1">

                <div id="tree_container" class="tree_container_style" onscroll="document.getElementById('treeview_vertical_scroll').value = this.scrollTop;">

                    <div class="treeview" id="treeview">
                        <asp:TreeView ID="TreeView1" runat="server" NodeWrap="false" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
                            <NodeStyle ForeColor="Black" HorizontalPadding="5" VerticalPadding="3" />
                            <HoverNodeStyle CssClass="hover_node" ForeColor="White" />
                            <SelectedNodeStyle Font-Bold="true" />
                        </asp:TreeView>
                    </div>
                </div>

                <div id="dropdown"
                    class="dropdown_style">
                    <asp:DropDownList ID="ArticleTypesList" runat="server" Height="25px" AutoPostBack="true"
                        Font-Names="Verdana" CssClass="productTypeDropdown" Font-Bold="False" OnSelectedIndexChanged="ArticleTypesList_SelectedIndexChanged">
                        <asp:ListItem Value="2">Внутренние изделия</asp:ListItem>
                        <asp:ListItem Selected="True" Value="3">Покупные изделия</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <input type="hidden" id="treeview_vertical_scroll" runat="server" />
            <asp:HiddenField ID="scroll2value" runat="server" />
            <asp:HiddenField ID="SelectedNodeID" runat="server" />
            <asp:HiddenField ID="ParentNodeID" runat="server" />

            <div id="right_side" class="resizable resizable2">
                <div id="productInfo" class="productDescription">
                    <div>
                        <asp:Panel ID="Panel1" runat="server">
                            <div style="top: 40px; position: absolute; width: 100%;">
                                <div style="width: 32px;">
                                </div>
                                <div id="drawing_name_header" style="width: 323px; position: absolute; top: 1px; left: 0px; font-size: 12px; background-color: #507CD1; color: White; text-align: center; height: 20px; padding-top: 4px; z-index: 90;">
                                    Drawing
                                </div>
                                <div id="product_name_header" style="margin-top: 1px; margin-left: 324px; margin-right: -2px; font-size: 12px; background-color: #507CD1; color: White; text-align: center; height: 20px; padding-top: 4px; z-index: 90;">
                                    Product name
                                </div>
                            </div>
                            <div id="grid_view_wrapper" style="font-size: 12px; font-weight: normal; border: solid 1px grey; overflow-y: scroll; overflow-x: hidden; /*box-shadow: 0px 2px 4px 0 rgba(0,0,0,.2);*/ top: 65px; position: absolute; border-top: none; width: 100%; bottom: -35px;">
                                <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound1"
                                    GridLines="None" CellPadding="4" ForeColor="Black" AutoGenerateColumns="False"
                                    ShowHeader="False" Style="margin-top: 0px">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:ImageField DataImageUrlField="Image" NullImageUrl="~/Icons/box_closed_delete.png">
                                        </asp:ImageField>
                                        <asp:BoundField DataField="Чертеж" HeaderText="Чертеж">
                                            <ItemStyle Wrap="False" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Изделия">
                                            <ItemStyle Wrap="False" />
                                        </asp:BoundField>
                                        <asp:ImageField DataImageUrlField="Text_tree" NullImageUrl="~/Icons/text_tree.png">
                                        </asp:ImageField>
                                        <asp:BoundField DataField="ID" />
                                    </Columns>
                                    <EditRowStyle BackColor="#7C6F57" />
                                    <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
                                    <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#E8E8E8" />
                                    <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                    <SortedAscendingHeaderStyle BackColor="#246B61" />
                                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                    <SortedDescendingHeaderStyle BackColor="#15524A" />
                                </asp:GridView>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <asp:Panel ID="AddInnerProductButton" runat="server">
                    <input type="button" class="button" value="Создать изделие" id="YesButton" onclick="CreateProduct('Создать изделие', 'SELL', '2');"
                        style="width: 133px; text-align: center; bottom: 35px; right: -19px; text-align: left; background-position: 129px; position: fixed; bottom: 25px; right: 4px;" />
                </asp:Panel>
                <asp:Panel ID="AddOuterProductButton" runat="server">
                    <input type="button" class="button" value="Создать изделие" id="CreateProductBtn" onclick="CreateProduct('Создать изделие', 'SELL', '2');"
                        style="width: 133px; text-align: center; bottom: 35px; right: -19px; text-align: left; background-position: 129px; position: fixed; bottom: 25px; right: 3px;" />
                </asp:Panel>
            </div>
        </div>
        <div class="modal"><!-- Place at bottom of page --></div>
        <div id="footer" class="footer">
            <uc1:footer ID="footer1" runat="server" />
        </div>
    </form>
</body>
</html>
