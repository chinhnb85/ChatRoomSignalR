using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lisb.Entity;
using Lisb.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ChatRoomSignalR.Hubs
{
    [HubName("chat")]
    public class Chat:Hub
    {
        private static readonly List<User> UsersList=new List<User>();
        private static readonly List<Message> MessagelList = new List<Message>();

        public override Task OnConnected()
        {
            var id = Context.ConnectionId;

            return base.OnConnected();
        }

        [HubMethodName("connect")]
        public void Connect(string username, string password)
        {
            var id = Context.ConnectionId;
            
            var userInfo = UserRepository.GetUserInfoByUserNameAndPassword(username, password);
            try
            {
                if (userInfo?.AdminCode == 0)
                {
                    var strg = UsersList.First(s => s.Tpflag == 1 && s.Freeflag == 1);
                    var userGroup = strg.UserGroup;

                    //strg.Freeflag = 0;

                    var userconnect = new User
                    {
                        ConnectionId = id,
                        Id = userInfo.Id,
                        UserName = username,
                        UserGroup = userGroup,
                        Freeflag = 0,
                        Tpflag = 0
                    };

                    UsersList.Add(userconnect);

                    //add user in group
                    Groups.Add(id, userGroup);

                    //call only user connect
                    Clients.Caller.onConnected(id, username, userInfo.Id, userGroup);

                    GetAllUser();
                }
                else
                {
                    var userconnect = new User
                    {
                        ConnectionId = id,
                        Id = userInfo?.Id ?? "0",
                        UserName = username,
                        UserGroup = userInfo?.AdminCode.ToString(),
                        Freeflag = 1,
                        Tpflag = 1
                    };

                    UsersList.Add(userconnect);

                    Groups.Add(id, userInfo?.AdminCode.ToString());

                    Clients.Caller.onConnected(id, username, userInfo?.Id, userInfo?.AdminCode.ToString());

                    GetAllUser();
                }                
            }
            catch
            {
                //string msg = "All Administrators are busy, please be patient and try again";
                Clients.Caller.NoExistAdmin();
            }
        }

        [HubMethodName("sendMessageToGroup")]
        public void SendMessageToGroup(string username, string message)
        {
            if (UsersList.Count > 0)
            {
                var strg = UsersList.First(s=>s.UserName==username);
                MessagelList.Add(new Message
                {
                    UserName = username,
                    MessageText = message,
                    UserGroup = strg.UserGroup
                });

                //call all user in group
                Clients.Group(strg.UserGroup).getMessages(username,message);
            }
        }

        //viết lại hàm khi user disconnected
        public override Task OnDisconnected(bool stopCalled)
        {
            var id = Context.ConnectionId;

            var item = UsersList.FirstOrDefault(x=>x.ConnectionId== id);
            if (item != null)
            {
                UsersList.Remove(item);
                
                if (item.Tpflag == 0)
                {
                    try
                    {
                        var stradmin = UsersList.First(s => s.Tpflag == 1 && s.UserGroup == item.UserGroup);
                        stradmin.Freeflag = 1;
                    }
                    catch
                    {
                        Clients.Caller.NoExistAdmin();
                    }
                }

                GetAllUser();

                //save conversation to dat abase
            }

            return base.OnDisconnected(stopCalled);
        }

        //get all user
        [HubMethodName("getAllUser")]
        public void GetAllUser()
        {            
            var list = UserRepository.GetAllUser();
            foreach (var item in list)
            {                
                if (UsersList.Any(x => x.UserName == item.UserName))
                {
                    item.Freeflag = 1;
                }
            }

            Clients.All.GetAllUser(list);
        }

        [HubMethodName("createGroupChat")]
        public void CreateGroupChat(string username)
        {
            var userInfo1 = UsersList.Any(x => x.UserName == username) ? UsersList.First(x => x.UserName == username) : UserRepository.GetUserInfoByUserName(username);

            var id = Context.ConnectionId;

            var userInfo2 = UsersList.First(x => x.ConnectionId == id);
            userInfo2.UserGroup = "chatgroup";//userInfo1.UserName + userInfo2.UserName;
            userInfo1.UserGroup = "chatgroup"; //userInfo1.UserName + userInfo2.UserName;

            //add in group
            Groups.Add(id, userInfo2.UserGroup);

            Clients.Caller.showGroupChatUserName(userInfo1);
        }

        [HubMethodName("sendMessageToGroupByUserName")]
        public void SendMessageToGroupByUserName(string username,string groupname, string message)
        {
            Clients.Group(groupname).getMessagesToGroupByUserName(username, groupname,message);
        }
    }
}