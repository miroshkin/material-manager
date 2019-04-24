using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Linq;
using System.Data;

public partial class Pages_CreateAndEditGroup : System.Web.UI.Page
{

    delegate void MyDelegate(object sender, EventArgs e);
    private event MyDelegate OnLoading;
    string targetGroup;

    protected void Page_Load(object sender, EventArgs e)
    {
        string eventtarget;
        string eventargument;


        Methods m = new Methods();

        if (!IsPostBack)
        {
            //Загружаем типы продукции в DropDownList
            ArticleTypesList.Items.AddRange(m.LoadProductionArticleTypesForGroups());
            //Выставляем значение выпадающего списка на Покупные изделия (1)
            ArticleTypesList.SelectedIndex = 1;
            //Обрабатываем событие выбора
            OnLoading += ArticleTypesList_SelectedIndexChanged;
            OnLoading(sender, e);
            OnLoading = null;

            //Устанавливаем начальное значение скролла дерева изделий (TreeView1)
            treeview_vertical_scroll.Value = "0";
            //Выбираем первый узел дерева и обрабатываем событие выбора
            TreeView1.Nodes[0].Select();
            OnLoading += TreeView1_SelectedNodeChanged;
            OnLoading(sender, e);
            OnLoading = null;

            //Присваиваем значения скрытым переменным
            //Идентификатор выбранной группа
            parentGroupID.Value = TreeView1.SelectedNode.Value;
            //Идентификатор выбранного типа продукции
            hiddenTypeID.Value = ArticleTypesList.SelectedValue.ToString();
        }
        else
        {
            //Присваиваем значения скрытым переменным
            //Идентификатор выбранной группа
            parentGroupID.Value = TreeView1.SelectedNode.Value;
            //Идентификатор выбранного типа продукции
            hiddenTypeID.Value = ArticleTypesList.SelectedValue.ToString();

            eventtarget = Request.Form["__EVENTTARGET"];
            eventargument = Request.Form["__EVENTARGUMENT"];
            targetGroup = eventargument;
            switch (eventtarget)
            {
                case "CreateGroup":
                    HandleTreeViewSelectedNodeChanged(sender, e);
                    break;
                case "ArticleTypesList":
                    TreeView1.Nodes[0].Select();
                    HandleTreeViewSelectedNodeChanged(sender, e);
                    break;
                case "RemoveGroup":
                    RemoveGroup(eventargument);
                    HandleTreeViewSelectedNodeChanged(sender, e);
                    break;
            }
        }
    }

    /// <summary>
    /// Вызов и обработка события выбора 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleTreeViewSelectedNodeChanged(object sender, EventArgs e)
    {
        OnLoading += TreeView1_SelectedNodeChanged;
        OnLoading(sender, e);
        OnLoading = null;
    }

