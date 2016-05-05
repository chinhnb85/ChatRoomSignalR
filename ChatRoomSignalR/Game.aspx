<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="ChatRoomSignalR.Game" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
    <link href="/Modules/Game/css/style.css" rel="stylesheet" />
    <link href="Modules/Game/css/jquery-ui.min.css" rel="stylesheet" />
    
    <script src="/Modules/Game/js/jquery-ui.min.js"></script>
    <script src="/Modules/Game/js/jquery.ui.touch-punch.min.js"></script>
    <script src="/Modules/Game/js/jquery.chunkIt.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="imageGrid1" data-chunkit-imgurl="/Modules/Game/img/up.jpg">
        <img src="/Modules/Game/img/up.jpg" width="300" alt=""/>
    </div>
    <div id="click">
        <a class="btn" style="color:#000; border:1px solid #ccc;">Start Game</a>
    </div>
    <script src="/Modules/Game/main.js"></script>
</asp:Content>
