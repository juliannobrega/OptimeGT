($scope.showBuscando = false);
($scope.showGrafico = false);

($scope.ObtenerContrato = function () {
    var Ejecutar = {
        GraficoId: $scope.item.id,
        Metodo: "ObtenerContrato",
        Parametros: [$scope.schema]
    }
    ajax.post("Grafico", "Ejecutar", Ejecutar, function (data) {
        $scope.contratos = data;
        $scope.selectedContrato = $scope.contratos[0];        
    }, $scope.onDefaultReturnFault);
});
($scope.ObtenerContrato());


($scope.ObtenerFechas = function () {
    var Ejecutar = {
        GraficoId: $scope.item.id,
        Metodo: "ObtenerFechas",
        Parametros: [$scope.schema]
    }
    ajax.post("Grafico", "Ejecutar", Ejecutar, function (data) {
        $scope.fechaMin = data;
        $scope.fechaMax = data;
        $scope.selectedFechaMin = $scope.fechaMin[0];
        $scope.selectedFechaMax = $scope.fechaMax[$scope.fechaMax.length - 1];
    }, $scope.onDefaultReturnFault);
});
($scope.ObtenerFechas());


($scope.ObtenerExcel = function () {
    if ($scope.selectedFechaMin != null) {
        var Ejecutar = {
            GraficoId: $scope.item.id,
            Metodo: "GetExcel",
            Parametros: [$scope.schema, $scope.selectedContrato.id,
            $scope.selectedFechaMin.id, $scope.selectedFechaMax.id]
        }
        ajax.postDownloadFile("Grafico", "Ejecutar", Ejecutar, "PreciosporOferta.xlsx", "xlsx", function (data) {
            $scope.ChartCollection = data;
        }, $scope.onDefaultReturnFault);
    }
});

($scope.ObtenerGraficos = function () {
    $scope.showGrafico = false;
    $scope.ChartCollection = { charts: [] };
    if ($scope.selectedFechaMin != null) {
        $scope.showBuscando = true;
        var Ejecutar = {
            GraficoId: $scope.item.id,
            Metodo: "ObtenerGraficos",
            Parametros: [$scope.schema, $scope.selectedContrato.id,
            $scope.selectedFechaMin.id, $scope.selectedFechaMax.id]
        }
        ajax.post("Grafico", "Ejecutar", Ejecutar, function (data) {
            $scope.ChartCollection = data;
            $scope.showGrafico = true;
            $scope.showBuscando = false;
        }, $scope.onDefaultReturnFault);
    }
});

($scope.options = {
    "chart": {
        "type": "multiBarChart",
        "height": 500,
        "stacked": true,
        "margin": {
            "top": 100,
            "right": 20,
            "bottom": 75,
            "left": 70
        },
        "useVoronoi": false,
        "clipEdge": true,
        "duration": 100,
        "useInteractiveGuideline": true,
        "xAxis": {
            "rotateLabels": -45,
            "axisLabelDistance": 20,
            "axisLabel": "Nodo",
            "showMaxMin": false,
            "tickFormat": function (d) {
                return $scope.ChartCollection.labelsX[d.toString()]
            }
        },
        "yAxis": {
            "axisLabel": "USD/MWh",
            "axisLabelDistance": 0,
            "tickFormat": function (d) {
                return d3.format(",.2f")(d);
            }
        },
        "reduceXTicks": false,
        "showControls": false,
        "legend": {
            "align": false
        },

    }
});

