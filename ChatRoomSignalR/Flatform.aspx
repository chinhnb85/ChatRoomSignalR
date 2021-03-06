﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Flatform.aspx.cs" Inherits="ChatRoomSignalR.Flatform" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script src="/Modules/Flatform/js/platform.js"></script>--%>
    <style>
        .gifs a {
            position: relative;
            display: block;
        }

        .gif-preload {
            display: none;
        }

        .gif-loading {
            position: absolute;
            width: 40px;
            height: 40px;
            font-size: 40px;
            color: #fff;
            top: 0;
            left: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="gifs row small-up-4">
      <div class="column"><a href="http://guycodeblog.mtv.com/wp-content/uploads/clutch/2012/06/CinChallenge-GuyLG.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/ObJN1.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://4.bp.blogspot.com/-6ocKfcpNm3U/UVnqv4Fr2iI/AAAAAAAALLY/Iq6asnzRM6Y/s1600/scratch-post.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/dBbTo5S.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/SxsGK.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://cdn.gifstache.com/2012/7/10/gifstache.com_323_1341954201.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://gifs.gifbin.com/082009/1249287969_pat_on_the_back_prank.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/zY4nD.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/uun2L.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/vFnd2.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/p5s51.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://weknowmemes.com/wp-content/uploads/2011/12/cat-jumps-off-ledge.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://thechive.files.wordpress.com/2010/06/54zhb1.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://gifs.gifbin.com/082009/1251020499_own-goal-with-face.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://i.imgur.com/lBuP9.gif" target="_blank">&nbsp;</a></div>
      <div class="column"><a href="http://files.myopera.com/mpatricio/albums/7003662/funny-gif-yoga-balls.gif" target="_blank">&nbsp;</a></div>
    </div>
    <script type="text/javascript">
        (function (w, d) {
            var id = 'embedly-platform', n = 'script';
            if (!d.getElementById(id)) {
                w.embedly = w.embedly || function () { (w.embedly.q = w.embedly.q || []).push(arguments); };
                var e = d.createElement(n); e.id = id; e.async = 1;
                e.src = ('https:' === document.location.protocol ? 'https' : 'http') + '://cdn.embedly.com/widgets/platform.js';
                var s = d.getElementsByTagName(n)[0];
                s.parentNode.insertBefore(e, s);
            }
        })(window, document);

        $.embedly.defaults.key = '1d5c48f7edc34c54bdae4c37b681ea2b';

        $('.gifs a').embedly({
            display: function (obj) {
                if (obj.type === 'photo') {

                    var $this = $(this);

                    // Create the static image src with Embedly Display.
                    var src = $.embedly.display.display(obj.url, {
                        query: {
                            animate: false
                        }
                    });

                    // Add static gif placeholder to the parent
                    $this.html('<img class="gif-holder" src="' + src + '" />');

                    // Start preloading the actually gif.
                    $this.append('<img class="gif-preload" src="' + obj.url + '" />');

                    // Create a promise so we can keep track of state.
                    $this.data('promise', $.Deferred());

                    // Get the element we added.
                    var elem = $this.find('.gif-preload').get(0);

                    // If the image is not in cache then onload will fire when it is.
                    elem.onload = function () {
                        $this.data('promise').resolve();
                    };

                    // If the image is already in the browsers cache call the handler.
                    if (elem.complete) {
                        $this.data('promise').resolve();
                    }
                    // Set the static gif url so we can use it later.
                    $(this).data('static_url', src);
                } else {
                    // remove li if it's not an image.
                    $(this).parent().remove();
                }
            }
        }).on('mouseenter', function () {
            var $this = $(this);

            // Set the hover state to true so that the load function knows to run.
            $this.data('hover', true);

            // Create a function to load the gif into the image.
            var load = function () {
                if ($this.data('hover') === true) {
                    // Remove the loading image if there is one
                    $this.find('.gif-loading').remove();

                    // Swap out the static src for the actually gif.
                    $this.find('img.gif-holder').attr('src', $this.data('embedly').url);
                }
            };
            // Add the load function to the done callback. If it's already resolved
            // this will fire immediately.
            $this.data('promise').done(load);

            // Add a spinner if it's not going to play right away.
            if ($this.data('promise').state() === 'pending') {
                // Add a loading spinner.
                $this.append('<i class="gif-loading fa fa-spinner fa fa-spin"></i>');

                // we need to center it over the image.
                $this.find('.gif-loading').css({
                    top: $this.height() / 2 - 20,
                    left: $this.width() / 2 - 20
                });
            }
        }).on('mouseleave', function () {
            var $this = $(this);

            // Make sure the load function knows we are no longer in a hover state.
            $this.data('hover', false);

            // Remove the spiner if it's there.
            $this.find('.gif-loading').remove();

            // Set the src to the static url.
            $this.find('img.gif-holder').attr('src', $(this).data('static_url'));
        });

    </script>
</asp:Content>
