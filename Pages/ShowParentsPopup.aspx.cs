using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_ShowParentsPopup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //Принимаем входные параметры

        //Идентификатор изделия
        string productID = Request.QueryString["ID"];
        string productName = Server.UrlDecode(Request.QueryString["productName"]);

        productNameLbl.Text = productName;
        
        if (GetParentProducts(productID) > 0)
        {
           amountLbl.Text = " входит в состав следующих изделий: ";
           Panel1.Visible = true;
        }
        else
        {
           amountLbl.Text  = " не входит в состав других изделий";
           Panel1.Visible = false;
        }
    }

    /// <summary>
    /// Определяет изделия, в которые входит изделие
    /// </summary>
    /// <returns>Количество изделий</returns>
    protected int GetParentProducts(string productID)
    {
        int ID;
        try
        {
            ID = Convert.ToInt32(productID);
        }
        catch
        {
            return 0;
        }

        int count = 0;
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {

            ISingleResult<ogk_GetArticleParentsResult> a = dataContext.ogk_GetArticleParents(ID);
            List<ogk_GetArticleParentsResult> b = a.ToList();
            count = b.Count;
            
            //Формируем вид таблицы для последующего заполнения
            DataTable dt = new DataTable();
            dt.Columns.Add("Image");
            dt.Columns.Add("Чертеж");
            dt.Columns.Add("Название");
            dt.Columns.Add("ID");

            string imageUrl = "";
            
            for (int i = 0; i < b.Count; i++)
            {
                if ((b.ElementAt(i).DRAWING == null) || (b.ElementAt(i).DRAWING == ""))
                {
                    b.ElementAt(i).DRAWING = "[Нет данных]";
                }

                switch (b.ElementAt(i).TYPE)
                { 
                    case 1:
                        imageUrl = "../Icons/package.png";
                        break;
                    case 2:
                        imageUrl = "../Icons/box.png";
                        break;
                    case 3:
                        imageUrl = "../Icons/box_closed.png";
                        break;
                    default:
                        imageUrl = "../Icons/package.png";
                        break;
                }
                //Добавляем строку в таблицу
                dt.Rows.Add(imageUrl, b.ElementAt(i).DRAWING, b.ElementAt(i).NAME,b.ElementAt(i).ID);
            }
            //Столбец изображений элементов
            GridView1.Columns[0].ItemStyle.CssClass = "productImageColumn";

            //Столбец чертежей/ГОСТов
            GridView1.Columns[1].ItemStyle.CssClass = "productDrawingColumn";

            //Столбец имен изделий
            GridView1.Columns[2].ItemStyle.CssClass = "productNameColumn";

            //Столбец ID 
            GridView1.Columns[3].ItemStyle.CssClass = "productIdColumn";

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        return count;
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Добавляем атрибут при наведении курсора мыши
            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#ffbb33';this.style.color='#ffffff'; this.style.transition='0.1s';");
            //Если строка таблицы в альтернативном стиле
            if (e.Row.RowState == DataControlRowState.Alternate)
            {
                //формируем один стиль
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';this.style.color='#000000';this.style.transition='0.1s';");
            }
            else
            {
                //иначе формируем другой стиль
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#E8E8E8';this.style.color='#000000';this.style.transition='0.1s';");
            }
            //Курсор в виде руки-указателя
            e.Row.Attributes.Add("style", "cursor:pointer;");

            //Название изделия 
            string productName = Methods.ReplaceSpecialSymbols_ToolTip(e.Row.Cells[2].Text);

            //Добавляем всплывающую подсказку
            e.Row.ToolTip = productName;

            //Добавляем сслылку для открытия изделия в новом окне c ID
            e.Row.Attributes.Add("onclick", "window.open('../Pages/StartPage.aspx?openArticleID=" + e.Row.Cells[3].Text + "', '_blank');");
        }
    }
    protected void GridView1_RowDataBound(object sender, EventArgs e)
    {

    }
}
