$(function () {
    $("#divChat").hide();
    $("#divLogin").show();

    var objHub = $.connection.chat;

    loadClientMethods(objHub);

    $.connection.hub.start().done(function () {
        loadEvents(objHub);
    });
});

function loadClientMethods(objHub) {

    objHub.client.NoExistAdmin = function () {
        var divNoExist = $('<div><p>There is no Admin to response you try again later</P></div>');
        $("#divChat").hide();
        $("#divLogin").show();

        $(divNoExist).hide();
        $('#divalarm').prepend(divNoExist);
        $(divNoExist).fadeIn(900).delay(9000).fadeOut(900);
    }

    objHub.client.getMessages = function (userName, message) {

        $("#txtMessage").val('');
        $('#divMessage').append('<div><p>' + userName + ': ' + message + '</p></div>');

        var height = $('#divMessage')[0].scrollHeight;
        $('#divMessage').scrollTop(height);
    }

    objHub.client.onConnected = function (id, userName, userId, userGroup) {

        var strWelcome = 'Welcome' + +userName;
        $('#welcome').append('<div><p>Welcome:' + userName + '</p></div>');

        $('#hId').val(id);
        $('#hUserId').val(userId);
        $('#hUserName').val(userName);
        $('#hGroup').val(userGroup);

        $("#divChat").show();
        $("#divLogin").hide();
    }

    objHub.client.getAllUser = function (data) {
        if (data != null) {
            $("#ulListUser").empty();
            $.each(data, function (i, item) {
                var cls = item.Freeflag === 0 ? "offline" : "online";
                $("#ulListUser").append('<li><a class="aDetailUser" data-id="' + item.UserName + '">' + item.UserName + ' <i class="' + cls + '"></i></a></li>');
            });

            $(".aDetailUser").click(function () {

                var username = $(this).attr("data-id");

                if (username.length > 0) {

                    objHub.server.createGroupChat(username);
                }
                else {
                    alert("Không thể gửi tin nhắn lúc này.");
                }
            });
        }
    }

    objHub.client.showGroupChatUserName = function (userInfo) {
        if (userInfo !== undefined) {
            var cls = userInfo.Freeflag === 0 ? "offline" : "online";
            var temp = '<div class="w_chat">' +
                            '<div class="title_chat">' +
                                '<p><i class="' + cls + '"></i> ' + userInfo.UserName + '</p>' +
                                '<span class="btnClose"><i class="fa fa-close"></i></span>' +
                            '</div>' +
                            '<div id="' + userInfo.UserGroup + '" class="body_chat">' +                               
                            '</div>' +
                            '<div class="foot_chat">' +
                                '<input id="' + userInfo.UserName + '" type="text" />' +
                                '<input class="btnSendMessageUserName" type="button" value="Send" data-user="' + userInfo.UserName + '" data-group="' + userInfo.UserGroup + '"/>' +
                            '</div>' +
                        '</div>';

            $("#divChatForUserName").append(temp);

            $('.btnSendMessageUserName').off("click").on("click",function () {
                var groupname = $(this).attr("data-group");
                var idmsg = $(this).attr("data-user");

                var msg = $("#" + idmsg).val();

                if (msg.length > 0) {

                    var userName = $('#hUserName').val();
                    

                    objHub.server.sendMessageToGroupByUserName(userName, groupname, msg);
                }
            });
        }
    }

    objHub.client.getMessagesToGroupByUserName = function (userName, groupname, message) {

        $("#" + groupname).val('');
        $("#" + groupname).append('<p>' + userName + ': ' + message + '</p>');

        var height = $("#" + groupname)[0].scrollHeight;
        $("#" + groupname).scrollTop(height);
    }
}

function loadEvents(objHub) {

    $("#btnLogin").click(function () {

        var name = $("#txtUserName").val();
        var pass = $("#txtPassword").val();

        if (name.length > 0 && pass.length > 0) {

            objHub.server.connect(name, pass);

        }
        else {
            alert("Please Insert UserName and Password");
        }
    });

    $('#btnSendMessage').click(function () {

        var msg = $("#txtMessage").val();

        if (msg.length > 0) {

            var userName = $('#hUserName').val();

            objHub.server.sendMessageToGroup(userName, msg);
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