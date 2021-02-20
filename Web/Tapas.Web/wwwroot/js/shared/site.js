window.onscroll = function () { scrollFunction() };

function scrollFunction() {
    if (document.getElementById("topBtn")) {
        var mybutton = document.getElementById("topBtn");
        if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
            mybutton.style.display = "block";
        } else {
            mybutton.style.display = "none";
        }
    }
}

function topFunction() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0; 
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
}

var lastScrollTop = 0;
var position = 0;
window.addEventListener("touchmove", function () {
    var st = window.pageYOffset || document.documentElement.scrollTop;
    st = parseInt(st);
    var n = document.querySelector("nav.navbar");
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

if (window.screen.width < 992 && window.location.href.match("GetProductsByCategory")) {
    $('html, body').animate({
        scrollTop: $('#products').offset().top - 50
    }, 500);
}
