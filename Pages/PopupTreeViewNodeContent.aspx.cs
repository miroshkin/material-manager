using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text.RegularExpressions;

public partial class Pages_PopupTreeViewNodeContent : System.Web.UI.Page
{
    string outerProductID;
    string productType;
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //Получение параметров 

        //Тип изделия
        productType = Request.QueryString["productType"];

        //Идентификатор выбранной из списка группы
        string groupID = Request.QueryString["groupID"];

        //Значение положения скролла в главном окне
        string scroll2position = Request.QueryString["scroll2value"];

        //Идентификатор изделия, в которое добавляем содержимое
        outerProductID = Request.QueryString["outerProductID"];

        //Загружаем описание изделия
        LoadDescription(productType, groupID);

    }

    private void LoadDescription(string productType, string groupID)
    {
        switch (productType)
        {
            case "SELL":
                //Загружаем продукцию 
                LoadProductsToGridView(productType, groupID);
                break;
            case "BUY":
                //Загружаем покупные изделия Тип - 3
                productType = "3";
                LoadProducts(productType, groupID);
                break;
            case "MAKE":
                //Загружаем дерево групп внутренних изделий
                //LoadInnerProductsToGridView(productType, groupID);
                //Загружаем внутренние изделия Тип - 2
                productType = "2";
                LoadProducts(productType, groupID);
                break;
        }
    }

    /// <summary>
    /// Загрузка продукции
    /// </summary>
    /// <param name="productType"></param>
    /// <param name="groupID"></param>
    private void LoadProductsToGridView(string productType, string groupID)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectProductionArticlesResult> a = dataContext.ogk_SelectProductionArticles(Convert.ToInt32(groupID));
        List<ogk_SelectProductionArticlesResult> b = a.ToList();
        DataTable dt = new DataTable();
        dt.Columns.Add("Image");
        dt.Columns.Add("Чертеж");
        dt.Columns.Add("Изделия");
        dt.Columns.Add("ID");

        for (int i = 0; i < b.Count; i++)
        {
            if ((b.ElementAt(i).DRAWING == null) || (b.ElementAt(i).DRAWING == ""))
            {
                b.ElementAt(i).DRAWING = "[Нет данных]";
            }
            dt.Rows.Add("../Icons/package.png", b.ElementAt(i).DRAWING, b.ElementAt(i).NAME, b.ElementAt(i).ID.ToString());
        }
        GridView1.DataSource = dt;
        GridView1.DataBind();
    }

    protected void GridView1_RowDataBound1(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#ffbb33';this.style.color='#ffffff'; this.style.transition='0.1s';");

            if (e.Row.RowState == DataControlRowState.Alternate)
            {
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';this.style.color='#000000';this.style.transition='0.1s';");
            }
            else
            {
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#E8E8E8';this.style.color='#000000';this.style.transition='0.1s';");
            }
            e.Row.Attributes.Add("style", "cursor:pointer;");
            e.Row.ToolTip = e.Row.Cells[2].Text;

            //Настраиваем ширину ячеек таблицы
            GridView1.Columns[0].ItemStyle.Width = 25;
            GridView1.Columns[0].ItemStyle.Wrap = false;
            GridView1.Columns[1].ItemStyle.Width = 80;
            GridView1.Columns[1].ItemStyle.CssClass = "column_style";
            GridView1.Columns[3].ItemStyle.CssClass = "productIdColumn";
            int id = Convert.ToInt32(e.Row.Cells[3].Text);
            e.Row.Attributes.Add("onclick", "__doPostBack('AddProduct','" + id + "');");
        }
    }

    //Список групп и подгрупп изделий для загрузки
    List<int> groupAndSubGroupsID = new List<int>();
    /// <summary>
    /// Загрузка покупных и внутренних изделий
    /// </summary>
    /// <param name="productType">Тип изделия (2 - внутреннее изделие, 3 - покупное)</param>
    /// <param name="groupID"></param>
    private void LoadProducts(string productType, string groupID)
    {
        //Выгружаем группу и подгруппы из дерева групп
        groupAndSubGroupsID.Clear();
        groupAndSubGroupsID.Add(Convert.ToInt32(groupID));

        //Загружаем подгруппы выбранной группы
        LoadSubgroups(Convert.ToInt32(productType), Convert.ToInt32(groupID));

        //Готовим таблицу dt  для загрузки
        DataTable dt = new DataTable();
        dt.Columns.Add("Image");
        dt.Columns.Add("Чертеж");
        dt.Columns.Add("Изделия");
        dt.Columns.Add("ID");

        //Определяем изображение изделия
        string imageURL = "";
        switch (productType)
        {
            case "2":
                imageURL = "../Icons/box.png";
                break;
            case "3":
                imageURL = "../Icons/box_closed.png";
                break;
        }

        //Выгружаем список изделий для каждой из подгрупп в таблицу dt
        for (int i = 0; i < groupAndSubGroupsID.Count; i++)
        {
            MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
            ISingleResult<ogk_SelectArticlesOfGroupResult> a = dataContext.ogk_SelectArticlesOfGroup(groupAndSubGroupsID[i], Convert.ToInt32(productType));
            List<ogk_SelectArticlesOfGroupResult> b = a.ToList();
            for (int j = 0; j < b.Count; j++)
            {
                if (b.ElementAt(j).DRAWING == null)
                {
                    b.ElementAt(j).DRAWING = "[Нет данных]";
                }
                dt.Rows.Add(imageURL, b.ElementAt(j).DRAWING, b.ElementAt(j).NAME, b.ElementAt(j).ID.ToString());
            }
        }
        GridView1.DataSource = dt;
        GridView1.DataBind();
    }


    /// <summary>
    /// Загружаем подгруппы выбранной группы 
    /// </summary>
    /// <param name="productType">Тип продукта - Внутреннее изделие, Покупное изделие, Продукция</param>
    /// <param name="groupID">Идентификатор выбранной группы</param>
    private void LoadSubgroups(int productType, int groupID)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(groupID, productType);
        List<ogk_SelectArticleGroupsResult> b = a.ToList();

        for (int i = 0; i < b.Count; i++)
        {
            groupAndSubGroupsID.Add(b.ElementAt(i).ID);
            LoadSubgroups(productType, b.ElementAt(i).ID);
        }
    }
}