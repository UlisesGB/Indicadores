$(document).ready(function () {

    $('txtId').hide();

});

function GuardarPosValorClick() {

    var valor = $('#txtValor').val().toUpperCase();

    if (valor.length != 3) {

        alert('El valor debe componerse de 3 caracteres')

    }
    else {

        var id = $('#txtId').val();
        var descripcion = $('#txtDescripcion').val();
        var IdMetadata = $('#ddlMetadata').val();

        //Crea objeto

        var objPosValor = {

            "IdPosvalor": id,
            "Valor": valor,
            "Descripcion": descripcion,
            "IdMetadata": IdMetadata
        }//fin obj

        var applyUrl = '';
        var applyType = '';

        if (id === '') {

            applyUrl = '/PosValor/GuardarPosValor';
            applyType = 'POST';
        }
        else {

            applyUrl = '/PosValor/EditarPosValor';
            applyType = 'PUT';
        }

        $.ajax({

            type: applyType,
            url: applyUrl,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(objPosValor),
            dataType: 'application/json',
            async: true,
            processData: false,
            cache: false,
            success: function (data) {
                alert('Datos grabados correctamente.');
            },
            error: function (jq, status, message) {
                alert('Ocurrio un error, es posible que sus cambios no se guardaran, favor verifique la lista.');
            }
        })


    }

}