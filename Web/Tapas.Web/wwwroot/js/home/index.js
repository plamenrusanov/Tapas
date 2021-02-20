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

var cart = JSON.parse(localStorage.getItem("cart"));
var elements = document.querySelectorAll("span.bad");
if (cart && elements) {
    for (var i = 0; i < elements.length; i++) {
        elements[i].innerHTML = cart.length;
    }
} else {
    for (var i = 0; i < elements.length; i++) {
        elements[i].innerHTML = 0;
    }
};


var lastScrollTop = 0;
var position = 0;
window.addEventListener("touchmove", function () {
    var st = window.pageYOffset || document.documentElement.scrollTop;
    st = parseInt(st);
    var n = document.querySelector("nav");
    if (st > lastScrollTop) {
        if (position > -56) {
            position += (lastScrollTop - st);
            position = position < -56 ? -56 : position;
            n.style.top = `${position}px`;
        }

    } else {
        if (position < 0) {
            position += (lastScrollTop - st);
            position = position > 0 ? 0 : position;
            n.style.top = `${position}px`;
        }
    }
    lastScrollTop = st <= 0 ? 0 : st;
}, false);