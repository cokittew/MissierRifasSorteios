OpenService = (urlService, idUser, signarute) => {
    $.ajax({
        type: "GET",
        url: urlService + "?userId=" + idUser + "&signature=" + signarute,
        success: function (res) {
            $("#serviceLoadArea").html(res);
        } 
    })
}

OpenRaffleList = (urlService, idUser) => {
    $.ajax({
        type: "GET",
        url: urlService + "?userId=" + idUser,
        success: function (res) {
            $("#raffleActionLoadArea").html(res);
        }
    })
}

OpenRaffleAddNew = (urlService, idUser, signature) => {
    $.ajax({
        type: "GET",
        url: urlService + "?userId=" + idUser + "&signature=" + signature,
        success: function (res) {
            $("#raffleActionLoadArea").html(res);
        }
    })
}

//Raffle - Tirar daqui depois
RaffleSearchFilterNameCode = (url) => {
    var code = document.getElementById("raffleCodeSearch");
    var name = document.getElementById("raffleNameNickSearch");
    var type = document.getElementById("raffleNameTypeSearch");

    url = url + "?code=" + code.value + "&nameNick=" + name.value + "&type=" + type.value;

    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#raffleListArea").empty();
            $("#raffleListArea").html(res);
        }
    })
}

RaffleChangeStatus = (url, raffleId) => {
    url = url + "?raffleId=" + raffleId;

    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#raffleListArea").empty();
            $("#raffleListArea").html(res);
        }
    })
}

RaffleBuySelectedNumber = (url, raffleId, userId, numbers) => {

    url = url + "?raffleId=" + raffleId + "&userId=" + userId + "&numbers=" + numbers + ","

    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#participateSelectNumberMainPage").empty();
            $("#participateSelectNumberMainPage").html(res);
        }
    })
}

AddSelectNumberInList = (url, number, raffleId, userId, add, numbers) => {

    url = url + "?numberSelected=" + number + "&raffleId=" + raffleId + "&userId=" + userId + "&add=" + add + "&numbers="+ numbers;

    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#raffleByNamberArea").empty();
            $("#raffleByNamberArea").html(res);
        }
    })
}

//Raffle MainPage - Payment Calculate

RaffleCalculateBuyNumber = () => {

    var quant = document.getElementById("QuantityOfNumberToBuy");
    var buyButton = document.getElementById("ConfirmSellNumber");
    var valueToPay = document.getElementById("calculateValue");
    var discount = document.getElementById("discountSold");

    if (quant.value > 29) {
        buyButton.removeAttribute("disabled");

        var valueInitial = 0.50;
        var value = 0.50;

        if (quant.value < 50) {
            value = parseFloat(quant.value * valueInitial).toFixed(2);
            discount.innerHTML = "<span class=\"text - light\">Descontos incríveis a partir de 50 números.</span>"
        }else if (quant.value >= 50 && quant.value <= 100) {
            valueInitial = 0.35;
            var discountValue = parseFloat(quant.value * 0.50).toFixed(2);
            value = parseFloat(quant.value * valueInitial).toFixed(2);
            discount.innerHTML = `<span class=\"text-light\">De R$ ${discountValue} por <b>R$ ${value}</b></span>`;
        }else if (quant.value > 100 && quant.value <= 400) {
            valueInitial = 0.30;
            var discountValue = parseFloat(quant.value * 0.50).toFixed(2);
            value = parseFloat(quant.value * valueInitial).toFixed(2);
            discount.innerHTML = `<span class=\"text-light\">De R$ ${discountValue} por <b>R$ ${value}</b></span>`;
        } else if (quant.value > 400) {
            valueInitial = 0.25;
            var discountValue = parseFloat(quant.value * 0.50).toFixed(2);
            value = parseFloat(quant.value * valueInitial).toFixed(2);
            discount.innerHTML = `<span class=\"text-light\">De R$ ${discountValue} por <b>R$ ${value}</b></span>`;
        }

        valueToPay.setAttribute("value", value );

    } else {
        buyButton.setAttribute("disabled", "disabled");
        valueToPay.setAttribute("value", 0);
        discount.innerHTML = "<span class=\"text - light\">Descontos a partir de 50 números.</span>"
    }

}


//Platform Raffle Datails

