($scope.showBuscando = false);
($scope.showGrafico = false);

($scope.ObtenerNombre = function () {
    var Ejecutar = {
        GraficoId: $scope.item.id,
        Metodo: "ObtenerNombre",
        Parametros: [$scope.schema]
    }
    ajax.post("Grafico", "Ejecutar", Ejecutar, function (data) {
        $scope.nombres = data;
        $scope.selectedNombre = $scope.nombres[0];
    }, $scope.onDefaultReturnFault);
});
($scope.ObtenerNombre());


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
            Parametros: [$scope.schema, $scope.selectedNombre.id,
            $scope.selectedFechaMin.id, $scope.selectedFechaMax.id]
        }
        ajax.postDownloadFile("Grafico", "Ejecutar", Ejecutar, "EnergiaporOferenteyContrato.xlsx", "xlsx", function (data) {
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
            Parametros: [$scope.schema, $scope.selectedNombre.id,
            $scope.selectedFechaMin.id, $scope.selectedFechaMax.id]
        }
        ajax.post("Grafico", "Ejecutar", Ejecutar, function (data) {
            $scope.ChartCollection = data;
            $scope.options = {
                "chart": {
                    "type": "multiChart",
                    "height": 500,
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
                        "rotateLabels": 26,
                        "ticks": 10,
                        "axisLabel": "",
                        "showMaxMin": false,
                        "tickFormat": function (d) {
                            return $scope.ChartCollection.labelsX[d.toString()]
                        }
                    },
                    "yAxis1": {
                        "axisLabel": "MWh",
                        "axisLabelDistance": 5,
                        "tickFormat": function (d) {
                            return d3.format(".0f")(d);
                        }
                    },
                    "legend": {
                        "align": false
                    },
                    "bars1": {
                        "dispatch": {},
                        "width": 960,
                        "height": 500,
                        "stacked": true,
                        "stackOffset": "zero",
                        "clipEdge": true,
                        "hideable": true,
                        "groupSpacing": 0.1,
                        "margin": {
                            "top": 0,
                            "right": 0,
                            "bottom": 0,
                            "left": 0
                        }
                    },
                    "yDomain1": [data.yminValue, data.ymaxValue]
                }
            };

            $scope.showGrafico = true;
            $scope.showBuscando = false;
            $scope.intervalos = (Math.trunc($scope.selectedFechaMax.id / 100) - Math.trunc($scope.selectedFechaMin.id / 100) + 1) * 12;
        }, $scope.onDefaultReturnFault);
    }
});