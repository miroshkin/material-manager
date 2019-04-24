using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data.Linq;

public partial class Pages_EditGroupPopup : System.Web.UI.Page
{
    Methods m = new Methods();

    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        
        //Принимаем входные параметры

        //Выполняемое действие
        string action = Server.UrlDecode(Request.QueryString["action"]);

        //Тип группы
        string typeID = Server.UrlDecode(Request.QueryString["typeID"]);

        //Идентификатор группы изделия
        string groupIDParameter = Server.UrlDecode(Request.QueryString["groupID"]);

        //Идентификатор родительской группы
        string parentGroupIDParameter = Server.UrlDecode(Request.QueryString["parentGroupID"]);

        //Название группы
        string groupNameParameter = Server.UrlDecode(Request.QueryString["groupName"]);

        //Префикс для изделий группы
        string prefixParameter = Server.UrlDecode(Request.QueryString["prefix"]);

        //Скрываем панель кнопок подтверждения добавления изделия
        btnPanel2.Visible = false;
        //Скрываем текст ошибки 
        ErrorMessage.Visible = false;
        //Скрываем маркеры ошибок справа от строк
        HideAllWarnings();

        //Записываем значение идентификатора в скрытую переменную
        groupID.Value = groupIDParameter;

        groupName.Text = getStringWithoutSpecialSymbols(groupNameParameter);
        prefixLabel.Text = getStringWithoutSpecialSymbols(prefixParameter);
    
        //Загружаем группы выбранного типа
        LoadProductionArticlesGroups(typeID, groupIDParameter);
        
        //Выбираем переданный в параметрах родительскую группу
        productionArticlesGroups.SelectedValue = parentGroupIDParameter;

        //Задаем тип окна для реакции на нажатие клавиши enter
        windowMarker.Value = "Save";
        
        switch (action)
        { 
            case "SaveChanges":
                SaveChanges(groupIDParameter, groupNameParameter, prefixParameter, parentGroupIDParameter,typeID);
                break;

            case "ChangeProductionType":
                //ChangeProductionType();
                break;
        }
        
