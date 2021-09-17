$("#addItem").click(function () {
    $.ajax({
        url: this.href,
        cache: false,
        success: function (html) { $("#seedTable tr").append(html); }
    });
    return false;
});

$("a.deleteRow").live("click", function () {
    $(this).parents("table.editorRow tr:last").remove();
    return false;
});

$('#addSeed').click(function ()
{
    $('#seedTable tr').load('/Admin/BlankRow/');
});

$(function () {
    // don't cache ajax or content won't be fresh
    $.ajaxSetup({
        cache: false
    });
    var ajax_load = "<img src='http://i.imgur.com/pKopwXp.gif' alt='loading...' />";

    // load() functions
    var loadUrl = "http://fiddle.jshell.net/dvb0wpLs/show/";
    $("#loadbasic").click(function () {
        $("#result").html(ajax_load).load(loadUrl);
    });

    // end  
});