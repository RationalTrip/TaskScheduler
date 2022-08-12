function Copy() {
    var dummy = document.createElement('input'),
        text = window.location.href;
    document.body.appendChild(dummy);
    dummy.value = text;
    dummy.select();
    document.execCommand('copy');
    document.body.removeChild(dummy);

    $("#notification").fadeIn(200);

    $("#notification").delay(800).fadeOut(500);
}

$('#copy-link').on('click', Copy);