        if((m.CountProductsInGroup(Convert.ToInt32(groupIDParameter)) > 0 )||(m.CountSubgroups(Convert.ToInt32(groupIDParameter)) > 0))
        {
            //Удаляем кнопку удалить группу
            deleteButtonPanel.Visible = false;
        }
    }

    private void SaveChanges(string groupIDParameter, string groupNameParameter, string prefixParameter ,string parentGroupIDParameter,string typeID)
    {
        //Обрезаем пробелы по концам строки
        groupNameParameter = groupNameParameter.Trim();
        prefixParameter = prefixParameter.Trim();
        parentGroupIDParameter = parentGroupIDParameter.Trim();

        //Заменяем несколько пробелов одним
        groupNameParameter = ReplaceSpacesWithOnlyOne(groupNameParameter);
        prefixParameter = ReplaceSpacesWithOnlyOne(prefixParameter);

          if (inputDataIsCorrect(groupNameParameter,groupIDParameter, parentGroupIDParameter,typeID) == true)
        {
            MainDataClassesDataContext context = new MainDataClassesDataContext();
            
            //Сохраняем название и префикс группы
            int a = context.ogk_EditArticleGroup(Convert.ToInt32(groupIDParameter), groupNameParameter, prefixParameter);
            
            //Сохраняем родительскую группу
            int b = context.ogk_MoveArticleGroup(Convert.ToInt32(groupIDParameter), Convert.ToInt32(parentGroupIDParameter));
            if (a > 0)
            {
                //Скрываем кнопки Сохранить Отмена и Удалить
                btnPanel1.Visible = false;
                
                //Показываем строку успешного изменения данных группы
                SuccessMessage.Visible = true;
                
                //Показываем кнопку ОК 
                btnPanel2.Visible = true;

                //Задаем тип окна для реакции на нажатие клавиши enter
                windowMarker.Value = "SaveOK";
                
                //Блокируем доступ к строковым полям редактирования названия,префикса,родительской группы
                groupName.Enabled = false;
                prefixLabel.Enabled = false;
                productionArticlesGroups.Enabled = false;
            }
        }
    }

    private string ReplaceSpacesWithOnlyOne(string inputString)
    {
        //Шаблон нескольких пробелов подряд
        Regex r = new Regex(@"\s+");
        //Заменяем несколько пробелов одним
        inputString = r.Replace(inputString, @" ");
        return inputString;
    }

    /// <summary>
    /// Проверяет длину названия группы
    /// и существование аналогичного имени в выбранной родительской группе
    /// </summary>
    /// <param name="groupNameParameter">Проверяемое имя группы</param>
    /// <param name="parentGroupID">Идентификатор родительской группы</param>
    /// <returns>Возвращает true если данные корректны, иначе false</returns>
    private bool inputDataIsCorrect(string groupName, string groupID, string parentGroupID,string typeID)
    {
        if ((groupNameIsNotEmpty(groupName) == true) & (groupNameIsNotExist(groupName, groupID, parentGroupID, typeID) == true))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Проверяет существование имени группы в выбранной группе
    /// </summary>
    /// <param name="groupName">Проверяемое название группы</param>
    /// <param name="parentGroupID">Идентификатор родительской группы</param>
    /// <returns></returns>
    private bool groupNameIsNotExist(string groupName,string groupIDParameter, string parentGroupID,string typeID)
    {
        groupName = groupName.Trim();

        MainDataClassesDataContext context = new MainDataClassesDataContext();
        int groupsWithThisName = Convert.ToInt32(context.ogk_GroupNameCheckUp(groupName, Convert.ToInt32(parentGroupID), Convert.ToInt32(typeID)));
        int groupID = Convert.ToInt32(context.ogk_GroupNameCheckUpGetID(groupName, Convert.ToInt32(parentGroupID)));
        if ((groupsWithThisName > 0) & (groupID != Convert.ToInt32(groupIDParameter)))
        {
            //Группа с таким же именем уже существует - ошибка
            groupNameMessage.Visible = true;
            ErrorMessage.Visible = true;
            ErrorMessage.Text = "* Группа с таким именем уже существует";
            return false;
        }
        else
        {
            //Изделие с таким же чертежом/ГОСТ не существует - все ОК
            return true;
        }
    }

    private bool groupNameIsNotEmpty(string groupName)
    {
        groupName = groupName.Trim();
        if (groupName.Length > 0)
        {
            return true;
        }
        else
        {
            //Отмечаем место ошибки звездочкой *
            groupNameMessage.Visible = true;
            //Показываем текст ошибки
            ErrorMessage.Visible = true;
            ErrorMessage.Text = "* Название группы должно быть непустым";
            return false;
        }
    }


    /// <summary>
    /// Возвращает отформатированную строку (замена "&nbsp;" на "")
    /// </summary>
    /// <param name="groupNameParameter"></param>
    /// <returns>Преобразованная строка</returns>
    private string getStringWithoutSpecialSymbols(string groupNameParameter)
    {
        groupNameParameter = groupNameParameter.Replace("&quot;", "\"");
        groupNameParameter = groupNameParameter.Replace("&nbsp;", "");
        return groupNameParameter;
    }
    /// <summary>
    /// Скрывает предупреждения об ошибках 
    /// </summary>
    private void HideAllWarnings()
    {
        //Скрываем * возле поля с названием группы
        groupNameMessage.Visible = false;
        //Скрываем ** возле поля с префиксом группы
        prefixMessage.Visible = false;
    }


    string prefix = "";
    /// <summary>
    /// Загружает все группы выбранного типа изделий
    /// </summary>
    /// <param name="selectedType">Идентификатор типа изделий</param>
    private void LoadProductionArticlesGroups(string selectedType, string groupID)
    {
        int typeID = Convert.ToInt32(selectedType);
        productionArticlesGroups.Items.Add(new ListItem("Все группы","0"));
        //Загружаем группы изделий выбранной категории
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(0, typeID);
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        for (int i = 0; i < b.Count; i++)
        {
            //Исключаем возможность добавления группы в саму себя, исключая название группы из списка 
            if (b.ElementAt(i).ID.ToString() == groupID) 
            {
                continue;
            }
            //string groupName = prefix + b.ElementAt(i).NAME.ToString();
            //groupName = CutGroupName(groupName);
            ListItem item = new ListItem(prefix + b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString());
            //Добавляем группу в список
            productionArticlesGroups.Items.Add(item);
            //Загружаем подгруппы 
            LoadSubgroups(item, groupID, typeID.ToString());
        }
    }

    /// <summary>
    /// Загружает подгруппы указанной группы
    /// </summary>
    /// <param name="item">Идентификатор выбранной группы</param>
    private void LoadSubgroups(ListItem item, string groupID, string typeID)
    {
        //Загружаем подгруппы 
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(Convert.ToInt32(item.Value), Convert.ToInt32(typeID));
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        if (b.Count > 0)
        {
            //Выделяем подгруппы префиксом из пробелов
            prefix += "\xA0\xA0\xA0\xA0";
            for (int i = 0; i < b.Count; i++)
            {
                //Исключаем возможность добавления группы в саму себя, исключая название группы из списка 
                if (b.ElementAt(i).ID.ToString() == groupID)
                {
                    continue;
                }
                ListItem li = new ListItem(prefix + b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString());
                productionArticlesGroups.Items.Add(li);
                LoadSubgroups(li, groupID, typeID);
            }
            //Укорачиваем префикс для загрузки последующих элементов
            prefix = prefix.Substring(4);
        }
    }
    

}
