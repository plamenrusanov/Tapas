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
for (var i = 0; i < elements.length; i++) {
    elements[i].innerHTML = cart.length;
}

if (window.screen.width < 992 && window.location.href.match("GetProductsByCategory")) {
    $('html, body').animate({
        scrollTop: $('#products').offset().top - 55
    }, 500);
}
