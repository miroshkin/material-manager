using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class footer : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpCookie userIsAuthentificated = Request.Cookies["UserIsAuthentificated"];

        if ((userIsAuthentificated != null) && (userIsAuthentificated.Value == "true"))
        {
            if (Request.Cookies["username"]!= null)
            {
                userName.Text = Request.Cookies["username"].Value.ToString();
                userName.Attributes.Add("onclick", "LogOut()");

            }
            else
            {
                userName.Text = "User";
                userName.Attributes.Add("onclick", "LogIn()");
            }
        }
        else
        {
            userName.Text = "User";
            userName.Attributes.Add("onclick", "LogIn()");
        }
        
    }
}