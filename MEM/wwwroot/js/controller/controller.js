function isString(obj) {
    return obj !== undefined && obj != null && obj.toLowerCase !== undefined;
}

function isArray(value) {
    return (!!value) && (value.constructor === Array);
}

function isObject(value) {
    return value instanceof Object || typeof value == "object";
}

function isDate(value) {
    return value instanceof Date && !isNaN(value.valueOf());
}

function isNumber(value) {
    return (typeof value == 'number') && !isNaN(value - 0) && value !== '';
}

function isInt(value) {
    return (typeof value == 'number') && !isNaN(value - 0) && parseInt(value) == value;
}

function getDivGeneral() {
    var div = document.getElementsByClassName("content-wrapper ng-scope");
    return div;
}

function getHeightDivGeneral() {
    var div = getDivGeneral();
    return div[0].offsetHeight;
}

function round(value, decimals) {
    value = value * 1;
    value = (value === undefined || value == null || isNaN(value)) ? 0 : value
    var result = value * Math.pow(10, decimals);
    var point = result.toString().indexOf('.');
    point = point == -1 ? result.toString().length : point;
    result = (result.toString().substr(0, point) * 1);
    return result / Math.pow(10, decimals);
}

function replaceNaNNull(value, replaceValue) {
    replaceValue = replaceValue === undefined ? 0 : replaceValue;
    if (isNaN(value) || value == null || value == "")
        return replaceValue;
    return value;
}

function replaceIntoArray(array, property, item) {
    var index = inexOfArray(array, property, item[property]);
    if (index >= 0) {
        array[index] = item;
        return true;
    }
    return false;
}

function inexOfArray(array, property, value) {
    if (!isArray(array))
        array = [array];
    for (var i = 0; i < array.length; i++) {
        if (array[i][property] == value)
            return i;
    }
    return -1;
}

function removeFromArray(item, array) {
    var index = array.indexOf(item);
    array.splice(index, 1);
};

function convertToTime(value) {
    var totalSec = value / 1000;
    var hours = parseInt(totalSec / 3600) % 24;
    var minutes = parseInt(totalSec / 60) % 60;
    var seconds = parseInt(totalSec % 60);
    return (hours < 10 ? "0" + hours : hours) + ":" + (minutes < 10 ? "0" + minutes : minutes) + ":" + (seconds < 10 ? "0" + seconds : seconds);
}

function findArray(array, obj) {
    var result = [];
    for (var i = 0; i < array.length; i++) {
        var isOk = true;

        for (var key in obj) {
            if (array[i][key] != obj[key]) {
                isOk = false;
                break;
            }
        }
        if (isOk)
            result.push(array[i]);
    }
    return result;
}

function registerController(moduleName, controllerName) {
    var queue = angular.module(moduleName)._invokeQueue;
    for (var i = 0; i < queue.length; i++) {
        var call = queue[i];
        if (call[0] == "$controllerProvider" &&
            call[1] == "register" &&
            call[2][0] == controllerName) {
            app.register.controller(controllerName, call[2][1]);
        }
    }
}

