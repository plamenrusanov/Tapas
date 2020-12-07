var table = `<div class="bg-light">
        <table class="table">
            <thead>
                <tr class="row">
                    <th class="col-md-1">
                        №
                    </th>
                    <th class="col-md-3">
                        Продукт
                    </th>
                    <th class="col-md-1">
                        Кол
                    </th>
                    <th class="col-md-2">
                        Добавки
                    </th>
                    <th class="col-md-1">
                        Цена
                    </th>
                    <th class="col-md-2">
                        Изискване
                    </th>
                    <th class="col-md-2">
                        Изтрий
                    </th>
                </tr>
            </thead>
            <tbody id="tbody">
            </tbody>
        </table>
    </div>    <div class="row">
        <h4 style="width:70%;">Общо за поръчката:</h4> <h4 id="grandTotal"></h4>
    </div>`

function getCart() {
    return JSON.parse(window.localStorage.getItem('cart'));
}

function getExtras(exArr) {
    var exHtml = "";
    for (var i = 0; i < exArr.length; i++) {
        exHtml +=`<tda class="row ml-0">
            <div>${exArr[i].qty} бр. ${exArr[i].name}</div>
        </tda>`;
    }
    return exHtml;
}

function display() {
    var holder = document.getElementById('cartHolder');
    var cart = getCart();
    if (cart === null || cart.length == 0) {
        holder.innerHTML = '<h4 class="text-center mt-5 mb-5">Количката е празна.</h4>';
        var p = document.getElementById('prepareOrder');
        p.style.display = 'none';
        return;
    }
    holder.innerHTML = table.trim();
    var tbody = document.getElementById('tbody');
    var grandTotal = 0.0;
    for (var i = 1; i <= cart.length; i++) {
        var item = cart[i - 1];
        var extras = getExtras(item.Extras);
        grandTotal += parseFloat(item.SubTotal.replace(',', '.'));
        var scItem =
        `<td class="col-md-1" width="45">
            <tda>${i}</tda>
        </td>
        <td class="col-md-3" id="name${i -1}">
            <h5><tda>${item.PName}</tda></h5>
                <tda>${item.SName}</tda>
        </td>
        <td class="col-md-1">
            <tda>${item.Qty}</tda>
        </td>
        <td class="col-md-2">
          ${extras}
        </td>
        <td class="col-md-1">
            <tda>${parseFloat(item.SubTotal.replace(',', '.')).toFixed(2)}</tda>
        </td>
        <td class="col-md-2">
            <tda>
                <a class="btn btn-primary btn-sm" onclick="getDescription(${i - 1})" style="color:white">Изискване</a>
                <button hidden data-toggle="modal" data-target="#exampleModalCenter" id="button${i - 1}"></button>
            </tda>
        </td>
        <td class="col-md-2">
            <tda><a class="btn btn-danger btn-sm" onclick="deleteItem(${i - 1})">Изтрий</a></tda>
        </td>`;
        var tr = document.createElement('tr');
        tr.className = 'row';
        tr.id = `tr${i - 1}`;
        tr.innerHTML = scItem.trim();
        tbody.appendChild(tr);

    }

    var gT = document.getElementById('grandTotal');
    gT.innerText = grandTotal.toFixed(2) + ' лв.';
    if (grandTotal < 15 && grandTotal >= 10) {
        var d = document.getElementById('delivery');
        d.style.display = 'block';
    } else if (grandTotal < 10) {
        var p = document.getElementById('prepareOrder');
        p.style.display = 'none';
    }
}

display();

function deleteItem(index) {
    var cart = getCart();
    cart.splice(index, 1);
    localStorage.setItem('cart', JSON.stringify(cart));
    display();
}

