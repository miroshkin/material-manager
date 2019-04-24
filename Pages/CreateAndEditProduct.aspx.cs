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
using System.Text;
using System.Web.Services;
using System.Drawing;

public partial class Pages_CreateAndEditProduct : System.Web.UI.Page
{
    delegate void MyDelegate(object sender, EventArgs e);
    private event MyDelegate OnLoading;
    string eventtarget;
    string eventargument;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Выставляем значение выпадающего списка на Покупные изделия (1)
            ArticleTypesList.SelectedIndex = 1;
            //Обрабатываем событие выбора
            OnLoading += ArticleTypesList_SelectedIndexChanged;
            OnLoading(sender, e);
            OnLoading -= ArticleTypesList_SelectedIndexChanged;

            //Устанавливаем начальное значение скролла дерева изделий (TreeView1)
            treeview_vertical_scroll.Value = "0";
            //Выбираем первый узел дерева и обрабатываем событие выбора
            TreeView1.Nodes[0].Select();
            OnLoading += TreeView1_SelectedNodeChanged;
            OnLoading(sender, e);
            OnLoading -= TreeView1_SelectedNodeChanged;
        }
        else
        {
            //Получаем параметры из функции __doPostBack
            eventtarget = Request.Form["__EVENTTARGET"];
            eventargument = Request.Form["__EVENTARGUMENT"];
            
            //Удаление изделия
            if (eventtarget == "RemoveProduct")
            {
                //Удаляем изделие
                RemoveProduct(eventargument);
                //Обновляем дерево при помощи обработки события выбора нового узла
                OnLoading += TreeView1_SelectedNodeChanged;
                OnLoading(sender, e);
                OnLoading = null;
            }
            else
            {
                OnLoading += TreeView1_SelectedNodeChanged;
                OnLoading(sender, e);
                OnLoading = null;
            }
        }
    }

    /// <summary>
    /// Удаляет изделие с указанным ID
    /// </summary>
    /// <param name="ID">Идентификатор изделия</param>
    private void RemoveProduct(string ID)
    {
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        //Используем хранимую процедуру
        int a = context.ogk_RemoveArticle(Convert.ToInt32(ID));
    }

    /// <summary>
    /// Действия после выбора узла дерева
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        //Загружаем изделия выбранной группы
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticlesOfGroupResult> a = dataContext.ogk_SelectArticlesOfGroup(Convert.ToInt32(TreeView1.SelectedNode.Value), Convert.ToInt32(ArticleTypesList.SelectedValue));
        List<ogk_SelectArticlesOfGroupResult> b = a.ToList();

        //Формируем вид таблицы для последующего заполнения
        DataTable dt = new DataTable();
        dt.Columns.Add("Image");
        dt.Columns.Add("Чертеж");
        dt.Columns.Add("Изделия");
        dt.Columns.Add("ID");
        dt.Columns.Add("Text_tree");

        string imageUrl;
        //Назначаем изображение в зависимости от выбранного типа изделий 
        //1 - покупное изделие, 0 - внутреннее изделие
        if (ArticleTypesList.SelectedIndex == 1)
        {
            imageUrl = "../Icons/box_closed.png";
        }
        else
        {
            imageUrl = "../Icons/box.png";
        }

        for (int i = 0; i < b.Count; i++)
        {
            if ((b.ElementAt(i).DRAWING == null) || (b.ElementAt(i).DRAWING == ""))
            {
                b.ElementAt(i).DRAWING = "[Нет данных]";
            }

            //Добавляем строку в таблицу
            dt.Rows.Add(imageUrl, b.ElementAt(i).DRAWING, b.ElementAt(i).NAME, b.ElementAt(i).ID.ToString());
        }

            //Показываем панель со списком изделий группы
            Panel1.Visible = true;
            ////Настраиваем ширину ячеек таблицы
            
            //Столбец изображений элементов
            GridView1.Columns[0].ItemStyle.CssClass = "productImageColumn";

            //Столбец чертежей/ГОСТов
            GridView1.Columns[1].ItemStyle.CssClass = "productDrawingColumn";
            
            //Столбец имен изделий
            GridView1.Columns[2].ItemStyle.CssClass = "productNameColumn";

            //Столбец ID 
            GridView1.Columns[3].ItemStyle.CssClass = "productEnteringColumn";

            //Столбец вхождения изделия в другие изделия 
            GridView1.Columns[4].ItemStyle.CssClass = "productIdColumn";

            GridView1.DataSource = dt;
            GridView1.DataBind();
            
            //Записываем значение выбранной группы  в скрытую переменную 
            hiddenGroupID.Value = TreeView1.SelectedNode.Value;
    }
    
    
    protected void ArticleTypesList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Загружаем группы изделий выбранной категории
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(0, Convert.ToInt32(ArticleTypesList.SelectedValue));
        List<ogk_SelectArticleGroupsResult> b = a.ToList();

        //Загрузка дерева изделий
        TreeView1.Nodes.Clear();

        for (int i = 0; i < b.Count; i++)
        {
            //Изображение группы изделий
            TreeNode tn = new TreeNode(b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString(), "../Icons/folder.png");
            //Добавляем всплывающую подсказку для длинных названий групп
            tn.ToolTip = b.ElementAt(i).NAME.ToString();
            TreeView1.Nodes.Add(tn);
            LoadTreeNodeChildren(tn);
        }

        //Выбираем первый узел дерева и обрабатываем событие выбора
        TreeView1.Nodes[0].Select();
        //Удаляем остальные обработчики
        OnLoading = null;
        OnLoading += TreeView1_SelectedNodeChanged;
        OnLoading(sender, e);
        OnLoading = null;
        
        //Сворачиваем все узлы
        TreeView1.CollapseAll();

        //Показываем кнопку добавления изделия необходимого типа
        if (ArticleTypesList.SelectedIndex == 1)
        {
            //Скрываем кнопку добавления внутреннего изделия
            AddInnerProductButton.Visible = false;
            //Показываем кнопку добавления покупного изделия
            AddOuterProductButton.Visible = true;
        }
        else
        {
            //Показываем кнопку добавления внутреннего изделия
            AddInnerProductButton.Visible = true;
            //Скрываем кнопку добавления покупного изделия
            AddOuterProductButton.Visible = false;
        }

        //Загружаем значение выбранного пункта в скрытую переменную
        hiddenTypeID.Value = ArticleTypesList.SelectedValue;
    }

    private void LoadTreeNodeChildren(TreeNode treeNode)
    {
        //Загружаем подгруппы 
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(Convert.ToInt32(treeNode.Value), Convert.ToInt32(ArticleTypesList.SelectedValue));
        List<ogk_SelectArticleGroupsResult> b = a.ToList();

        for (int i = 0; i < b.Count; i++)
        {
            //Изображение подгруппы изделий такое же как и у группы
            TreeNode tn = new TreeNode(b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString(), "../Icons/folder.png");
            //Добавляем всплывающую подсказку для длинных названий групп
            tn.ToolTip = b.ElementAt(i).NAME.ToString();
            treeNode.ChildNodes.Add(tn);
            LoadTreeNodeChildren(tn);
        }
    }

    protected void GridView1_RowDataBound1(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            //Название изделия 
            string productName = Methods.ReplaceSpecialSymbols_ToolTip(e.Row.Cells[2].Text);

            //Добавляем всплывающую подсказку
            e.Row.ToolTip = productName;
            int id = Convert.ToInt32(e.Row.Cells[4].Text);

            //Привязываем функцию - по щелчку переходим в окно редактирования изделия
            e.Row.Cells[0].Attributes.Add("onclick", "EditProduct('" + id + "');");
            e.Row.Cells[1].Attributes.Add("onclick", "EditProduct('" + id + "');");
            e.Row.Cells[2].Attributes.Add("onclick", "EditProduct('" + id + "');");

            int parents;
            using (MainDataClassesDataContext dc = new MainDataClassesDataContext())
            {
                parents = Convert.ToInt32(dc.ogk_GetArticleParentsCount(id));
            }

            if (parents > 0)
            {
                e.Row.Cells[3].Attributes.Add("onclick", "ShowParents('" + id + "','" + Server.UrlEncode(productName) + "');");
                //Всплывающая подсказка для кнопки отображения изделий, в которые входит product
                e.Row.Cells[3].ToolTip = "Вхождения";
            }
            else
            {
                //Выводим пустую строку вместо изображения(Скрываем возможность просмотра Вхождений)
                e.Row.Cells[3].Text = "";
                e.Row.Cells[3].Attributes.Add("onclick", "EditProduct('" + id + "');");
            }

            
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
            
            

        }
    }
}