angular.module('TablePaging', []);
angular.module('TablePaging').directive('ngTablePaging', function ($compile, ngTableEventsChannel) {
    "use strict";
    return {
        restrict: 'A',
        scope: { ngTablePaging: '=' },
        transclude: true,
        controller: function ($scope, $element) {
            $scope.onPrev = function () {
                $scope.ngTablePaging.page($scope.ngTablePaging.page() - 1);
            };

            $scope.onNext = function () {
                $scope.ngTablePaging.page($scope.ngTablePaging.page() + 1);
            };

            $scope.onPage = function (page) {
                $scope.ngTablePaging.page(page);
            };
        },
        link: function ($scope, $element, attrs) {

            if ($scope.ngTablePaging) {

                //$scope.ngTablePaging.onPaginReload = function (recordCount) {
                $scope.ngTablePaging.onAfterReloadData = function (ngDataBuscar) {

                    var template = '';

                    template = template + '<tr><td colspan="500"><div class="col-sm-12" style="text-align:left;">'
                    template = template + '<ul class="pagination pagination-sm" style="margin:0px;">';

                    var buttons = $scope.ngTablePaging.generatePagesArray(ngDataBuscar.pageIndex, ngDataBuscar.recordCount, ngDataBuscar.pageSize, 10);

                    var number = -1;

                    for (var i = 0; i < buttons.length; i++) {
                        //buttons[i].type
                        //buttons[i].number
                        //buttons[i].current
                        //buttons[i].active
                        switch (buttons[i].type) {
                            case "prev":
                                template = template + '<li><a href="" ng-click="onPage(' + buttons[i].number + ')"><i class="fa fa-chevron-circle-left" aria-hidden="true"></i></a></li>'
                                break;
                            case "first":
                            case "page":
                            case "last":
                                number = buttons[i].number;
                                if (buttons[i].current)
                                    template = template + '<li class="active" ><a href="" ng-click="onPage(' + buttons[i].number + ')">' + buttons[i].number + ' <span class="sr-only"></span></a></li>'
                                else
                                    template = template + '<li ><a href="" ng-click="onPage(' + buttons[i].number + ')">' + buttons[i].number + ' <span class="sr-only"></span></a></li>'
                                break;
                            case "more":
                                if (i == 2)
                                    template = template + '<li ><a href="" ng-click="onPage(' + (buttons[i + 1].number - 1) + ')"><i class="fa fa-ellipsis-h" aria-hidden="true"></i><span class="sr-only"></span></a></li>'
                                else
                                    template = template + '<li ><a href="" ng-click="onPage(' + (number + 1) + ')"><i class="fa fa-ellipsis-h" aria-hidden="true"></i><span class="sr-only"></span></a></li>'
                                break;
                            case "next":
                                template = template + '<li ><a href="" ng-click="onPage(' + buttons[i].number + ')"><i class="fa fa-chevron-circle-right" aria-hidden="true"></i></a></li>'
                                break;
                        }
                    }

                    $scope.recordCount = ngDataBuscar.recordCount;

                    template = template + '</ul>';
                    template = template + '<div class="pull-left" id="DivModulos"></div>';
                    template = template + '<div class="pull-right"><b class="pull-right">Registros : ' + ngDataBuscar.recordCount + '</b></div>';
                    template = template + '</div></td></tr>';

                    var newElement = angular.element(template);
                    $compile(newElement)($scope);
                    $element.html(newElement);
                };
            }
        }
    };
});

if (componentes === undefined || componentes == null) {
    componentes = [];
}

componentes.push('ui.bootstrap');
componentes.push('ngSanitize');
componentes.push('ngTable');
componentes.push('ui.select');
componentes.push('TablePaging');
componentes.push('ui.bootstrap');
componentes.push('ngRoute');
componentes.push('ngMap');
componentes.push('nvd3');
componentes.push('ui.ace');


var app = angular.module('AngularJSApp', componentes);

app.config(function ($controllerProvider, $compileProvider, $filterProvider, $provide) {
    app.register = {
        controller: $controllerProvider.register,
        directive: $compileProvider.directive,
        filter: $filterProvider.register,
        factory: $provide.factory,
        service: $provide.service
    };
});

