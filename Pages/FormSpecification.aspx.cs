using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Specification : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string productID = Request.QueryString["productID"].Trim();
        productIDHiddenField.Value = productID;
        MainDataClassesDataContext dataContext = new MainDataClassesDataContext();
        List<ogk_GetProductSpecificationResult> list = dataContext.ogk_GetProductSpecification(Convert.ToInt32(productID)).ToList();

        //Формируем вид таблицы для последующего заполнения
        DataTable dt = new DataTable();
        dt.Columns.Add("COMPONENT_NAME");
        dt.Columns.Add("ARTICLE_GROUP_NAME");
        dt.Columns.Add("DRAWING");
        dt.Columns.Add("UNIT_NAME");
        dt.Columns.Add("QUANTITY");
        dt.Columns["QUANTITY"].DataType = Type.GetType("System.String");

        //Заполняем таблицу данными из запроса
        foreach (var item in list)
        {
            dt.Rows.Add(item.COMPONENT_NAME, item.ARTICLE_GROUP_NAME, item.DRAWING, item.UNIT_NAME, Convert.ToDouble(item.QUANTITY).ToString("0.#########"));
        }

        SpecificationView.DataSource = dt;
        SpecificationView.DataBind();

    }

    protected void SpecificationView_RowDataBound(object sender, GridViewRowEventArgs e)
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
            ////Курсор в виде руки-указателя
            //e.Row.Attributes.Add("style", "cursor:pointer;");

        }
    }
}