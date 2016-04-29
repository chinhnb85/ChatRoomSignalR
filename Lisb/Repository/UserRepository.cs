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

        public static List<UserInfo> GetAllUser()
        {
            var list = new List<UserInfo>();
            try
            {
                using (var connection = new SqlConnection(ConnectString))
                {
                    string query = "select * from [Account] where status=1";

                    connection.Open();

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        var render = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(render);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                list.Add(new UserInfo
                                {
                                    UserId = Convert.ToInt32(dt.Rows[i]["Id"].ToString()),
                                    UserName = dt.Rows[i]["UserName"].ToString(),
                                    AdminCode = Convert.ToInt32(dt.Rows[i]["AdminCode"].ToString())
                                });
                            }
                            
                        }
                    }
                }

                return list;
            }
            catch
            {
                return list;
            }
        }

        public static UserInfo GetUserInfoByUserName(string username)
        {
            var userInfo = new UserInfo();
            try
            {
                using (var connection = new SqlConnection(ConnectString))
                {
                    string query = "select * from [Account] where username='" + username + "'";

                    connection.Open();

                    using (var cmd = new SqlCommand(query, connection))
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
