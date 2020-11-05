function setValues(co, na, pr) {
    var modal = document.getElementById("closeModal");
    modal.click();
    var code = document.getElementById(`MistralCode`);
    code.value = co;
    var price = document.getElementById(`Price`);
    price.value = pr;
    var price = document.getElementById(`Price2`);
    price.innerText = pr;
    var n = document.getElementById(`MistralName`);
    n.value = na;
    var n2 = document.getElementById(`MistralName2`);
    n2.innerText = na;
    var input = document.getElementById(`getMProduct`);
    input.value = na;
}

function displayProducts(result) {
    var el = document.getElementById(`btnDeliveryTax`);
    el.click();
    var list = document.getElementById("listProducts");
    list.innerHTML = "";
    var i;
    for (i = 0; i < result.length; i++) {
        var li = document.createElement("li");
        li.className = "btn";
        li.setAttribute("onclick", `setValues(${result[i].code}, "${result[i].name}", "${result[i].salesPrice.toFixed(2)}")`);
        li.style.width = "100%";
        li.style.marginBottom = "2px";
        var span1 = document.createElement("span");
        span1.textContent = `${result[i].code}. ${result[i].name}  ${result[i].salesPrice.toFixed(2)} лв.`;
        li.appendChild(span1);
        list.appendChild(li);
    }

}

function getProduct() {
    var input = document.getElementById(`getMProduct`).value;

    $.ajax({
        url: `/Administration/Mistral/GetMProduct?name=${input}`,
        success: function (response) {
            displayProducts(response);
        }
    });
}


