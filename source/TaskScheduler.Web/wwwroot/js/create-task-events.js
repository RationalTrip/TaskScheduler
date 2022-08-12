function isEmptyDate(str) {
    return str === null || str.match(/^ *$/) !== null;
}


function StringValueToDateTime(dateStr) {
    //datetime pattern "dd/MM/yyyy HH:mm"
    return new Date(dateStr);
}

function DateTimeToStringValue(date) {
    return date.toISOString().substring(0, 16);
}

function DateToStringValue(date) {
    return date.toISOString().substring(0, 10);
}

$("#TaskStart").on("change", function () {
    try {
        var stringVal = $(this).val();

        var dateTime = StringValueToDateTime(stringVal);

        var dateString = DateToStringValue(dateTime);

        $("#RepetitiveStart").val(dateString);
    } catch (error){
        console.log(error);
    }
});