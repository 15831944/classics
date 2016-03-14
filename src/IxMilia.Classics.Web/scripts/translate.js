(function () {
    function getTranslation() {
        $("#results").html("");
        var latin = $("#latin-text").val();
        $.ajax({
            url: "/LatinService.svc/Translate",
            data: { latin: latin },
            cache: false,
            dataType: "json",
            success: function (data, status, response) {
                $("#results").html(JSON.stringify(data.d));
            },
            error: function (request, status, err) {
                $("#results").html("error: " + err);
            }
        });
    }

    $(document).ready(function () {
        $("#latin-text").keydown(function (event) {
            if (event.keyCode == 13) {
                getTranslation();
            }
        });
        $("#translate-button").click(function () {
            getTranslation();
        });
        $("#latin-text").focus();
    });
})();
