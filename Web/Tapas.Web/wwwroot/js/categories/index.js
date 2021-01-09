var count;
if (document.getElementsByTagName('tbody')) {
    var el = document.getElementById('tbody');
    count = el.childElementCount;
}

var models = [];
var el = document.getElementById('tbody');
for (var i = 0; i < el.childElementCount; i++) {
    var model = {
        'id': document.getElementById(`id${i}`).value,
        'name': document.getElementById(`n${i}`).innerText,
        'position': document.getElementById(`p${i}`).valueAsNumber,
    };
    models.push(model);
}

function up(i) {
    i = parseInt(i);
    if (i <= 0) {
        return;
    }
    $(`#tr${i}`).insertBefore(`#tr${i - 1}`);
    models.find(o => o.id === document.getElementById(`id${i}`).value).position -= 1;
    models.find(o => o.id === document.getElementById(`id${i - 1}`).value).position += 1; 
    document.getElementById(`tr${i}`).id = 'tr';
    document.getElementById(`p${i}`).id = 'p';
    document.getElementById(`plus${i}`).id = 'plus';
    document.getElementById(`minus${i}`).id = 'minus';
    document.getElementById(`id${i}`).id = 'id';

    // up
    document.getElementById(`tr${i - 1}`).id = `tr${i}`;
    document.getElementById(`p${i - 1}`).id = `p${i}`;
    var v = document.getElementById(`p${i}`).value;
    document.getElementById(`p${i}`).value = (parseInt(v) + 1);
    document.getElementById(`plus${i - 1}`).id = `plus${i}`;
    document.getElementById(`plus${i}`).setAttribute('onclick', `up(${i})`);
    document.getElementById(`minus${i - 1}`).id = `minus${i}`;
    document.getElementById(`minus${i}`).setAttribute('onclick', `down(${i})`);
    document.getElementById(`id${i - 1}`).id = `id${i}`;

    // down
    document.getElementById('tr').id = `tr${i - 1}`;
    document.getElementById('p').id = `p${i - 1}`;
    v = document.getElementById(`p${i - 1}`).value;
    document.getElementById(`p${i - 1}`).value = (parseInt(v) - 1);
    document.getElementById(`plus`).id = `plus${i - 1}`;
    document.getElementById(`plus${i - 1}`).setAttribute('onclick', `up(${i - 1})`);
    document.getElementById(`minus`).id = `minus${i - 1}`;
    document.getElementById(`minus${i - 1}`).setAttribute('onclick', `down(${i - 1})`);
    document.getElementById('id').id = `id${i - 1}`;
    if (i == 1) {
        document.getElementById(`plus${i}`).style.display = 'flex';
        document.getElementById(`plus${i - 1}`).style.display = 'none';
    }
    if (i == (count - 1)) {
        document.getElementById(`minus${i}`).style.display = 'none';
        document.getElementById(`minus${i - 1}`).style.display = 'flex';
    }
}

function down(i) {
    i = parseInt(i);
    if (i >= count - 1) {
        return;
    }
    $(`#tr${i}`).insertAfter(`#tr${i + 1}`);
    models.find(o => o.id === document.getElementById(`id${i}`).value).position += 1;
    models.find(o => o.id === document.getElementById(`id${i + 1}`).value).position -= 1; 
    document.getElementById(`tr${i}`).id = 'tr';
    document.getElementById(`p${i}`).id = 'p';
    document.getElementById(`plus${i}`).id = 'plus';
    document.getElementById(`minus${i}`).id = 'minus';
    document.getElementById(`id${i}`).id = 'id';
    // up
    document.getElementById(`tr${i + 1}`).id = `tr${i}`;
    document.getElementById(`p${i + 1}`).id = `p${i}`;
    var v = document.getElementById(`p${i}`).value;
    document.getElementById(`p${i}`).value = (parseInt(v) - 1);
    document.getElementById(`plus${i + 1}`).id = `plus${i}`;
    document.getElementById(`plus${i}`).setAttribute('onclick', `up(${i})`);
    document.getElementById(`minus${i + 1}`).id = `minus${i}`;
    document.getElementById(`minus${i}`).setAttribute('onclick', `down(${i})`);
    document.getElementById(`id${i + 1}`).id = `id${i}`;
    // down
    document.getElementById('tr').id = `tr${i + 1}`;
    document.getElementById('p').id = `p${i + 1}`;
    v = document.getElementById(`p${i + 1}`).value;
    document.getElementById(`p${i + 1}`).value = (parseInt(v) + 1);
    document.getElementById(`plus`).id = `plus${i + 1}`;
    document.getElementById(`plus${i + 1}`).setAttribute('onclick', `up(${i + 1})`);
    document.getElementById(`minus`).id = `minus${i + 1}`;
    document.getElementById(`minus${i + 1}`).setAttribute('onclick', `down(${i + 1})`);
    document.getElementById('id').id = `id${i + 1}`;
    if (i == 0) {
        document.getElementById(`plus${i}`).style.display = 'none';
        document.getElementById(`plus${i + 1}`).style.display = 'flex';
    }
    if (i == (count - 2)) {
        document.getElementById(`minus${i}`).style.display = 'flex';
        document.getElementById(`minus${i + 1}`).style.display = 'none';
    }
}

function setPosition() {
    var m = JSON.stringify(models);
    $.post(`/Administration/Categories/SavePositions`,
        {
            models: m
        }, function () {
            alert('Успешно запазихте подредбата!');
        }); 
}