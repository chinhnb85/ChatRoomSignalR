
$(function () {
    IMSSRChat.Init();
});

IMSSRChat = new function () {
    
    this.Init = function() {
        var objHub = $.connection.chatRoom;        

        $.connection.hub.start().done(function () {
            IMSSRChat.loadEvents(objHub);
        });

        IMSSRChat.loadClientMethods(objHub);
    };

    this.loadEvents = function (objHub) {

        $("#btnLogin").click(function () {

            var name = $("#txtUserName").val();
            var pass = $("#txtPassword").val();

            if (name.length > 0 && pass.length > 0) {

                objHub.server.connect(name, pass).fail(function(e) {
                    alert(e);
                });

            }
            else {
                alert("Please Insert UserName and Password");
            }
        });        

        $("#txtPassword").keypress(function (e) {
            if (e.which === 13) {
                $("#btnLogin").click();
            }
        });

        $("#txtMessage").keypress(function (e) {
            if (e.which === 13) {
                $('#btnSendMessage').click();
            }
        });
    }

    this.loadClientMethods = function (objHub) {
        
        objHub.client.onErrorMessage = function (msg) {
            var divNoExist = $('<div><p>' + msg + '</P></div>');
            $(divNoExist).hide();
            $('#chatError').prepend(divNoExist);
            $(divNoExist).fadeIn(900).delay(9000).fadeOut(900);
        }

        objHub.client.onGetAllOnlineUsers = function (chatUsers) {
            var e = $('#new-online-contacts').tmpl(chatUsers);            
            $("#chatOnlineUser").empty();
            $("#chatOnlineUser").html(e);

            $(".chatLink").off("click").on("click", function() {
                if (objHub == null) {
                    alert("Gặp sự cố kết nối tới server. Hay liên hệ với Admin!");
                    return;
                }                
                var senderId = $('#hUserId').val();
                var senderName = $('#hUserName').val();
                var toUserId = $(this).attr("data-id");
                var toUserName = $(this).attr("data-username");

                objHub.server.initiateChat(senderId, senderName, toUserId, toUserName).fail(function (e) {
                    alert(e);
                });
            });
        };

        objHub.client.onConnected = function (user) {
            
            $('#welcome').append('<div><p>Welcome:' + user.UserName + '</p></div>');
            $('#hUserId').val(user.Id);
            $('#hUserName').val(user.UserName);
            $("#divLogin").hide();
        }

        objHub.client.initiateChatUI = function (chatRoom) {
            var chatRoomDiv = $('#chatRoom' + chatRoom.RoomId);
            if (($(chatRoomDiv).length > 0)) {
                var chatRoomText = $('#newmessage' + chatRoom.RoomId);
                var chatRoomSend = $('#chatsend' + chatRoom.RoomId);
                var chatRoomEndChat = $('#chatend' + chatRoom.RoomId);

                chatRoomText.show();
                chatRoomSend.show();
                chatRoomEndChat.show();
            }
            else {
                var e = $('#new-chatroom-template').tmpl(chatRoom);
                var c = $('#new-chat-header').tmpl(chatRoom);
                
                //dialog options
                var dialogOptions = {
                    "id": '#messages' + chatRoom.RoomId,
                    "title": c,
                    "width": 360,
                    "height": 365,
                    "modal": false,
                    "resizable": false,
                    "close": function () { javascript: IMSSRChat.endChat(objHub,'' + chatRoom.RoomId + ''); $(this).remove(); }
                };

                // dialog-extend options
                var dialogExtendOptions = {
                    "closable": true,
                    "maximizable": false,
                    "minimizable": true,
                    "dblclick": 'maximize',
                    "titlebar": 'transparent'
                };

                e.dialog(dialogOptions).dialogExtend(dialogExtendOptions);

                $('#sendmessage' + chatRoom.RoomId).keypress(function (e) {
                    if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
                        $('#chatsend' + chatRoom.RoomId).click();
                        return false;
                    }                    
                });

                $(".chatSend").off("click").on("click", function () {
                    var roomId = $(this).attr("data-roomid");
                    var chatRoomNewMessage = $('#newmessage' + roomId);

                    if (chatRoomNewMessage.val() == null || chatRoomNewMessage.val() === "")
                        return false;

                    var senderId = $('#hUserId').val();
                    var senderName = $('#hUserName').val();

                    var chatMessage = {
                        UserId: senderId,
                        UserName: senderName,
                        ConversationId: roomId,
                        MessageText: chatRoomNewMessage.val()
                    };

                    chatRoomNewMessage.val('');
                    chatRoomNewMessage.focus();
                    objHub.server.sendChatMessage(chatMessage).fail(function (e) {
                        alert(e);
                    });

                    return false;
                });
            }
        };

        objHub.client.receiveChatMessage = function (message, room) {
            objHub.client.initiateChatUI(room);
            var chatRoom = $('#chatRoom' + message.ConversationId);
            var chatRoomMessages = $('#messages' + message.ConversationId);
            var e = $('#new-message-template').tmpl(message).appendTo(chatRoomMessages);
            e[0].scrollIntoView();
            chatRoom.scrollIntoView();
        };        

        objHub.client.receiveEndChatMessage = function (chatMessage) {
            var chatRoom = $('#chatRoom' + chatMessage.ConversationId);
            var chatRoomMessages = $('#messages' + chatMessage.ConversationId);
            var chatRoomText = $('#newmessage' + chatMessage.ConversationId);
            var chatRoomSend = $('#chatsend' + chatMessage.ConversationId);
            var chatRoomEndChat = $('#chatend' + chatMessage.ConversationId);            

            var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);

            chatRoomText.hide();
            chatRoomSend.hide();
            chatRoomEndChat.hide();

            e[0].scrollIntoView();
            chatRoom.scrollIntoView();
        };

        objHub.client.receiveLeftChatMessage = function (chatMessage) {
            var chatRoom = $('#chatRoom' + chatMessage.ConversationId);
            var chatRoomMessages = $('#messages' + chatMessage.ConversationId);
            var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);
            e[0].scrollIntoView();
            chatRoom.scrollIntoView();
        };

        objHub.client.updateChatUI = function (chatRoom) {
            var chatRoomHeader = $('#chatRoomHeader' + chatRoom.RoomId);
            var c = $('#new-chat-header').tmpl(chatRoom);
            chatRoomHeader.html(c);
        };
    }

    this.endChat = function (objHub,roomId) {
        var chatRoomNewMessage = $('#newmessage' + roomId);

        var senderId = $('#hUserId').val();
        var senderName = $('#hUserName').val();

        var chatMessage = {
            UserId: senderId,
            UserName: senderName,
            ConversationId: roomId,
            MessageText: chatRoomNewMessage.val()
        };
        chatRoomNewMessage.val('');
        chatRoomNewMessage.focus();
        objHub.server.endChat(chatMessage).fail(function (e) {
            alert(e);
        });
    };

};