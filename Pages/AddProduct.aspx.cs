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

public partial class Pages_AddProduct : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string productType = Request.QueryString["productType"];
        string groupID = Request.QueryString["groupID"];
        string outerProductID = Request.QueryString["outerProductID"];
        string scroll2position = Request.QueryString["scroll2value"];

        if (!IsPostBack)
        {
            LoadTree(productType, groupID);
        }
    }

    private void CreateStartUpScript(string productType, string groupID)
    {
        String csname1 = "PopupScript";
        Type cstype = this.GetType();

        ClientScriptManager cs = Page.ClientScript;

        if (!cs.IsStartupScriptRegistered(cstype, csname1))
        {
            StringBuilder cstext1 = new StringBuilder();
            cstext1.Append("<script type=text/javascript> AddNodeContent('" + productType + "','" + groupID + "') </");
            cstext1.Append("script>");
            cs.RegisterStartupScript(cstype, csname1, cstext1.ToString());
        }
    }

    /// <summary>
    /// Загрузка дерева групп продукции
    /// </summary>
    /// <param name="productType">Тип изделия (SELL - продукция, MAKE - внутреннее изделий, BUY - покупное изделие )</param>
    /// <param name="groupID">Идентификатор группы</param>
    public void LoadProductGroupsTree(string productType, string groupID)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectProductionTypesResult> a = dataContext.ogk_SelectProductionTypes();
        List<ogk_SelectProductionTypesResult> b = a.ToList();
        for (int i = 0; i < b.Count; i++)
        {
            TreeNode tn = new TreeNode();
            tn.NavigateUrl = "javascript:AddNodeContent('" + productType + "','" + b.ElementAt(i).ID.ToString() + "')";
            tn.Value = b.ElementAt(i).ID.ToString();
            tn.Text = b.ElementAt(i).NAME;
            tn.ToolTip = b.ElementAt(i).NAME;
            TreeView2.Nodes.Add(tn);
        }
        CreateStartUpScript(productType, b.ElementAt(0).ID.ToString());
    }

    /// <summary>
    /// Загрузка дерева покупных изделий
    /// </summary>
    /// <param name="productType">Тип изделия (SELL - продукция, MAKE - внутреннее изделий, BUY - покупное изделие )</param>
    /// <param name="groupID">Идентификатор</param>
    private void LoadOuterProductGroupsTree(string productType, string groupID)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();

        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(0, 3); // 0 - корневой узел дерева, 3 - покупное изделие
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        for (int i = 0; i < b.Count; i++)
        {
            TreeNode tn = new TreeNode();

            tn.Value = b.ElementAt(i).ID.ToString();
            tn.NavigateUrl = "javascript:AddNodeContent('" + productType + "','" + b.ElementAt(i).ID.ToString() + "')";
            tn.Text = b.ElementAt(i).NAME;
            tn.ToolTip = b.ElementAt(i).NAME;
            TreeView2.Nodes.Add(tn);
            LoadChildrenTreeNodes(tn, Convert.ToInt32(tn.Value), 3);
        }
        TreeView2.CollapseAll();
        CreateStartUpScript(productType, b.ElementAt(0).ID.ToString());
    }

    /// <summary>
    /// Загрузка дочерних узлов дерева групп изделий
    /// </summary>
    /// <param name="parentID">Идентификатор родительского узла</param>
    /// <param name="productType">Тип изделий (1 - Продукция ,2 - Внутреннее изделие ,3 - Покупное изделие)</param>
    private void LoadChildrenTreeNodes(TreeNode tnParent, int parentID, int productType)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(parentID, productType);
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        string productTypeName = "";
        switch (productType)
        {
            case 1:
                productTypeName = "SELL";
                break;
            case 2:
                productTypeName = "MAKE";
                break;
            case 3:
                productTypeName = "BUY";
                break;
        }
        for (int i = 0; i < b.Count; i++)
        {
            TreeNode tn = new TreeNode();
            tn.NavigateUrl = "javascript:AddNodeContent('" + productTypeName + "','" + b.ElementAt(i).ID.ToString() + "')";
            tn.Value = b.ElementAt(i).ID.ToString();
            tn.Text = b.ElementAt(i).NAME;
            tn.ToolTip = b.ElementAt(i).NAME;
            tnParent.ChildNodes.Add(tn);
            LoadChildrenTreeNodes(tn, Convert.ToInt32(tn.Value), productType);
        }
    }

    /// <summary>
    /// Загрузка дерева групп внутренних изделий
    /// </summary>
    /// <param name="productType">Тип изделия (SELL - продукция, MAKE - внутреннее изделий, BUY - покупное изделие )</param>
    /// <param name="groupID">Идентификатор группы</param>
    private void LoadInnerProductGroupsTree(string productType, string groupID)
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();

        // 0 - корневой узел дерева, 2 - внутреннее изделие
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(0, 2);
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        for (int i = 0; i < b.Count; i++)
        {
            TreeNode tn = new TreeNode();
            tn.NavigateUrl = "javascript:AddNodeContent('" + productType + "','" + b.ElementAt(i).ID.ToString() + "')";
            tn.Value = b.ElementAt(i).ID.ToString();
            tn.Text = b.ElementAt(i).NAME;
            tn.ToolTip = b.ElementAt(i).NAME;
            TreeView2.Nodes.Add(tn);
            LoadChildrenTreeNodes(tn, Convert.ToInt32(tn.Value), 2);
        }
        TreeView2.CollapseAll();
        CreateStartUpScript(productType, b.ElementAt(0).ID.ToString());

    }

    private void SelectedNodeChanged_Product()
    {
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectProductionArticlesResult> a = dataContext.ogk_SelectProductionArticles(Convert.ToInt32(TreeView2.SelectedValue));
        List<ogk_SelectProductionArticlesResult> b = a.ToList();
        DataTable dt = new DataTable();
        dt.Columns.Add("Название");
        dt.Columns.Add("Чертеж");

        TreeView2.Nodes.Clear();
        for (int i = 0; i < b.Count; i++)
        {
        }
    }

    private void LoadTreeAndDescription(string productType, string groupID)
    {
        LoadTree(productType, groupID);
    }

    private void LoadTree(string productType, string groupID)
    {
        switch (productType)
        {
            case "SELL":
                //Загружаем дерево групп продукции 
                LoadProductGroupsTree(productType, groupID);
                break;
            case "BUY":
                //Загружаем дерево групп покупных изделий
                LoadOuterProductGroupsTree(productType, groupID);
                break;
            case "MAKE":
                //Загружаем дерево групп внутренних изделий
                LoadInnerProductGroupsTree(productType, groupID);
                break;
        }
    }
}