﻿using System;
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
            DeleteUser(Context.ConnectionId);
            GetAllOnlineUsers();
            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region public methods

        [HubMethodName("connect")]
        public bool Connect(string username,string userId,string appfrom)
        {            
            try
            {
                var user=new User();                
                switch (appfrom.Replace("\0",""))
                {
                    case "AppSeo":
                        user = UserRepositoryAppSeo.GetUserInfoByUserName(username);                        
                        break;
                    case "AppDemo":
                        user = UserRepository.GetUserInfoByUserName(username);
                        break;
                }
                
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId) || user == null || string.IsNullOrEmpty(appfrom))
                {
                    return false;
                }                

                if (GetChatUserByUserId(user.Id) == null)
                {
                    user.AppFrom = appfrom.Replace("\0", "");
                    AddUser(user);
                }
                else
                {
                    user.AppFrom = appfrom.Replace("\0", "");
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
                    if (!CheckIfRoomExists(fromUser, toUser))
                    {
                        //Create New Chat Room
                        var chatRoom = new Room();
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
                            //Load history
                            var listHistory = ConversationRepository.GetHistoryConversation(fromUser.Id, toUser.Id,fromUser.AppFrom);
                            Clients.Caller.loadHistoryUI(chatRoom, listHistory);
                        }
                    }
                    else
                    {
                        var chatRoom = GetRoomId(fromUser, toUser);
                        if (true)
                        {
                            var listHistory = ConversationRepository.GetHistoryConversation(fromUser.Id, toUser.Id, fromUser.AppFrom);

                            //Generate Client UI for this room                            
                            Clients.Caller.initiateChatUI(chatRoom);
                            //Load history                            
                            Clients.Caller.loadHistoryUI(chatRoom, listHistory);
                        }
                    }                    
                }
                return true;
            }
            catch
            {
                const string msg = "Gặp sự cố trong việc khởi động chat!";
                Clients.Caller.OnErrorMessage(msg);
                return false;                
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
                    if (ChatRooms[room.RoomId].InitiatedBy == message.UserId)
                    {
                        message.MessageText = string.Format("{0} đã ngừng trò chuyện. kết thúc chat!", message.FullName);
                        if (ChatRooms.TryRemove(room.RoomId, out room))
                        {
                            Clients.Group(room.RoomId).receiveEndChatMessage(message);
                            foreach (var user in room.Users)
                            {
                                if (user.RoomIds.Contains(room.RoomId))
                                {
                                    user.RoomIds.Remove(room.RoomId);
                                    Groups.Remove(user.ConnectionId, room.RoomId);
                                }
                            }
                        }
                    }
                    else
                    {
                        var user = GetChatUserByUserId(message.UserId);
                        if (user != null && user.RoomIds.Contains(room.RoomId))
                        {
                            room.Users.Remove(user);
                            user.RoomIds.Remove(room.RoomId);
                            if (room.Users.Count < 2)
                            {
                                message.MessageText = string.Format("{0} đã ngừng trò chuyện. kết thúc chat!", message.FullName);
                                if (ChatRooms.TryRemove(room.RoomId, out room))
                                {
                                    Clients.Group(room.RoomId).receiveEndChatMessage(message);
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
                                message.MessageText = string.Format("{0} đã ngừng trò chuyện.", message.FullName);
                                Groups.Remove(user.ConnectionId, room.RoomId);
                                Clients.Group(user.ConnectionId).receiveEndChatMessage(message);
                                Clients.Group(room.RoomId).receiveLeftChatMessage(message);
                                Clients.Group(room.RoomId).updateChatUI(room);
                            }
                        }
                    }
                }                
                return true;
            }
            catch
            {
                const string msg = "Gặp sự cố trong việc kết thúc chat!";
                Clients.Caller.OnErrorMessage(msg);
                return false;                
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

                    var useridto = room.InitiatedTo;
                    if (useridto == message.UserId)
                    {
                        useridto = room.InitiatedBy;
                    }
                    User user;
                    var appfrom = string.Empty;
                    if (ChatUsers.TryGetValue(message.UserId, out user))
                    {
                        appfrom = user.AppFrom;
                    }

                    var listHistory = ConversationRepository.GetHistoryConversation(message.UserId, useridto, appfrom);

                    Clients.Group(message.ConversationId).receiveChatMessage(message, room,listHistory);                    

                    //save message to data

                    var conversation= new Conversation
                    {
                        MsgId = message.Id,
                        Msg = message.MessageText,
                        MsgDate = message.MsgDate,
                        ConversationId = message.ConversationId,
                        InitiatedBy = message.UserId,                        
                        InitiatedTo = useridto,
                        AppFrom = appfrom,
                        UserName = message.UserName,
                        FullName = message.FullName
                    };
                    ConversationRepository.SaveConversation(conversation);

                    return true;
                }
                return false;
            }
            catch
            {
                const string msg = "Gặp sự cố trong việc gửi message!";
                Clients.Caller.OnErrorMessage(msg);
                return false;                
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
            var user = new User {Id = obj.Id, UserName = obj.UserName,FullName = obj.FullName,Avatar = obj.Avatar,ConnectionId = Context.ConnectionId,AppFrom = obj.AppFrom};
            ChatUsers[obj.Id] = user;            
        }

        private void ModifyUser(User obj)
        {
            var user = GetChatUserByUserId(obj.Id);
            user.UserName = obj.UserName;
            user.FullName = obj.FullName;
            user.Avatar = obj.Avatar;
            user.ConnectionId = Context.ConnectionId;
            user.Avatar = obj.AppFrom;
            ChatUsers[obj.Id] = user;            
        }

        private bool DeleteUser(string connectionId)
        {
            var returnValue = false;
            var user = GetChatUserByConnectionId(connectionId);
            if (user != null && ChatUsers.ContainsKey(user.Id))
            {
                User oUser;
                returnValue = ChatUsers.TryRemove(user.Id, out oUser);

                //remoave from all groups and chatrooms
                foreach (string roomId in oUser.RoomIds)
                {
                    ChatRooms[roomId].Users.Remove(oUser);

                    Groups.Remove(oUser.ConnectionId, roomId);

                    //notify user left chat
                    Message chatMessage = new Message();
                    chatMessage.ConversationId = roomId;
                    chatMessage.UserId = oUser.Id;
                    chatMessage.UserName = oUser.UserName;
                    if (ChatRooms[roomId].InitiatedBy == oUser.Id)
                    {
                        chatMessage.MessageText = string.Format("{0} đã ngừng trò chuyện. kết thúc chat!", oUser.FullName);
                        Room room;

                        if (ChatRooms.TryRemove(roomId, out room))
                        {
                            foreach (var messageReceipient in room.Users)
                            {
                                if (messageReceipient.RoomIds.Contains(roomId))
                                {
                                    messageReceipient.RoomIds.Remove(roomId);
                                }
                            }
                            Clients.Group(roomId).receiveEndChatMessage(chatMessage);
                        }
                    }
                    else
                    {
                        if (ChatRooms[roomId].Users.Count() < 2)
                        {
                            chatMessage.MessageText = string.Format("{0} đã ngừng trò chuyện. kết thúc chat!", oUser.FullName);
                            Room room;
                            if (ChatRooms.TryRemove(roomId, out room))
                            {
                                foreach (var messageReceipient in room.Users)
                                {
                                    if (messageReceipient.RoomIds.Contains(roomId))
                                    {
                                        messageReceipient.RoomIds.Remove(roomId);
                                    }
                                }
                                Clients.Group(roomId).receiveEndChatMessage(chatMessage);
                            }
                        }
                        else
                        {
                            chatMessage.MessageText = string.Format("{0} đã ngừng trò chuyện.", oUser.FullName);
                            Clients.Group(roomId).receiveLeftChatMessage(chatMessage);
                        }
                    }
                }
            }
            return returnValue;
        }

        private User GetChatUserByUserId(string userId)
        {
            return ChatUsers.Values.FirstOrDefault(u => u.Id == userId);
        }

        private User GetChatUserByConnectionId(string connectionId)
        {
            return ChatUsers.Values.FirstOrDefault(u => u.ConnectionId == connectionId);
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
                return false;

                //return toUser.RoomIds.Select(roomId => (from mr in ChatRooms[roomId].Users where mr.Id == fromUser.Id select mr).Count()).Any(count => count > 0);

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