    /// <summary>
    ///Удаляет группу изделий с указанным идентификатором
    /// 
    /// </summary>
    /// <param name="groupID">Идентификатор группы</param>
    private void RemoveGroup(string groupIDparameter)
    {
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            int result = dataContext.ogk_RemoveArticleGroup(Convert.ToInt32(groupIDparameter));
        }
    }

    protected void ArticleTypesList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Загружаем группы изделий выбранной категории
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(0, Convert.ToInt32(ArticleTypesList.SelectedValue));
        List<ogk_SelectArticleGroupsResult> b = a.ToList();

        //Загрузка дерева изделий
        TreeView1.Nodes.Clear();

        //Загрузка корневого узла всех изделий
        TreeNode rootNode = new TreeNode("Все группы", "0", "../Icons/folders.png");
        rootNode.ToolTip = "Все группы";
        TreeView1.Nodes.Add(rootNode);

        for (int i = 0; i < b.Count; i++)
        {
            //Изображение группы изделий
            TreeNode tn = new TreeNode(b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString(), "../Icons/folder.png");
            tn.ToolTip = b.ElementAt(i).NAME.ToString();
            TreeView1.Nodes[0].ChildNodes.Add(tn);
            LoadTreeNodeChildren(tn);
        }
        //Сворачиваем все дерево групп
        TreeView1.CollapseAll();
        //Выбираем первый узел дерева
        TreeView1.Nodes[0].Select();
        //Разворачиваем первый узел дерева
        TreeView1.Nodes[0].Expand();

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
            treeNode.ChildNodes.Add(tn);
            LoadTreeNodeChildren(tn);
        }
    }

    protected void GridView1_RowDataBound1(object sender, GridViewRowEventArgs e)
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
            //Добавляем всплывающую подсказку
            e.Row.ToolTip = Methods.ReplaceSpecialSymbols_ToolTip(e.Row.Cells[1].Text);
            int id = Convert.ToInt32(e.Row.Cells[3].Text);
            //Название группы
            string groupName = Methods.ReplaceSpecialSymbols(e.Row.Cells[1].Text);
            //Префикс
            string prefix = Methods.ReplaceSpecialSymbols(e.Row.Cells[2].Text);
            //Привязываем функцию - по щелчку переходим в окно редактирования группы
            e.Row.Attributes.Add("onclick", "EditGroup('" + id + "','" + groupName + "','" + prefix + "');");
        }
    }

    

    /// <summary>
    /// Действия после выбора узла дерева
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        //Загружаем подгруппы 
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(Convert.ToInt32(TreeView1.SelectedNode.Value), Convert.ToInt32(ArticleTypesList.SelectedValue));
        List<ogk_SelectArticleGroupsResult> b = a.ToList();

        //Формируем вид таблицы для последующего заполнения
        DataTable dt = new DataTable();
        dt.Columns.Add("Image");
        dt.Columns.Add("Название группы");
        dt.Columns.Add("Префикс");
        dt.Columns.Add("ID");

        string imageUrl = "../Icons/folder.png";

        for (int i = 0; i < b.Count; i++)
        {
            //Добавляем строку в таблицу
            dt.Rows.Add(imageUrl, b.ElementAt(i).NAME, b.ElementAt(i).PREFIX, b.ElementAt(i).ID.ToString());
        }

        //Показываем панель со списком изделий группы
        Panel1.Visible = true;
        ////Настраиваем ширину ячеек таблицы

        //Столбец изображений элементов
        GridView1.Columns[0].ItemStyle.CssClass = "productImageColumn";

        //Столбец названий групп
        GridView1.Columns[1].ItemStyle.CssClass = "groupNameColumn";

        //Столбец префиксов
        GridView1.Columns[2].ItemStyle.CssClass = "groupPrefix";

        //Столбец ID групп
        GridView1.Columns[3].ItemStyle.CssClass = "productIdColumn";

        GridView1.DataSource = dt;
        GridView1.DataBind();

        if (IsPostBack)
        {
            //Записываем значение выбранной группы  в скрытую переменную 
            parentGroupID.Value = TreeView1.SelectedNode.Value;
            TreeView1.SelectedNode.ChildNodes.Clear();
            //Если добавляем группу во "Все группы" 
            if (targetGroup == "0")
            {
                //Запоминаем путь до выбранного узла дерева
                string nodePath = TreeView1.SelectedNode.ValuePath.ToString();
                //Перезагружаем дерево
                LoadTreeView();
                //Отыскиваем выбранный ранее узел в новом дереве и выбираем его
                TreeView1.FindNode(nodePath).Select();
                ExpandBranchNodes(TreeView1.SelectedNode);
            }
            else
            {
                LoadTreeNodeChildren(TreeView1.SelectedNode);
            }
        }
    }

    private void ExpandBranchNodes(TreeNode treeNode)
    {
        if (treeNode.Depth == 0)
        {
            return;
        }
        treeNode.Parent.Expand();
        ExpandParentNode(treeNode.Parent);
    }

    private void ExpandParentNode(TreeNode treeNode)
    {
        if (treeNode.Depth == 0)
        {
            treeNode.Expand();
            return;
        }
        else
        {
            treeNode.Parent.Expand();
            ExpandParentNode(treeNode.Parent);
        }
    }

    private void LoadTreeView()
    {
        //Загружаем группы изделий выбранной категории
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> q = context.ogk_SelectArticleGroups(0, Convert.ToInt32(ArticleTypesList.SelectedValue));
        List<ogk_SelectArticleGroupsResult> z = q.ToList();

        //Загрузка дерева изделий
        TreeView1.Nodes.Clear();

        //Загрузка корневого узла всех изделий
        TreeNode rootNode = new TreeNode("Все группы", "0", "../Icons/folders.png");
        TreeView1.Nodes.Add(rootNode);

        for (int i = 0; i < z.Count; i++)
        {
            //Изображение группы изделий
            TreeNode tn = new TreeNode(z.ElementAt(i).NAME.ToString(), z.ElementAt(i).ID.ToString(), "../Icons/folder.png");
            TreeView1.Nodes[0].ChildNodes.Add(tn);
            LoadTreeNodeChildren(tn);
        }
        //Сворачиваем все дерево групп
        TreeView1.CollapseAll();
        //Выбираем первый узел дерева
        TreeView1.Nodes[0].Select();
        //Разворачиваем первый узел дерева
        TreeView1.Nodes[0].Expand();
    }
}