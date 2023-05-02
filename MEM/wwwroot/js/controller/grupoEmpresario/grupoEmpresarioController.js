app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    $scope.roles = [];

    CrudController($scope, NgTableParams, ajax, $uibModal, "GrupoEmpresario");

    $scope.filterCondition.Nombre = "con";
    $scope.filterCondition.Detalle = "con";


    $scope.$watch("selecteditem.limite", function (newValue, oldValue) {
        if (newValue == null || newValue < 1 || newValue > 10) {
            $scope.selecteditem.limite = 5;
        }
    });

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
});
registerController('AngularJSApp', 'appController');