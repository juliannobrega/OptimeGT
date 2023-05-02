($scope.ObtenerExcel = function () {    
    var Ejecutar = {
        GraficoId: $scope.item.id,
        Metodo: "GetExcel",
        Parametros: [$scope.schema]
    }
    ajax.postDownloadFile("Grafico", "Ejecutar", Ejecutar, "TablaResultado.xlsx", "xlsx", function (data) {
        $scope.ChartCollection = data;
    }, $scope.onDefaultReturnFault);
    
});

($scope.ObtenerBDLic = function () {    
   $scope.showBuscando = true;
    var fileName = "BD - Licitacion.xlsx";
    var Ejecutar = {
        GraficoId: $scope.item.id,
        Metodo: "GetExcelBD",
        Parametros: [$scope.schema]
    }
    ajax.postDownloadFile("Grafico", "Ejecutar", Ejecutar, fileName,"xlsx", function (data) {
        $scope.ChartCollection = data;
        $scope.showBuscando = false;
    }, $scope.onDefaultReturnFault);
   
});


($scope.ObtenerGraficos = function () {      
        $scope.showBuscando = true;
        var Ejecutar = {
            GraficoId: $scope.item.id,
            Metodo: "ObtenerGraficos",
            Parametros: [$scope.schema]
        }
        ajax.post("Grafico", "Ejecutar", Ejecutar, function (data) {
            for (var i = 0; i < data.length; i++) {
                data[i].precio_Potencia = data[i].precio_Potencia;
                data[i].precio_Energia = data[i].precio_Energia;
                data[i].potencia_Media_Adjudicada = data[i].potencia_Media_Adjudicada;
                data[i].periodos_de_Potencia_Adjudicada = data[i].periodos_de_Potencia_Adjudicada;
                if (data[i].iD_Oferente == 'Total')
                    data[i].precio_Monomico = data[i].precio_Monomico;
                else
                    data[i].precio_Monomico = '-';
            }
            $scope.data = data;
            $scope.showBuscando = false;
        }, $scope.onDefaultReturnFault);
    
});
($scope.ObtenerGraficos());



