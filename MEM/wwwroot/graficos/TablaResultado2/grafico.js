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
                data[i].flexibilidad = data[i].flexibilidad;
                data[i].precio_Potencia = data[i].precio_Potencia;
                data[i].precio_Energia = data[i].precio_Energia;
                data[i].potencia_Media_Ofertada = data[i].potencia_Media_Ofertada.format(0, 3, ',');
                data[i].potencia_Media_Adjudicada = data[i].potencia_Media_Adjudicada.format(0, 3, ',');
                data[i].periodos_de_Potencia_Adjudicada = data[i].periodos_de_Potencia_Adjudicada;
                data[i].periodos_de_Energia_Adjudicada = data[i].periodos_de_Energia_Asignada;
                data[i].energia_Asignada = data[i].energia_Asignada.format(0, 3, ',');
                data[i].costo_Total_Potencia = data[i].costo_Total_Potencia.format(0, 3, ',');
                data[i].costo_Total_Energia = data[i].costo_Total_Energia.format(0, 3, ',');
                data[i].costo_Total = data[i].costo_Total.format(0, 3, ',');
            }
            $scope.data = data;
            $scope.showBuscando = false;
        }, $scope.onDefaultReturnFault);
});

    Number.prototype.format = function(n, x, s, c) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
    num = this.toFixed(Math.max(0, ~~n));
    return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));};

($scope.ObtenerGraficos());
