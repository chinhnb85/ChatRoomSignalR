<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="ChatRoomSignalR.Chat" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Modules/Chat/css/jquery-ui.min.css" rel="stylesheet" />
    <link href="/Modules/Chat/css/style.css" rel="stylesheet" />
    <script src="/Modules/Chat/js/jquery-ui.min.js"></script>    
    <script src="/Modules/Chat/js/jquery.dialogextend.min.js"></script>
    <script src="/Modules/Chat/js/jquery.tmpl.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="welcome"></div>
    <div id="chatError"></div>
    <div id="chatRooms"></div>
    <div id="chatOnlineUser"></div>
    
    <input id="hUserId" type="hidden" />    
    <input id="hUserName" type="hidden" />
    <input id="hFullName" type="hidden" />

    <script src="/Modules/Chat/main.js"></script>
    
    <!--template chat-->
    <script id="new-online-contacts" type="text/x-jquery-tmpl">        
        <ul>
        {%each Users%}
            <li id="chatLink${Id}">
                <a class="chatLink" href="javascript:;" data-id="${Id}" data-username="${UserName}">
                    <img class="imgAvatar" src="{%if $value.Avatar=='' %}/Modules/Chat/css/images/noavatar.png{%else%}${Avatar}{%/if%}"/>
                    ${FullName}
                    <span class="online"></span>
                </a>
            </li>
        {%/each%}
        </ul>        
    </script>
    
    <script id="new-chatroom-template" type="text/x-jquery-tmpl">
        <div id="chatRoom${RoomId}" class="chatRoom">
            <ul id="messages${RoomId}" class="chatMessages"></ul>
            <form id="sendmessage${RoomId}" action="#">
                <input type="text" id="newmessage${RoomId}" class="chatNewMessage" placeholder="Nhập tin nhắn"/>
                <div class="clear"></div>
                <input type="button" id="chatsend${RoomId}" value="Gửi" class="chatSend" data-roomid="${RoomId}" />                
            </form>
        </div>
    </script>

    <script id="new-chat-header" type="text/x-jquery-tmpl">
        <div id="chatRoomHeader${RoomId}">
            {%each Users%}
                {%if $index == 0%}
                    ${UserName}
                {%else%}
                    , ${UserName}
                {%/if%}
            {%/each%}
        </div>
    </script>
    
    <script id="new-message-template" type="text/x-jquery-tmpl">
    <li class="message" id="m-${Id}">
        <strong>${DisplayPrefix}</strong>
        {{html MessageText}}
    </li>
    </script>
    
    <script id="new-notify-message-template" type="text/x-jquery-tmpl">
        <li class="message" id="m-${Id}">
            <strong>{{html MessageText}}</strong>
        </li>
    </script>

</asp:Content>
