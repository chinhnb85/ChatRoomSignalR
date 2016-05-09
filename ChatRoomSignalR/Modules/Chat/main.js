
$(function () {
    IMSSRChat.Init();
});



IMSSRChat = new function () {
    var chatRooms = 0;   

    window.onbeforeunload = function () {
        //if (chatRooms > 0)
        //    return "Tất cả các ô chat của bạn sẽ kết thúc!";
    };

    this.Init = function() {
        var objHub = $.connection.chatRoom;        

        $.connection.hub.start().done(function () {
            //IMSSRChat.loadEvents(objHub);
            var token = getURLParameters("token");
            var param = Base64.decode(token).split("&");
            var name=null, userid=null, appfrom=null;
            if (param.length > 2) {
                name = param[0];
                userid = param[1];                
                appfrom = param[2];
            }
            objHub.server.connect(name, userid, appfrom).fail(function (e) {
                alert(e);
            });
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
            $("#chatOnlineUser").show();
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
            
            //$('#welcome').append('<div><p>Welcome:' + user.UserName + '</p></div>');
            $('#hUserId').val(user.Id);
            $('#hUserName').val(user.UserName);
            $('#hFullName').val(user.FullName);
        }

        objHub.client.initiateChatUI = function (chatRoom) {
            var chatRoomDiv = $('#chatRoom' + chatRoom.RoomId);
            if (($(chatRoomDiv).length > 0)) {
                var chatRoomText = $('#newmessage' + chatRoom.RoomId);
                var chatRoomSend = $('#chatsend' + chatRoom.RoomId);                

                chatRoomText.show();
                chatRoomSend.show();                
            }
            else {
                var e = $('#new-chatroom-template').tmpl(chatRoom);
                var c = $('#new-chat-header').tmpl(chatRoom);

                var u = $('#hUserId').val() === chatRoom.InitiatedBy ? chatRoom.Users[1].FullName : chatRoom.Users[0].FullName;
                
                chatRooms++;

                //dialog options
                var dialogOptions = {
                    "id": '#messages' + chatRoom.RoomId,
                    "title": u,
                    "width": 280,
                    "height": 355,
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
                    "minimizeLocation": "left"
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
                    var fullName = $('#hFullName').val();

                    var chatMessage = {
                        UserId: senderId,
                        UserName: senderName,
                        FullName: fullName,
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
            //chatRoom.scrollIntoView();
        };        

        objHub.client.receiveEndChatMessage = function (chatMessage) {
            var chatRoom = $('#chatRoom' + chatMessage.ConversationId);
            var chatRoomMessages = $('#messages' + chatMessage.ConversationId);
            var chatRoomText = $('#newmessage' + chatMessage.ConversationId);
            var chatRoomSend = $('#chatsend' + chatMessage.ConversationId);                        

            chatRooms--;

            var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);

            chatRoomText.hide();
            chatRoomSend.hide();            

            e[0].scrollIntoView();
            //chatRoom.scrollIntoView();
        };

        objHub.client.receiveLeftChatMessage = function (chatMessage) {
            var chatRoom = $('#chatRoom' + chatMessage.ConversationId);
            var chatRoomMessages = $('#messages' + chatMessage.ConversationId);
            var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);
            e[0].scrollIntoView();
            //chatRoom.scrollIntoView();
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

function getURLParameters(paramName) {
    var sURL = window.document.URL.toString();
    if (sURL.indexOf("?") > 0) {
        var arrParams = sURL.split("?");
        var arrURLParams = arrParams[1].split("&");
        var arrParamNames = new Array(arrURLParams.length);
        var arrParamValues = new Array(arrURLParams.length);

        var i = 0;
        for (i = 0; i < arrURLParams.length; i++) {
            var sParam = arrURLParams[i].split("=");
            arrParamNames[i] = sParam[0];
            if (sParam[1] != "")
                arrParamValues[i] = unescape(sParam[1]);
            else
                arrParamValues[i] = "No Value";
        }

        for (i = 0; i < arrURLParams.length; i++) {
            if (arrParamNames[i] == paramName) {
                //alert("Parameter:" + arrParamValues[i]);
                return arrParamValues[i];
            }
        }
        return "No Parameters Found";
    }
}

var Base64 = {

    // private property
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",

    // public method for encoding
    encode: function (input) {
        if (typeof (input) == "undefined") var input = "";
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {

            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
			this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
			this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

        }

        return output;
    },

    // public method for decoding
    decode: function (input) {
        if (typeof (input) == "undefined") var input = "";
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

        while (i < input.length) {

            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }

        }

        output = Base64._utf8_decode(output);

        return output;

    },

    // private method for UTF-8 encoding
    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    },

    // private method for UTF-8 decoding
    _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;

        while (i < utftext.length) {

            c = utftext.charCodeAt(i);

            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }

        }

        return string;
    }

}