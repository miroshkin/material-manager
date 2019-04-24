using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Confirmation : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        deletingProductName.Text = Server.UrlDecode(Request.QueryString["productName"]);
    }
}