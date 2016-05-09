using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChatRoomSignalR
{
    public partial class Game : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserName"] == null)
                {
                    Response.Redirect("/Login.aspx");
                }
                const string imsChatUrl = "http://192.168.38.97:8080/Chat.aspx?token={0}";
                var token = Base64Encode(Session["UserName"] + "&" + Session["UserId"] + "&AppDemo");
                var iframe = "<iframe src=\""+ string.Format(imsChatUrl, token) + "\" allowtransparency=\"true\" frameBorder=\"0\" width=\"100%\" height=\"100%\"></iframe>";
                ScriptManager.RegisterStartupScript(this, GetType(), "iframechat", @"loadIFrameChat('"+ iframe + "')", true);
            }
        }

        protected void lbtIMSChat_OnClick(object sender, EventArgs e)
        {
            const string imsChatUrl = "http://192.168.38.97:8080/Chat.aspx?token={0}";
            var token = Base64Encode(Session["UserName"] + "&" + Session["UserId"] + "&AppDemo");
            Response.Redirect(string.Format(imsChatUrl, token));
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}