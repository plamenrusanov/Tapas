
function DisplaySizeName(sizeId) {
    var sizeName = document.getElementById(sizeId).innerHTML;
    document.getElementById("sizeName").innerHTML = sizeName;
}

function GetModel(sizeId) {
    DisplaySizeName(sizeId);
    $.ajax({
        url: `/Administration/Sizes/GetSizeModel?sizeId=${sizeId}`,
        success: function (response) {
            var e = document.getElementById("chooseSize");
            e.style.display = "none";
            var d = document.getElementById("detailSection");
            d.innerHTML = response;
            d.className = "col-md-6 p-0";
            var c = document.getElementById("desc");
            c.style.display = "block";
            var b = document.getElementById("submitSection");
            b.style.display = "flex";
            if (document.getElementById("sectionExtras")) {
                var a = document.getElementById("sectionExtras");
                a.style.display = "block";
            }
          
        }
    });
}

function getPrice(str) {
    var index = str.indexOf(" ");
    str = str.substring(0, index);
    str = str.replace(",", ".");
    var number = parseFloat(str);
    return number;
}

function updateSubTotal(subTotal) {
    subTotal = subTotal.toFixed(2);
    subTotal = subTotal.replace(".", ",");
    document.getElementById("subTotal").innerHTML = `${subTotal} лв.`;
}

function collectExtrasPrice() {
    var total = 0;
    var extras = document.getElementsByClassName("qtyExtra");
    for (var i = 0; i < extras.length; i++) {
        var qty = parseInt(extras[i].value, 10)
        if (qty > 0) {
            var id = extras[i].id.substring(8, extras[i].id.length);
            var price = document.getElementById(`price${id}`).innerText;
            price = getPrice(price);
            total += price * qty;
        }
    }

    return total;
}

function updateQuantity(qty) {
   
    var productPrice = document.getElementById("productPrice").innerText;
    productPrice = getPrice(productPrice);
    var packagePrice = document.getElementById("packagePrice").innerText;
    packagePrice = getPrice(packagePrice);
    var maxProd = document.getElementById("maxProducts").innerText;
    maxProd = parseInt(maxProd);
    var subTotal = (productPrice * qty) + (Math.ceil(qty / maxProd) * packagePrice);
    var pr = collectExtrasPrice();
    return subTotal + pr;
}


function minus() {
    var input = document.getElementById('theInput');
    if (input.value > 1) {
        input.value = parseInt(input.value, 10) - 1;
        var pr = updateQuantity(input.value);
        updateSubTotal(pr);
    }
}

function plus() {
    var input = document.getElementById('theInput');
    if (input.value < 10) {
        input.value = parseInt(input.value, 10) + 1;
        var pr = updateQuantity(input.value);
        updateSubTotal(pr);
    }
}

function down(id) {
    var qtyEl = document.getElementById(`quantity${id}`);
    if (qtyEl.value > 0) {
        qtyEl.value = parseInt(qtyEl.value, 10) - 1;
        var input = document.getElementById('theInput');
        var pr = updateQuantity(input.value);
        updateSubTotal(pr);
    }
}
function up(id) {
    var qtyEl = document.getElementById(`quantity${id}`);
    if (qtyEl.value < 5) {
        qtyEl.value = parseInt(qtyEl.value, 10) + 1;
        var input = document.getElementById('theInput');
        var pr = updateQuantity(input.value);
        updateSubTotal(pr);
    }
}


function showExtras() {
    var content = document.getElementById("content");
    if (content.style.display === "block") {
        content.style.display = "none";
    } else {
        content.style.display = "block";
    }
}

//window.onhashchange = function () {
//    var referer = window.document.referrer;
//    var winloc = window.location.href;
//    if (winloc.indexOf('#') < 0) {
//        location.href = winloc.substring(0, winloc - 2);
//        var d = document.getElementById("detailSection");
//        d.style.display = "none";
//        var e = document.getElementById("chooseSize");
//        e.style.display = "block";
//    }
//}

