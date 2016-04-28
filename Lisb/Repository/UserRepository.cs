using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lisb.Entity;

namespace Lisb.Repository
{
    public class UserRepository
    {
        private static readonly string ConnectString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        public static UserInfo GetUserInfoByUserNameAndPassword(string username,string password)
        {
            var userInfo = new UserInfo();
            try
            {
                using (var connection=new SqlConnection(ConnectString))
                {
                    string query = "select * from [Account] where username='" + username+"' and password='"+password+"'";
                    connection.Open();

                    using (var cmd=new SqlCommand(query,connection))
                    {
                        var render = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(render);
                        if (dt.Rows.Count > 0)
                        {
                            userInfo.UserId = Convert.ToInt32(dt.Rows[0]["Id"].ToString());
                            userInfo.UserName = dt.Rows[0]["UserName"].ToString();
                            userInfo.AdminCode = Convert.ToInt32(dt.Rows[0]["AdminCode"].ToString());
                        }
                    }
                }

                return userInfo;
            }
            catch
            {
                return userInfo;
            }
        }
    }
}
