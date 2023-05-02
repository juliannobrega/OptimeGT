app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    $scope.roles = [];

    CrudController($scope, NgTableParams, ajax, $uibModal, "Usuario");

    $scope.filterCondition.Nombre = "con";
    $scope.filterCondition.Apellido = "con";
    $scope.filterCondition.Usuario = "con";
    $scope.filterCondition.Email = "con";
    $scope.filterCondition.GrupoEmpresario = "=|T";

    $scope.onBuscarAfter = function(data) {
        for (var i = 0; i < data.data.length; i++) {
            data.data[i].claveChequed = data.data[i].clave;
        }

        $rootScope.$broadcast('UpdateGrupoInfo', null);
    }

    $scope.getPerfiles = function () {
        ajax.post("Usuario", "GetPerfiles", null, function (data) {
            $scope.roles = data;
        });
    };

    $scope.onVerMisDatos = function () {
        ajax.post("Usuario", "GetUsuarioUnico", null, function (data) {
            $scope.selecteditem = data;
            $scope.onModificarAfter();
            $scope.changeMode($scope.MODO_EDITAR);
        });
    };
   

    $scope.onAgregarAfter = function () {
        $scope.selecteditem.estado = "A";
        $scope.selecteditem.perfilId = $scope.roles[0].perfilId;
        return true;
    };

    $scope.onModificarAfter = function () {
        $scope.showButtonAdd = false;
        $scope.filterObject.GrupoId = $scope.selecteditem.grupoEmpresario;
    };

    try {
        if (done !== undefined) {
            $scope.onVerMisDatos();
        }
    } catch (e) { }

    $scope.getPerfiles();

    ajax.get("Usuario", "BuscarGrupoEmpresarioAll", null, function (data) {
        $scope.grupos = data;
    }, $scope.onDefaultReturnFault);

    ajax.get("Usuario", "BuscarGrupoEmpresarioToAdd", null, function (data) {
        $scope.gruposToAdd = data;
        $scope.gruposToAddBack = data;
        $scope.grupoEmpresario = data[0].id;
    }, $scope.onDefaultReturnFault);

    $scope.verPerfil = true;
    $scope.$watch("selecteditem.perfilId", function (newValue, oldValue) {
        if ($scope.gruposToAddBack && newValue) {
            ajax.get("Perfil", "IsAdminPerfil", newValue, function (isAdmin) {
                if (isAdmin) {
                    $scope.verPerfil = false;
                    $scope.gruposToAdd = null;
                    $scope.selecteditem.grupoEmpresario = -1;
                } else {
                    $scope.verPerfil = true;
                    $scope.gruposToAdd = $scope.gruposToAddBack;
                    $scope.selecteditem.grupoEmpresario = $scope.gruposToAddBack[0].id;
                }

            }, $scope.onDefaultReturnFault);
        }       
    });

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
});
registerController('AngularJSApp', 'appController');