GetParticipantInformationByNumber = (url, route, raffleId, status) => {

    var manual = document.getElementById('CloseOption').getAttribute('value');
    if (manual != 1) {
        document.getElementById("receiptImage").setAttribute("src", "");
        $("#hideImageCard").hide();
        document.getElementById("receiptImageModal").setAttribute("src", "");
    }
        
    $.ajax({
        type: "GET",
        url: url + route,
        dataType : 'json',
        success: function (res) {
            $("#AllSearches").empty();
            $("#alertMessage").empty();
            var result = JSON.stringify(res);

            var json = result.replace("'", "\"");
            var json = json.replace("'", "\"");
            var obj = JSON.parse(json);

            if (document.getElementById("Manual").value == "manual") {
                document.getElementById("ParticipantNumberInformation").innerHTML = `<div class="col-sm-12 col-lg-12">
                                        <span class="lead mt-3" id="paricipantNick">Usuário: <b>${obj.NickName}</b></span>
                                    </div>
                                    <div class="col-sm-12 col-lg-12 mt-1">
                                        <span class="lead mt-3" id="">Número: </span>
                                        <span class="btn btn-danger" id="paricipantNumber"> ${obj.Number}</span>
                                    </div>`
            } else {

                document.getElementById("ParticipantNumberInformation").innerHTML = `<div class="col-sm-12 col-lg-12">
                                        <span class="lead mt-3" id="paricipantNick">Usuário: <b>${obj.NickName}</b></span>
                                    </div>
                                    <div class="col-sm-12 col-lg-12 mt-1">
                                        <span class="lead mt-3" id="">Número: </span>
                                        <span class="btn btn-warning" id="paricipantNumber"> ${obj.Number}</span>
                                    </div>
                                    <div class="col-sm-12 col-lg-12 mt-1 mb-2">
                                        <span class="lead mt-3" id="">Números Vinculados: </span>
                                        <span class="btn btn-info" id="paricipantNumber"> <b>${obj.Numbers}</b></span>
                                    </div>`


                if (manual != 1) {
                    if (obj.File != undefined && obj.File != "") {
                        $("#hideImageCard").show();

                        var f = obj.File.split(';');
                        var f2 = f[0].split(':');

                        const blob = b64toBlob(f[1].replace("base64,", ""), f2[1]);
                        const blobUrl = URL.createObjectURL(blob);
                        document.getElementById("receiptImage").setAttribute("src", blobUrl);
                        document.getElementById("receiptImageModal").setAttribute("src", blobUrl);

                        //var formattedValue = value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                        document.getElementById("receiptValueUserSaid").innerText = "R$" + obj.Value;
                        document.getElementById("receiptTitle").innerText = obj.NickName + " - " + obj.Numbers + " - R$" + obj.Value;

                    } else {
                        // $("#confirmPaymentFormDiv").hide();//   document.getElementById("confirmPaymentFormDiv").hide();
                    }
                }
            }

            document.getElementById("NumberBought").setAttribute("value", obj.Number);

            if (manual != 1)
                document.getElementById("NumberBoughtRefuse").setAttribute("value", obj.Number);

            if (document.getElementById("CloseOption").value == 1) {
                document.getElementById("Identity").value = '';

                $("#Identity").hide();
            }

            var check = document.getElementById("concience");
            var checkRefuse = document.getElementById("concienceRefuse");
            
            if (check.checked)
                check.click();

            if (checkRefuse.checked)
                checkRefuse.click();

            if (status == 3) {
                $("#confirmPaymentFormDiv").hide();
                $("#refusePaymentFormDiv").hide();
            } else {
                $("#confirmPaymentFormDiv").show();
                $("#refusePaymentFormDiv").show();
            }
        },
        error: function (res) {

            document.getElementById("ParticipantNumberInformation").value = '';

            var json = res.responseText.replace("'", "\"");
            var json = json.replace("'", "\"");
            var obj = JSON.parse(json);

            var alert = document.getElementById("alertMessage");
            alert.innerHTML = `<div class="container-fluid mb-2">\
                <div class="alert alert-danger text-center" role="alert">
                  ${obj.msg}
                </div>
            </div>`

        }

    })
}

