using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_RemoveProductConfirmation : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Выводим название удаляемого изделия
        deletingProductName.Text = Server.UrlDecode(Request.QueryString["productName"]);
        //Выводим количество изделий, в составе которого содержится удаляемое изделие
        ParentProductsLabel.Text = CountParentProducts().ToString();
    }

    /// <summary>
    /// Определяет число изделий в которые входит удаляемое изделие
    /// </summary>
    /// <returns>Количество изделий</returns>
    protected int CountParentProducts()
    {
        int productID = 0;
        try
        {
            //Получаем параметр из строки запроса HTTP
            productID = Convert.ToInt32(Server.UrlDecode(Request.QueryString["productID"]));
        }
        catch 
        { 
            return 0;
        }

        int count = 0;
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            //Вызываем процедуру подсчета вхождений изделия в состав других изделий
            count = dataContext.ogk_GetArticleParentsCount(productID).Value;
        }
        return count;

    }
}