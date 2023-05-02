var rp = null;

if ($(".main-sidebar").length === 1) {
    app.controller('mainSidebarController', function ($scope, ajax, $sce, $uibModal, $route, $routeParams, $location) {

        BaseController($scope, $uibModal);

        $scope.deliberatelyTrustDangerousSnippet = function (htmlText) {
            return $sce.trustAsHtml(htmlText);
        };

        $scope.menues = [];
        ajax.post("Menu", "Buscar", null, function (data) {
            
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data[i].child.length; j++) {                    
                    rp.when('/' + data[i].child[j].menuUrl, {
                        templateUrl: config.baseURL + data[i].child[j].menuUrl
                    })
                }
            }

            rp.when('/', {
                templateUrl: config.baseURL + "/Escenario/Index"
            })

            $scope.menues = data;

            if (window.location.href.indexOf('#') == -1) {
                window.location.href = config.baseURL + '#!/';
            }
            else if (window.location.href.substr(window.location.href.length - 1, 1) != "/") {
                window.location.href = window.location.href + "/";
            } else {
                window.location.href = config.baseURL;
            }
        });

        $scope.OnClick = function (item) {
            for (var i = 0; i < $scope.menues.length; i++) {
                for (var j = 0; j < $scope.menues[i].child.length; j++) {
                    $scope.menues[i].child[j].selected = false;
                }
            }
            item.selected = true;
        };

        $scope.getPath = function (value) {            
            return '#!' + value;
        }

        setTimeout(function () {
            $(".main-sidebar").removeClass("hidden");
        }, 1000);

        $scope.$route = $route;
        $scope.$location = $location;
        $scope.$routeParams = $routeParams;
    });
}

app.directive("ngGrafico", function ($compile, ajax, $uibModal) {
    "use strict";
    return {
        restrict: 'E',
        scope: {
            template: '=',
            script: '=',
            schema: '=',
            item: '=',
            nodo: '='
        },
        transclude: true,
        controller: function ($scope, ajax, $element, $uibModal) {

            BaseController($scope, $uibModal);

            $scope.onDefaultReturnFault = function (data) {
                var popup = false;
                if (data.data != null && data.data.isError !== undefined && data.data.isError) {
                    if (isArray(data.data.data)) {
                        angular.forEach(data.data.data, function (item) {
                            $scope.errorObject[item.memberNames[0]] = item.errorMessage;
                        });
                        return;
                    }
                    else if (isString(data.data.data)) {
                        popup = true;
                        $scope.onOpenDialog("Error", JSON.stringify(data.data.data), null, null, "modalOk.html", null, null, "fa-exclamation", "modal-danger");
                    }

                }
                if (popup == false) $scope.onOpenDialog("Error", JSON.stringify(data), null, null, "modalOk.html", null, null, "fa-exclamation", "modal-danger");
            };

            $scope.$watch("script", function (newValue, oldValue) {
                if (newValue == null || newValue === undefined)
                    return;
                eval(newValue);
            });
        },
        link: function ($scope, $element, attrs) {

            var newElement = angular.element($scope.template);
            $compile(newElement)($scope);
            $element.html(newElement);

            $scope.$watch("script", function (newValue, oldValue) {
                var newElement = angular.element($scope.template);
                $compile(newElement)($scope);
                $element.html(newElement);
            });

            $scope.$watch("template", function (newValue, oldValue) {
                var newElement = angular.element($scope.template);
                $compile(newElement)($scope);
                $element.html(newElement);
            });
        }
    };
});

app.directive("ngTsc", function ($compile, ajax, $uibModal) {
    "use strict";
    return {
        restrict: 'E',
        scope: {
            template: '=',
            script: '=',
            item: '=',
        },
        transclude: true,
        controller: function ($scope, ajax, $element, $uibModal) {

            BaseController($scope, $uibModal);

            $scope.onDefaultReturnFault = function (data) {
                $scope.showBuscando = false;
                var popup = false;
                if (data.data != null && data.data.isError !== undefined && data.data.isError) {
                    if (isArray(data.data.data)) {
                        angular.forEach(data.data.data, function (item) {
                            $scope.errorObject[item.memberNames[0]] = item.errorMessage;
                        });
                        return;
                    }
                    else if (isString(data.data.data)) {
                        popup = true;
                        $scope.onOpenDialog("Error", JSON.stringify(data.data.data), null, null, "modalOk.html", null, null, "fa-exclamation", "modal-danger");
                    }

                }
                if (popup == false) $scope.onOpenDialog("Error", JSON.stringify(data), null, null, "modalOk.html", null, null, "fa-exclamation", "modal-danger");
            };

            $scope.$watch("script", function (newValue, oldValue) {
                if (newValue == null || newValue === undefined)
                    return;
                try {
                    eval(newValue);
                } catch (e) {
                    console.log(e)
                }
            });
        },
        link: function ($scope, $element, attrs) {

            var newElement = angular.element($scope.template);
            $compile(newElement)($scope);
            $element.html(newElement);

            $scope.$watch("script", function (newValue, oldValue) {
                var newElement = angular.element($scope.template);
                $compile(newElement)($scope);
                $element.html(newElement);
            });

            $scope.$watch("template", function (newValue, oldValue) {
                var newElement = angular.element($scope.template);
                $compile(newElement)($scope);
                $element.html(newElement);
            });
        }
    };
});

app.controller('LoadingPageController', function ($scope) {
    BaseController($scope, null);
    $scope.showCargando = true;
    $scope.$on('showLoadingPage', function (newValue, oldValue) {
        $scope.showCargando = oldValue;
        $scope.onApply();
    });
});

app.controller('GrupoInfoController', function ($scope, ajax) {
    BaseController($scope, ajax);
    //$scope.showCargando = true;    
    $scope.$on('UpdateGrupoInfo', function (event, item) {
        $scope.OnGetGrupoInfo();
        $scope.onApply();
    });


    $scope.ShowRightMenuGrupo = false;
    $scope.onClickShowRightMenuGrupo = function () {
        $scope.ShowRightMenuGrupo = !$scope.ShowRightMenuGrupo;
        return $scope.ShowRightMenuGrupo;
    }; 
    $scope.showGrupoInfo = false;
    $scope.OnGetGrupoInfo = function () {
        ajax.get("GrupoEmpresario", "GetGrupoInfo", null, function (data) {            
            if (data.grupo != null) {                
                $scope.showGrupoInfo = true;
                $scope.grupoInfoNombre = data.grupo.nombre;
                $scope.grupoInfoUsuarios = data.usuarios;
                $scope.grupoInfoLimite = data.countEscenarios + "/" + data.grupo.limite;
            }
        });
    };
    $scope.OnGetGrupoInfo();
});

app.config(function ($routeProvider, $locationProvider) {
    rp = $routeProvider;
});