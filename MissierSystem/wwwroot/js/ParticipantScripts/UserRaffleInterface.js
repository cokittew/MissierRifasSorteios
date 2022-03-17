var numbersSelectedToFutureBuyList = [];
var numbersSelectedToFutureBuyListAmorin = [];
var numbersSelectedToFutureView = "";


var numbersSelectedReceipt = [];
var totalReceiptNumberSelected = 0;

RafflePreSaveSelectNumber = (numberSelected, max) => {
    console.log(numberSelected)
    if (max > 0) {
        var participantUserSelectNumber = document.getElementById('participantUserSelectNumber');
        var serverSendNumbers = document.getElementById('serverSendNumbers');
        var tableNumberSelectShowStatus = document.getElementById(numberSelected);
        var btnReserve = document.getElementById("btnReserve");

        var canAdd = true;
        numbersSelectedToFutureBuyList.forEach((number) => {
            var find = false;
            if (numberSelected == number) {
                find = true;
            }

            if (find) {
                var pos = numbersSelectedToFutureBuyList.indexOf(numberSelected);
                //console.log('Indice do número encontrado: ' + pos);
                numbersSelectedToFutureBuyList.splice(pos, 1);
                tableNumberSelectShowStatus.className = "btn btn-success m-1"
                //console.log("Removido: " + numbersSelectedToFutureBuyList);
                canAdd = false;
            }

        })

        if (canAdd) {
            if (numbersSelectedToFutureBuyList.length < max) {
                numbersSelectedToFutureBuyList.push(numberSelected);
                tableNumberSelectShowStatus.className = "btn btn-info m-1"
            }
        }

        if (numbersSelectedToFutureBuyList.length > 0)
            btnReserve.removeAttribute('disabled');
        else
            btnReserve.setAttribute('disabled', 'true');

        participantUserSelectNumber.innerHTML = numbersSelectedToFutureBuyList.toLocaleString();
        serverSendNumbers.setAttribute("value", numbersSelectedToFutureBuyList.toLocaleString())


    } else {
        var alert = document.getElementById('divAlertMessage');
        var alertText = document.getElementById('alertMessage');
        alert.className = "alert alert-danger text-center"
        alertText.innerHTML = "Infelizmente você já atingiu o limite máximo de números para esse sorteio.";
    }
 
}


RafflePreSaveSelectNumberAmorin = (numberSelected, max) => {
    console.log(max);
    if (max > 0) {
        var participantUserSelectNumber = document.getElementById('participantUserSelectNumber');
        var serverSendNumbers = document.getElementById('serverSendNumbers');
        var tableNumberSelectShowStatus = document.getElementById(numberSelected);
        var btnReserve = document.getElementById("btnReserve");

        var canAdd = true;

        //var stop = parseInt(numberSelected) + 3;

        numbersSelectedToFutureBuyList.forEach((number) => {
            var find = false;
            if (numberSelected == number) {
                find = true;
            }

            if (find) {
                //for (var i = parseInt(numberSelected); i <= stop; i++) {
                //    var pos = numbersSelectedToFutureBuyList.indexOf(i);
                //    numbersSelectedToFutureBuyList.splice(pos, 1);
                //}
                var pos = numbersSelectedToFutureBuyList.indexOf(numberSelected);
                numbersSelectedToFutureBuyList.splice(pos, 1);
                var pos = numbersSelectedToFutureBuyList.indexOf(getAnimalName(numberSelected));
                numbersSelectedToFutureBuyListAmorin.splice(pos, 1);

                tableNumberSelectShowStatus.className = "btn btn-success m-1"
                canAdd = false;
            }

        })

        if (canAdd) {

            if (numbersSelectedToFutureBuyList.length < max) {

                //for (var i = parseInt(numberSelected); i <= stop; i++) {
                //    numbersSelectedToFutureBuyList.push(i);
                //}
                numbersSelectedToFutureBuyList.push(numberSelected);
                numbersSelectedToFutureBuyListAmorin.push(getAnimalName(numberSelected));

                tableNumberSelectShowStatus.className = "btn btn-info m-1"
            }
        }

        if (numbersSelectedToFutureBuyList.length > 0)
            btnReserve.removeAttribute('disabled');
        else
            btnReserve.setAttribute('disabled', 'true');

        participantUserSelectNumber.innerHTML = numbersSelectedToFutureBuyListAmorin.toLocaleString();
        serverSendNumbers.setAttribute("value", numbersSelectedToFutureBuyList.toLocaleString())


    } else {
        var alert = document.getElementById('divAlertMessage');
        var alertText = document.getElementById('alertMessage');
        alert.className = "alert alert-danger text-center"
        alertText.innerHTML = "Infelizmente você já atingiu o limite máximo de bichos para esse sorteio.";
    }

}