app.service('ajax', function ($http, $rootScope) {
    var self = this;
    self.foulRedirect = function (response, returnFault) {       
        if (response.data.data == "Redirected") {
            window.location.href = config.baseURL;
        } else {
            returnFault(response);
        }
    }

    self.post = function (controller, method, senderObject, returnResult, returnFault) {
        $http.post(config.baseURL + controller + '/' + method  + "?t=" + (new Date()).getTime(), senderObject).then(
            function (response) { //success
                if (returnResult !== undefined && returnResult != null) {
                    if (response.data.isError == false)
                        returnResult(response.data.data);
                    else
                        self.foulRedirect(response, returnFault);
                }
            }, function (response) { //Error
                if (returnFault !== undefined && returnFault != null)
                    returnFault("Se ha producido un error en el sistema, por favor contacte con el administrador.");
            });
    };

    self.postUpload = function (controller, method, senderObject, file, returnResult, returnFault) {
        var dataSend = new FormData();
        dataSend.append("JsonOject", JSON.stringify(senderObject));
        dataSend.append("FileSend", file[0], file[0].fileName);

        $http.post(config.baseURL + controller + '/' + method, dataSend, {
            headers: {
                "Content-Type": undefined,
            }
        }).then(
            function (response) { //success
                if (returnResult !== undefined && returnResult != null) {
                    if (response.data.isError == false)
                        returnResult(response.data.data);
                    else
                        self.foulRedirect(response, returnFault);
                }
            }, function (response) { //Error
                if (returnFault !== undefined && returnFault != null)
                    returnFault(response);
            });
    };

    self.postDownloadFile = function (controller, method, senderObject, fileName, fileType, returnResult, returnFault) {
        $http.post(config.baseURL + controller + '/' + method, senderObject, { responseType: 'arraybuffer' }).then(
            function (response) { //success
                if (returnResult !== undefined && returnResult != null) {
                    var a = document.createElement("a");
                    var file = new Blob([response.data], { type: fileType });
                    if (window.navigator.msSaveOrOpenBlob) // IE10+
                        window.navigator.msSaveOrOpenBlob(file, fileName);
                    else { // Others
                        var url = URL.createObjectURL(file);
                        a.href = url;
                        a.download = fileName;
                        document.body.appendChild(a);
                        a.click();
                        setTimeout(function () {
                            document.body.removeChild(a);
                            window.URL.revokeObjectURL(url);
                        }, 0);
                    }
                    returnResult(response.data);                   

                }
            }, function (response) { //Error
                if (returnFault !== undefined && returnFault != null)
                    returnFault(response);
            });
    };

    self.get = function (controller, method, senderObject, returnResult, returnFault) {
        var myURL = config.baseURL + controller + '/' + method;
        if (senderObject != null && senderObject != "") myURL += '/' + senderObject;
        //$http.get(config.baseURL + controller + '/' + method + '/' + senderObject).then(
        $http.get(myURL + "?t=" + (new Date()).getTime()).then(
            function (response) { //success
                if (returnResult !== undefined && returnResult != null) {
                    if (response.data.isError == false)
                        returnResult(response.data.data);
                    else
                        self.foulRedirect(response, returnFault);
                }
            }, function (response) { //Error
                if (returnFault !== undefined && returnFault != null)
                    returnFault(response);
            });
    };

});

app.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter, { 'event': event });
                });

                event.preventDefault();
            }
        });
    };
});

app.directive('isNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            scope.$watch(attrs.ngModel, function (newValue, oldValue) {

                var arr = String(newValue).split("");
                if (arr.length === 0) return;
                if (arr.length === 1 && (arr[0] == '-' || arr[0] === '.')) return;
                if (arr.length === 2 && newValue === '-.') return;
                if (isNaN(newValue)) {
                    var scopeVar = scope;
                    var a = attrs.ngModel.split(".");
                    for (i = 0; i < a.length - 1; i++) {
                        if (scopeVar === undefined || scopeVar == null || newValue == null) {
                            return;
                        }
                        scopeVar = scopeVar[a[i]];
                    }
                    scopeVar[a[a.length - 1]] = oldValue;
                }
            });
        }
    };
});

app.directive('isHexa', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            scope.$watch(attrs.ngModel, function (newValue, oldValue) {
                var ChartsPermit = '0123456789abcdefABCDEF'.split("");
                var arr = String(newValue).split("");
                if (arr.length === 0) return;
                var isFail = false;
                for (var i = 0; i < arr.length; i++) {
                    if (ChartsPermit.indexOf(arr[i]) == -1) {
                        isFail = true;
                        break;
                    }
                }
                if (isFail) {
                    var scopeVar = scope;
                    var a = attrs.ngModel.split(".");
                    for (i = 0; i < a.length - 1; i++) {
                        if (scopeVar === undefined || scopeVar == null || newValue == null) {
                            return;
                        }
                        scopeVar = scopeVar[a[i]];
                    }
                    scopeVar[a[a.length - 1]] = oldValue;
                }
            });
        }
    };
});

