<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChatRoomSignalR.Default" %>
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
    
    <div id="divListUser"><ul id="ulListUser"></ul></div>
    
    <div id="divChatForUserName">
        
    </div>
    
    <input id="hUserId" type="hidden" />
    <input id="hId" type="hidden" />
    <input id="hUserName" type="hidden" />
    <input id="hGroup" type="hidden" />
    
<script src="/Modules/Home/main.js"></script>

</asp:Content>
