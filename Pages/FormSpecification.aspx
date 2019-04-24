<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FormSpecification.aspx.cs" Inherits="Pages_Specification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Styles/Main.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="specification_header">
            
            <div class="specification_header_column_1">
                Группа
            </div>
            <div class="specification_header_column_2">
                Наименование
            </div>
            <div class="specification_header_column_3">
                ГОСТ
            </div>
            <div class="specification_header_column_4">
                Ед.изм
            </div>
            <div class="specification_header_column_5">
                Норма
            </div>
        </div>

        <div class="specification_gridview">
            <asp:GridView ID="SpecificationView" runat="server" AutoGenerateColumns="False" ShowHeader="False" OnRowDataBound="SpecificationView_RowDataBound" GridLines="None">
                <AlternatingRowStyle BackColor="White" BorderStyle="None" Height="29px" />
                <Columns>
                    <asp:TemplateField HeaderText="ARTICLE_GROUP_NAME">
                        <ItemTemplate>
                            <div style="padding-left: 5px;width: 270px; overflow: hidden; white-space: normal;">
                                <%# Eval("ARTICLE_GROUP_NAME") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="COMPONENT_NAME">
                        <ItemTemplate>
                            <div style="width: 330px; overflow: hidden; ">
                                <%# Eval("COMPONENT_NAME") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DRAWING">
                        
                        <ItemTemplate>
                            <div style="width: 250px;overflow: hidden; white-space: nowrap; ">
                                <%# Eval("DRAWING") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="UNIT_NAME">
                        <ItemTemplate>
                            <div style="width: 55px; overflow: hidden; white-space: nowrap;text-align:center;">
                                <%# Eval("UNIT_NAME") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="QUANTITY" >
                        <ItemTemplate>
                            <div style="width:  90px;overflow: hidden; white-space: nowrap;text-align:left; ">
                                <%# Eval("QUANTITY","{0:G9}") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle BackColor="#E8E8E8" BorderStyle="None" Height="29px" />
                                    <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                    <SortedAscendingHeaderStyle BackColor="#246B61" />
                                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                    <SortedDescendingHeaderStyle BackColor="#15524A" />
            </asp:GridView>
        </div>

        <input type="button" class="cancel_button cancel_button_specification_form" value="Отмена" id="CancelButton" onclick="close_dialog('#FormSpecification');"
             />

            <input type="button" class="specification_excel_button" value="Сохранить как таблицу Excel" id="ExcelReport" onclick= "window.open('../Pages/ReportPage.aspx?is_xls=1&productID=' + document.getElementById('productIDHiddenField').value + '&report_name=' + encodeURIComponent('/Электронная конструкторская документация/Отчеты/Нормы расхода материалов'), '_blank');" style="font-size: 12px;"/>
            <input type="button" class="specification_pdf_button" value="Построить отчет" id="PdfReport" onclick= "window.open('../Pages/ReportPage.aspx?is_xls=0&&productID=' + document.getElementById('productIDHiddenField').value + '&report_name=' + encodeURIComponent('/Электронная конструкторская документация/Отчеты/Нормы расхода материалов'), '_blank');" style="font-size: 12px;"/>
            <asp:HiddenField ID="productIDHiddenField" runat="server" />
    </form>
</body>
</html>
