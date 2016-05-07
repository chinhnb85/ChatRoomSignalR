$(function () {
    var $container = $('#list');
    var columns = 3;
    var containerWidth = $container.width()-50;
    $(window).resize(function () {
        columns = $(window).width() > 640 ? 3 : $(window).width() > 320 ? 2 : 1;
        containerWidth = $('#list').width()-50;
    });

    $container.imagesLoaded(function () {
        $container.masonry({
            itemSelector: '.box',
            columnWidth: 100,
            isAnimated: true
        });
    });

    var pageNo = 1, pageSize = 25;
    function loadMore() {
        $('#imgLoad').show();
        $.ajax({
            type: "POST",
            url: "PinterrestByMasonry.aspx/GetData",
            data: JSON.stringify({ pageNo: pageNo + 1, pageSize: pageSize }),
            dataType: "json",
            contentType: "application/json",
            complete: function (response) {
                $('#imgLoad').hide();
            },
            success: function (response) {
                if (response.d.length > 0) {
                    var ctrls = [];
                    for (var i = 0; i < response.d.length; i++) {
                        ctrls.push('<div class="box"> <img src="' + response.d[i].Url + '" /><p>' + response.d[i].Description + '</p></div>');
                    }
                    var $newElems = $(ctrls.join(''));
                    $container.append($newElems);
                    $newElems.css({ opacity: 0 });
                    $newElems.imagesLoaded(function () {
                        // show elems now they're ready
                        $newElems.css({ opacity: 1 });
                        $container.masonry('appended', $newElems, true);
                    });
                    pageNo++;
                }
            }
        });
    }

    $(window).scroll(function () {
        if ($(window).scrollTop() === $(document).height() - $(window).height() && !($('#imgLoad').is(':visible'))) {
            loadMore();
        }
    });
});