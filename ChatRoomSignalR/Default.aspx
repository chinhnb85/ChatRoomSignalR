﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChatRoomSignalR.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="divLogin" class="mylogin">
        User Name:<input id="txtUserName" type="text" /><br />
        Password :<input id="txtPassword" type="password" /><br />
        <input id="btnLogin" type="button" value="Login" />
        <div id="divalarm"></div>
    </div>
    
    <div id="divChat" class="mylogin">

        <div id="welcome"></div><br />
        <input id="txtMessage" type="text" />
        <input id="btnSendMessage" type="button" value="Send" />
        <div id="divMessage"></div>

    </div>
    
    <input id="hUserId" type="hidden" />
    <input id="hId" type="hidden" />
    <input id="hUserName" type="hidden" />
    <input id="hGroup" type="hidden" />
    
<script src="/scripts/jquery-2.2.3.min.js"></script>
<script src="scripts/jquery.signalR-2.2.0.min.js"></script>
<script src="/signalr/hubs" type="text/javascript"></script>

<script type="text/javascript">
    
    $(function() {
        $("#divChat").hide();
        $("#divLogin").show();

        var objHub = $.connection.chat;

        loadClientMethods(objHub);

        $.connection.hub.start().done(function() {
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
</script>

</asp:Content>
