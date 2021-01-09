function setValues(co, na, pr, index) {
    var modal = document.getElementById("closeModal");
    modal.click();
    var code = document.getElementById(`Sizes_${index}__MistralCode`);
    code.value = co;
    var price = document.getElementById(`Sizes_${index}__Price`);
    price.value = pr;
    var price2 = document.getElementById(`Price2${index}`);
    price2.innerText = pr;
    var n = document.getElementById(`Sizes_${index}__MistralName`);
    n.value = na;
    var input = document.getElementById(`getMProduct${index}`);
    input.value = na;
}

function displayProducts(result, index) {
    var el = document.getElementById(`btnDeliveryTax`);
    el.click();
    var list = document.getElementById("listProducts");
    list.innerHTML = "";
    var i;
    for (i = 0; i < result.length; i++) {
        var li = document.createElement("li");
        li.className = "btn";
        li.setAttribute("onclick", `setValues(${result[i].code}, "${result[i].name}", "${result[i].salesPrice.toFixed(2)}", ${index})`);
        li.style.width = "100%";
        li.style.marginBottom = "2px";
        var span1 = document.createElement("span");
        span1.textContent = `${result[i].code}. ${result[i].name}  ${result[i].salesPrice} лв.`;
        li.appendChild(span1);      
        list.appendChild(li);
    }

}

function getProduct(i) {
    var input = document.getElementById(`getMProduct${i}`).value;
//
// $.ajax({
//     url: `/Administration/Mistral/GetMProduct?name=${input}`,
//     success: function (response) {
//         displayProducts(response, i);
//     }
// });
}


