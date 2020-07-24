var connection = null;

$(document).ready(() => setupConnection());


function setupConnection(){
    connection = new signalR.HubConnectionBuilder().withUrl("/userOrdersHub").build();

    connection.on("UserAlertMessage", function (message) { alert(message); });

    connection.on("UserStatusChanged", function (order, status) {       
            var li = document.getElementById(`li${order}`);
            if (status === "Unprocessed") {
                li.className = "btn btn-danger btn-lg";
            } else if (status === "Processed") {
                li.className = "btn btn-warning btn-lg";
            } else if (status === "OnDelivery") {
                li.className = "btn btn-success btn-lg";
            } else if (status === "Delivered") {
                li.className = "btn btn-info btn-lg";
            }
            li.click();
            playMusic();       
    });

    connection.on("SuccessfullySetRating", function () {
        if (document.getElementById("OrderId")) {
            var orderId = document.getElementById("OrderId").value;
            orderDetails(orderId);
        }
       
    });

    connection.on("UserFinished", function () { connection.stop(); });

    connection.start().catch(err => console.error(err.toString()));
};

function playMusic() {
    var audio = document.getElementById("audio");
    audio.play();
}

function orderDetails(orderId) {
    $.ajax({
        url: `/Orders/UserOrderDetails?orderId=${orderId}`,
        cache: false,
        success: function (response) {
            document.getElementById("detailSection").innerHTML = response;
        }
    });
}

function displayDeliveryTax() {
    var el = document.getElementById(`btnDeliveryTax`);
    el.click();
}

function sendRating(rating, message) {
    connection.invoke("UserSetRating", rating, message);
};


function getRating() {
    var dataRating = [];
    var message;
    var holder = document.getElementById("ratingHolder");
    for (var i = 0; i < holder.childElementCount; i++) {
        var productItemId = holder.children[i].firstElementChild.innerHTML;
        if (document.querySelector(`input[name="rating-input-${productItemId}"]:checked`)) {
            var rating = document.querySelector(`input[name="rating-input-${productItemId}"]:checked`).value;
            dataRating.push({ itemId: productItemId, rating: rating })
        } else {
            if (document.getElementById(`rating${productItemId}`)) {
                var r = document.getElementById(`rating${productItemId}`);
                r.style.backgroundColor = "red";
                r.style.borderRadius = "0.25rem";
                return;
            }
        }
    }
    if (document.getElementById("clientComment")) {
        message = document.getElementById("clientComment").value;
    }
    sendRating(dataRating, message);
}