function getAnimalName(numberSelected) {
    if (numberSelected == 1)
        return "AVESTRUZ";
    else if (numberSelected == 5)
        return "ÁGUIA";
    else if (numberSelected == 9)
        return "BURRO";
    else if (numberSelected == 13)
        return "BORBOLETA";
    else if (numberSelected == 17)
        return "CACHORRO";
    else if (numberSelected == 21)
        return "CABRA";
    else if (numberSelected == 25)
        return "CARNEIRO";
    else if (numberSelected == 29)
        return "CAMELO";
    else if (numberSelected == 33)
        return "COBRA";
    else if (numberSelected == 37)
        return "COELHO";
    else if (numberSelected == 41)
        return "CAVALO";
    else if (numberSelected == 45)
        return "ELEFANTE";
    else if (numberSelected == 49)
        return "GALO";
    else if (numberSelected == 53)
        return "GATO";
    else if (numberSelected == 57)
        return "JACARÉ";
    else if (numberSelected == 61)
        return "LEÃO";
    else if (numberSelected == 65)
        return "MACACO";
    else if (numberSelected == 69)
        return "PORCO";
    else if (numberSelected == 73)
        return "PAVÃO";
    else if (numberSelected == 77)
        return "PERU";
    else if (numberSelected == 81)
        return "TOURO";
    else if (numberSelected == 85)
        return "TIGRE";
    else if (numberSelected == 89)
        return "URSO";
    else if (numberSelected == 93)
        return "VEADO";
    else if (numberSelected == 97)
        return "VACA";
    else
        return "ERROR!!";


}

RaffleReceiptSaveSelectNumber = (numberSelected, numberValue) => {

    var participantUserSelectNumber = document.getElementById('receiptNumberReference');
    var serverSendNumbers = document.getElementById('serverSendNumbersReceipt');
    var receiptValue = document.getElementById('receiptValue');
    var tableNumberSelectShowStatus = document.getElementById('toReceipt'+numberSelected);

    var canAdd = true;
    numbersSelectedReceipt.forEach((number) => {
        var find = false;
        if (numberSelected == number) {
            find = true;
        }

        if (find) {
            var pos = numbersSelectedReceipt.indexOf(numberSelected);
            numbersSelectedReceipt.splice(pos, 1);
            tableNumberSelectShowStatus.className = "btn btn-success m-1";
            totalReceiptNumberSelected -= 1;
            canAdd = false;
        }

    })

    if (canAdd) {
        numbersSelectedReceipt.push(numberSelected);
        tableNumberSelectShowStatus.className = "btn btn-info m-1";
        totalReceiptNumberSelected += 1;
    }

    participantUserSelectNumber.setAttribute("value", numbersSelectedReceipt.toLocaleString());
    serverSendNumbers.setAttribute("value", numbersSelectedReceipt.toLocaleString());

    var total = totalReceiptNumberSelected * parseFloat(numberValue.replace(",", "."));

    receiptValue.setAttribute("value", total.toLocaleString('pt-BR', { currency: 'BRL' }));

    VerifyNumberToAllowSendReceipt();
}


VerifyNumberToAllowSendReceipt = () => {
    var participantUserSelectNumber = document.getElementById('receiptNumberReference');
    var btnSendReceipt = document.getElementById('btnSendReceipt');

    //console.log(participantUserSelectNumber.getAttribute("value"));

    if (participantUserSelectNumber.getAttribute("value") != "") {
        btnSendReceipt.removeAttribute("disabled")

    } else {
        btnSendReceipt.setAttribute("disabled", "disabled")
    }

}