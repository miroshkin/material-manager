using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Web.Configuration;

public partial class pages_ReportPage : System.Web.UI.Page
{
    //private string USER_LOGIN = "Администратор";
    //private string USER_PASSWORD = "SERVER@2012";
    //private string USER_DOMAIN = "REPORT-SERVER";
    //ReportViewer1.ServerReport.ReportServerUrl = new Uri("http://REPORT-SERVER/reportserver_sqlexpress");


    private string USER_LOGIN = WebConfigurationManager.AppSettings["USER_LOGIN"];
    private string USER_PASSWORD = WebConfigurationManager.AppSettings["USER_PASSWORD"];
    private string USER_DOMAIN = WebConfigurationManager.AppSettings["USER_DOMAIN"];

    private string REPORT_SERVER_URL = WebConfigurationManager.AppSettings["REPORT_SERVER_URL"];

    protected void Page_Load(object sender, EventArgs e)
    {
        string report_name = Server.UrlDecode(Request.QueryString["report_name"]);

        ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

        ReportViewer1.ServerReport.ReportServerCredentials = new MyReportServerCredentials(USER_LOGIN, USER_PASSWORD, USER_DOMAIN);

        ReportViewer1.ServerReport.ReportServerUrl = new Uri(REPORT_SERVER_URL);

        ReportViewer1.ServerReport.ReportPath = report_name;

        ReportViewer1.ShowParameterPrompts = false;
        ReportViewer1.ShowPrintButton = true;
        ReportViewer1.ZoomMode = ZoomMode.FullPage;

        string is_xls = Request.QueryString["is_xls"];

        if (report_name == "/Электронная конструкторская документация/Отчеты/Нормы расхода материалов")
        {
            string id = Request.QueryString["productID"];

            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter();
            parameters[0].Name = "PRODUCT_ID";
            parameters[0].Values.Add(id);
            ReportViewer1.ServerReport.SetParameters(parameters);
        }
       
        string mimeType;
        string encoding;
        string extension;
        string[] streamids;
        Warning[] warnings;

        if (is_xls == "1")
        {
            byte[] bytes = ReportViewer1.ServerReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.ContentType = mimeType;

            Response.AddHeader("Content-Disposition", "attachment; filename=" + report_name + "." + extension);
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.End();
            return;
        }
        else
        {
            byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);

            Response.End();
        }
    }

    // Класс для авторизации на сервере отчетов
    public class MyReportServerCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        private string _userName;
        private string _password;
        private string _domain;

        public MyReportServerCredentials(string userName, string password, string domain)
        {
            _userName = userName;
            _password = password;
            _domain = domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(_userName, _password, _domain);
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string user, out string password, out string authority)
        {
            authCookie = null;
            user = null;
            password = null;
            authority = null;
            return false;
        }
    }
}