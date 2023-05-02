app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope, NgMap) {

    //$scope.selector = { schema: "mem_escenario" };
    $scope.show = false;
    BaseController($scope, $uibModal);

    $scope.onChangeSchema = function () {
        $scope.show = false;
        $scope.selected = null;
        $scope.selectedTipo = null;       
        ajax.get("Grafico", "BuscarTipos", null, function (data) {
            $scope.tipos = data;
            $scope.onApply();
        }, $scope.onDefaultReturnFault);
        $scope.onApply();
    }

    $scope.onChangeTipo = function () {
        $scope.show = false;
        $scope.selected = null;
        ajax.get("Grafico", "BuscarGraficosPorTipo", [$scope.selectedTipo.id], function (data) {
            $scope.graficos = data;
            $scope.onApply();
        }, $scope.onDefaultReturnFault);

        $scope.onApply();        
    }

    $scope.onChangeGrafico = function () {
        $scope.show = false;
        $scope.selected.schema = $scope.selector.schema;        
        $scope.onApply();
    }

  
    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
});
registerController('AngularJSApp', 'appController');