using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class Pages_AddGroupPopup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //Принимаем входные параметры

        //Идентификатор родительской группы
        string parentGroupIDParameter = Request.QueryString["parentGroupID"].Trim();

        //Название группы
        string groupNameParameter = Request.QueryString["groupName"].Trim();

        //Префикс для изделий группы
        string prefixParameter = Request.QueryString["prefix"].Trim();

        //Тип продукции
        string typeIDParameter = Request.QueryString["hiddenTypeID"].Trim();

        //Скрываем панель кнопок подтверждения добавления группы
        btnPanel2.Visible = false;

        //Скрываем маркеры ошибок (*) справа от строк
        HideAllWarnings();

        //Задаем тип окна для реакции на нажатие клавиши enter
        windowMarker.Value = "Create";

        //Первый вызов окна
        if ((groupNameParameter == "undefined") & (prefixParameter == "undefined"))
        {
            //Название изделия, чертежа, цену изделия делаем пустыми
            groupName.Text = "";
            prefix.Text = "";
        }
        else
        {
            //Заполняем поле окна с названием группы
            groupName.Text = groupNameParameter;
            //Заполняем поле окна с префиксом
            prefix.Text = prefixParameter;

            //Добавляем группу в базу 
            AddGroup(typeIDParameter, parentGroupIDParameter, groupNameParameter, prefixParameter);
        }
    }

    private void AddGroup(string typeIDParameter, string parentGroupIDParameter, string groupNameParameter, string prefixParameter)
    {
        //Удаляем пробелы по концам строк 
        typeIDParameter = typeIDParameter.Trim();
        parentGroupIDParameter = parentGroupIDParameter.Trim();
        groupNameParameter = groupNameParameter.Trim();
        prefixParameter = prefixParameter.Trim();

        //Проверяем входную информацию по формату и
        //ищем конфликты совпадения имен групп
        if (inputDataIsCorrect(groupNameParameter, parentGroupIDParameter, Convert.ToInt32(typeIDParameter)))
        {
            try
            {
                //Добавляем группу в базу
                MainDataClassesDataContext context = new MainDataClassesDataContext();
                int a = context.ogk_AddArticleGroup(Convert.ToInt32(parentGroupIDParameter), groupNameParameter, prefixParameter, Convert.ToInt32(typeIDParameter));
            }
            catch
            {
            }
            //Скрываем кнопки Сохранить и Отмена
            btnPanel1.Visible = false;
            //Показываем кнопку ОК
            btnPanel2.Visible = true;
            //Блокируем доступ к полям редактирования
            groupName.Enabled = false;
            prefix.Enabled = false;
            //Задаем тип окна для реакции на нажатие клавиши enter
            windowMarker.Value = "CreateOK";

        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Проверяет длину названия группы
    /// и существование аналогичного имени в выбранной родительской группе
    /// </summary>
    /// <param name="groupNameParameter">Проверяемое имя группы</param>
    /// <param name="parentGroupID">Идентификатор родительской группы</param>
    /// <returns>Возвращает true если данные корректны, иначе false</returns>
    private bool inputDataIsCorrect(string groupName, string parentGroupID,int typeIDParameter)
    {
        if ((groupNameIsNotEmpty(groupName) == true) && (groupNameIsNotExist(groupName, parentGroupID, typeIDParameter) == true))
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
    private bool groupNameIsNotExist(string groupName, string parentGroupID, int typeIDParameter)
    {
        groupName = groupName.Trim();

        MainDataClassesDataContext context = new MainDataClassesDataContext();
        int groupsWithThisName = Convert.ToInt32(context.ogk_GroupNameCheckUp(groupName, Convert.ToInt32(parentGroupID), typeIDParameter));

        if (groupsWithThisName > 0)
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

    private void HideAllWarnings()
    {
        //Скрываем * справа от имени группы
        groupNameMessage.Visible = false;
        //Скрываем ** справа от названия префикса
        prefixMessage.Visible = false;
    }
}