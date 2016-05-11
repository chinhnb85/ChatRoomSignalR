using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Lisb.Entity;

namespace Lisb.Repository
{
    public class UserRepositoryAppSeo
    {
        private static readonly string ConnectString = ConfigurationManager.ConnectionStrings["conn_appseo"].ConnectionString;        

        public static User GetUserInfoByUserName(string username)
        {
            var userInfo = new User();
            try
            {
                using (var connection = new SqlConnection(ConnectString))
                {
                    string query = "select * from [User] where username='" + username + "'";

                    connection.Open();

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        var render = cmd.ExecuteReader();                        
                        DataTable dt = new DataTable();
                        dt.Load(render);
                        if (dt.Rows.Count > 0)
                        {
                            userInfo.Id = dt.Rows[0]["UserId"]+"AppSeo";
                            userInfo.UserName = dt.Rows[0]["Username"].ToString();
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
