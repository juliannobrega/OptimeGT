app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    CrudController($scope, NgTableParams, ajax, $uibModal, "Perfil");

    $scope.filterCondition.Nombre = "con";
    $scope.filterCondition.Accesos = "in";

    $scope.getAccesos = function () {
        ajax.post("Perfil", "GetAccesos", null, function (data) {
            $scope.accesos = data;
        });
    };

    $scope.onAgregarAfter = function () {
        $scope.selecteditem.estado = "A";
        for (var i = 0; i < $scope.accesos.length; i++) {
            $scope.accesos[i].isSelected = false;
        }
    };

    $scope.onModificarAfter = function () {
        $.each($scope.accesos, function (key0, item) {
            item.isSelected = false;
        });
        $.each($scope.selecteditem.accesos, function (key1, acc) {
            $.each($scope.accesos, function (key2, item) {
                if (item.accesoId == acc.accesoId) {
                    item.isSelected = acc.grantPermition == "1";
                }
            });
        });
        return true;
    };

    $scope.onChageAcceso = function (item) {
        item.isSelected = !item.isSelected;
        if ($scope.selecteditem.accesos === undefined || $scope.selecteditem.accesos === null)
            $scope.selecteditem.accesos = [];

        for (var i = 0; i < $scope.selecteditem.accesos.length; i++) {

            if ($scope.selecteditem.accesos[i].accesoId == item.accesoId) {
                $scope.selecteditem.accesos[i].grantPermition = item.isSelected ? "1" : "0";

                return;
            }
            $scope.selecteditem.accesos[i].perfilId = $scope.selecteditem.perfilId;
        }
        if (item.isSelected == true) {
            $scope.selecteditem.accesos.push({ accesoId: item.accesoId, grantPermition: "1" });
        }

    };

    $scope.getAccesos();

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);

});
registerController('AngularJSApp', 'appController');
