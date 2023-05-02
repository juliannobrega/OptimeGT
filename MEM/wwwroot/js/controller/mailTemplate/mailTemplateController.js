app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    $scope.roles = [];
    CrudController($scope, NgTableParams, ajax, $uibModal, "MailTemplate");

    $scope.filterCondition.Nombre = "con";
    $scope.filterCondition.Folder = "con";
   
    $scope.onAgregarAfter = function () {
        $scope.selecteditem.estado = "A";
        $scope.selecteditem.Id = $scope.roles[0].Id;
        return true;
    };

    $scope.onModificarAfter = function () {
        $scope.showButtonAdd = false;
        $scope.GetMailTemplate();
    };

    $scope.GetMailTemplate = function () {
        ajax.get("MailTemplate", "GetMailTemplate", $scope.selecteditem.id, function (data) {
            $scope.selecteditem = data;
        }, $scope.onDefaultReturnFault);
    }

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
});
registerController('AngularJSApp', 'appController');