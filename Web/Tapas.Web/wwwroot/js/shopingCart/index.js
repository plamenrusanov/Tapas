
function setDescription() {
    var index = document.getElementById("sciId").value;
    var cart = JSON.parse(localStorage.getItem('cart'));
    if (cart === null || cart.length == 0 || index < 0 || index >= cart.length) {
        return;
    }
    var message = document.getElementById("message-text").value;
    if (message.length <= 150) {
        cart[index].Description = message;
        localStorage.setItem('cart', JSON.stringify(cart));
        var el = document.getElementById("closeModal");
        el.click();

    }
}


function getDescription(index) {
    index = parseInt(index);
    var cart = JSON.parse(localStorage.getItem('cart'));
    if (cart === null || cart.length == 0 || index < 0 || index >= cart.length) {
        return;
    }
    var el = document.getElementById(`button${index}`);
    el.click();
    var elem = document.getElementById("message-text");
    elem.value = cart[index].Description;
    var productName = document.getElementById(`name${index}`).innerText;
    document.getElementById("exampleModalLongTitle").innerText = productName;
    var sciId = document.getElementById("sciId");
    sciId.value = index;
}

function displayDeliveryTax() {
    var el = document.getElementById(`btnDeliveryTax`);
    el.click();
}

var area = document.getElementById("message-text");
var message = document.getElementById("message");
var maxLength = 150;
var checkLength = function () {
    if (area.value.length <= maxLength) {
        message.innerHTML = `${area.value.length}/150`;
    }
}
setInterval(checkLength, 10);
