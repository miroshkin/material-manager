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
using System.Globalization;
using System.Drawing;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;
using Newtonsoft.Json;


public partial class StartPage : System.Web.UI.Page
{
    delegate void MyDelegate(object sender, EventArgs e);
    delegate void MyDelegate_2(object sender, TreeNodeEventArgs e);
    private event MyDelegate OnLoading;

    protected void Page_Load(object sender, EventArgs e)
    {
        string eventtarget;
        string eventargument;
        string operationType;
        string articleID;


        if (!IsPostBack)
        {
            
            //Скрываем кнопки Вставить/Вырезать изделие
            PasteButton.Visible = false;
            CutButton.Visible = false;

            try
            {
                //Получаем параметры загрузки страницы
                articleID = Request.QueryString["openArticleID"].Trim();
            }
            catch (NullReferenceException)
            {
                articleID = null;
            }

            if (articleID != null)
            {
                //Загружаем дерево с раскрытием по необходимому ID
                LoadPageWithArticleID(articleID);
            }
            else
            {
                //Загружаем страницу в первый раз
                LoadPageForTheFirstTime(sender, e);
            }
        }
        else
        {
            eventtarget = Request.Form["__EVENTTARGET"];
            eventargument = Request.Form["__EVENTARGUMENT"];
            operationType = eventtarget;



            try
            {
                switch (operationType)
                {
                    //Операция добавления изделия
                    case "AddProduct":
                        AddProduct(TreeView1.SelectedNode.Value, eventargument);
                        OnLoading += TreeView1_SelectedNodeChanged;
                        OnLoading(sender, e);
                        OnLoading = null;
                        break;

                    //Операция исключения изделия
                    case "ExcludeProduct":
                        ExcludeProduct(eventargument, TreeView1.SelectedNode.Value);
                        OnLoading += TreeView1_SelectedNodeChanged;
                        OnLoading(sender, e);
                        OnLoading = null;
                        break;
                    //Переход на уровень выше в дереве изделий
                    case "GoToUpperLevel":
                        GoToUpperLevel();
                        OnLoading += TreeView1_SelectedNodeChanged;
                        OnLoading(sender, e);
                        OnLoading = null;
                        break;
                    case "ProductionTypesList":
                        OnLoading += productionTypeList_SelectedIndexChanged;
                        OnLoading += TreeView1_SelectedNodeChanged;
                        OnLoading(sender, e);
                        OnLoading = null;
                        break;
                    case "LogIn":
                        Credentials deserializedArgs = JsonConvert.DeserializeObject<Credentials>(Request["__EVENTARGUMENT"]);
                        string user_name = deserializedArgs.user_name;
                        string password = deserializedArgs.password;

                        if (AuthenticateUser(user_name, password))
                        {
                            Label label = (Label)header1.FindControl("userName");
                            label.Text = user_name;
                        }

                        break;
                }


            }
            catch
            {
                CreateErrorScript("Ни одно изделие не выбрано");
            }
            finally {
                SetRelocationButtonsVisibility();
            }  
        }

        //BlockControls(Account_Login.UserIsAuthentificated(Request.Cookies["UserIsAuthentificated"]));
    }

    

    /// <summary>
    /// Блокирует кнопки в зависимости от типа пользователя
    /// </summary>
    /// <param name="show"></param>
    private void BlockControls(bool show)
    {
        TopRightPanel.Visible = show;
        PasteButton.Visible = show;
        CutButton.Visible = show;
        SaveChangesButton.Visible = show;
        AmountDropDownList.Enabled = show;
        AmountTextBox.Enabled = show;
    }

    /// <summary>
    /// Учетные данные пользователя
    /// </summary>
    public class Credentials
    {
        public string user_name { get; set; }
        public string password { get; set; }
    }

    private void SetButtonToolTip(Button button, string buttonText)
    {
        button.ToolTip = buttonText;
    }
    /// <summary>
    /// Загружает дерево изделий с разворачиванием первого узла с указанным ID изделия
    /// </summary>
    /// <param name="articleID">Идентификатор изделия</param>
    private void LoadPageWithArticleID(string articleID)
    {
        //Загружаем типы продукции в выпадающий список 
        LoadProductionTypes();
        //Значение количества изделий по умолчанию - 0
        AmountTextBox.Text = "0";

        //Название изделия
        ProductName.Text = "";

        //Название типа продукции
        TypeName.Text = "";

        //Название ГОСТа / Чертежа
        DrawingName.Text = "";
        //Скрываем таблицу с данными об изделии
        Table1.Visible = false;

        //Загружаем единицы измерения и масштабы
        LoadUnitsAndScales();

        //Показываем
        SaveChangesButton.Visible = true;

        TreeView1.PopulateNodesFromClient = true;

        //Устанавливаем начальное значение скролла дерева изделий (TreeView1)
        treeview_vertical_scroll.Value = "0";

        ShowQuantityBlock(false);

        ExpandTreeNode(articleID);
    }

    private void ExpandTreeNode(string articleID)
    {
        List<int> node = new List<int>();
        node = GetNodePath(articleID);

        //Адрес начального узла дерева
        string path = node[0].ToString();

        //Определяем тип продукта 
        int type;


        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            try
            {
                type = (int)dataContext.ogk_GetArticleProductionType(Convert.ToInt32(path));
            }
            catch (Exception)
            {
                LoadPageForTheFirstTime(new object(), new EventArgs());
                ISingleResult<ogk_SelectArticleResult> a = dataContext.ogk_SelectArticle(Convert.ToInt32(articleID));
                List<ogk_SelectArticleResult> b = a.ToList();
                string message = Server.UrlEncode("<b>" + b[0].NAME + "</b>") + " не входит в состав ни одного из изделий";
                CreateErrorScript(message);
                return;
            }
        }

        //Загружаем изделия определенного типа
        ProductionTypesList.SelectedValue = type.ToString();

        TreeView1.PopulateNodesFromClient = true;

