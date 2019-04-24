using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_RemoveGroupConfirmation : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        Methods m = new Methods();
        //Выводим название удаляемого изделия
        deletingGroupName.Text = Server.UrlDecode(Request.QueryString["groupName"]);
        deletingGroupID.Value = Request.QueryString["groupID"];
        //Задаем тип окна для реакции на нажатие клавиши enter
        windowMarker.Value = "RemoveOK";
    }
}