GetParticipantInformationByNumberAmorin = (url, route, raffleId, status) => {

    var manual = document.getElementById('CloseOption').getAttribute('value');
    if (manual != 1) {
        document.getElementById("receiptImage").setAttribute("src", "");
        $("#hideImageCard").hide();
        document.getElementById("receiptImageModal").setAttribute("src", "");
    }

    $.ajax({
        type: "GET",
        url: url + route,
        dataType: 'json',
        success: function (res) {
            $("#AllSearches").empty();
            $("#alertMessage").empty();
            var result = JSON.stringify(res);

            var json = result.replace("'", "\"");
            var json = json.replace("'", "\"");
            var obj = JSON.parse(json);

            if (document.getElementById("Manual").value == "manual") {
                document.getElementById("ParticipantNumberInformation").innerHTML = `<div class="col-sm-12 col-lg-12">
                                        <span class="lead mt-3" id="paricipantNick">Usuário: <b>${obj.NickName}</b></span>
                                    </div>
                                    <div class="col-sm-12 col-lg-12 mt-1">
                                        <span class="lead mt-3" id="">Número: </span>
                                        <span class="btn btn-danger" id="paricipantNumber"> ${obj.Number}</span>
                                    </div>`
            } else {

                var cc = obj.Numbers.split(',');
                var listAnimalName = "| ";

                cc.forEach((num) => {
                    listAnimalName += getAnimalName(num) + " | ";
                });

                    


                document.getElementById("ParticipantNumberInformation").innerHTML = `<div class="col-sm-12 col-lg-12">
                                        <span class="lead mt-3" id="paricipantNick">Usuário: <b>${obj.NickName}</b></span>
                                    </div>
                                    <div class="col-sm-12 col-lg-12 mt-1">
                                        <span class="lead mt-3" id="">Bicho: </span>
                                        <span class="btn btn-warning" id="paricipantNumber"> ${getAnimalName(obj.Number)}</span>
                                    </div>
                                    <div class="col-sm-12 col-lg-12 mt-1 mb-2">
                                        <span class="lead mt-3" id="">Bichos Vinculados: </span>
                                        <span class="btn btn-info" id="paricipantNumber"> <b>${listAnimalName}</b></span>
                                    </div>`


                if (manual != 1) {
                    if (obj.File != undefined && obj.File != "") {
                        $("#hideImageCard").show();

                        var f = obj.File.split(';');
                        var f2 = f[0].split(':');

                        const blob = b64toBlob(f[1].replace("base64,", ""), f2[1]);
                        const blobUrl = URL.createObjectURL(blob);
                        document.getElementById("receiptImage").setAttribute("src", blobUrl);
                        document.getElementById("receiptImageModal").setAttribute("src", blobUrl);

                        //var formattedValue = value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                        document.getElementById("receiptValueUserSaid").innerText = "R$" + obj.Value;
                        document.getElementById("receiptTitle").innerText = obj.NickName + " - " + obj.Numbers + " - R$" + obj.Value;

                    } else {
                        // $("#confirmPaymentFormDiv").hide();//   document.getElementById("confirmPaymentFormDiv").hide();
                    }
                }
            }

            document.getElementById("NumberBought").setAttribute("value", obj.Number);

            if (manual != 1)
                document.getElementById("NumberBoughtRefuse").setAttribute("value", obj.Number);

            if (document.getElementById("CloseOption").value == 1) {
                document.getElementById("Identity").value = '';

                $("#Identity").hide();
            }

            var check = document.getElementById("concience");
            var checkRefuse = document.getElementById("concienceRefuse");

            if (check.checked)
                check.click();

            if (checkRefuse.checked)
                checkRefuse.click();

            if (status == 3) {
                $("#confirmPaymentFormDiv").hide();
                $("#refusePaymentFormDiv").hide();
            } else {
                $("#confirmPaymentFormDiv").show();
                $("#refusePaymentFormDiv").show();
            }
        },
        error: function (res) {

            document.getElementById("ParticipantNumberInformation").value = '';

            var json = res.responseText.replace("'", "\"");
            var json = json.replace("'", "\"");
            var obj = JSON.parse(json);

            var alert = document.getElementById("alertMessage");
            alert.innerHTML = `<div class="container-fluid mb-2">\
                <div class="alert alert-danger text-center" role="alert">
                  ${obj.msg}
                </div>
            </div>`

        }

    })
}

const b64toBlob = (b64Data, contentType = '', sliceSize = 512) => {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);

        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
}


RaffleManualConfirmation = (number) => {

    if (number > 0) {
        document.getElementById("ParticipantNumberInformation").innerHTML = `
<div class="col-sm-12 col-lg-12 mt-1">
                                        <span class="lead mt-3" id="">Número: </span>
                                        <span class="btn btn-danger" id="paricipantNumber"> ${number}</span>
                                    </div>`

        document.getElementById("NumberBought").setAttribute("value", number)

        if (document.getElementById("CloseOption").getAttribute("value") != "1")
            document.getElementById("NumberBoughtRefuse").setAttribute("value", number)

        $("#Identity").show();

        var check = document.getElementById("concience");
        if (check.checked) {
            check.click();
        }
    }
 
}

