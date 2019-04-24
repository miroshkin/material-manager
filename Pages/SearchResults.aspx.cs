using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Linq;
using System.Data;

public partial class Pages_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Название искомого изделия
        string productNameParameter = Server.UrlDecode(Request.QueryString["productName"]);
        //Чертеж искомого изделия
        string productDrawingParameter = Server.UrlDecode(Request.QueryString["productDrawing"]);
        //Вызов окна поиска в первый раз - true, иначе false
        string searchForTheFirstTime = Server.UrlDecode(Request.QueryString["searchForTheFirstTime"]);

        productName.Text = productNameParameter;
        productDrawing.Text = productDrawingParameter;
        
        NoProductsFoundMessage.Visible = false;
        LoadProductsToGridView(productNameParameter, productDrawingParameter, searchForTheFirstTime);
    }

    /// <summary>
    /// Загружает список найденных изделий в окно поиска
    /// </summary>
    /// <param name="productNameParameter">Название изделия</param>
    /// <param name="productDrawingParameter">Чертеж/ГОСТ изделия</param>
    /// <param name="searchForTheFirstTime">Если поиск проводится впервые = true, иначе false</param>
    private void LoadProductsToGridView(string productNameParameter, string productDrawingParameter, string searchForTheFirstTime)
    {
        if((productNameParameter=="")&&(productDrawingParameter==""))
        {
            //Скрываем список найденных изделий
            GridViewPanel.Visible = false;
            if (searchForTheFirstTime == "false")
            {
                //Показываем сообщение о том, что никаких изделий не найдено
                NoProductsFoundMessage.Visible = true;
            }
            else
            {
                //Скрываем сообщение о том, что никаких изделий не найдено
                NoProductsFoundMessage.Visible = false;
            }
            return;
        }
        
        //Добавляем % для поиска вхождений искомых названий и чертежей
        productNameParameter = "%" + productNameParameter + "%";
        productDrawingParameter = "%" + productDrawingParameter + "%";


        //Если чертежа нет, то ищем изделие по имени
        if (productDrawingParameter == "%%")
        {
            //Ищем изделие по названию 
            LoadProductsToGridView_SearchName(productNameParameter);
        }

        else
        {

            MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
            ISingleResult<ogk_SearchForProductionArticleResult> a = dataContext.ogk_SearchForProductionArticle(productNameParameter, productDrawingParameter);
            
            List<ogk_SearchForProductionArticleResult> b = a.ToList();

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
                //В зависимости от типа найденного изделия , формируем ссылку на изображение
                string imageUrl = "";
                switch (b.ElementAt(i).TYPE_NAME)
                {
                    case "Продукция":
                        imageUrl = "../Icons/package.png";
                        break;
                    case "Внутреннее изделие":
                        imageUrl = "../Icons/box.png";
                        break;
                    case "Покупное изделие":
                        imageUrl = "../Icons/box_closed.png";
                        break;
                }
                dt.Rows.Add(imageUrl, b.ElementAt(i).DRAWING, b.ElementAt(i).NAME, b.ElementAt(i).ID.ToString());
            }
            //Если изделий не найдено
            if (b.Count == 0)
            {
                //Скрываем панель вывода списка изделий
                GridViewPanel.Visible = false;
                //Показываем сообщение о том , что никаких изделий не найдено
                NoProductsFoundMessage.Visible = true;
            }
            
            GridView1.DataSource = dt;
            GridView1.DataBind();

        }
    }


    /// <summary>
    /// Поиск изделия по названию
    /// </summary>
    /// <param name="productNameParameter"></param>
    private void LoadProductsToGridView_SearchName(string productNameParameter)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SearchForProductionArticleName_AllResult> a = dataContext.ogk_SearchForProductionArticleName_All(productNameParameter);
        List<ogk_SearchForProductionArticleName_AllResult> b = a.ToList();

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
            //В зависимости от типа найденного изделия , формируем ссылку на изображение
            string imageUrl = "";
            switch (b.ElementAt(i).TYPE_NAME)
            {
                case "Продукция":
                    imageUrl = "../Icons/package.png";
                    break;
                case "Внутреннее изделие":
                    imageUrl = "../Icons/box.png";
                    break;
                case "Покупное изделие":
                    imageUrl = "../Icons/box_closed.png";
                    break;
            }
            dt.Rows.Add(imageUrl, b.ElementAt(i).DRAWING, b.ElementAt(i).NAME, b.ElementAt(i).ID.ToString());
        }
        //Если изделий не найдено
        if (b.Count == 0)
        {
            //Скрываем панель вывода списка изделий
            GridViewPanel.Visible = false;
            //Показываем сообщение о том , что никаких изделий не найдено
            NoProductsFoundMessage.Visible = true;
        }
        GridView1.DataSource = dt;
        GridView1.DataBind();
    }

    /// <summary>
    /// Загружает список первых n названий изделий с указанной частью названием
    /// </summary>
    /// <param name="name">Искомое название</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static List<string> GetProductList_Name(string name)
    {
        //Добавляем символы для поиска вхождений указанного названия
        name = "%" + name + "%";
        //Устанавливаем количество изделий в выборке
        int number = 5;
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult <ogk_SearchForProductionArticleNameResult> a =  context.ogk_SearchForProductionArticleName(name, number);
        List<ogk_SearchForProductionArticleNameResult> List = a.ToList();
        List<string> productNameList = new List<string>();
        for (int i = 0; i < List.Count; i++)
        {
            productNameList.Add(List[i].NAME.ToString());
        }
        return productNameList;
    }

    /// <summary>
    /// Загружает список первых n  изделий с указанным ГОСТом/Чертежом
    /// </summary>
    /// <param name="drawing">Искомый ГОСТ/Чертеж</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static List<string> GetProductList_Drawing(string drawing)
    {
        //Добавляем символы для поиска вхождений указанного ГОСТа/Чертежа в другие ГОСТ
        drawing = "%" + drawing + "%";
        //Определяем количество ГОСТов/Чертежей в выборке
        int number = 5;
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_SearchForProductionArticleDrawingResult> a = context.ogk_SearchForProductionArticleDrawing(drawing, number);
        List<ogk_SearchForProductionArticleDrawingResult> List = a.ToList();
        List<string> productDrawingList = new List<string>();
        for (int i = 0; i < List.Count; i++)
        {
            productDrawingList.Add(List[i].DRAWING.ToString());
        }
        return productDrawingList;
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

            e.Row.Attributes.Add("onclick", "this.style.font-weight = 'bold'");
            


            //Настраиваем ширину ячеек таблицы
            GridView1.Columns[0].ItemStyle.Width = 25;
            GridView1.Columns[0].ItemStyle.Wrap = false;
            GridView1.Columns[1].ItemStyle.Width = 80;
            GridView1.Columns[1].ItemStyle.CssClass = "column_style";
            GridView1.Columns[3].ItemStyle.CssClass = "productIdColumn";
            int id = Convert.ToInt32(e.Row.Cells[3].Text);

            //TODO 1 - Добавляем атрибут onclick  для вызова перехода к группе изделия
            e.Row.Cells[0].Attributes.Add("onclick", "ShowParents('" + id + "','" + e.Row.Cells[2].Text + "');");
            e.Row.Cells[1].Attributes.Add("onclick", "ShowParents('" + id + "','" + e.Row.Cells[2].Text + "');");
            e.Row.Cells[2].Attributes.Add("onclick", "ShowParents('" + id + "','" + e.Row.Cells[2].Text + "');");
            e.Row.Cells[3].Attributes.Add("onclick", "ShowParents('" + id + "','" + e.Row.Cells[2].Text + "');");

            //TODO 2 - Добавляем javascript функцию перехода к группе изделия

            //TODO 3 - Выделяем изделие в группе при помощи border

            //TODO 4 - Проверяем наличие существующего окна поиска

            
        }
    }
}