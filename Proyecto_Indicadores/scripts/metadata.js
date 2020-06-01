$(document).ready(function () {
    $('#txtId').hide();
});

function SaveMetadaClick() {
    var pre = $("#txtPrefijo").val();
    pre = pre.toUpperCase();

    if (pre.length === 3) {

        var ult = pre.charAt(pre.length - 1);
        if (ult === '_') {
            var id = $("#txtId").val();
            var nombre = $("#txtNombre").val();
            var descr = $("#txtDescripcion").val();
            var ObjMetadata = { "IdMetadata": id, "Nombre": nombre, "Descripcion": descr, "Prefijo": pre };
            var applyUrl = '';
            var applyType = '';
            if (id === '') {
                applyUrl = '/Metadata/SaveMetadata';
                applyType = 'POST';
            }
            else {
                applyUrl = '/Metadata/EditMetadata';
                applyType = "PUT";
            }

            $.ajax({
                type: applyType,
                url: applyUrl,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(ObjMetadata),
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
            });
        }
        else {
            alert('El prefijo debe terminar con "_".');
        }
    }
    else {
        alert('El prefijo debe ser de 3 caracteres.');
    }
}