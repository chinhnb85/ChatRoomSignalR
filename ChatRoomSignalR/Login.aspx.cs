using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lisb.Entity;
using Lisb.Repository;

namespace ChatRoomSignalR
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {         
                Session["UserName"] = "admin";
            }            
        }

        [WebMethod]
        public static User CheckLogin(string userName, string passWord)
        {
            var user = UserRepository.GetUserInfoByUserNameAndPassword(userName, passWord);
            if (user != null)
            {
                HttpContext.Current.Session["UserName"] = user.UserName;
                HttpContext.Current.Session["UserId"] = user.Id;
            }          
            return user;
        }        
    }
}