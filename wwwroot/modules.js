var FX_URL = "https://fxappdev6f822e30.azurewebsites.net/";

$(function(){
    console.log("start js...");

    $.get( FX_URL + "api/ping", function( data ) {
        console.log("start fx", data);
    });

});