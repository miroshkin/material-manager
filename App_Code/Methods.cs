using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for Methods
/// </summary>
public class Methods
{
    /// <summary>
    /// Загружает все типы изделий, кроме продукции
    /// </summary>
    /// <returns></returns>
    public ListItem[] LoadProductionArticleTypesForGroups()
    {
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleTypesForGroupsResult> a = context.ogk_SelectArticleTypesForGroups(); ;
        List<ogk_SelectArticleTypesForGroupsResult> list = a.ToList();

        ListItem[] types = new ListItem[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            ListItem li = new ListItem(list.ElementAt(i).NAME, list.ElementAt(i).ID.ToString());
            types[i] = li;
        }
        return types;
    }

    /// <summary>
    /// Определяет количество подгрупп в группе
    /// </summary>
    /// <param name="groupID">Идентификатор выбранной группы</param>
    /// <returns></returns>
    public int CountSubgroups(int groupID)
    {
        int count = 0;
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            //Вызываем процедуру подсчета подгрупп в группе
            count = dataContext.ogk_GetGroupChildrenCount(groupID).Value;
        }
        return count;
    }

    /// <summary>
    /// Определяет число изделий в группе
    /// </summary>
    /// <returns>Количество изделий</returns>
    public int CountProductsInGroup(int groupID)
    {
        int count = 0;
        using (MainDataClassesDataContext dataContext = new MainDataClassesDataContext())
        {
            //Вызываем процедуру подсчета изделий в группе
            count = dataContext.ogk_GetProductsCountForArticleGroup(groupID).Value;
        }
        return count;
    }

    /// <summary>
    /// Заменяет "&quot;" и  "&#39;" на кавычки " и ' для описания события OnClick
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceSpecialSymbols(string str)
    {
        str = str.Replace("&#39;", @"\'");
        str = str.Replace("&quot;", "\"");

        return str;
    }

    /// <summary>
    /// Заменяет "&quot;" и  "&#39;" на кавычки " и ' для ToolTip
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceSpecialSymbols_ToolTip(string str)
    {
        str = str.Replace("&#39;", @"'");
        str = str.Replace("&quot;", "\"");
        str = str.Replace("&#171;", @"«");
        str = str.Replace("&#187;", @"»");
        str = str.Replace("&gt;", @">");
        str = str.Replace("&lt;", @"<");
        
        return str;
    }
}