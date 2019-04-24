using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.Linq;

public partial class Pages_CreateProduct : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Отключаем кеш браузера
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //Принимаем входные параметры

        //Идентификатор группы изделия
        string groupIDParameter = Request.QueryString["groupID"].Trim();

        //Идентификатор типа изделия
        string typeIDParameter = Request.QueryString["typeID"].Trim();

        //Наименование изделия
        string productNameParameter = Request.QueryString["productName"].Trim();
        
        //Название чертежа/ГОСТа
        string drawingParameter = Request.QueryString["drawing"].Trim();
        
        //Цена изделия
        string priceParameter = Request.QueryString["price"].Trim(); 

        //Скрываем панель кнопок подтверждения добавления изделия
        btnPanel2.Visible = false;
        //Скрываем маркеры ошибок справа от строк
        HideAllWarnings();

        //Задаем тип окна для реакции на нажатие клавиши enter
        windowMarker.Value = "Create";

        //Первый вызов окна
        if ((productNameParameter == "undefined") & (drawingParameter == "undefined") & (priceParameter == "undefined"))
        {
            //Название изделия, чертежа, цену изделия делаем пустыми
            productName.Text = "";
            drawing.Text = "";
            price.Text = "0";
        }
        else
        {
            //Заполняем поле окна с названием изделия
            productName.Text = productNameParameter;
            //Заполняем поле окна с названием чертежа/ГОСТ
            drawing.Text = drawingParameter;
            //Заполняем поле окна с ценой изделия
            price.Text = priceParameter;

            //Добавляем продукт в базу 
            AddProduct(productNameParameter, drawingParameter, groupIDParameter, priceParameter, typeIDParameter);
        }
    }

    /// <summary>
    /// Добавляет изделие с указанными параметрами в базу
    /// </summary>
    /// <param name="productName">Имя изделия</param>
    /// <param name="drawing">Чертеж/ГОСТ изделия</param>
    /// <param name="price">Цена изделия [руб.,коп.]</param>
    private void AddProduct(string productNameParameter, string drawingParameter, string groupID, string priceParameter, string typeID)
    {
        //Удаляем пробелы по концам строк 
        productNameParameter = productNameParameter.Trim();
        drawingParameter = drawingParameter.Trim();
        priceParameter = priceParameter.Trim();

        //Проверяем входную информацию по формату и
        //ищем конфликты совпадения ГОСТов
        if (inputDataIsCorrect(productNameParameter, drawingParameter, priceParameter))
        {
            try
            {
                //Добавляем изделие в базу
                MainDataClassesDataContext context = new MainDataClassesDataContext();
                int a = context.ogk_AddArticle(productNameParameter, drawingParameter, Convert.ToInt32(groupID), Convert.ToDecimal(priceParameter), Convert.ToInt32(typeID));
            }
            catch
            {
                //Обрабатываем ошибку
                CreateErrorScript("Выполнение данной операции невозможно");
            }
            //Скрываем кнопки Сохранить и Отмена
            btnPanel1.Visible = false;
            //Показываем кнопку ОК
            btnPanel2.Visible = true;
            //Блокируем доступ к полям редактирования
            productName.Enabled = false;
            drawing.Enabled = false;
            price.Enabled = false;
            //Задаем тип окна для реакции на нажатие клавиши enter
            windowMarker.Value = "CreateOK";
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Проверяет введенную информацию по формату и возможное существование похожего изделия в базе
    /// </summary>
    /// <param name="productName">Имя изделия</param>
    /// <param name="drawing">ГОСТ/Чертеж</param>
    /// <param name="price">Цена изделия в 0000000 руб.00 коп.</param>
    public bool inputDataIsCorrect(string productName, string drawing, string price)
    {
        //При первом обращении к странице проверку не выполняем
        if ((productName == "undefined") & (drawing == "undefined") & (price == "undefined"))
            return true;
        //Проверка существования аналогичного чертежа + Проверка формата введенной цены
        if ((productNameIsNotEmpty(productName) == true) & (productIsNotExist(drawing, productName) == true) & (priceIsInCorrectFormat(price) == true))
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
    private bool productIsNotExist(string drawing, string productName)
    {
        //Если название чертежа пустое - конец проверки
        if (drawing == "")
        {
            return true;
        }
        MainDataClassesDataContext context = new MainDataClassesDataContext();
        ISingleResult<ogk_DrawingAndNameCheckUpCreateResult> a = context.ogk_DrawingAndNameCheckUpCreate(drawing, productName);
        List<ogk_DrawingAndNameCheckUpCreateResult> b = a.ToList();
        int articlesWithThisDrawing = b.Count;
        if (articlesWithThisDrawing > 0)
        {
            //Изделие с таким же чертежом/ГОСТ уже существует - ошибка
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
    /// Проверяет цену по формату [XXXXXX,XX] 
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
    }

    /// <summary>
    /// Генерирует скрипт вывода ошибки 
    /// </summary>
    ///<param name="message">Сообщение для вывода</param>
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
}