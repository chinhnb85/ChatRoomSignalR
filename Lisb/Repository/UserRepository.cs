using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Lisb.Entity;

namespace Lisb.Repository
{
    public class UserRepository
    {
        private static readonly string ConnectString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        public static User GetUserInfoByUserNameAndPassword(string username,string password)
        {
            var userInfo = new User();
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
                            userInfo.Id = dt.Rows[0]["Id"].ToString();
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

        public static List<User> GetAllUser()
        {
            var list = new List<User>();
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
                                list.Add(new User
                                {
                                    Id = dt.Rows[i]["Id"].ToString(),
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

        public static User GetUserInfoByUserName(string username)
        {
            var userInfo = new User();
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
                            userInfo.Id = dt.Rows[0]["Id"].ToString();
                            userInfo.UserName = dt.Rows[0]["UserName"].ToString();
                            userInfo.FullName = dt.Rows[0]["FullName"].ToString();
                            userInfo.Avatar = dt.Rows[0]["Avatar"].ToString();
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
