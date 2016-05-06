using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Lisb.Entity;
using Lisb.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ChatRoomSignalR.Hubs
{
    [HubName("chatRoom")]
    public class ChatRoom:Hub
    {
        private static readonly ConcurrentDictionary<string, User> ChatUsers = new ConcurrentDictionary<string, User>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, Room> ChatRooms = new ConcurrentDictionary<string, Room>(StringComparer.OrdinalIgnoreCase);

        #region override methods

        public override Task OnConnected()
        {
            var connectiontId = Context.ConnectionId;
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {            
            DeleteUser();
            GetAllOnlineUsers();
            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region public methods

        [HubMethodName("connect")]
        public bool Connect(string username,string password)
        {            
            try
            {
                var user = UserRepository.GetUserInfoByUserNameAndPassword(username, password);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || user == null)
                {
                    return false;
                }                

                if (GetChatUserByUserId(user.Id) == null)
                {
                    AddUser(user);
                }
                else
                {
                    ModifyUser(user);
                }

                //call only user connect
                Clients.Caller.onConnected(user);

                GetAllOnlineUsers();

                return true;
            }
            catch
            {
                const string msg = "Gặp sự cố trong việc kết nối tới server!";
                Clients.Caller.OnErrorMessage(msg);
                return false;
            }
        }

        [HubMethodName("initiateChat")]
        public bool InitiateChat(string fromUserId, string fromUserName, string toUserId, string toUserName)
        {
            try
            {
                if (string.IsNullOrEmpty(fromUserId) || string.IsNullOrEmpty(fromUserName) || string.IsNullOrEmpty(toUserId) || string.IsNullOrEmpty(toUserName))
                {
                    return false;
                }

                var fromUser = GetChatUserByUserId(fromUserId);
                var toUser = GetChatUserByUserId(toUserId);

                if (fromUser != null && toUser != null)
                {
                    var chatRoom = GetRoomId(fromUser, toUser);

                    if (chatRoom==null)//!CheckIfRoomExists(fromUser, toUser)
                    {
                        //Create New Chat Room
                        chatRoom = new Room();
                        chatRoom.InitiatedBy = fromUser.Id;
                        chatRoom.InitiatedTo = toUser.Id;

                        chatRoom.Users.Add(fromUser);
                        chatRoom.Users.Add(toUser);
                        
                        fromUser.RoomIds.Add(chatRoom.RoomId);
                        toUser.RoomIds.Add(chatRoom.RoomId);

                        //Create SignalR Group for this chat room and add users connection to it
                        Groups.Add(fromUser.ConnectionId, chatRoom.RoomId);
                        Groups.Add(toUser.ConnectionId, chatRoom.RoomId);

                        //Add Chat room object to collection
                        if (ChatRooms.TryAdd(chatRoom.RoomId, chatRoom))
                        {
                            //Generate Client UI for this room
                            Clients.Caller.initiateChatUI(chatRoom);
                        }
                    }
                    else
                    {                                                
                        //Generate Client UI for this room
                        Clients.Caller.initiateChatUI(chatRoom);                        
                    }                    
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Problem in starting chat!");
            }
        }

        [HubMethodName("endChat")]
        public bool EndChat(Message message)
        {
            try
            {
                Room room;
                if (ChatRooms.TryGetValue(message.ConversationId, out room))
                {
                    if (ChatRooms[room.RoomId].InitiatedBy == message.UserId.ToString())
                    {
                        message.MessageText = string.Format("{0} left the chat. Chat Ended!", message.UserName);
                        if (ChatRooms.TryRemove(room.RoomId, out room))
                        {
                            Clients.Client(room.RoomId).receiveEndChatMessage(message);
                            foreach (var messageReceipient in room.Users)
                            {
                                if (messageReceipient.RoomIds.Contains(room.RoomId))
                                {
                                    messageReceipient.RoomIds.Remove(room.RoomId);
                                    Groups.Remove(messageReceipient.ConnectionId, room.RoomId);
                                }
                            }
                        }
                    }
                    else
                    {
                        var messageRecipient = GetChatUserByUserId(message.UserId);
                        if (messageRecipient != null && messageRecipient.RoomIds.Contains(room.RoomId))
                        {
                            room.Users.Remove(messageRecipient);
                            messageRecipient.RoomIds.Remove(room.RoomId);
                            if (room.Users.Count < 2)
                            {
                                message.MessageText = string.Format("{0} left the chat. Chat Ended!", message.UserName);
                                if (ChatRooms.TryRemove(room.RoomId, out room))
                                {
                                    Clients.Client(room.RoomId).receiveEndChatMessage(message);
                                    foreach (var messageReceipient in room.Users)
                                    {
                                        if (messageReceipient.RoomIds.Contains(room.RoomId))
                                        {
                                            messageReceipient.RoomIds.Remove(room.RoomId);
                                            Groups.Remove(messageReceipient.ConnectionId, room.RoomId);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                message.MessageText = string.Format("{0} left the chat.", message.UserName);
                                Groups.Remove(messageRecipient.ConnectionId, room.RoomId);
                                Clients.Client(messageRecipient.ConnectionId).receiveEndChatMessage(message);
                                Clients.Client(room.RoomId).receiveLeftChatMessage(message);
                                Clients.Client(room.RoomId).updateChatUI(room);
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Problem in ending chat!");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Problem in ending chat!");
            }
        }

        [HubMethodName("sendChatMessage")]
        public bool SendChatMessage(Message message)
        {
            try
            {
                Room room;
                if (ChatRooms.TryGetValue(message.ConversationId, out room))
                {
                    message.Id = Guid.NewGuid().ToString();
                    message.MsgDate = DateTime.Now;
                    Clients.Group(message.ConversationId).receiveChatMessage(message, room);
                    return true;
                }
                else
                {
                    throw new InvalidOperationException("Problem in sending message!");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Problem in sending message!");
            }
        }


        #endregion

        #region public private

        private void GetAllOnlineUsers()
        {
            try
            {
                OnlineUsers onlineUsers = new OnlineUsers();
                foreach (var item in ChatUsers)
                {
                    onlineUsers.Users.Add(item.Value);
                }
                Clients.All.onGetAllOnlineUsers(onlineUsers);
            }
            catch
            {                
                const string msg = "Không thể lấy được danh sách đang online!";
                Clients.Caller.onErrorMessage(msg);
            }
        }

        private void AddUser(User obj)
        {
            var user = new User {Id = obj.Id, UserName = obj.UserName, ConnectionId = Context.ConnectionId};
            ChatUsers[obj.Id] = user;            
        }

        private void ModifyUser(User obj)
        {
            var user = GetChatUserByUserId(obj.Id);
            user.UserName = obj.UserName;
            user.ConnectionId = Context.ConnectionId;
            ChatUsers[obj.Id] = user;            
        }

        private bool DeleteUser()
        {            
            User oUser;
            return ChatUsers.TryRemove(Context.ConnectionId, out oUser);            
        }

        private User GetChatUserByUserId(string userId)
        {
            return ChatUsers.Values.FirstOrDefault(u => u.Id == userId);
        }

        private bool CheckIfRoomExists(User fromUser, User toUser)
        {
            try
            {
                if (fromUser.RoomIds.Select(roomId => (from mr in ChatRooms[roomId].Users
                                                       where mr.Id == toUser.Id
                                                       select mr).Count()).Any(count => count > 0))
                {
                    return true;
                }

                return toUser.RoomIds.Select(roomId => (from mr in ChatRooms[roomId].Users where mr.Id == fromUser.Id select mr).Count()).Any(count => count > 0);

                //foreach (var roomId in fromUser.RoomIds)
                //{
                //    var count = ChatRooms[roomId].Users.Select(x => x.Id = toUser.Id).Count();
                //    if (count > 0)
                //    {
                //        return true;
                //    }
                //}
                //return false;
            }
            catch
            {
                return false;
            }            
        }

        private Room GetRoomId(User fromUser, User toUser)
        {
            try
            {
                return (from room in ChatRooms where room.Value.InitiatedBy == toUser.Id && room.Value.InitiatedTo == fromUser.Id select room.Value).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}