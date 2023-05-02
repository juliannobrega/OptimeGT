($scope.excelExtension = ["xlsx", "xlsm", "xlsb", "xltx", "xltm", "xlt", "xls", "xml", "xlam", "xla", "xlw", "xlr"]);
($scope.onChangeFile = function () {
    $("#file").click();
});

($scope.fileNameChanged = function (file) {
    $scope.file = file.files;
    $scope.ImportarExcel();
});

($scope.ImportarExcel = function () {
    var fileExtension = $scope.file[0].name.split(".");
    if ($scope.excelExtension.includes(fileExtension[fileExtension.length - 1])) {
        $scope.showBuscando = true;
        $scope.showBuscandoTablas = true;
        var Ejecutar = {
            Id: $scope.item.id,
            Metodo: "ImportarExcel",
            Parametros: [$scope.item.schema]
        }
        ajax.postUpload("Supuesto", "EjecutarUpload", Ejecutar, $scope.file, function (data) {
            $scope.supuestosInfo = data;
            $scope.ObtenerInfoImport();           
        }, $scope.onDefaultReturnFault);  
    } else {
        $scope.onOpenDialog("Importar", 'Formato de archivo incorrecto, el archivo deber tener la extensi&oacute;n "xlsx".', null
            , "sm", "modalOk.html", null, null, "fa-exclamation", "modal-danger");        
    };    
});

($scope.ObtenerExcel = function () {
    $scope.showBuscando = true;
    var fileName = "BD - Licitacion.xlsx";
    var Ejecutar = {
        Id: $scope.item.id,
        Metodo: "GetExcel",
        Parametros: [$scope.item.schema, $scope.item.selectdIsSup]
    }
    ajax.postDownloadFile("Supuesto", "Ejecutar", Ejecutar, fileName, "xlsx", function (data) {
        $scope.showBuscando = false;
    }, $scope.onDefaultReturnFault);
});


($scope.ObtenerInfoImport = function () {
    ajax.get("Supuesto", "SupuestoInfo", $scope.supuestosInfo.id, function (data) {        
        $scope.supuestosInfo = data;
        if (data.error) {
            $scope.showBuscando = false;
            $scope.showBuscandoTablas = false;
            $scope.onOpenDialog("Importar", data.errorMsj, null
                , "sm", "modalOk.html", null, null, "fa-exclamation", "modal-danger");
        } else {
            if ($scope.supuestosInfo.fin) {
                $scope.onOpenDialog("Importar", "Se ha importado exitosamente", null
                    , "sm", "modalOk.html");
                $("#file").val(null);
                $scope.showBuscandoTablas = false;
            } else {
                $scope.supuestoTablaNombre = $scope.supuestosInfo.nombre;
                $scope.supuestoTablaPorcentaje = $scope.supuestosInfo.index + "/" + $scope.supuestosInfo.total;
                setTimeout(function () {
                    $scope.ObtenerInfoImport();
                }, 1000);
            }
        }
    }, $scope.onDefaultReturnFault);
});