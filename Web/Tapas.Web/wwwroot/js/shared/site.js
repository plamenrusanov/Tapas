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
    if (st > (lastScrollTop + 5) && st > 50) {
        var id = null;
        clearInterval(id);
        id = this.setInterval(frameHide, 5); 
        function frameHide() {
            position--;
            position = position < -56 ? -56 : position;
            n.style.top = `${position}px`
            if (position <= -56) {
                clearInterval(id);
            }
        }
        lastScrollTop = st <= 0 ? 0 : st; 
    } else if (st < (lastScrollTop - 5)) {
        var id = null;
        clearInterval(id);
        id = this.setInterval(frameHide, 5);
        function frameHide() {
            position++;
            position = position > 0 ? 0 : position;
            n.style.top = `${position}px`
            if (position >= 0) {
                clearInterval(id);
            }
        }
        lastScrollTop = st <= 0 ? 0 : st; 
    }
}, false);

//function myMove() {

//    var elem = document.getElementById("animate");
//    var pos = 0;
//    clearInterval(id);
//    id = setInterval(frame, 5);
//    function frame() {
//        if (pos == 350) {
//            clearInterval(id);
//        } else {
//            pos++;
//            elem.style.top = pos + "px";
//            elem.style.left = pos + "px";
//        }
//    }
//}
if (window.screen.width < 992 && window.location.href.match("GetProductsByCategory")) {
    $('html, body').animate({
        scrollTop: $('#products').offset().top - 50
    }, 500);
}
