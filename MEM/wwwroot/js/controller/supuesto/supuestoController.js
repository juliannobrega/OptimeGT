app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    $scope.tableDefaultSorting = { Grupo : "asc"};

    $scope.showSupuesto = false;
    $scope.showLoad = false;
    $scope.showCargando = false;

    $scope.aceHtml = {
        workerPath: '/lib/ace',
        theme: 'chrome',
        require: ['ace/ext/language_tools'],
        advanced: {
            enableBasicAutocompletion: true,
            enableSnippets: true,
            enableLiveAutocompletion: true
        },
        mode: "html",
    };

    $scope.aceJS = {
        workerPath: '/lib/ace',
        theme: 'chrome',
        require: ['ace/ext/language_tools'],
        advanced: {
            enableBasicAutocompletion: true,
            enableSnippets: true,
            enableLiveAutocompletion: true
        },
        mode: "javascript",
    };

    $scope.aceCS = {
        workerPath: '/lib/ace',
        theme: 'chrome',
        require: ['ace/ext/language_tools'],
        advanced: {
            enableBasicAutocompletion: true,
            enableSnippets: true,
            enableLiveAutocompletion: true
        },
        mode: "csharp",
    };

    $scope.options = {};
    $scope.data = null;
    $scope.template = "";

    CrudController($scope, NgTableParams, ajax, $uibModal, "Supuesto");

    $scope.filterCondition.Nombre = "con";
    $scope.filterCondition.Descripcion = "con";
    $scope.filterCondition.Folder = "con";

    $scope.onAgregarAfter = function () {
        $scope.showSupuesto = false;
        $scope.showLoad = false;
        $scope.showCargando = false;
        $scope.script = "";
        $scope.template = "";
        $scope.selecteditem.estado = "A";
        return true;
    };

    $scope.onModificarAfter = function () {
        $scope.showSupuesto = false;
        $scope.showLoad = false;
        $scope.showCargando = false;
        $scope.script = "";
        $scope.template = "";
        return true;
    };

    $scope.onBuscarAfter = function () {
        $scope.selecteditem = null;
    }

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
});
registerController('AngularJSApp', 'appController');