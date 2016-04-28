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
        private static readonly List<UserInfo> UsersList=new List<UserInfo>();
        private static readonly List<MessageInfo> MessagelList = new List<MessageInfo>();

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

                    strg.Freeflag = 0;

                    UsersList.Add(new UserInfo
                    {
                        ConnectionId = id,
                        UserId = userInfo.UserId,
                        UserName = username,
                        UserGroup = userGroup,
                        Freeflag = 0,
                        Tpflag = 0
                    });

                    Groups.Add(id, userGroup);
                    Clients.Caller.onConnected(id, username, userInfo.UserId, userGroup);

                }
                else
                {
                    UsersList.Add(new UserInfo
                    {
                        ConnectionId = id,
                        UserId = userInfo?.UserId ?? 0,
                        UserName = username,
                        UserGroup = userInfo?.AdminCode.ToString(),
                        Freeflag = 1,
                        Tpflag = 1
                    });

                    Groups.Add(id, userInfo?.AdminCode.ToString());
                    Clients.Caller.onConnected(id, username, userInfo?.UserId, userInfo?.AdminCode.ToString());
                }
            }
            catch
            {
                string msg = "All Administrators are busy, please be patient and try again";
                Clients.Caller.NoExistAdmin();
            }
        }

        [HubMethodName("sendMessageToGroup")]
        public void SendMessageToGroup(string username, string message)
        {
            if (UsersList.Count > 0)
            {
                var strg = UsersList.First(s=>s.UserName==username);
                MessagelList.Add(new MessageInfo
                {
                    UserName = username,
                    Message = message,
                    UserGroup = strg.UserGroup
                });

                Clients.Group(strg.UserGroup).getMessages(username,message);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = UsersList.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
            if (item != null)
            {
                UsersList.Remove(item);

                var id = Context.ConnectionId;
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
                //save conversation to dat abase
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}