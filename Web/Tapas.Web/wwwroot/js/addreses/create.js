function Send(x, y) {
    $.ajax({
        url: `/Addresses/GetAddressFromLocation?latitude=${x}&longitude=${y}`,
        dataType: "json",
        success: function (response) {
            if (response.city !== "undefined") {
                document.getElementById("city").value = response.city;
            }
            if (response.borough !== "undefined") {
                document.getElementById("borough").value = response.borough;
            }
            if (response.street !== "undefined") {
                document.getElementById("street").value = response.street;
            }
            if (response.streetNumber !== "undefined") {
                document.getElementById("streetNumber").value = response.streetNumber;
            }
            if (response.block !== "undefined") {
                document.getElementById("block").value = response.block;
            }
            if (response.entry !== "undefined") {
                document.getElementById("entry").value = response.entry;
            }
            if (response.latitude !== "undefined") {
                document.getElementById("latitude").value = response.latitude;
            }
            if (response.longitude !== "undefined") {
                document.getElementById("longitude").value = response.longitude;
            }
            if (response.displayName !== "undefined") {
                document.getElementById("displayName").value = response.displayName;
            }
        }
    });
}

function getAddressFromLocation () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            Send(position.coords.latitude, position.coords.longitude);
        });
    }
}