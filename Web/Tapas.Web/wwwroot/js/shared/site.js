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
var up = null;
['scroll', 'touchmove', 'mousewheel'].forEach(evn =>
    window.addEventListener(evn, function () {
        if (window.innerWidth < 992) {
            var st = window.pageYOffset || document.documentElement.scrollTop;
            st = parseInt(st);
            var n = document.querySelector("nav.navbar");
            if (st > lastScrollTop) { // нагоре

                if (!up) {
                    up = true;
                    var offset = n.offsetTop === 0 ? st : n.offsetTop;
                    n.style.position = "absolute";
                    var top = st > offset ? offset : st;
                    n.style.top = `${top}px`;
                }
                lastScrollTop = st <= 0 ? 0 : st;
                position = lastScrollTop;
            } else if (st < lastScrollTop) { // надолу    
                if (st < position && up) {
                    up = false;
                    n.style.position = "absolute";
                    var top = (st - 56) < n.offsetTop ? n.offsetTop : st - 56;
                    n.style.top = `${top}px`;
                } else if (st + 56 < position) {
                    n.style.position = "fixed";
                    n.style.top = "0px";
                }
                lastScrollTop = st <= 0 ? 0 : st;
            }
        }

    }, false)
);

if (window.screen.width < 992 && window.location.href.match("GetProductsByCategory")) {
    $('html, body').animate({
        scrollTop: $('#products').offset().top
    }, 500);
}
