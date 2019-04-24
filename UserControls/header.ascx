<%@ Control Language="C#" AutoEventWireup="true" CodeFile="header.ascx.cs" Inherits="UserControls_header" %>
<script type="text/javascript">
    //Открытие меню при наведении
    function openbox(id) {
        display = document.getElementById(id).style.display;
        if (display == 'none') {
            document.getElementById(id).style.display = 'block';
        } else {
            document.getElementById(id).style.display = 'none';
        }
    }

    //Вызываем Окно поиска изделия
    function SearchProduct(productName, productDrawing) {
        //fade_in();
        $('#SearchResultsWindow').dialog('close');
        var searchForTheFirstTime = $('#searchForTheFirstTime').val();

        $('<div title="Search" id="SearchResultsWindow"/>').appendTo('body').load("/Pages/SearchResults.aspx?productName=" + productName + "&productDrawing=" + productDrawing
        + "&searchForTheFirstTime=" + searchForTheFirstTime).dialog({
            beforeClose: function (event, ui) {
                //fade_out();
            },
            show: { effect: 'fade', duration: 300 },
            hide: { effect: 'fade', duration: 300 },
            closeOnEscape: true,
            draggable: true,
            resizable: false,
            width: 640,
            height: 480,
            close: function (event, ui) {
                $('#SearchResultsWindow').remove();
            },
            open: function () {
                  
            }
        });
    }

    //Показать окно вхождения изделий в другое изделие
    function ShowParents(ID, productName) {

        $('#ShowParentsPopUp').dialog('close');
        fade_in();

        $('<div title= "Изделие" id="ShowParentsPopUp"/>').appendTo('body').load("/Pages/ShowParentsPopUp.aspx?ID=" + ID + "&productName=" + encodeURIComponent(productName)).dialog({
            beforeClose: function (event, ui) {
                fade_out();
            },
            show: { effect: 'fade', duration: 300 },
            hide: { effect: 'fade', duration: 300 },
            closeOnEscape: true,
            draggable: true,
            resizable: false,
            width: 800,
            height: 600,
            close: function (event, ui) {
                $('#ShowParentsPopUp').remove();
            },
            open: function () {
                $('#productID').val(ID);
            }
        });
    }

    //Получаем cookies по имени
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
    //Сохраняем cookies
    function setCookie(cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    }
</script>
<div>
    <div class="logo" title="Переход к МЕНЕДЖЕРУ ИЗДЕЛИЙ" style="height: 30px; width: 96px;"
        onclick="document.location.href = '../Pages/StartPage.aspx';">
    </div>
    <div style="color: White; background-color: transparent; position: fixed; top: 1px;
        font-size: 14px; font-family: Verdana; left: 96px; z-index: 500;">
    </div>
    <input id="searchForTheFirstTime" type="hidden" value="true"/>
    
 
    
    <div id="search" style="right: 160px;">
        <input type="button" value="Search" id="SearchButton" onclick="$('#searchForTheFirstTime').val('true');SearchProduct('','');"
            class="search_button" />
    </div>

    <div class="navigation" style="position: absolute; top: 0px; right: 0px; font-size: 12px;">
        <ul class="nav" id="nav">
            <li style="background-color: #507CD1; cursor: default; height: 30px; border: solid 1px #7DA1E8;
                border-top: none; cursor: pointer; border-bottom: none;" onmouseover="openbox('menu');"
                onmouseout="openbox('menu');">Menu
                <ul id="menu" style="display: none;">
                    <li id="ProductManagerMenuItem" onclick="document.location.href = '../Pages/StartPage.aspx';">
                        <a href="#">Material manager </a></li>
                    <li id="ProductEditorMenuItem" onclick="document.location.href = '../Pages/CreateAndEditProduct.aspx';">
                        <a href="#">Product editor </a></li>
                    <li id="GroupEditorMenuItem" onclick="document.location.href = '../Pages/CreateAndEditGroup.aspx';">
                        <a href="#">Group editor </a></li>
                </ul>
            </li>
        </ul>
    </div>
</div>