function getAccountInfo() {
    var d = document.getElementById('accountInfo');
    d.style.display = "flex";
    if (localStorage.getItem('username')) {
        var u = localStorage.getItem('username');
        if (document.getElementById('username')) {
            var el = document.getElementById('username');
            el.value = u;
        }
    }
    if (localStorage.getItem('phone')) {
        var p = localStorage.getItem('phone');
        if (document.getElementById('phone')) {
            var el = document.getElementById('phone');
            el.value = p;
        }
    }
    if (localStorage.getItem('address')) {
        var a = localStorage.getItem('address');
        a = JSON.parse(a);
        displayAddress(a);
    } else {
        getAddressFromLocation();
    }
}

function fChange(elm) {
    var adr = document.getElementById("addressDiv");
    if ($(elm).is(':checked')) {
        if (adr) {
            adr.style.display = "none";
        }
    } else {
        if (adr) {
            adr.style.display = "block";
        }
    }
}

function sendOrder() {
    if (document.getElementById('username')) {
        var el = document.getElementById('username');
        if (checkUsername(el.value)) {
            localStorage.setItem('username', el.value);
            document.getElementById('Username').value = el.value;
        } else {
            alert('Името трябва да дълго между 3 и 20 символа!');
            return;
        }
        
    }
    if (document.getElementById('phone')) {
        var el = document.getElementById('phone');
        if (checkPhoneNumber(el.value)) {
            localStorage.setItem('phone', el.value);
            document.getElementById('Phone').value = el.value;
        } else {
            alert('Телефония номер е задължителен!');
            return;
        }
       
    }
    if (document.getElementById('Cutlery')) {
        var el = document.getElementById('Cutlery');
        document.getElementById('CutleryCount').value = el.value;
    }
    if (document.getElementById('addInfoOrder')) {
        var el = document.getElementById('addInfoOrder');
        document.getElementById('AddInfoOrder').value = el.value;
    }
    var address = collectAddress();
    localStorage.setItem('address', address);
    document.getElementById('Address').value = address;

    var cart = localStorage.getItem('cart');
    document.getElementById('Cart').value = cart;

    if (document.getElementById('TakeAway')) {
        var el = document.getElementById('TakeAway');
        document.getElementById('Takeaway').value = el.checked;
    }

    var btn = document.getElementById('form-submit');
    btn.click();
    
}

function collectAddress() {
    var a = {
        'latitude': document.getElementById('latitude').value,
        'longitude': document.getElementById('longitude').value,
        'displayName': document.getElementById('displayName').value,
        'city': document.getElementById('city').value,
        'borough': document.getElementById('borough').value,
        'street': document.getElementById('street').value,
        'streetNumber': document.getElementById('streetNumber').value,
        'block': document.getElementById('block').value,
        'entry': document.getElementById('entry').value,
        'floor': document.getElementById('floor').value,
        'addInfo': document.getElementById('addInfoAddress').value,
    }
    return JSON.stringify(a);
}

function displayAddress(a) {
    if (a.city !== "undefined") {
        document.getElementById("city").value = a.city;
    }
    if (a.borough !== "undefined") {
        document.getElementById("borough").value = a.borough;
    }
    if (a.street !== "undefined") {
        document.getElementById("street").value = a.street;
    }
    if (a.streetNumber !== "undefined") {
        document.getElementById("streetNumber").value = a.streetNumber;
    }
    if (a.block !== "undefined") {
        document.getElementById("block").value = a.block;
    }
    if (a.entry !== "undefined") {
        document.getElementById("entry").value = a.entry;
    }
    if (a.floor !== 'undefined') {
        document.getElementById('floor').value = a.floor;
    }
    if (a.addInfoAddress !== 'undefined') {
        document.getElementById('addInfoAddress').value = a.addInfo;
    }
    if (a.latitude !== "undefined") {
        document.getElementById("latitude").value = a.latitude;
    }
    if (a.longitude !== "undefined") {
        document.getElementById("longitude").value = a.longitude;
    }
    if (a.displayName !== "undefined") {
        document.getElementById("displayName").value = a.displayName;
    }
}

function checkUsername(u) {
    if (u.length < 3 || u.length > 20) {
        return false
    }
    return true;
}

function checkPhoneNumber(number) {
    if (number.length >= 10 && number.match(/\d/g)) {
        return true;
    }
    return false;
}