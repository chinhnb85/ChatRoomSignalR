<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ChatRoomSignalR.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <script src="/Scripts/jquery-2.2.3.min.js"></script>
    <script src="/Modules/Login/main.js"></script>
    <style>
        #imgLoad
        {
            position:fixed;
            bottom:0;   
            left:40%;
            display:none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divLogin" class="mylogin">
        User Name:<input id="txtUserName" type="text" /><br />
        Password :<input id="txtPassword" type="password" /><br />
        <input id="btnLogin" type="button" value="Login" />        
    </div>
    <div id="imgLoad">
       <img src="/Modules/Pinterest/img/6RMhx.gif" />
   </div>
    </form>
</body>
</html>
