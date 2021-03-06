var FX_URL = "https://fxappdev714ac8ff.azurewebsites.net/";

$(function(){
    var modelVersion = "";

    $.get( FX_URL + "api/ping", function( data ) 
    {
        if(data !== "")
        {
            var tokens = data.replace(/"/g, "").split("/");
            modelVersion = tokens[tokens.length -1].replace(".zip","");
            console.log("start fx", modelVersion);
            $("#lblModelVersion").text(modelVersion);    
        }
    });
    

    $("#btnSubmit").click(function(event){
        var response = $("#divResponse");
        var text = $("#txtSentiment").val();
        if(text.length==0) {
            
            return;
        }
        $.post( FX_URL + "api/predict", JSON.stringify({ sentimentText: text }), function( data ) {
            console.log("start fx", data);
            var formatted = data.prediction ? "Positive" : "Negative";
            var confidence = data.prediction ? data.probability : 1 - data.probability;
            formatted += ", I'm " +  parseFloat(confidence * 100).toFixed(2)+"% confident"            
            response.text(formatted);
        }); 
    });
});