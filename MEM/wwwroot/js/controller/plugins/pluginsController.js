app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    $scope.fileName = "";
    
    CrudController($scope, NgTableParams, ajax, $uibModal, "plugin");

    $scope.filterCondition.Nombre = "con";

    $scope.onChangeFile = function () {
        $("#file").click();
    }

    $scope.fileNameChanged = function (file) {
        $scope.file = file.files;
        $scope.fileName = file.files[0].name;
        $scope.onApply();
    }    

    $scope.onGuardar = function () {
        if ($scope.file == null) {
            $scope.onOpenDialog("Guardar", "Debe seleccionar un archivo.", function () {
            }, "sm", "modalOk.html", null, null, "fa-exclamation", "modal-danger");
        } else {
            if ($scope.onGuardarBefore()) {
                $scope.onOpenDialog("Guardar", "¿Está seguro de Guardar?", function () {
                    ajax.postUpload("plugin", "Guardar", $scope.selecteditem, $scope.file, function (data) {
                        $scope.onGuardarAfter();
                        $scope.onCancelar();
                    }, $scope.onDefaultReturnFault);
                }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o", "");
            }
        }        
    };

    $scope.onCancelarAfter = function () {
        this;
        $scope.file = null;
        $scope.fileName = "";
        $scope.onApply();
       
    };   

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
});
registerController('AngularJSApp', 'appController');