<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StartPage.aspx.cs" Inherits="StartPage" %>

<%@ Register Src="../UserControls/header.ascx" TagName="header" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/footer.ascx" TagName="footer" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/TopRightPanelButtons.ascx" TagName="TopRightPanelButtons"
    TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MATERIAL MANAGER</title>
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/MyJavaScript.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../Styles/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/jquery-ui.structure.min.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        //Закрываем диалоговые окна
        function close_dialog(dialogName) {
            $(dialogName).dialog('close');
        }

        function fade_in() {
            if ($("div").is(".overlay") == false) {
                $('<div class="ui-widget-overlay" id="overlay"/>').appendTo('body').show({ effect: 'fade', duration: 50 });
            }
        }

        function fade_out() {
            if ($("div").is(".overlay") == false) {
                $('#overlay').fadeOut(50, function () { $('#overlay').remove(); });
            }
        }

        function AddProduct(header, productType, groupID) {
            fade_in();
            $('#AddProduct').dialog('close');

            $('<div title="' + header + '" id="AddProduct"/>').appendTo('body').load("/Pages/AddProduct.aspx?productType=" + productType + "&groupID=" + groupID + "&scroll2value=" + document.getElementById('scroll2value').value).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: false,
                width: 1024,
                height: 600,
                close: function (event, ui) {
                    $('#AddProduct').remove();
                },
                open: function () {
                }
            });
        }

        function FormSpecification(header, productID) {
            fade_in();



            $('#FormSpecification').dialog('close');


            $('<div title="' + header + '" id="FormSpecification"/>').appendTo('body').load("/Pages/FormSpecification.aspx?productID=" + productID + "&scroll2value=" + document.getElementById('scroll2value').value).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: false,
                width: 1024,
                height: 600,
                close: function (event, ui) {
                    $('#FormSpecification').remove();
                },
                open: function () {

                }
            });
        }

        function ExcludeProduct() {
            __doPostBack('ExcludeProduct', document.getElementById('ParentNodeID').value);
        }

        function AddNodeContent(productType, groupID) {
            $("#popup_description").load("/Pages/PopupTreeViewNodeContent.aspx?productType=" + productType + "&groupID=" + groupID + "&outerProductID=" + document.getElementById('SelectedNodeID').value);
        }

        function OnConfirm() {
            fade_in();
            var encoded = encodeURIComponent($('#ProductName').html());
            $('<div title="Подтверждение" id="Confirmation"/>').appendTo('body').load("/Pages/Confirmation.aspx?productName=" + encoded).dialog({
                beforeClose: function (event, ui) {
                    fade_out();
                },
                show: { effect: 'fade', duration: 300 },
                hide: { effect: 'fade', duration: 300 },
                closeOnEscape: true,
                draggable: true,
                resizable: false,
                width: 400,
                height: 250,
                close: function (event, ui) {
                    $('#Confirmation').remove();
                },
                open: function () {
                }
            });
        }

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
                resizable: false,
                width: 400,
                height: 180,
                close: function (event, ui) {
                    $('#Error').remove();
                },
                open: function () {
                }
            });
        }


        function DisableMenuItem() {
            $('#ProductManagerMenuItem').css('background-color', '#999999');
            $('#ProductManagerMenuItem').prop("onclick", null);
        }

        $(function () {
            $(".resizable1").resizable(
            {
                autoHide: true,
                handles: 'e',
                resize: function (e, ui) {
                    var parent = ui.element.parent();

                    var left_side_width = ui.element.width() - 18 + "px";
                    var dropdown_width = ui.element.width() - 16 + "px";
                    document.getElementById('dropdown').style.width = dropdown_width;
                    document.getElementById('tree_container').style.width = left_side_width;

                    var remainingSpace = parent.width() - ui.element.outerWidth(),
                        divTwo = ui.element.next(),
                        divTwoWidth = (remainingSpace - (divTwo.outerWidth() - divTwo.width())) / parent.width() * 100 + "%";
                    divTwo.width(divTwoWidth);
                    var delimiter_width = remainingSpace - 18 + "px";
                    try {
                        document.getElementById('delimiter').style.width = delimiter_width;
                    }
                    catch (err) {
                    }
                    try {
                        if (remainingSpace < 576) {
                            document.getElementById('SaveChangesButton').style.top = "60px";
                            document.getElementById('SaveChangesButton').style.left = "-4px";
                        }
                        else {
                            document.getElementById('SaveChangesButton').style.top = "-8px";
                            document.getElementById('SaveChangesButton').style.left = "350px";
                        }
                    }
                    catch (err) {
                    }

                },
                stop: function (e, ui) {
                    var parent = ui.element.parent();
                    ui.element.css(
                    {
                        width: ui.element.width() / parent.width() * 100 + "%",
                    });
                    //Сохраняем положение позиции сплиттера
                    setCookie('splitter_position', ui.element.width() / parent.width() * 100, 365);
                }
            });
        });
        

        function SetElementsSizes() {
            var splitter_position = getCookie('splitter_position');
            $('#left_side').css('width', splitter_position + "%");
            $('#right_side').css('width', 100 - splitter_position + "%");
            var left_side_width = $('#left_side').width() - 18;
            var dropdown_width = $('#left_side').width() - 16;
            document.getElementById('dropdown').style.width = dropdown_width + "px";
            document.getElementById('tree_container').style.width = left_side_width + "px";

            //Вычисляем ширину разделителя
            var delimiter_width = $('#left_side').parent().width() - $('#left_side').width() - 18;

            try {
                document.getElementById('delimiter').style.width = delimiter_width + "px";
            }
            catch (err) {
            }

            var right_side_width = $('#left_side').parent().width() - $('#left_side').width();
            try {
                if (remainingSpace < 576) {
                    document.getElementById('SaveChangesButton').style.top = "60px";
                    document.getElementById('SaveChangesButton').style.left = "-4px";
                }
                else {
                    document.getElementById('SaveChangesButton').style.top = "-8px";
                    document.getElementById('SaveChangesButton').style.left = "350px";
                }
            }
            catch (err) {
            }
        }

        //Обработчик изменения окна браузера
        $(window).resize(function () {
            var left_side_width = $('#left_side').width() - 18;
            var right_side_width = $('#left_side').parent().width() - $('#left_side').width() - 18;
            var dropdown_width = $('#left_side').width() - 16;
            document.getElementById('dropdown').style.width = dropdown_width + "px";
            document.getElementById('tree_container').style.width = left_side_width + "px";
            try {
                document.getElementById('delimiter').style.width = right_side_width + "px";
            }
            catch (err) {
            }
        });

        $(function () {
            var user_is_authentificated = getCookie("UserIsAuthentificated");
            if (user_is_authentificated != 'true') {
                $('#TopRightPanel').hide();
                $('#CutButton').hide();
                $('#PasteButton').hide();
                $('#SaveChangesButton').hide();
                $("#AmountTextBox").prop("disabled", true);
                $("#AmountDropDownList").prop("disabled", true);
            }
        })
        
    </script>
