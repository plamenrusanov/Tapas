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