app.directive('isText', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            scope.$watch(attrs.ngModel, function (newValue, oldValue) {
                var ChartsPermit = '0123456789-/*+{}[]()\\!?¿¡_.:,;'.split("");
                var arr = String(newValue).split("");
                if (arr.length === 0) return;
                var isFail = false;
                for (var i = 0; i < arr.length; i++) {
                    if (ChartsPermit.indexOf(arr[i]) != -1) {
                        isFail = true;
                        break;
                    }
                }
                if (isFail) {
                    var scopeVar = scope;
                    var a = attrs.ngModel.split(".");
                    for (i = 0; i < a.length - 1; i++) {
                        if (scopeVar === undefined || scopeVar == null || newValue == null) {
                            return;
                        }
                        scopeVar = scopeVar[a[i]];
                    }
                    scopeVar[a[a.length - 1]] = oldValue;
                }
            });
        }
    };
});

function BaseController($scope, $uibModal) {
    $scope.onApply = function () {
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };

    $scope.modalInstance = null;

    $scope.CloseDialog = function () {
        try {
            if ($scope.modalInstance !== null)
                $scope.modalInstance.hide();
        }
        catch (err) {

        }
    };

    function defaultModalController($scope, $uibModalInstance, $sce, data) {
        $scope.data = data;

        $scope.deliberatelyTrustDangerousSnippet = function (htmlText) {
            return $sce.trustAsHtml(htmlText);
        };

        $scope.onOk = function () {
            $uibModalInstance.close($scope.data);
        };

        $scope.onCancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }

    $scope.onOpenDialog = function (titulo, mensaje, returnFunction, size, templateUrl, extraData, controller, icono, claseColor) {
        size = (size == undefined || size == null) ? 'lg' : size; //'lg' 'md' 'sm'
        templateUrl = (templateUrl === undefined || templateUrl == null) ? "modalOkCancel.html" : templateUrl; //"modalOkCancel.html" "modalOk.html" "modalYesNo.html"
        returnFunction = (returnFunction == undefined || returnFunction == null) ? function (result) { } : returnFunction;
        controller = (controller == undefined || controller == null) ? defaultModalController : controller;
        claseColorModal = (claseColor == undefined || claseColor == null) ? "" : claseColor;
        aplicarOutline = (claseColorModal != "") ? true : false;
        iconoTituloModal = (icono == undefined || icono == null) ? "" : icono;

        ///registerController('AngularJSApp', controller);

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: templateUrl,
            controller: controller,
            size: size,
            windowClass: claseColorModal,
            resolve: {
                data: function () {
                    return { titulo: titulo, mensaje: mensaje, extraData: extraData, iconoTituloModal: iconoTituloModal, aplicarOutline: aplicarOutline };
                }
            }
        });

        modalInstance.result.then(function (result) {
            returnFunction(result);
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.GetEstadoNombre = function (value) {
        switch (value.toLowerCase()) {
            case "a":
                return "ACTIVO";
                break;
            case "b":
                return "BORRADO";
                break;
            case "d":
                return "DESACTIVADO";
                break;
            case "p":
                return "PROCESANDO";
                break;
            case "s":
                return "SI";
                break;
            case "n":
                return "NO";
                break;
            default:
                return "SIN ESTADO";
                break;
        }
    };

    $scope.onCopiarAlPortapapeles = function (texto, titulo) {
        var aux = document.createElement("input");
        aux.setAttribute("value", texto);
        document.body.appendChild(aux);
        aux.select();
        document.execCommand("copy");
        document.body.removeChild(aux);

        $scope.onOpenDialog(titulo, "Se ha copiado exitosamente el siguiente texto al portapapeles <strong>" + texto + "</strong>", null, "md", "modalOk.html", null, null, "fa-clipboard", "modal-info");
    }

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

    $.AdminLTE.layout.fix();
};

function CrudController($scope, NgTableParams, ajax, $uibModal, controllerName) {
    $scope.popup = {};
    $scope.dateOptions = {
        startingDay: 1
    };

    $scope.format = 'dd/MM/yyyy';
    $scope.altInputFormats = ['d!/M!/yyyy'];

    BaseController($scope, $uibModal);

    $scope.selecteditem = {};

    $scope.MODO_BUSCAR = "buscar";
    $scope.MODO_EDITAR = "editar";
    $scope.MODO_AGREGAR = "agregar";
    $scope.MODO_VER = "ver";

    $scope.showButtonAdd = false;
    $scope.showButtonSave = false;
    $scope.showButtonCancel = false;
    $scope.showButtonFiltro = false;
    $scope.showButtonBorrarTodo = false;


    $scope.showTableButtonEdit = true;
    $scope.showTableButtonDelete = true;
    $scope.showTableButtonVer = false;

    $scope.changeMode = function (mode) {
        $scope.modo = mode;

        $scope.showButtonAdd = false;
        $scope.showButtonSave = false;
        $scope.showButtonCancel = false;
        $scope.showButtonFiltro = false;
        $scope.showButtonBorrarTodo = false;

        switch ($scope.modo) {
            case $scope.MODO_BUSCAR:
                $scope.showButtonAdd = true;
                $scope.showButtonBorrarTodo = true;
                break;
            case $scope.MODO_EDITAR:
            case $scope.MODO_AGREGAR:
                $scope.showButtonSave = true;
                $scope.showButtonCancel = true;
                break;
            case $scope.MODO_VER:
                //solo lo usa grafico
                $scope.showButtonFiltro = true;
                $scope.showButtonCancel = true;
                break;
        }
        $scope.onApply();
    };

    $scope.errorObject = {};

    $scope.filterCondition = { Estado: '=|T' };
    $scope.filterObject = { Estado: 'A' };

    if ($scope.tableDefaultPageCount === undefined)
        $scope.tableDefaultPageCount = 25;

    if ($scope.tableDefaultSorting === undefined)
        $scope.tableDefaultSorting = {};

    if ($scope.tableDefaultFilter === undefined)
        $scope.tableDefaultFilter = {};

    $scope.tableBuscar = new NgTableParams({
        page: 1,                                // show first page
        count: $scope.tableDefaultPageCount,    // count per page
        sorting: $scope.tableDefaultSorting,    // sort
        filter: $scope.tableDefaultFilter       // filter
    }, {
            counts: [],                 // hide page counts control
            total: 1,
            getData: function (params) {
                var result = [];

                var page = new Paging({
                    pageIndex: params.page(),
                    pageSize: params.count(),
                    //pageFilter: params.filterby()
                    //pageOrder: params.orderBy()
                });

                page.filter = $scope.GetFilters();

                page.order = [];
                for (var i = 0; i < params.orderBy().length; i++) {
                    page.order.push({ Property: params.orderBy()[i].substring(1), Direction: params.orderBy()[i].substring(0, 1) })
                }

                if ($scope.onBuscarBefore(page)) {
                    ajax.post(controllerName, "Buscar", page, function (data) {

                        $scope.onBuscarAfter(data);

                        for (var i = 0; i < data.data.length; i++)
                            result.push(data.data[i]);

                        params.total(0);//(data.recordCount); Estoes para que no aparezca la otra paginación.

                        $scope.tableBuscar.onAfterReloadData(data); //Refresca la paginación

                    }, $scope.onDefaultReturnFault);

                    return result;
                }
            }
        });

    $scope.GetFilters = function () {
        var result = [];
        for (var key in $scope.filterObject) {
            if ($scope.filterObject[key] > '') { //&& (key != 'Estado' || (key == 'Estado' && $scope.filterObject[key] != 'T'))
                result.push({ Property: key, Condition: $scope.filterCondition[key], Value: $scope.filterObject[key] })
            }
        }
        return result;
    }

    $scope.onBuscar = function () {
        $scope.tableBuscar.reload();
    };

    $scope.onAgregar = function () {
        if ($scope.onAgregarBefore()) {
            $scope.changeMode($scope.MODO_AGREGAR);
            $scope.selecteditem = {};
            $scope.onAgregarAfter();
        }
    };

    $scope.onModificar = function (item) {
        if ($scope.onModificarBefore(item)) {
            $scope.changeMode($scope.MODO_EDITAR);
            $scope.selecteditem = item;
            $scope.onModificarAfter();
        }
    };

    $scope.onVer = function (item) {
        if ($scope.onVerBefore(item)) {
            $scope.changeMode($scope.MODO_VER);
            $scope.selecteditem = item;
            //$scope.onVerAfter();
        }
    };

    $scope.onBorrar = function (item) {
        if ($scope.onBorrarBefore(item)) {
            $scope.onOpenDialog("Borrar", "¿Realmente desea Borrar este item?", function () {
                ajax.post(controllerName, "Borrar", item, function (data) {
                    $scope.onCancelar();
                }, $scope.onDefaultReturnFault);
            }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o", "modal-danger");
        }
    };

    $scope.onGuardar = function () {
        if ($scope.onGuardarBefore($scope.selecteditem)) {
            $scope.onOpenDialog("Guardar", "¿Está seguro de Guardar?", function () {
                ajax.post(controllerName, "Guardar", $scope.selecteditem, function (data) {
                    $scope.onGuardarAfter();
                    $scope.onCancelar();
                }, $scope.onDefaultReturnFault);
            }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o", "");
        }
    };

    $scope.onCancelar = function () {
        $scope.changeMode($scope.MODO_BUSCAR);
        $scope.onBuscar();
        $scope.errorObject = {};
        $scope.onCancelarAfter();
    };

    $scope.onDefaultReturnFault = function (data) {
        $scope.showLoad = false;
        $scope.showCargando = false;
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
        if (popup == false) $scope.onOpenDialog("Error", "Se ha producido un error. Comuníquese con el administrador.", null, null, "modalOk.html", null, null, "fa-exclamation", "modal-danger");
    };

    $scope.hasError = function (value) {
        if ($scope.errorObject !== undefined)
            return $scope.errorObject[value] > ''
        return false;
    }

    $scope.errorClass = function (value) {
        return $scope.hasError(value) ? 'has-error' : '';
    }

    $scope.errorRemove = function (value) {
        if ($scope.errorObject !== undefined)
            $scope.errorObject[value] = '';
    }

    $scope.errorMessage = function (value) {
        if ($scope.errorObject !== undefined)
            return $scope.errorObject[value];
        return '';
    }

    $scope.onAgregarBefore = function () { return true; };
    $scope.onAgregarAfter = function () { return true; };
    $scope.onModificarBefore = function (item) { return true; };
    $scope.onModificarAfter = function () { return true; };
    $scope.onGuardarBefore = function (item) { return true; };
    $scope.onGuardarAfter = function () { return true; };
    $scope.onBorrarBefore = function (item) { return true; };
    $scope.onBorrarAfter = function () { return true; };
    $scope.onBuscarBefore = function () { return true; };
    $scope.onBuscarAfter = function () { return true; };
    $scope.onVerBefore = function (item) { return true; };
    $scope.onVerAfter = function () { return true; };
    $scope.onCancelarAfter = function () { return true; };

    $scope.changeMode($scope.MODO_BUSCAR);
    
    $scope.ShowAdvanceSearch = false;
    $scope.onClickShowSearch = function () {
        $scope.ShowAdvanceSearch = !$scope.ShowAdvanceSearch;
        return $scope.ShowAdvanceSearch;
    };

    $scope.ShowRightMenu = false;
    $scope.onClickShowRightMenu = function () {
        $scope.ShowRightMenu = !$scope.ShowRightMenu;
        return $scope.ShowRightMenu;
    };
}

var Paging = function (value) {
    var self = this;
    value = (value == null || value === undefined) ? {} : value;

    self.pageIndex = value.pageIndex === undefined ? null : value.pageIndex;
    self.pageSize = value.pageSize === undefined ? null : value.pageSize;
    self.pageCount = value.pageCount === undefined ? null : value.pageCount;
    self.recordCount = value.recordCount === undefined ? null : value.recordCount;
    self.filter = value.filter === undefined ? null : value.filter;
    self.order = value.order === undefined ? null : value.order;
    self.data = value.data === undefined ? null : value.data;
};

function isArray(value) {
    return (!!value) && (value.constructor === Array);
}

setTimeout(function () {
    $(".wrapper").removeClass("hidden");
    $("body").removeClass("hidden");
    setTimeout(function () {
        $.AdminLTE.layout.fix();
    }, 200);
}, 200);