        OnLoading += productionTypeList_SelectedIndexChanged;
        OnLoading(new object(), new EventArgs());
        OnLoading -= productionTypeList_SelectedIndexChanged;

        TreeView1.CollapseAll();
        path = "";
        TreeNode tn = new TreeNode();
        for (int i = 0; i < node.Count; i++)
        {
            path += node[i].ToString();
            tn = TreeView1.FindNode(path);
            tn.Select();
            tn.Expand();
            CollapseNodes(path);
            path += "/";
        }

        SetScrollPosition(tn);

        OnLoading += TreeView1_SelectedNodeChanged;
        OnLoading(new object(), new EventArgs());
        OnLoading -= TreeView1_SelectedNodeChanged;

    }

    /// <summary>
    /// Устанавливает скролл на корневой узел дерева
    /// [Ex: Node1 -> Node 2 -> Node 3 
    /// при входных узлах Node 2 и Node 3 спозиционируется на Node 1]
    /// </summary>
    /// <param name="tn">Выбранный узел дерева</param>
    private void SetScrollPosition(TreeNode tn)
    {
        //Находим корневой узел
        tn = FindParent(tn);
        //Вычисляем индекс узла
        int index = TreeView1.Nodes.IndexOf(tn);
        //Высота строки дерева в пикселях
        int rowHeight = 24;
        //Выставляем положение скролла
        treeview_vertical_scroll.Value = Convert.ToString(index * rowHeight);
    }

    private void CollapseNodes(string path)
    {
        foreach (TreeNode tn in TreeView1.FindNode(path).ChildNodes)
        {
            tn.Collapse();
        }
    }

    /// <summary>
    /// Формирует массив ID изделий для открытия ветки с изделием
    /// </summary>
    /// <param name="articleID"></param>
    /// <returns></returns>
    private List<int> GetNodePath(string articleID)
    {
        List<int> node = new List<int>();

        node.Add(Convert.ToInt32(articleID));

        int parentID = GetParent(articleID, ref node);

        if (GetParent(parentID.ToString(), ref node) != 0)
        {
            GetParent(GetParent(node[node.Count - 1].ToString(), ref node).ToString(), ref node);
        }
        //Инвертируем узлы
        node.Reverse();
        return node;
    }

    /// <summary>
    /// Определяет ID родительского изделия
    /// </summary>
    /// <param name="articleID"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private int GetParent(string articleID, ref List<int> node)
    {
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            ISingleResult<ogk_GetArticleParentsResult> a = dataContext.ogk_GetArticleParents(Convert.ToInt32(articleID));
            List<ogk_GetArticleParentsResult> b = a.ToList();
            if (b.Count > 0)
            {
                node.Add(b[0].ID);
                return b[0].ID;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Загружает страницу в первый раз
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadPageForTheFirstTime(object sender, EventArgs e)
    {
        //Загружаем типы продукции в выпадающий список 
        LoadProductionTypes();
        //Значение количества изделий по умолчанию - 0
        AmountTextBox.Text = "0";

        //Название изделия
        ProductName.Text = "";

        //Название типа продукции
        TypeName.Text = "";

        //Название ГОСТа / Чертежа
        DrawingName.Text = "";
        //Скрываем таблицу с данными об изделии
        Table1.Visible = false;

        //Загружаем единицы измерения и масштабы
        LoadUnitsAndScales();

        //Показываем
        SaveChangesButton.Visible = true;

        //Выставляем значение выпадающего списка на Телемеханику(2)
        ProductionTypesList.SelectedIndex = 2;

        TreeView1.PopulateNodesFromClient = true;

        OnLoading += productionTypeList_SelectedIndexChanged;
        OnLoading(sender, e);
        OnLoading -= productionTypeList_SelectedIndexChanged;

        //Показываем первый узел дерева
        TreeView1.Nodes[0].Selected = true;
        TreeView1.CollapseAll();
        //Устанавливаем начальное значение скролла дерева изделий (TreeView1)
        treeview_vertical_scroll.Value = "0";

        OnLoading += TreeView1_SelectedNodeChanged;
        OnLoading(sender, e);

        ShowQuantityBlock(false);
    }
    /// <summary>
    /// Производит переход на родительский узел в дереве изделий
    /// </summary>
    private void GoToUpperLevel()
    {
        //Если узел - корневой, то ничего не делаем
        if (TreeView1.SelectedNode.Depth > 0)
        {
            //иначе выделяем родительский узел
            TreeView1.SelectedNode.Parent.Select();
        }
    }

    /// <summary>
    /// Загружает типы продукции в левый верхний выпадающий список
    /// </summary>
    protected void LoadProductionTypes()
    {
        //MainDataClassesDataContext dataContext = new MainDataClassesDataContext();

        //ISingleResult<ogk_SelectProductionTypesResult> a = dataContext.ogk_SelectProductionTypes();
        //List<ogk_SelectProductionTypesResult> b = a.ToList();
        //for (int i = 0; i < b.Count; i++)
        //{
        //    ListItem item = new ListItem();
        //    item.Value = b.ElementAt(i).ID.ToString();
        //    item.Text = b.ElementAt(i).NAME;
        //    ProductionTypesList.Items.Add(item);
        //}

        ProductionTypesList.Items.Add(new ListItem("Electronic components", "Electronic components"));
        ProductionTypesList.Items.Add(new ListItem("Metal alloys", "Metal alloys"));
        ProductionTypesList.Items.Add(new ListItem("Plastics", "Plastics"));

        

    }

    /// <summary>
    /// Генерация скрипта ошибки 
    /// </summary>
    private void CreateErrorScript(string message)
    {
        String csname1 = "ErrorScript";
        Type cstype = this.GetType();

        ClientScriptManager cs = Page.ClientScript;

        if (!cs.IsStartupScriptRegistered(cstype, csname1))
        {
            StringBuilder cstext1 = new StringBuilder();

            cstext1.Append("<script type=text/javascript> Error('" + message + "'); </");
            cstext1.Append("script>");
            cs.RegisterStartupScript(cstype, csname1, cstext1.ToString());
        }
    }

    /// <summary>
    /// Выбор раздела изделий
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void productionTypeList_SelectedIndexChanged(object sender, EventArgs e)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        //ISingleResult<ogk_SelectProductionArticlesResult> a = dataContext.ogk_SelectProductionArticles(Convert.ToInt32(ProductionTypesList.SelectedValue));
        //List<ogk_SelectProductionArticlesResult> b = a.ToList();
        List<ogk_SelectProductionArticlesResult> b = new List<ogk_SelectProductionArticlesResult>();


        switch (ProductionTypesList.SelectedValue)
        {
            case ("Electronic components"):
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 1,
                    NAME = "Capacitor",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 2,
                    NAME = "Diode",
                    ORDER_NUMBER = 200,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 3,
                    NAME = "Transistor",
                    ORDER_NUMBER = 300,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 4,
                    NAME = "Integrated Circuit(IC)",
                    ORDER_NUMBER = 400,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 5,
                    NAME = "Relay",
                    ORDER_NUMBER = 500,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 6,
                    NAME = "Inductor",
                    ORDER_NUMBER = 600,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 7,
                    NAME = "Crystal",
                    ORDER_NUMBER = 600,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                break;

            case "Metal alloys":
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 8,
                    NAME = "Al - Li(aluminum, lithium, sometimes mercury)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 9,
                    NAME = "Alnico(aluminum, nickel, copper)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 10,
                    NAME = "Wood's metal (bismuth, lead, tin, cadmium)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 11,
                    NAME = "Rose metal(bismuth, lead, tin)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                break;

            case "Plastics":
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 12,
                    NAME = "PET(Polyethylene Terephthalate)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 13,
                    NAME = "HDPE (High-Density Polyethylene)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 14,
                    NAME = "PVC (Polyvinyl Chloride)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 15,
                    NAME = "LDPE (Low-Density Polyethylene)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 16,
                    NAME = "PP (Polypropylene)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
                b.Add(new ogk_SelectProductionArticlesResult()
                {
                    ID = 17,
                    NAME = "PS (Polystyrene)",
                    ORDER_NUMBER = 100,
                    DRAWING = "2233er",
                    OTHERS_PRICE = 100
                });
               
                break;
        
    }

        DataTable dt = new DataTable();
        dt.Columns.Add("Название");
        dt.Columns.Add("Чертеж");

        //Загрузка дерева изделий
        TreeView1.Nodes.Clear();

        for (int i = 0; i < b.Count; i++)
        {
            //Изображение изделия
            TreeNode tn = new TreeNode(b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString(), "../Icons/package.png");

            //Добавляем во всплывающую подсказку название продукти и ГОСТ/Чертеж при его наличии
            if ((b.ElementAt(i).DRAWING == null) || (b.ElementAt(i).DRAWING.ToString() == ""))
            {
                tn.ToolTip = b.ElementAt(i).NAME.ToString();
            }
            else
            {
                tn.ToolTip = b.ElementAt(i).NAME.ToString() + " [" + b.ElementAt(i).DRAWING.ToString() + "]";
            }

            int childNodes = Convert.ToInt32(dataContext.ogk_GetArticleChildren(b.ElementAt(i).ID));

            //Добавляем фиктивный узел для изменения изображения узла
            if (childNodes > 0)
            {
                tn.ChildNodes.Add(new TreeNode());
            }
            TreeView1.Nodes.Add(tn);
            dt.Rows.Add(b.ElementAt(i).NAME, b.ElementAt(i).DRAWING);
        }
        //Выбираем первый узел дерева
        TreeView1.Nodes[0].Select();
    }

    /// <summary>
    /// Действия при нажатии на ссылку изделия
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        //Показываем данные об  изделии
        Table1.Visible = true;
        //Показываем блок с единицами измерения
        ShowQuantityBlock(true);
        if (TreeView1.SelectedNode.Depth > 0)
        {
            //Показываем картинку изделия
            Panel1.Visible = true;
            //Скрываем кнопку вверх на уровень
            Panel2.Visible = false;

        }
        else
        {
            //Скрываем картинку изделия
            Panel1.Visible = false;
            //Показываем кнопку вверх на уровень
            Panel2.Visible = true;
        }
        //Загружаем информацию об изделии
        LoadProductDescription();
    }

    /// <summary>
    /// Загружает дочерние узлы 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TreeView1_TreeNodeExpanded(object sender, TreeNodeEventArgs e)
    {
        //Очищаем дочерние узлы выбранного узла
        e.Node.ChildNodes.Clear();
        //Загружаем дочерние узлы
        LoadTreeNodeChildren(e.Node);
        //Раскрываем узел
        e.Node.Expand();
    }

    /// <summary>
    /// Загружает дочерние узлы указанного узла
    /// </summary>
    /// <param name="treeNode"></param>
    private void LoadTreeNodeChildren(TreeNode treeNode)
    {
        if (treeNode.ChildNodes.Count > 0)
        {
            return;
        }

        try
        {
            //Обработка узлов пустышек
            if (treeNode.Value == "")
            {
                return;
            }
            MainDataClassesDataContext context = new MainDataClassesDataContext();
            //ISingleResult<ogk_SelectProductionTreeChildrenResult> a = context.ogk_SelectProductionTreeChildren(Convert.ToInt32(treeNode.Value));
            List<ogk_SelectProductionTreeChildrenResult> list = new List<ogk_SelectProductionTreeChildrenResult>();

            switch (treeNode.Value)
            {
                case "4":
                    list.Add(new ogk_SelectProductionTreeChildrenResult(){ID = 101,NAME = "IC 1"});
                    list.Add(new ogk_SelectProductionTreeChildrenResult(){ID = 102,NAME = "IC 2"});
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 103, NAME = "IC 3" });

                    break;
                case "8":
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 104, NAME = "Alloy 1" });
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 105, NAME = "Alloy 2" });
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 106, NAME = "Alloy 3" });
                    break;
                case "14":
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 107, NAME = "Plastic 1" });
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 108, NAME = "Plastic 2" });
                    list.Add(new ogk_SelectProductionTreeChildrenResult() { ID = 109, NAME = "Plastic 3" });
                    break;

            }


            

            //Адрес изображения для узла
            string imageUrl;
            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i).TYPE_CODE == "BUY")
                {
                    imageUrl = "../Icons/box_closed.png";
                }
                else
                {
                    imageUrl = "../Icons/box.png";
                }
                TreeNode tn = new TreeNode(list.ElementAt(i).NAME.ToString(), list.ElementAt(i).CHILD_ID.ToString(), imageUrl);

                //Добавляем во всплывающую подсказку название элемента и ГОСТ/Чертеж при наличии
                if ((list.ElementAt(i).DRAWING == null) || (list.ElementAt(i).DRAWING.ToString() == ""))
                {
                    tn.ToolTip = list.ElementAt(i).NAME.ToString();
                }
                else
                {
                    //ГОСТ/Чертеж в квадратных скобках
                    tn.ToolTip = list.ElementAt(i).NAME.ToString() + " [" + list.ElementAt(i).DRAWING.ToString() + "]";
                }

                int childNodes = Convert.ToInt32(context.ogk_GetArticleChildren(list.ElementAt(i).CHILD_ID));

                //Добавляем фиктивный узел для изменения изображения узла
                if (childNodes > 0)
                {
                    tn.ChildNodes.Add(new TreeNode());
                }
                treeNode.ChildNodes.Add(tn);
            }
        }
        catch
        {
            //Сообщаем об ошибке 
            CreateErrorScript("The operation cannot be performed.");
        }
    }

    /// <summary>
    /// Загружает дочерние узлы указанного узла
    /// </summary>
    /// <param name="treeNode"></param>
    private void LoadTreeNodes(TreeNode treeNode)
    {
        try
        {
            MainDataClassesDataContext context = new MainDataClassesDataContext();
            ISingleResult<ogk_SelectProductionTreeChildrenResult> a = context.ogk_SelectProductionTreeChildren(Convert.ToInt32(treeNode.Value));
            List<ogk_SelectProductionTreeChildrenResult> list = a.ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Название");
            dt.Columns.Add("Чертеж");
            dt.Columns.Add("Количество");

            //Адрес изображения для узла
            string imageUrl;
            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i).TYPE_CODE == "BUY")
                {
                    imageUrl = "../Icons/box_closed.png";
                }
                else
                {
                    imageUrl = "../Icons/box.png";
                }
                TreeNode tn = new TreeNode(list.ElementAt(i).NAME.ToString(), list.ElementAt(i).CHILD_ID.ToString(), imageUrl);

                //Добавляем во всплывающую подсказку название элемента и ГОСТ/Чертеж при наличии
                if ((list.ElementAt(i).DRAWING == null) || (list.ElementAt(i).DRAWING.ToString() == ""))
                {
                    tn.ToolTip = list.ElementAt(i).NAME.ToString();
                }
                else
                {
                    //ГОСТ/Чертеж в квадратных скобках
                    tn.ToolTip = list.ElementAt(i).NAME.ToString() + " [" + list.ElementAt(i).DRAWING.ToString() + "]";
                }

                int childNodes = Convert.ToInt32(context.ogk_GetArticleChildren(list.ElementAt(i).CHILD_ID));

                //Добавляем фиктивный узел для изменения изображения узла
                if (childNodes > 0)
                {
                    tn.ChildNodes.Add(new TreeNode());
                }
                treeNode.ChildNodes.Add(tn);
            }
        }
        catch
        {
            //Сообщаем об ошибке 
            CreateErrorScript("Операция не может быть выполнена");
        }
    }

    /// <summary>
    /// Загрузка информации об изделии в правую часть страницы
    /// </summary>
    private void LoadProductDescription()
    {
        //Загружаем продукцию
        if (TreeView1.SelectedNode.Depth == 0)
        {
            //MainDataClassesDataContext context = new MainDataClassesDataContext();
            //ISingleResult<ogk_SelectProductResult> a = context.ogk_SelectProduct(Convert.ToInt32(TreeView1.SelectedNode.Value));
            //List<ogk_SelectProductResult> list = a.ToList();

            ////Название изделия
            //ProductName.Text = list.ElementAt(0).NAME;
            //TypeName.Text = list.ElementAt(0).TYPE_NAME;
            //SelectedNodeID.Value = list.ElementAt(0).ID.ToString();
            //ParentNodeID.Value = null;
            //ShowButtonsOnPanel(list.ElementAt(0).TYPE_NAME);
            ////Название чертежа / ГОСТ
            //DrawingName.Text = (list.ElementAt(0).DRAWING == "") ? "<Нет данных>" : list.ElementAt(0).DRAWING;
            //Название изделия

            ProductName.Text = TreeView1.SelectedNode.Text;
            TypeName.Text = "TYPE_NAME";
            SelectedNodeID.Value = "2";
            ParentNodeID.Value = null;
            ShowButtonsOnPanel("Продукция");
            TopRightPanelButtons1.Visible = true;
            //Скрываем кнопку "Исключить изделие"
            TopRightPanelButtons1.FindControl("ExcludeButtonPanel").Visible = false;

            ConfirmationString.Visible = false;
            //Скрываем блок для редактирования количества изделий
            ShowQuantityBlock(false);
        }
        else
        {
            ProductName.Text = TreeView1.SelectedNode.Text;
            
            ////Загружаем покупные и внутренние изделия, входящие в состав продукции
            //MainDataClassesDataContext context2 = new MainDataClassesDataContext();
            //ISingleResult<ogk_SelectProductWithUnitsResult> b = context2.ogk_SelectProductWithUnits(Convert.ToInt32(TreeView1.SelectedNode.Parent.Value),
            //Convert.ToInt32(TreeView1.SelectedNode.Value));
            //List<ogk_SelectProductWithUnitsResult> list = b.ToList();

            //ProductName.Text = list.ElementAt(0).NAME;
            //SelectedNodeID.Value = list.ElementAt(0).ID.ToString();
            //ParentNodeID.Value = TreeView1.SelectedNode.Parent.Value;

            ////Показываем только необходимые кнопки для каждого вида продукции
            //ShowButtonsOnPanel(list.ElementAt(0).TYPE);

            ////Заполняем поля описания изделия
            ////Тип изделия 
            //TypeName.Text = list.ElementAt(0).TYPE;

            ////Выбираем единицы измерения 
            //AmountDropDownList.SelectedValue = list.ElementAt(0).UNIT_ID.ToString();

            ////Чертеж/ГОСТ изделия
            //DrawingName.Text = (list.ElementAt(0).DRAWING == "") ? "<Нет данных>" : list.ElementAt(0).DRAWING;

            ////Загружаем масштаб в скрытую переменную
            //Scale.Value = list.ElementAt(0).SCALE.ToString();

            ////Показываем панель кнопок справа
            TopRightPanelButtons1.Visible = true;

            ////Загружаем количество изделий в необходимом формате
            //double a = (double)list.ElementAt(0).QUANTITY;
            //AmountTextBox.Text = a.ToString("###0.#########");

            ////Показываем строку успешного изменения количества изделия
            ConfirmationString.Visible = false;

            ////Показываем кнопку исключить в правой верхней панели кнопок
            TopRightPanelButtons1.FindControl("ExcludeButtonPanel").Visible = true;

            ////Показываем блок для редактирования количества изделий
            ShowQuantityBlock(true);
        }
    }

    /// <summary>
    /// Проверяет введенную информацию на правильность и запоминает изменения в базе
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        //Загружаем значение масштаба (SCALE) из базы
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectScaleResult> b = dataContext.ogk_SelectScale(Convert.ToInt32(AmountDropDownList.SelectedValue));
        List<ogk_SelectScaleResult> list = b.ToList();
        Scale.Value = list[0].SCALE.ToString();

        string pattern = "";
        switch (Scale.Value)
        {
            //Если единицы измерения штуки и комплекты
            case "1":
                pattern = @"^\d{1,3}$";
                ConfirmationString.Text = "Введите значение от 0 до 1000";
                break;
            //Если единицы измерения м
            case "1000":
                pattern = @"^\d{1,3}([,]\d{1,3})?$";
                ConfirmationString.Text = "Введите значение от 0 до 1000 (возможная точность - 3 знака после запятой)";
                break;
            //Если единицы измерения кв.м
            case "1000000":
                pattern = @"^\d{1,3}([,]\d{1,6})?$";
                ConfirmationString.Text = "Введите значение от 0 до 1000 (возможная точность - 6 знаков после запятой)";
                break;
            //Если единицы измерения куб.м
            case "1000000000":
                pattern = @"^\d{1,3}([,]\d{1,9})?$";
                ConfirmationString.Text = "Введите значение от 0 до 1000 (возможная точность - 9 знаков после запятой)";
                break;
        }

        //Валидация строки с ед.измерения
        if (Regex.IsMatch(AmountTextBox.Text, pattern))
        {
            //Сохраняем изменения в базе
            int outerArticleID = Convert.ToInt32(TreeView1.SelectedNode.Parent.Value);
            int innerArticleID = Convert.ToInt32(TreeView1.SelectedNode.Value);
            long quantity = (long)(Convert.ToDouble(AmountTextBox.Text) * Convert.ToInt32(Scale.Value));
            int unitID = Convert.ToInt32(AmountDropDownList.SelectedValue);
            dataContext.ogk_EditProductionTreeItem(outerArticleID, innerArticleID, quantity, unitID);
            //Формируем строку подтверждения операции добавления
            ConfirmationString.ForeColor = System.Drawing.Color.Green;
            ConfirmationString.Text = "Изменения успешно сохранены";
            ConfirmationString.Visible = true;
        }
        else
        {
            ConfirmationString.ForeColor = System.Drawing.Color.Red;
            ConfirmationString.Visible = true;
        }
    }

    /// <summary>
    /// Скрывает сообщение об изменении количества изделий
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void HideConfirmationString(object sender, EventArgs e)
    {
        e.ToString();
        ConfirmationString.Visible = false;
    }


    /// <summary>
    /// Показывает блок информации о количестве изделия, если true
    /// </summary>
    /// <param name="yes"></param>
    private void ShowQuantityBlock(bool yes)
    {
        if (yes)
        {
            QuantityBlock.Visible = true;
        }
        else
        {
            QuantityBlock.Visible = false;
        }
    }

    /// <summary>
    /// Скрывает/показывает кнопки в правой верхней панели в зависимости от вида изделий
    /// </summary>
    /// <param name="productionType">Тип продукции: Продукция, Внутреннее изделие, Покупное изделие</param>
    public void ShowButtonsOnPanel(string productionType)
    {
        //Панель кнопок добавления изделий 
        Control addButtonsPanel = TopRightPanelButtons1.FindControl("AddButtonsPanel");

        switch (productionType)
        {
            case "Продукция":
                addButtonsPanel.Visible = true;
                break;
            case "Внутреннее изделие":
                addButtonsPanel.Visible = true;
                break;
            case "Покупное изделие":
                addButtonsPanel.Visible = false;
                break;
        }
    }
    /// <summary>
    /// Загружает существующие единицы измерения и масштабы
    /// </summary>
    private void LoadUnitsAndScales()
    {
        //MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        //ISingleResult<ogk_SelectUnitsResult> b = dataContext.ogk_SelectUnits();
        //List<ogk_SelectUnitsResult> list = b.ToList();

        //for (int i = 0; i < list.Count; i++)
        //{
        //    ListItem item = new ListItem();
        //    item.Value = list[i].ID.ToString();
        //    item.Text = list[i].NAME.ToString();
        //    AmountDropDownList.Items.Add(item);
        //}
        AmountDropDownList.Items.Add(new ListItem("pcs", "asdf"));
        AmountDropDownList.Items.Add(new ListItem("pcs1", "asdf"));
        AmountDropDownList.Items.Add(new ListItem("pcs2", "asdf"));
        AmountDropDownList.Items.Add(new ListItem("pcs3", "asdf"));
        AmountDropDownList.Items.Add(new ListItem("pcs4", "asdf"));
    }

    /// <summary>
    /// Добавляет изделие в базу данных и в дерево изделий
    /// </summary>
    /// <param name="outProductID">Номер изделия в которое добавляют новое изделие</param>
    /// <param name="innProductID">Номер добавляемого изделия</param>
    private void AddProduct(string outProductID, string innProductID)
    {
        //Разворачиваем выбранный узел дерева изделий
        TreeView1.SelectedNode.Expand();

        //Добавляем изделие в базу 
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            //Единица измерений по умолчанию (шт.)
            int unitID = dataContext.ogk_GetDefaultProductionUnitID().Value;

            //Номер добавляемого изделия
            int innerProductID = Convert.ToInt32(innProductID);

            //Номер изделия, в которое добавляем изделие
            int outerProductID = Convert.ToInt32(outProductID);

            //Количество изделий по умолчанию = 1
            int number = 1;

            int resultCode = dataContext.ogk_GroupProducts(outerProductID, innerProductID, number, unitID);
            if (resultCode <= 0)
            {
                //Сообщаем об ошибке добавления 
                CreateErrorScript("Операция не может быть выполнена");
                return;
            }
        }
        //Создаем Новый узел
        TreeNode tn = new TreeNode();
        //Загружаем данные для добавляемого узла
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectProductWithUnitsResult> a = context.ogk_SelectProductWithUnits(Convert.ToInt32(outProductID), Convert.ToInt32(innProductID));
        List<ogk_SelectProductWithUnitsResult> list = a.ToList();
        //Название узла
        tn.Text = list.ElementAt(0).NAME;
        //Текст всплывающей подсказки
        tn.ToolTip = tn.Text;
        //Идентификатор изделия
        tn.Value = list.ElementAt(0).ID.ToString();
        //Изображение для вида изделия
        if (list.ElementAt(0).TYPE == "Покупное изделие")
        {
            //Покупное изделие
            tn.ImageUrl = "../Icons/box_closed.png";
        }
        else
        {
            //Внутреннее изделие
            tn.ImageUrl = "../Icons/box.png";
        }
        //Сдвигаем вниз полосу прокрутки на высоту ячейки таблицы TreeView (22 пикселя)
        int scroll = Convert.ToInt32(treeview_vertical_scroll.Value) + 22;
        treeview_vertical_scroll.Value = scroll.ToString();

        //Добавляем узел в дочерние узлы выбранного изделия
        TreeView1.SelectedNode.ChildNodes.Add(tn);

        //Выбираем добавленный узел
        TreeView1.SelectedNode.ChildNodes[TreeView1.SelectedNode.ChildNodes.Count - 1].Selected = true;

        //Добавляем фиктивный узел для отображения
        //того, что узел не пуст (имеет внутри себя изделия)
        int nodeChildren = Convert.ToInt32(context.ogk_GetArticleChildren(list.ElementAt(0).ID));
        if (nodeChildren > 0)
        {
            tn.ChildNodes.Add(new TreeNode());
        }
    }
    /// <summary>
    /// Исключает выбранный продукт из списка изделий, входящих в состав формируемого изделия
    /// </summary>
    /// <param name="outProductID">Номер изделия, из которого исключаем</param>
    /// <param name="innProductID">Номер исключаемого изделия</param>
    private void ExcludeProduct(string outProductID, string innProductID)
    {
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            //Номер исключаемого изделия
            int innerProductID = Convert.ToInt32(innProductID);
            //Номер изделия, из которого исключаем изделие
            int outerProductID;
            //Проверка при удалении root узла дерева
            try
            {
                outerProductID = Convert.ToInt32(outProductID);
            }
            catch
            {
                //Сообщаем об ошибке исключения
                CreateErrorScript("Операция не может быть выполнена");
                return;
            }

            int resultCode = dataContext.ogk_UngroupProducts(outerProductID, innerProductID);
            if (resultCode <= 0)
            {
                //Сообщаем об ошибке исключения
                CreateErrorScript("Операция не может выполнена");
                return;
            }
        }
        //Узнаем корневой узел ветки
        TreeNode parentNode = FindParent(TreeView1.SelectedNode);
        //Запоминаем путь до родителя удаляемого узла
        string nodePath = TreeView1.SelectedNode.Parent.ValuePath;
        //Запоминаем индекс родительской ветки
        int index = TreeView1.Nodes.IndexOf(parentNode);
        //Удаляем родительскую ветку
        TreeView1.Nodes.RemoveAt(index);

        //Формируем новую ветку
        //Удаляем дочерние узлы
        parentNode.ChildNodes.Clear();

        //Перезагружаем родительскую ветку
        LoadParentBranch(parentNode, nodePath);

        //LoadTreeNodeChildren(parentNode);

        //Добавляем новую ветку на место прежней
        TreeView1.Nodes.AddAt(index, parentNode);

        //Раскрываем узлы дерева по очереди
        ExpandTreeView(TreeView1, nodePath);

    }

    private void ExpandTreeView(TreeView TreeView1, string nodePath)
    {
        List<int> nodeList = GetNodesList(nodePath);
        string path = nodeList[0].ToString();
        for (int i = 1; i < nodeList.Count; i++)
        {
            TreeView1.FindNode(path).Expand();
            path += "/" + nodeList[i];
        }
        TreeView1.FindNode(path).Expand();
        TreeView1.FindNode(path).Select();
    }

    private void LoadParentBranch(TreeNode parentNode, string nodePath)
    {
        //Формируем список узлов для загрузки
        List<int> nodesToLoad = GetNodesList(nodePath);

        if (nodesToLoad.Count == 1)
        {
            LoadTreeNodeChildren(parentNode);
            return;
        }

        for (int i = 1; i < nodesToLoad.Count; i++)
        {
            if (parentNode == null)
            {
                break;
            }
            //Загружаем узлы
            LoadTreeNodeChildren(parentNode);

            parentNode = FindSubNode(parentNode, nodesToLoad[i]);
        }
    }

    private static List<int> GetNodesList(string nodePath)
    {
        List<int> nodes = new List<int>();

        MatchCollection mc = Regex.Matches(nodePath, "[0-9]+");

        try
        {
            foreach (Match item in mc)
            {
                nodes.Add(int.Parse(item.Value));
            }
        }
        catch (Exception)
        {
            return null;
        }
        return nodes;
    }



    private TreeNode FindSubNode(TreeNode parentNode, int childNodeId)
    {
        TreeNode searchForTreeNode = new TreeNode();

        foreach (TreeNode tn in parentNode.ChildNodes)
        {
            if (tn.ValuePath == parentNode.ValuePath + "/" + childNodeId)
            {
                return tn;
            }
        }
        return null;
    }

    /// <summary>
    /// Находит root узел дерева по текущему узлу
    /// </summary>
    /// <param name="tn"></param>
    /// <returns></returns>
    private TreeNode FindParent(TreeNode tn)
    {
        if (tn.Depth == 0)
        {
            return tn;
        }
        else
        {
            tn = tn.Parent;
            return FindParent(tn);
        }
    }

    /// <summary>
    /// Обрабатывает событие закрытия узла
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TreeView1_TreeNodeCollapsed(object sender, TreeNodeEventArgs e)
    {
        if (NodesAreOnSameBranch(e.Node, TreeView1.SelectedNode))
        {
            //Выделяем закрытый узел
            e.Node.Select();
            //Обрабатываем событие выбора узла
            OnLoading += TreeView1_SelectedNodeChanged;
            OnLoading(sender, e);
            OnLoading -= TreeView1_SelectedNodeChanged;
        }
        //Очищаем ветку после закрытия списка
        e.Node.ChildNodes.Clear();
        e.Node.ChildNodes.Add(new TreeNode());
    }

    /// <summary>
    /// Определяет лежат ли узлы в одной ветке дерева
    /// </summary>
    /// <param name="tnSenior">Верхний узел (ближе к root)</param>
    /// <param name="tnJeremy">Нижний узел (дальше от root)</param>
    /// <returns></returns>
    private bool NodesAreOnSameBranch(TreeNode tnSenior, TreeNode tnJeremy)
    {
        //Определяем адрес верхнего узла
        string node1Path = tnSenior.ValuePath;
        //Определяем адрес нижнего узла
        string node2Path = tnJeremy.ValuePath;

        //Если адрес верхнего узла полностью входит в адрес нижнего узла, 
        //то узлы лежат в одной ветке дерева
        if (node2Path.StartsWith(node1Path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetRelocationButtonsVisibility()
    {
        //Настраиваем отображение кнопки "Вырезать"
        SetCutButtonVisibility();
        //Настраиваем отображение кнопки "Вставить"
        SetPasteButtonVisibility();
    }

    private void SetCutButtonVisibility()
    {
        if ((TreeView1.SelectedNode.Parent == null))
        {
            CutButton.Visible = false;
            PasteButton.Style["margin-left"] = "-6px";
        }
        else
        {
            CutButton.Visible = true;
            PasteButton.Style["margin-left"] = "-2px";
            SetButtonToolTip(CutButton, "Вырезать изделие\"" + TreeView1.SelectedNode.Text);
        }
    }

    private void SetPasteButtonVisibility()
    {
        int oldParentID = Convert.ToInt32(Session["old_parent_id"]);
        string imageUrl = TreeView1.SelectedNode.ImageUrl;
        switch (imageUrl)
        {
            case "../Icons/box.png":
                //Внутренняя продукция
                SetVisibleProperty(oldParentID);
                break;
            //Продукция
            case "../Icons/package.png":
                SetVisibleProperty(oldParentID);
                break;
            //Покупная продукция
            case "../Icons/box_closed.png":
                PasteButton.Visible = false;
                break;
        }
    }

    private void SetVisibleProperty(int oldParentID)
    {
        if (oldParentID != 0)
        {
            //Предотвращаем добавление в то же место
            if (oldParentID.ToString() == TreeView1.SelectedNode.Value)
            {
                PasteButton.Visible = false;
            }
            //Предотвращаем добавление изделия в само себя
            else if (Session["product_id"].ToString() == TreeView1.SelectedNode.Value.ToString())
            {
                PasteButton.Visible = false;
            }
            else
            {
                PasteButton.Visible = true;
                SetButtonToolTip(PasteButton, "Вставить изделие\"" + Session["product_name"] + "\"");
            }
        }
        else
        {
            PasteButton.Visible = false;
        }
    }

    /// <summary>
    /// Обрабатываем нажатие на кнопку "Вырезать изделие"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cut_button_Click(object sender, EventArgs e)
    {
        SaveSessionState();
        SetRelocationButtonsVisibility();
    }

    

    /// <summary>
    /// Обрабатываем нажатие на кнопку "Вставить изделие"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void paste_button_Click(object sender, EventArgs e)
    {
        
        try
        {
            //ID старого родительского изделия
            int outerProductID = Convert.ToInt32(Session["old_parent_id"]);
            //ID перемещаемго изделия
            int innerProductID = Convert.ToInt32(Session["product_id"]);
            //ID типа изделия
            int oldTypeID = Convert.ToInt32(Session["old_type_id"]);

            //ID нового родительского изделия
            int newOuterProductID = Convert.ToInt32(TreeView1.SelectedNode.Value);
            //ID нового родительского изделия
            int newTypeID = Convert.ToInt32(ProductionTypesList.SelectedItem.Value);

            if (ParametersAreOK(outerProductID, innerProductID, newOuterProductID))
            {
                MainDataClassesDataContext dataContext = new MainDataClassesDataContext();

                //Изменяем родителя для изделия


                if (dataContext.ogk_ChangeParent(outerProductID, innerProductID, newOuterProductID) > 0)
                {
                    string newParentNodeValuePath = TreeView1.SelectedNode.ValuePath;
                    string oldParentNodeValuePath = Session["old_parent_node_valuepath"].ToString();


                    if (oldTypeID == newTypeID)
                    {
                        //Определяем, является ли эта ветвь той же самой 
                        if (ItIsTheSameBranch(oldParentNodeValuePath, newParentNodeValuePath))
                        {
                            //Обновляем ветвь, в которую вставили изделие
                            TreeViewBranchReload(newParentNodeValuePath);
                        }
                        else
                        {
                            //Обновляем ветвь, из которой вырезали изделие
                            TreeViewBranchReload(oldParentNodeValuePath);

                            //Обновляем ветвь, в которую вставили изделие
                            TreeViewBranchReload(newParentNodeValuePath);

                        }
                    }
                    else
                    {
                        //Обновляем ветвь, в которую вставили изделие
                        TreeViewBranchReload(newParentNodeValuePath);
                    }


                    

                    //Выбираем узел изделия
                    foreach (TreeNode tn in TreeView1.SelectedNode.ChildNodes)
                    {
                        if (tn.Value == innerProductID.ToString())
                        {
                            tn.Select();
                            OnLoading += TreeView1_SelectedNodeChanged;
                            OnLoading(sender, e);
                            OnLoading = null;
                        }
                    }
                    //Удаляем данные сессии
                    ClearSessionState();
                    SetRelocationButtonsVisibility();
                }
                else
                {
                    throw new ArgumentException("Перенос изделия невозможен");
                }
            }
            else
            {
                throw new ArgumentException("Перенос изделия невозможен");
            }
        }
        catch (Exception ex)
        {
            CreateErrorScript(ex.Message);
        }
    }

    private bool ItIsTheSameBranch(string oldParentNodeValuePath, string newParentNodeValuePath)
    {
        //Находим узел в дереве
        TreeNode tnOld = TreeView1.FindNode(oldParentNodeValuePath);
        TreeNode tnNew = TreeView1.FindNode(newParentNodeValuePath);

        //Узнаем корневой узел ветки
        TreeNode parentNode1 = FindParent(tnOld);
        TreeNode parentNode2 = FindParent(tnNew);
        
        if (parentNode1 == parentNode2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ParametersAreOK(int outerProductID, int innerProductID, int newOuterProductID)
    {
        if (outerProductID == newOuterProductID)
        {
            return false;
        }
        else
        {
            if (RelocationProductIsAlreadyPresent(newOuterProductID, innerProductID))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private bool RelocationProductIsAlreadyPresent(int newOuterProductID, int innerProductID)
    {
        try
        {
            MainDataClassesDataContext dc = new MainDataClassesDataContext();
            ISingleResult<ogk_SelectProductionTreeChildrenResult> result = dc.ogk_SelectProductionTreeChildren(newOuterProductID);
            List<ogk_SelectProductionTreeChildrenResult> list = result.ToList();
            foreach (var item in list)
            {
                if (item.CHILD_ID == innerProductID)
                {
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CreateErrorScript(ex.Message);
            return true;
        }

    }

    /// <summary>
    /// Перезагружает ветвь
    /// </summary>
    /// <param name="treeNodeValuePath">Адрес узла, до которого будет перезагружена ветвь</param>
    private void TreeViewBranchReload(string treeNodeValuePath)
    {
        //Находим узел в дереве
        TreeNode tn = TreeView1.FindNode(treeNodeValuePath);

        //Узнаем корневой узел ветки
        TreeNode parentNode = FindParent(tn);

        ////Запоминаем путь до родителя удаляемого узла
        string nodePath = tn.ValuePath;

        //Запоминаем индекс родительской ветки
        int index = TreeView1.Nodes.IndexOf(parentNode);
        //Удаляем родительскую ветку
        TreeView1.Nodes.RemoveAt(index);

        //Формируем новую ветку
        //Удаляем дочерние узлы
        parentNode.ChildNodes.Clear();

        //Перезагружаем родительскую ветку
        LoadParentBranch(parentNode, nodePath);

        //Добавляем новую ветку на место прежней
        TreeView1.Nodes.AddAt(index, parentNode);
        ExpandTreeView(TreeView1, nodePath);
    }

    /// <summary>
    /// Сохраняет данные сессии
    /// </summary>
    private void SaveSessionState()
    {
        Session["old_parent_node_valuepath"] = TreeView1.SelectedNode.Parent.ValuePath;
        Session["old_parent_id"] = TreeView1.SelectedNode.Parent.Value;
        Session["product_name"] = TreeView1.SelectedNode.Text;
        Session["product_id"] = TreeView1.SelectedNode.Value;
        Session["old_type_id"] = ProductionTypesList.SelectedItem.Value;
    }

    /// <summary>
    /// Удаляет данные сессии
    /// </summary>
    private void ClearSessionState()
    {
        //ID перемещаемого изделия
        Session.Remove("product_id");
        //Название перемещаемого изделия
        Session.Remove("product_name");
        //ID бывшего родителя изделия
        Session.Remove("old_parent_id");
        //Путь к родительскому узлу изделия
        Session.Remove("old_parent_node_valuepath");
        //Тип изделия
        Session.Remove("old_type_id");
    }

    public static bool AuthenticateUser(string name, string password)
    {
        string con = ConfigurationManager.ConnectionStrings["MEZ_ProductionConnectionString"].ConnectionString;
        using (SqlConnection sqlCon = new SqlConnection(con))
        {
            SqlCommand cmd = new SqlCommand("AuthentificateUser", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;

            string encryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");

            SqlParameter userName = new SqlParameter("@Name", name);
            SqlParameter userPassword = new SqlParameter("@Password", encryptedPassword);

            cmd.Parameters.Add(userName);
            cmd.Parameters.Add(userPassword);

            sqlCon.Open();
            int returnCode = (int)cmd.ExecuteScalar();
            if (returnCode == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}