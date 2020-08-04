function orderDetails(orderId) {
    $.ajax({
        url: `/Orders/Details?orderId=${orderId}`,
        cache: false,
        success: function (response) {
            document.getElementById("detailSection").innerHTML = response;
        }
    });
}

function changeStatus(status) {

    if (document.getElementById(`order`)) {
        var orderId = document.getElementById(`order`).innerText;
        if (status === "Processed") {
            var time = document.getElementById(`theInput`).value;
            if (true) {
                var deliveryFee = document.getElementById(`DeliveryTaxId`).value;
            }
        }
        $.ajax({
            url: `/Orders/ChangeStatus?orderId=${orderId}&status=${status}&setTime=${time}&deliveryFee=${deliveryFee}`,
            success: function (response) {
                var li = document.getElementById(`li${orderId}`);
                switch (status) {
                    case "Processed": li.className = "btn btn-warning"; break;
                    case "OnDelivery": li.className = "btn btn-success"; break;
                    case "Delivered": li.className = "btn btn-info"; break;
                    default: break;
                }
                li.click();
            }
        });
    }
   
}

function minus() {
    var input = document.getElementById('theInput');
    if (input.value > 15) {
        input.value = parseInt(input.value, 10) - 5;
    }
}

function plus() {
    var input = document.getElementById('theInput');
    if (input.value < 100) {
        input.value = parseInt(input.value, 10) + 5;
    }
}