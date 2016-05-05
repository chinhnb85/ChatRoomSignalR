(function () {
    function swapNodes(a, b) {
        var aparent = a.parentNode;
        var asibling = a.nextSibling === b ? a : a.nextSibling;
        b.parentNode.insertBefore(a, b);
        aparent.insertBefore(b, asibling);
    }

    function splitimage() {
        var $grid;
        $('#imageGrid1').chunkIt({
            maxContainerWidth: 800,
            cellsInRow: 6, // Number of cells in grid row.
            cellsInColumn: 6,
            shuffle: true, // Number of cells in grid column.
            scaleImage: true,
            onGetGridElem: function ($event, currentObj, $gridElem) {
                // Callback function will be executed on generating grid's DOM element.
                $gridElem.width($gridElem.width() + 50 //For borders adjustment
                );
                $grid = $gridElem.hide();
            },
            onAfterFinish: function ($event, chunkItObj) {
                // Initiating sortable plugin here
                $("#imageGrid1 li").draggable({
                    revert: true,
                    revertDuration: 0,
                    cursor: "move"
                }).droppable({
                    drop: function (event, ui) {
                        swapNodes($(this).get(0), $(ui.draggable).get(0));
                        $("#imageGrid1 li").each(function (index, elem) {

                            if ('cell_' + (index + 1) !== $(elem).attr('id')) {
                                return false;
                            } else if ($("#imageGrid1 li").length === index + 1) {
                                setTimeout(function () { alert("Hey! You win the game"); }, 0);

                            }
                            return false;
                        });
                    }
                });
                $grid.fadeIn();

            }
        });

    }

    $(function () {
        var setupGame = function () {
            splitimage();
            $('#click').append('Please drag and drop after loading game...');
        };

        // On click and replace.
        $("#click").one('click', setupGame);
    });

})();