</head>
<body class="body" onload="showPageLoadingGif();document.getElementById('tree_container').scrollTop = document.getElementById('treeview_vertical_scroll').value;
    DisableMenuItem();SetElementsSizes();AddLoadingGif();">
    <%-- Показываем gif загрузки страницы через Postback --%>
    <div id="loading" style="position: absolute; width: 100%; height: 100%; left: 50%; top: 50%;">
        <img src="../Icons/ajax-loader.gif" alt=""/>
    </div>
    <script type="text/jscript">
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
        <div id="header" class="header">
            <uc2:header ID="header1" runat="server" />
        </div>
        <div class="sectionName">
            MATERIAL MANAGER
        </div>
        <div class="wrap">
            <div id="left_side" class="resizable resizable1">
                <div id="dropdown" class="dropdown_style">
                    <asp:DropDownList ID="ProductionTypesList" runat="server" Height="25px" AutoPostBack="true"
                        Font-Names="Verdana" OnSelectedIndexChanged="productionTypeList_SelectedIndexChanged"
                        CssClass="dropdown" Font-Bold="False">
                    </asp:DropDownList>
                </div>
                <div id="tree_container" style="left: 7px; position: absolute; top: 39px; bottom: 6px; overflow-x: scroll; overflow-y: scroll; right: 75%; /*box-shadow: 0px 2px 4px 0 rgba(0,0,0,.2);*/ border: solid 1px grey; min-width: 150px; width: 97%;"
                    onscroll="document.getElementById('treeview_vertical_scroll').value = this.scrollTop;">
                    <div class="treeview" id="treeview">
                        <asp:TreeView ID="TreeView1" runat="server" NodeWrap="false" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged"
                            OnTreeNodeExpanded="TreeView1_TreeNodeExpanded" CollapseImageUrl="~/Icons/navigate_open.png"
                            ExpandImageUrl="~/Icons/navigate_close.png" NoExpandImageUrl="~/Icons/bullet_square_grey.png"
                            OnTreeNodeCollapsed="TreeView1_TreeNodeCollapsed">
                            <NodeStyle ForeColor="Black" HorizontalPadding="5" VerticalPadding="3" />
                            <HoverNodeStyle CssClass="hover_node" ForeColor="White" />
                            <SelectedNodeStyle Font-Bold="true" />
                        </asp:TreeView>
                    </div>
                </div>
            </div>
            <div id="right_side" class="resizable resizable2">
                <asp:Panel ID="Panel1" runat="server">
                    <input type="button" class="buttonGoToUpperLevel" style="background-image: url('../Icons/nav_left_blue.png');"
                        title="Вверх на уровень" id="upperLevel" onclick="__doPostBack('GoToUpperLevel', '');" />
                </asp:Panel>
                <asp:Panel ID="Panel2" runat="server">
                    <input type="button" class="buttonUpperLevel" style="background-image: url('../Icons/package.png');"
                        title="Верхний уровень" value="" id="Button1" />
                </asp:Panel>
                <div id="productDescription" class="productDescriptionStartPage">
                    <div id="ProductionName">
                        <asp:Label ID="ProductName" CssClass="product_name" runat="server" Text="Label"></asp:Label>
                    </div>

                    <div id="description" style="font-size: 13px; font-weight: normal;">
                        <asp:Table ID="Table1" runat="server" CssClass="description_table">
                            <asp:TableRow>
                                <asp:TableCell CssClass="tablecell">Тип:</asp:TableCell>
                                <asp:TableCell>
                                    <asp:Label ID="TypeName" runat="server" Text="Label"></asp:Label>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell CssClass="tablecell"><b>ГОСТ/Чертеж:</b></asp:TableCell>
                                <asp:TableCell>
                                    <asp:Label ID="DrawingName" runat="server" Text="Label"></asp:Label>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>


                        <div id="button_block">
                            <div id="Div1" style="float: left;">

                                <div id="button_panel">
                                    <table>
                                        <tr>
                                            <td>
                                                <input type="button" class="button" value="Material consumption rates"
                                        style="top: 42px; margin-left: 37px; width: 235px; background-image: url(../Icons/scroll.png); background-position: 205px; background-repeat: no-repeat;"
                                        onclick="FormSpecification('Нормы расхода материалов: ' + document.getElementById('ProductName').textContent, document.getElementById('SelectedNodeID').value);" />
                                            </td>
                                            <td>
                                    <asp:Button ID="CutButton" runat="server" CssClass="button cut_button" ToolTip="Вырезать изделие" OnClick="cut_button_Click" />
                                            </td>
                                            <td>
                                    <asp:Button ID="PasteButton" runat="server" CssClass="button paste_button" ToolTip="Вставить изделие" OnClick="paste_button_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    
                                </div>

                                

                            </div>
                            <div id="Div2" style="position: fixed; right: 0px; float: right;">
                                <asp:Panel ID="TopRightPanel" runat="server" CssClass="button_panel">
                                    <div class="top_right_panel2">
                                        <uc3:TopRightPanelButtons ID="TopRightPanelButtons1" runat="server" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <asp:PlaceHolder ID="QuantityBlock" runat="server">
                            <div id="delimiter" class="delimiter">
                            </div>
                            <div id="amount" class="amount">
                                <asp:Label ID="QuantityLabel" runat="server" Text="Quantity:">
                                </asp:Label>
                                <asp:TextBox ID="AmountTextBox" runat="server" Width="120px" CssClass="amount_textbox"></asp:TextBox>
                                <asp:DropDownList ID="AmountDropDownList" runat="server" CssClass="amount_drop_down_list">
                                </asp:DropDownList>
                                <asp:Button ID="SaveChangesButton" runat="server" Text="Сохранить изменения" CssClass="save_changes_button"
                                    OnClick="Button1_Click" />
                                <asp:Label ID="ConfirmationString" runat="server" CssClass="confirmation_string"
                                    Text="Изменения успешно сохранены">
                                </asp:Label>
                                <asp:HiddenField ID="Scale" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
        <input type="hidden" id="treeview_vertical_scroll" runat="server" />
        <asp:HiddenField ID="scroll2value" runat="server" />
        <asp:HiddenField ID="SelectedNodeID" runat="server" />
        <asp:HiddenField ID="ParentNodeID" runat="server" />
        <div class="modal">
            <!-- Place at bottom of page -->
        </div>
        <div id="footer" class="footer">
            <uc1:footer ID="footer1" runat="server" />
        </div>
    </form>
</body>
</html>
