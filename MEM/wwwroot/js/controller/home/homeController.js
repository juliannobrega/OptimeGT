app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope, NgMap) {
    BaseController($scope, $uibModal);

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 1000);
    registerController('AngularJSApp', 'appController');
});
