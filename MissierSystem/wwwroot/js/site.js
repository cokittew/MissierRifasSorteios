
/*User Platform Methods*/

function hideShowPlatformSidebarMenu() {
    var sideBar = document.getElementById("sidebar");
    var container = document.getElementById("my-container");

    container.classList.toggle("active-cont");
    sideBar.classList.toggle("active-nav");
    
}

function mask(o, f) {
    setTimeout(function () {
        var v = mphone(o.value);
        if (v != o.value) {
            o.value = v;
        }
    }, 1);
}

function mphone(v) {
    var r = v.replace(/\D/g, "");
    r = r.replace(/^0/, "");
    if (r.length > 10) {
        r = r.replace(/^(\d\d)(\d{5})(\d{4}).*/, "($1) $2-$3");
    } else if (r.length > 5) {
        r = r.replace(/^(\d\d)(\d{4})(\d{0,4}).*/, "($1) $2-$3");
    } else if (r.length > 2) {
        r = r.replace(/^(\d\d)(\d{0,5})/, "($1) $2");
    } else if (r.length > 0) {
        r = r.replace(/^(\d*)/, "($1");
    }
    return r;
}


$('#FullName').keypress(function (e) {
    var keyCode = (e.keyCode ? e.keyCode : e.which); // Variar a chamada do keyCode de acordo com o ambiente.
    //alert(keyCode);
    if ((keyCode > 122 || keyCode < 97) && (keyCode > 90 || keyCode < 65) && (keyCode > 90 || keyCode < 65) && (keyCode > 251 || keyCode < 192)  && (keyCode != 32)) {
        e.preventDefault();
    } else {
        

    }
});

