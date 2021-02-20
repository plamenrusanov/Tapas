
var dr = document.getElementById("mobileDropdown");
function showHideDropdown() {
    if (dr.style.display == "none" || dr.style.display == "") {
        dr.style.display = "block";
    } else {
        dr.style.display = "none";
    }
};

//var l = document.getElementById("list");
//l.addEventListener("blur", function (e) {
//    document.addEventListener("click", function (event) {
//        var target = event.target || event.srcElement,
//        text = target.textContent || target.innerText;
//        if (text != "li.layoutUlLiDrop") {
//            dr.style.display = "none";
//        }
//        alert("events");
//    });

//});

function bl(e) {
    document.addEventListener("click", function (event) {
        var target = event.target || event.srcElement,
            text = target.className;
        if (text != "dropLinks" && text != "list") {
            dr.style.display = "none";
        }
    });
}