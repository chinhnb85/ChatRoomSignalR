using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Lisb.Entity;

namespace Lisb.Repository
{
    public class ConversationRepository
    {
        private static readonly string ConnectString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;        

        public static bool SaveConversation(Conversation conversation)
        {
            if (conversation == null) return false;
            try
            {
                using (var connection = new SqlConnection(ConnectString))
                {
                    string query = "insert into [Conversation](MsgId,Msg,MsgDate,ConversationId,InitiatedBy,InitiatedTo,AppFrom,UserName,FullName)" +
                                   " values('" + conversation.MsgId+
                                   "','" + conversation.Msg +
                                   "','" + conversation.MsgDate +
                                   "','" + conversation.ConversationId +
                                   "','" + conversation.InitiatedBy +
                                   "','" + conversation.InitiatedTo +
                                   "','" + conversation.AppFrom +
                                   "','" + conversation.UserName +
                                   "',N'" + conversation.FullName +
                                   "')";

                    connection.Open();

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        var render = cmd.ExecuteNonQuery();                        

                        if (render > 0)
                        {
                            return true;
                        }                                                
                    }

                    connection.Close();
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static List<Conversation> GetHistoryConversation(string initiatedBy,string initiatedTo,string appfrom)
        {
            var list = new List<Conversation>();
            try
            {
                using (var connection = new SqlConnection(ConnectString))
                {
                    string query = "select * from [Conversation] where (InitiatedBy='"+ initiatedBy+ "' and InitiatedTo='" + initiatedTo+ "') or (InitiatedBy='" + initiatedTo + "' and InitiatedTo='" + initiatedBy + "')";

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
                                list.Add(new Conversation
                                {
                                    MsgId = dt.Rows[i]["MsgId"].ToString(),
                                    Msg = dt.Rows[i]["Msg"].ToString(),
                                    MsgDate = Convert.ToDateTime(dt.Rows[i]["MsgDate"].ToString()),
                                    UserName = dt.Rows[i]["UserName"].ToString(),
                                    FullName = dt.Rows[i]["FullName"].ToString()
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
    }
}
