function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}

function OnDataChangeEvent() {
    var parent = this.parentNode;
    var popoverDiv = parent.parentNode;

    if (isEmptyOrSpaces(this.innerHTML)) {
        parent.classList.add("d-none");
        parent.setAttribute("data-bs-content", "");

        popoverDiv.classList.remove("input-group");
    } else {
        parent.classList.remove("d-none");
        parent.setAttribute("data-bs-content", this.innerHTML);

        popoverDiv.classList.add("input-group");
    }
}

$('.span-validator').each(function () {
    $(this).on('DOMSubtreeModified', OnDataChangeEvent);
    $(this).trigger("DOMSubtreeModified");
});