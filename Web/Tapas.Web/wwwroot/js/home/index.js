var dropdown = document.getElementsByClassName("dropdown-btn");
var i;

for (i = 0; i < dropdown.length; i++) {
    dropdown[i].addEventListener("click", function () {
        this.classList.toggle("active");
        var dropdownContent = document.getElementsByClassName("dropdown-container");
        if (dropdownContent[0].style.display === "block") {
            dropdownContent[0].style.display = "none";
        } else {
            dropdownContent[0].style.display = "block";
        }
    });
}

//$(window).on('storage', message_receive);
//function message_receive(ev) {
//    if (ev.originalEvent.key != 'cart') return;
//    var cart = JSON.parse(localStorage.getItem(ev.originalEvent.key));
//    var elements = document.querySelectorAll("span.bad");
//    for (var i = 0; i < elements.length; i++) {
//        elements[i].innerHTML = cart.length;
//    }
//}

var cart = JSON.parse(localStorage.getItem("cart"));
var elements = document.querySelectorAll("span.bad");
for (var i = 0; i < elements.length; i++) {
    elements[i].innerHTML = cart.length;
}