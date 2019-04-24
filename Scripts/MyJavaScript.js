function AddLoadingGif() {
    $body = $("body");

    $(document).on({
        ajaxStart: function () { $("body").addClass("loading"); },
        ajaxStop: function () { $("body").removeClass("loading"); }
    });
}

function showPageLoadingGif() {
    if (ns4) { ld.visibility = "hidden"; }
    else if (ns6 || ie4) ld.display = "none";
}