AllowConfirmPayment = () => {
    var paymentButton = document.getElementById("confirmPay");
    var number = document.getElementById("NumberBought");
    var receipt = document.getElementById("receiptImage");
    var check = document.getElementById("concience");

    var checkOtherOption = document.getElementById("concienceRefuse");

    if (check.checked) {
        if (number.value > 0 && receipt.getAttribute('src') != "") {
            paymentButton.removeAttribute("disabled");

            if (checkOtherOption.checked)
                checkOtherOption.click();

        } else {
            paymentButton.setAttribute("disabled", "disabled");
        }
    } else {
        paymentButton.setAttribute("disabled", "disabled");
    } 
}

AllowConfirmPaymentManualOption = () => {
    var paymentButton = document.getElementById("confirmPay");
    var number = document.getElementById("NumberBought");
    var check = document.getElementById("concience");

    if (check.checked) {
        if (number.value > 0) {

            if (document.getElementById("Identity").value.length > 9)
                paymentButton.removeAttribute("disabled");
            else
                check.click();

        } else {
            paymentButton.setAttribute("disabled", "disabled");
        }
    } else {
        paymentButton.setAttribute("disabled", "disabled");
    }
}

Restartconcience = () => {
    var check = document.getElementById("concience");
    if (check.checked)
    {
        check.click();
    }
}

RefuseConfirmPayment = () => {
    var paymentButton = document.getElementById("refusePay");
    var number = document.getElementById("NumberBoughtRefuse");
    var check = document.getElementById("concienceRefuse");

    var checkOtherOption = document.getElementById("concience");

    if (check.checked) {

        if (number.value > 0) {
            paymentButton.removeAttribute("disabled");
            if (checkOtherOption.checked)
                checkOtherOption.click();

        } else {
            paymentButton.setAttribute("disabled", "disabled");
        }
    } else {
        paymentButton.setAttribute("disabled", "disabled");
    }

}

UpdateRaffleInformtions = (url) => {

    var raffleId = document.getElementById("raffleIdUpdate");
    var description = document.getElementById("description");
    var maxNumberUser = document.getElementById("userMaxNumber");
    var beginDate = document.getElementById("beginDate");
    var endDate = document.getElementById("endDate");
    var alertt = document.getElementById("alert");

    if (description.value == null)
        description.value = '';

    url += "?raffleIdUpdate=" + raffleId.value + "&description=" + description.value + "&maxNumberUser=" + maxNumberUser.value + "&beginDate=" + beginDate.value + "&endDate=" + endDate.value;
    alert(url);
    $.ajax({
        type: "POST",
        url: url,
        data: {
            raffleId: raffleId.value
        },
        success: function (res) {
            var result = JSON.stringify(res);
            //alert(t);
            var json = result.replace("'", "\"");
            var json = json.replace("'", "\"");
            var obj = JSON.parse(json);

            alertt.innerHTML = `<div class="container-fluid mb-2">
                <div class="alert alert-success text-center" role="alert">
                  ${obj.msg}
                </div>
            </div>`
        },
        error: function (res) {
            var json = res.responseText.replace("'", "\"");
            var json = json.replace("'", "\"");
            var obj = JSON.parse(json);

           
            alertt.innerHTML = `<div class="container-fluid mb-2">
                <div class="alert alert-danger text-center" role="alert">
                  ${obj.msg}
                </div>
            </div>`
        }

    });

    return false;
}

GetParticipantInformationBySearch = (url) => {
    alert("GetParticipantInformationBySearch")
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#AllSearches").empty();
        },
        error: function (res) {

        }

    })
}

//Forms Answers/Subimits


//Add Amorin Metthodds

RaffleSelectTypeEvent = () => {
    var RaffleType = document.getElementById('RaffleType');
    var val = document.getElementById('RaffleType').value;

    var maxNumbers = document.getElementById('RaffleMaxNumberLimited');
    var maxWinners = document.getElementById('RaffleWinnersNumber');
    var manualAction = document.getElementById('RaffleCloseOption');
    var numbersWeHave = document.getElementById('numbersWeHave').value;

    if (val == 1) {
        maxNumbers.removeAttribute('disabled')
        maxWinners.removeAttribute('disabled')
        manualAction.removeAttribute('disabled')

    } else if (val == 3) {
        if (numbersWeHave >= 100) {
            maxNumbers.setAttribute('disabled', 'disabled')
            maxNumbers.value = 100;
            maxWinners.setAttribute('disabled', 'disabled')
            maxWinners.value = 1;
            if (manualAction.checked) {
                manualAction.click();
            }
            manualAction.setAttribute('disabled', 'disabled')

            
        } else {
            alert("Você não possui números suficientes para essa modalidade de sorteio.");
            RaffleType.value = 1;

        }
        
    }


}


