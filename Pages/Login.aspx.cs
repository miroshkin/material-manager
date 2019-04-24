using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Data.Linq;

public partial class Account_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LogIn.Attributes.Add("onclick", "Authentificate();return false;");

        //Заполняем список пользователей программы
        MainDataClassesDataContext dc = new MainDataClassesDataContext();
        ISingleResult<ogk_GetUsersResult> result = dc.ogk_GetUsers();
        List<ogk_GetUsersResult> userList = result.ToList();
        for (int i = 0; i < userList.Count; i ++ )
        {
            user_name.Items.Add(new ListItem(userList[i].NAME));
        }
    }


    /// <summary>
    /// Аутентификация пользователя в базе данных
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [WebMethod]
    public static bool AuthentificateUser(string name, string password) {
        string con = ConfigurationManager.ConnectionStrings["MEZ_ProductionConnectionString"].ConnectionString;
        using (SqlConnection sqlCon = new SqlConnection(con))
        {
            SqlCommand cmd = new SqlCommand("ogk_AuthentificateUser",sqlCon);
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