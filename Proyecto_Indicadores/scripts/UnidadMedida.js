$(document).ready(function () {

    $('txtId').hide();

});

function GuardarUnidadMedidaClick() {

    var siglas = $('#txtSiglas').val().toUpperCase();

    if (siglas.length != 3) {

        alert('Las siglas deben componerse de 3 caracteres')

    }
    else {

        var id = $('#txtId').val();
        var descripcion = $('#txtDescripcion').val();

        //Crea objeto

        var objUnidadMedida = {

            "IdUnidad": id,
            "Siglas": siglas,
            "Descripcion": descripcion
        }//fin obj

        var applyUrl = '';
        var applyType = '';

        if (id === '') {

            applyUrl = '/UnidadMedida/GuardarUnidadMedida';
            applyType = 'POST';
        }
        else {

            applyUrl = '/UnidadMedida/EditarUnidadMedida';
            applyType = 'PUT';
        }

        $.ajax({

            type: applyType,
            url: applyUrl,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(objUnidadMedida),
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

function EliminarUnidadMedidaClick() {

    //La idea era eliminar el registro usando JS para poder devolver un alert pero no pude, 
    //Se hace directo desde el controller y no devuelve ninguna  notificacion porque no se pueden devolver notificaciones desde el controller


}