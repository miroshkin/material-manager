using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Linq;
using System.Text.RegularExpressions;

public partial class Pages_EditProductPopup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        
        //Получаем инфо о необходимом действии
        string action = Request.QueryString["action"];
        
        //Идентификатор изделия
        string IDparameter = Request.QueryString["ID"];
        
        //Группа изделия
        string groupIDParameter = Request.QueryString["groupID"];

        //Тип изделия 
         string typeIDParameter = Request.QueryString["typeID"];

        //Наименование изделия
        string productNameParameter = Server.UrlDecode(Request.QueryString["productName"]);
        
        //Чертеж изделия
        string drawingParameter = Server.UrlDecode(Request.QueryString["drawing"]);

        //Цена изделия
        string priceParameter = Server.UrlDecode(Request.QueryString["price"]);

        //Скрываем сообщения об ошибках изменения 
        HideAllWarnings();

        //Скрываем панель удачного изменения изделия
        btnPanel2.Visible = false;

        //Задаем тип окна для реакции на нажатие клавиши enter
        windowMarker.Value = "Save";

        LoadProductionArticleTypes();

        switch (action) 
        {
            //Сохраняем изменения информации об изделии
            case "SaveChanges":
                productionArticlesTypes.SelectedValue = typeIDParameter;
                LoadProductionArticlesGroups(typeIDParameter);
                for (int i = 0; i < productionArticlesGroups.Items.Count; i++)
                {
                    if (productionArticlesGroups.Items[i].Value == groupIDParameter.Trim())
                    {
                        productionArticlesGroups.Items[i].Selected = true;
                    }
                }
                SaveChanges(IDparameter, productNameParameter, drawingParameter, groupIDParameter, priceParameter, typeIDParameter);
                LoadProductionInfoPartly(productNameParameter, drawingParameter, priceParameter);
                //LoadProductionInfo(IDparameter);

                break;
            //Меняем тип изделия 
            case "ChangeProductionType":
                productionArticlesTypes.SelectedValue = typeIDParameter;
                LoadProductionArticlesGroups(typeIDParameter);
                LoadProductionInfoPartly(productNameParameter,drawingParameter, priceParameter);
                break;
            //Загружаем информацию об изделии
            case "LoadProductionInfo":
                LoadProductionInfo(IDparameter);
                break;
        }
    }

    // Загружает все типы изделий, кроме продукции
    private void LoadProductionArticleTypes()
    {
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleTypesForGroupsResult> a = context.ogk_SelectArticleTypesForGroups(); ;
        List<ogk_SelectArticleTypesForGroupsResult> list = a.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            ListItem li = new ListItem(list.ElementAt(i).NAME, list.ElementAt(i).ID.ToString());
            productionArticlesTypes.Items.Add(li);
        }
    }

    private void LoadProductionInfoPartly(string productNameParameter, string drawingParameter, string priceParameter)
    {
        //Загружаем данные об изделии в окно
        //Название изделия
        productName.Text = productNameParameter.Trim();
        //Чертеж(ГОСТ)
        drawing.Text = drawingParameter.Trim();
        //Цена изделия
        price.Text = priceParameter.Trim();
    }

    /// <summary>
    /// Загружает в окно информацию об изделии
    /// </summary>
    /// <param name="ID">Идентификатор изделия</param>
    private void LoadProductionInfo(string ID)
    {
        //Загружаем данные об изделии в окно
        //Edit Article
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleResult> a = context.ogk_SelectArticle(Convert.ToInt32(ID));
        List<ogk_SelectArticleResult> list = a.ToList();

        //Название изделия
        productName.Text = list.ElementAt(0).NAME;
        //Чертеж(ГОСТ)
        drawing.Text = list.ElementAt(0).DRAWING;
        //Цена изделия
        price.Text = list.ElementAt(0).SELF_PRICE.ToString("F2");//.Replace('.',',');
        //Тип изделия
        int typeID = list.ElementAt(0).TYPE;
        productionArticlesTypes.SelectedValue = typeID.ToString();

        //Идентификатор группы изделия
        string groupID = list.ElementAt(0).PRODUCTION_GROUP_ID.Value.ToString();
        LoadProductionArticlesGroups(productionArticlesTypes.SelectedValue);
        //Выбираем из списка группу изделия
        for (int i = 0; i < productionArticlesGroups.Items.Count; i++)
        {
            if (productionArticlesGroups.Items[i].Value == groupID)
            {
                productionArticlesGroups.Items[i].Selected = true;
            }
        }
    }
    
   //Сохраняет изменен ия для данного изделия 
    private void SaveChanges(string IDparameter, string productNameParameter, string drawingParameter, string groupIDParameter, string priceParameter, string typeIDParameter)
    {
        //Обрезаем пробелы по концам строк 
        productNameParameter = productNameParameter.Trim();
        drawingParameter = drawingParameter.Trim();
        priceParameter = priceParameter.Trim();

        if (inputDataIsCorrect(IDparameter,productNameParameter, drawingParameter, priceParameter))
        {
            try
            {
                int ID = Convert.ToInt32(IDparameter);
                string Name = productNameParameter;
                string Drawing = drawingParameter;
                int Groupid = Convert.ToInt32(groupIDParameter);
                //priceParameter = priceParameter.Replace(',', '.');
                decimal Price = Convert.ToDecimal(priceParameter);
                int TypeID = Convert.ToInt32(typeIDParameter);
                MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
                int result = dataContext.ogk_EditArticle(ID, Name, Drawing, Groupid, Price, TypeID);
                btnPanel1.Visible = false;
                SuccessMessage.Visible = true;
                btnPanel2.Visible = true;

                productionArticlesGroups.Enabled = false;
                productionArticlesTypes.Enabled = false;
                productName.Enabled = false;
                drawing.Enabled = false;
                price.Enabled = false;

                //Задаем тип окна для реакции на нажатие клавиши enter
                windowMarker.Value = "SaveOK";
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Ошибка сохранения изделия");
                ShowErrorMessage(ex.Message);

            }
        }
        else
        {
            return;
        }
    }
    
    //Префикс для визуального разделения групп в списке
    string prefix = "";
    /// <summary>
    /// Загружает все группы выбранного типа изделий
    /// </summary>
    /// <param name="selectedType">Идентификатор типа изделий</param>
    private void LoadProductionArticlesGroups(string selectedType)
    {
        int typeID = Convert.ToInt32(selectedType);
        
        //Загружаем группы изделий выбранной категории
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(0, typeID);
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        for (int i = 0; i < b.Count; i++)
        {
            //string groupName = prefix + b.ElementAt(i).NAME.ToString();
            //groupName = CutGroupName(groupName);
            ListItem item = new ListItem(prefix + b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString());
            //Добавляем группу в список
            productionArticlesGroups.Items.Add(item);
            //Загружаем подгруппы 
            LoadSubgroups(item);
        }
    }
    
    /// <summary>
    /// Загружает подгруппы указанной группы
    /// </summary>
    /// <param name="item">Идентификатор выбранной группы</param>
    private void LoadSubgroups(ListItem item)
    {
        //Загружаем подгруппы 
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        ISingleResult<ogk_SelectArticleGroupsResult> a = dataContext.ogk_SelectArticleGroups(Convert.ToInt32(item.Value), Convert.ToInt32(productionArticlesTypes.SelectedValue));
        List<ogk_SelectArticleGroupsResult> b = a.ToList();
        if (b.Count > 0)
        {
            //Выделяем подгруппы префиксом из пробелов
            prefix += "\xA0\xA0\xA0\xA0";
            for (int i = 0; i < b.Count; i++)
            {
                ListItem li = new ListItem(prefix + b.ElementAt(i).NAME.ToString(), b.ElementAt(i).ID.ToString());
                productionArticlesGroups.Items.Add(li);
                LoadSubgroups(li);
            }
            //Укорачиваем префикс для загрузки последующих элементов
            prefix = prefix.Substring(4);
        }
    }

    /// <summary>
    /// Обрезает название группы до определенной длины, для того, 
    /// чтобы выпадающий список по ширине совпадал с закрытым списком
    /// </summary>
    /// <param name="groupName">Название группы</param>
    /// <returns></returns>
    private string CutGroupName(string groupName)
    {
        //Опытная длина строки
        if (groupName.Length > 40)
        {
            //3 Символа обрезаем для многоточия
            groupName = groupName.Substring(0, groupName.Length - 3) + "...";
            return groupName;
        }
        return groupName;
    }

    /// <summary>
    /// Скрывает строки, указывающую на ошибки данных
    /// </summary>
    private void HideAllWarnings()
    {
        //Скрываем * возле поля с названием изделия
        productNameMessage.Visible = false;
        //Скрываем ** возле поля с названием чертежа
        drawingMessage.Visible = false;
        //Скрываем *** возле поля с ценой
        priceMessage.Visible = false;

        SuccessMessage.Visible = false;
    }

    /// <summary>
    /// Проверяет введенную информацию по формату и возможное существование похожего изделия в базе
    /// </summary>
    /// <param name="productName">Имя изделия</param>
    /// <param name="drawing">ГОСТ/Чертеж</param>
    /// <param name="price">Цена изделия в 0000000 руб.00 коп.</param>
    public bool inputDataIsCorrect(string IDparameter, string productName, string drawing, string price)
    {
        //При первом обращении к странице проверку не выполняем
        if ((productName == "undefined") & (drawing == "undefined") & (price == "undefined"))
            return true;
        //Проверка существования аналогичного чертежа + Проверка формата введенной цены
        if ((productNameIsNotEmpty(productName) == true) & (drawingIsNotExist(IDparameter, drawing, productName) == true) & (priceIsInCorrectFormat(price) == true))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Определяет пуста ли строка с именем изделия
    /// </summary>
    /// <param name="productName"></param>
    /// <returns></returns>
    private bool productNameIsNotEmpty(string productName)
    {
        //Если строка непустая
        if (productName.Length > 0)
        {
            return true;
        }
        else
        //Если строка пустая
        {
            //Показываем * справа от введенной строки
            productNameMessage.Visible = true;
            //Выводим сообщение об ошибке
            ShowErrorMessage("*Наименование изделия\n должно быть непустым");
            return false;
        }
    }

    /// <summary>
    /// Проверяет существует ли изделие с указанным чертежом/ГОСТ
    /// </summary>
    /// <param name="drawing">Искомый чертеж/ГОСТ</param>
    private bool drawingIsNotExist(string IDparameter, string drawing, string productName)
    {
        //Если название чертежа пустое - конец проверки
        if (drawing == "")
        {
            return true;
        }
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_DrawingAndNameCheckUpEditResult> a = context.ogk_DrawingAndNameCheckUpEdit(drawing, Convert.ToInt32(IDparameter), productName);
        List<ogk_DrawingAndNameCheckUpEditResult> b = a.ToList();
        //Количество изделий с таким же именем и чертежом
        int numberOfProducts = b.Count();
        
        if (numberOfProducts > 0)
        {
            //Изделие с таким же чертежом/ГОСТ уже существует - ошибка
            productNameMessage.Visible = true;
            drawingMessage.Visible = true;
            ShowErrorMessage("*,** Изделие с заданными именем и чертежом(ГОСТ) уже существует");
            return false;
        }
        else
        {
            //Изделие с таким же чертежом/ГОСТ не существует - все ОК
            return true;
        }
    }

    /// <summary>
    /// Выводит ошибки при создании изделия
    /// </summary>
    /// <param name="message">Сообщение ошибки</param>
    private void ShowErrorMessage(string message)
    {
        //Выводим ошибку в одну из предусмотренных строк
        //Если нет ошибок - выводим сообщение в первую строку
        if (ErrorMessage1.Text.Length == 0)
        {
            ErrorMessage1.Text = message;
        }
        //Иначе выводим сообщение во вторую строку
        else if (ErrorMessage2.Text.Length == 0)
        {
            ErrorMessage2.Text = message;
        }
        //Иначе выводим сообщение в третью строку
        else
        {
            ErrorMessage3.Text = message;
        }
    }

    /// <summary>
    /// Проверяет цену по формату [XXXXXX,XXXX] 
    /// </summary>
    /// <param name="price">Цена в виде строки</param>
    private bool priceIsInCorrectFormat(string priceParameter)
    {
        //Проверяем строку через регулярные выражения
        if (Regex.IsMatch(priceParameter, @"^\d{1,6}([,]\d{1,4})?$"))
        {
            return true;
        }
        else
        {
            //Показываем *** возле поля ввода цены
            priceMessage.Visible = true;
            //Выводим сообщение об ошибке
            ShowErrorMessage("***Цена изделия в неверном формате"); ;
            return false;
        }
    }
}