function isEmptyStr(str) {
    return str === null || str.match(/^ *$/) !== null;
}

$.validator.addMethod('taskEndTest', function (value, element) {
    var startStr = $("#TaskStart").val();
    var endStr = $("#TaskEnd").val();

    if (isEmptyStr(startStr) || isEmptyStr(endStr))
        return false;

    var dateStart = new Date(startStr);
    var dateEnd = new Date(endStr);

    return dateEnd >= dateStart;
}, 'Task Start should be before Task End!');

$.validator.addMethod('taskRepetitiveEndTest', function (value, element) {
    if ($("#IsRepetitive").prop("checked") == false)
        return false;

    var startStr = $("#TaskStart").val();
    var endStr = $("#RepetitiveEnd").val();

    if (isEmptyStr(startStr) || isEmptyStr(endStr))
        return false;

    var dateStart = new Date(startStr);
    var dateEnd = new Date(endStr);

    return dateEnd >= dateStart;
}, 'Task Start should be before Repetitive End!');

$.validator.addMethod('taskRepetitiveEndRequired', function (value, element) {
    if ($("#IsRepetitive").prop("checked") == false)
        return false;

    var endStr = $("#RepetitiveEnd").val();

    return !isEmptyStr(endStr);
}, 'Repetitive End is required!');

document.addEventListener("DOMContentLoaded", function () {
    $("input[id*=RepetitiveEnd]").rules("remove", "required");

    $("input[id*=TaskEnd]").rules("add", "taskEndTest");

    $("input[id*=RepetitiveEnd]").rules("add", "taskRepetitiveEndRequired");
    $("input[id*=RepetitiveEnd]").rules("add", "taskRepetitiveEndTest");
});

