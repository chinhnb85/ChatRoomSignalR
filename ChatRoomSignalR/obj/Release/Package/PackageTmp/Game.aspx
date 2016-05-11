<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="ChatRoomSignalR.Game" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
    <link href="/Modules/Game/css/style.css" rel="stylesheet" />
    <link href="Modules/Game/css/jquery-ui.min.css" rel="stylesheet" />
    
    <script src="/Modules/Game/js/jquery-ui.min.js"></script>
    <script src="/Modules/Game/js/jquery.ui.touch-punch.min.js"></script>
    <script src="/Modules/Game/js/jquery.chunkIt.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="col-lg-12">
        <asp:LinkButton title="IMS Chat" runat="server" OnClick="lbtIMSChat_OnClick" OnClientClick="window.document.forms[0].target='_blank';">
            <span class="fa fa-wechat"></span>
            Chat
        </asp:LinkButton>
        <a class="btn btn-sm btn-success btnOpenChat">
            <span class="fa fa-wechat"></span>
            Chat
        </a>
    </div>
    <div id="imageGrid1" data-chunkit-imgurl="/Modules/Game/img/up.jpg">
        <img src="/Modules/Game/img/up.jpg" width="300" alt=""/>
    </div>
    <div id="click">
        <a class="btn" style="color:#000; border:1px solid #ccc;">Start Game</a>
    </div>
    <div class="IMSChat">
        <a class="btn btn-sm btn-danger btnClose">
            <span class="fa fa-close"></span>
            Đóng
        </a>
        <%--<iframe src="http://192.168.38.97:8080/Chat.aspx?token=YWRtaW4mMiZBcHBTZW8=" allowtransparency="true" frameBorder="0" width="100%" height="100%"></iframe>--%>
    </div>
    <script src="/Modules/Game/main.js"></script>

</asp:Content>
