<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchResults.aspx.cs" Inherits="Pages_Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function ShowProductNameList() {
            
            $.ajax({
                type: "POST",
                url: "SearchResults.aspx/GetProductList_Name",
                data: '{name: "' + $('#productName').val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnNameSuccess,
                failure: function (response) {
                    alert('Error' + response.d);
                }
            });
        }
        
        function ShowProductDrawingList() {
            $.ajax({
                type: "POST",
                url: "SearchResults.aspx/GetProductList_Drawing",
                data: '{drawing: "' + $('#productDrawing').val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnDrawingSuccess,
                failure: function (response) {
                    alert('Error' + response.d);
                }
            });
        }

        function OnNameSuccess(response) {
            var availableTags = response.d;
            $('#productName').autocomplete({ source: availableTags });
        }

        function OnDrawingSuccess(response) {
            var availableTags = response.d;
            $('#productDrawing').autocomplete({ source: availableTags });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="font-family: Verdana; font-size: 12px;">
        <style>
            .full {
                font-size: 12px;
                font-family: Verdana;
                color: Red;
            }

            #select {
                font-family: Verdana;
                font-size: 12px;
            }

            .search_area_name {
                position: absolute;
                top: 23px;
                left: 4px;
                width: 525px;
            }

            .search_area_drawing {
                position: absolute;
                top: 68px;
                left: 4px;
                width: 525px;
            }

            .find_button {
                width: 88px;
                text-align: center;
                position: absolute;
                right: 4px;
                top: 97px;
            }

            .search_area_name_header {
                top: 6px;
                position: absolute;
                left: 4px;
                font-weight: bold;
            }

            .search_area_drawing_header {
                top: 51px;
                position: absolute;
                left: 4px;
                font-weight: bold;
            }

            .NoProductsFoundMessage {
                position: absolute;
                top: 137px;
                left: 4px;
            }
        </style>
        <div class="popup_delimiter" style="width: 640px; left: 0px;">
        </div>
        <div class="search_area_name_header">
            Product name
        </div>
        <div class="search_area_drawing_header">
            Drawing/Standard
        </div>
        <div class="full">
            <asp:TextBox ID="productName" CssClass="search_area_name" runat="server" AutoPostBack="false"
                onkeyup="if (event.keyCode == 13){ $('#FindButton').click();return false;}else{ShowProductNameList(); return (event.keyCode != 13);}"
                ></asp:TextBox>
        </div>
        <asp:TextBox ID="productDrawing" CssClass="search_area_drawing" runat="server" AutoPostBack="false"
            onkeyup="if (event.keyCode == 13) { $('#FindButton').click();return false;}else {ShowProductDrawingList();return (event.keyCode != 13);}"
            ></asp:TextBox>
        <input type="button" class="button" style="top: 60px; position: absolute; width: 88px; text-align: center; right: 0px;"
            value="Find" id="FindButton" onclick="$('#searchForTheFirstTime').val('false'); SearchProduct(encodeURIComponent($('#productName').val()), encodeURIComponent($('#productDrawing').val()));" 
             />
        <input type="button" class="button" value="Close" id="NoButton" onclick="close_dialog('#SearchResultsWindow');"
            style="width: 88px;text-align: center;position: absolute;bottom: 1px;right: -4px;font-size: 13px;" />
        <asp:Panel ID="GridViewPanel" runat="server">
            <div id="GridView_position" style="position: absolute; top: 104px; width: 633px; height: 280px; overflow: scroll; left: 4px;">
                <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound1"
                    Width="800px" GridLines="None" CellPadding="4" ForeColor="Black" AutoGenerateColumns="False"
                    ShowHeader="False">
                    <AlternatingRowStyle BackColor="White"/>
                    <Columns>
                        <asp:ImageField DataImageUrlField="Image" NullImageUrl="~/Icons/box_closed_delete.png"
                            ShowHeader="False">
                        </asp:ImageField>
                        <asp:BoundField DataField="Чертеж" HeaderText="Чертеж">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Изделия">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
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
        <asp:Label ID="NoProductsFoundMessage" CssClass="NoProductsFoundMessage" runat="server" Text="Nothing found"></asp:Label>
    </form>
</body>
</html>
