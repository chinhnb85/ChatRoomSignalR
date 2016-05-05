<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PinterrestByMasonry.aspx.cs" Inherits="ChatRoomSignalR.PinterrestByMasonry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="/Modules/Pinterest/js/imagesloaded.pkgd.min.js"></script>
    <script src="/Modules/Pinterest/js/masonry.pkgd.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="list">
       <asp:Repeater ID="Repeater1" runat="server">
           <ItemTemplate>
               <div class="box">
                   <img src="<%# Eval("Url") %>" />
                   <p><%# Eval("Description") %></p>
               </div>
           </ItemTemplate>
       </asp:Repeater>
   </div>
   <div id="imgLoad">
       <img src="/Modules/Pinterest/img/6RMhx.gif" />
   </div>
    <script src="/Modules/Pinterest/pinterest.js"></script>
</asp:Content>
