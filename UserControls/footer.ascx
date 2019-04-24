<%@ Control Language="C#" AutoEventWireup="true" CodeFile="footer.ascx.cs" Inherits="footer" %>

<script type="text/javascript">
    //Открываем окно для аутентификации
    function LogIn() {
        fade_in();
        $('#LogInForm').dialog('close');

        $('<div title="Вход" id="LogInForm"/>').appendTo('body').load("/Pages/Login.aspx").dialog({
            beforeClose: function (event, ui) {
                fade_out();
            },
            show: { effect: 'fade', duration: 300 },
            hide: { effect: 'fade', duration: 300 },
            closeOnEscape: true,
            draggable: true,
            resizable: false,
            width: 300,
            height: 154,
            close: function (event, ui) {
                $('#LogInForm').remove();
            },
            open: function () {

            }
        });
    }
    //Выход пользователя
    function LogOut() {
        setCookie("username", "", 0);
        setCookie("UserIsAuthentificated", "false", 30);
        __doPostBack('', '');
    }

    //Меняем кнопки в зависимости от состояния аутентификации пользователя
    $(function () {
        var user_is_authentificated = getCookie("UserIsAuthentificated");
        //Пользователь аутентифицирован
        if (user_is_authentificated != 'true') {
            $('#LogOutButton').hide();
        }
            //Пользователь не прошел аутентификацию
        else {
            $('#LogInButton').hide();
        }
    })

    //Процесс аутентификации пользователя
    function Authentificate() {
        var user_name = $("#user_name option:selected").text();

        $.ajax({
            type: "POST",
            url: 'Login.aspx/AuthentificateUser',
            data: '{name: "' + user_name + '", password: "' + $('#password').val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d == true) {
                    setCookie("username", user_name,30);
                    setCookie("UserIsAuthentificated", 'true', 30);
                    $('#LogInForm').dialog('close');
                    __doPostBack('', '');
                }
                else if (response.d == false) {
                    $('#Message').text('Пароль указан неверно');
                }
            },
            error: function (e) {
                Error("Ошибка при входе пользователя");
            }
        });

    }
</script>

Test development 2019
<div style="left: 10px; bottom: 0px; position: absolute;">
    Electronic Documentation v.1.9.2
</div>
<asp:Label ID="userName" Text="User" runat="server" CssClass="user_name_footer">
</asp:Label>

<img id="LogOutButton" src="../Icons/logout_16.png" class="log_out_button" onclick="LogOut()" />
<img id="LogInButton" src="../Icons/login_16.png" class="log_out_button" onclick="LogIn()" />