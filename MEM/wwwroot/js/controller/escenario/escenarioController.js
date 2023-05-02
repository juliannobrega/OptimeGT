
app.controller('appController', function ($scope, NgTableParams, ajax, $uibModal, $rootScope) {

    CrudController($scope, NgTableParams, ajax, $uibModal, "Escenario");

    $scope.showTableButtonVer = true;
    $scope.showButtonGuardar = false;

    $scope.filterCondition.Nombre = "con";
    $scope.filterCondition.BaseDatos = "con";
    $scope.filterObject.Estado = "T";
    $scope.filterCondition.Publico = "con";


    $rootScope.$on('OnEventBuscar', function (evt, nuevo) {
        $scope.onBuscar();
    });

    $scope.onCancelarAfter = function () {
        $scope.showButtonGuardar = false;
    };

    $scope.onAgregarAfter = function () {
        $scope.isClonar = false;
        $scope.selecteditem.estado = "A";
        return true;

    };

    $scope.onModificarBefore = function (item) {
        $scope.showButtonGuardar = true;
        return !(item.estado == "P");
    };

    $scope.onModificarAfter = function () {
        $scope.showButtonAdd = false;
        $scope.onClickMod(-1);

    };

    $scope.onVerBefore = function (item) {
        //$scope.changeMode($scope.MODO_VER);
        return (item.estado == "G");
    }

    $scope.onVerAfter = function (item) {
        $scope.changeMode($scope.MODO_VER);        
    }

    $scope.GetEstadoEscenarioNombre = function (value) {
        switch (value.toLowerCase()) {
            case "s":
                return "SIN BASE";
            case "e":
                return "EDICION";
            case "g":
                return "GENERADO";
            case "p":
                return "PROCESANDO";            
        }
    };   

    $scope.generarDatos = false;
    $scope.onGenerarEscenario = function () {
        $scope.generarDatos = true;
        if ($scope.isClonar || $scope.selecteditem.escenarioOrigenId != null) {
            if ($scope.onGuardarBefore($scope.selecteditem)) {
                $scope.onOpenDialog("Generar", "¿Está seguro de Generar el Modelo?", function () {
                    ajax.post("Escenario", "Clonar", $scope.selecteditem, function (data) {
                        if (data == "LIMITE") {
                            $scope.onOpenDialog("Error", "HA LLEGADO AL LÍMITE MÁXIMO DE ESCENARIOS CREADOS POR GRUPO ECONÓMICO", function () {                               
                            }, "sm", "modalOk.html", null, null, "fa-exclamation", "modal-danger");

                        } else {
                            $scope.onGuardarAfter();
                            $scope.onCancelar();
                        }
                    }, $scope.onDefaultReturnFault);
                }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o", "");
            }
        } else {
            $scope.onGuardar();
        }        
    };

    $scope.onEditar = function () {
        $scope.onOpenDialog("Editar", "¿Está seguro de guardar los cambios?", function () {
            ajax.post("Escenario", "Editar", $scope.selecteditem, function (data) {
                $scope.onGuardarAfter();
                $scope.onCancelar();
            }, $scope.onDefaultReturnFault);
        }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o", "");
    };

    $scope.showBuscando = false;
    $scope.onBorrarTodos = function () {
        $scope.onOpenDialog("Borrar Todos los Escenarios", "¿Está seguro de borrar todos los Escenarios?", function () {
            $scope.showBuscando = true; 
            ajax.post("Escenario", "BorrarTodos", null, function (data) {
                if (data == "OK") {
                    $scope.onOpenDialog("Borrar Escenarios", "Se han borrado todos Escenarios.", function () {
                    }, "sm", "modalOk.html", null, null, "fa-check", "");
                    $scope.showBuscando = false;
                };
                $scope.showBuscando = false;
            }, $scope.onDefaultReturnFault);
            
        }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o", "");
        
    };

    $scope.onEjecutarModeloFull = function (item) {        
        $scope.onOpenDialog("Ejecutar Modelo", "¿Quiere ejecutar el modelo? Este proceso puede demorar varios minutos", function () {
            ajax.get("Escenario", "EjecutarModeloFull", item.id, function (data) {
                $scope.onShowLog(item);
                $scope.onCancelar();
                $scope.onApply();
            }, $scope.onDefaultReturnFault);
            $scope.onApply();
        }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o");
    }

    $scope.onGenerarMods = function () {
        var item = $scope.selecteditem;
        $scope.onOpenDialog("Generar tablas Modelo", "¿Quiere generar tablas modelo? Este proceso puede demorar varios minutos", function () {
            //volver al menu
            $scope.onCancelar();
            $scope.onShowLog(item);
            ajax.get("Escenario", "GenerarMod", item.id, function (data) {                
                $scope.onApply();
            }, $scope.onDefaultReturnFault);
            $scope.onApply();
        }, "sm", "modalYesNo.html", null, null, "fa-question-circle-o");
    }

    $scope.isClonar = false;
    $scope.onClonar = function(row) {
        ajax.post("Escenario", "ControlLimite", null, function(data) {
            if (data == true) {
                $scope.onOpenDialog("Error", "HA LLEGADO AL LÍMITE MÁXIMO DE ESCENARIOS CREADOS POR GRUPO ECONÓMICO", function() {
                }, "sm", "modalOk.html", null, null, "fa-exclamation", "modal-danger");

            } else {
                $scope.isClonar = true;
                if (row.estado != "S") {
                    $scope.changeMode($scope.MODO_AGREGAR);
                    $scope.selecteditem = {};
                    $scope.selecteditem.nombre = row.nombre + " CLON";
                    $scope.selecteditem.estado = row.estado;
                    $scope.selecteditem.pluginId = row.pluginId;
                    $scope.selecteditem.fechaInicio = row.fechaInicio;
                    $scope.selecteditem.fechaFin = row.fechaFin;
                    $scope.selecteditem.semanaInicio = row.semanaInicio;
                    $scope.selecteditem.semanaFin = row.semanaFin;
                    $scope.selecteditem.escenarioOrigenId = row.id;
                    $scope.selecteditem.generarDatos = true;
                    $scope.selecteditem.escenarioBaseNombre = row.nombre;
                }
            }
        }, $scope.onDefaultReturnFault);
        
    };

    $scope.onGuardarBefore = function () {
        $scope.selecteditem.generarDatos = $scope.generarDatos;
        $scope.generarDatos = false;
        $scope.showButtonGuardar = false;
        return true;
    }

    $scope.getEscenarioBase = function () {
        ajax.get("Escenario", "GetEscenarioBase", null, function (data) {
            $scope.escenariosBase = data;
            $scope.onApply();
        }, $scope.onDefaultReturnFault);
        $scope.onApply();
    };
    $scope.getEscenarioBase();

    $scope.$watch("selecteditem.escenarioOrigenId", function (newValue, oldValue) {
        if ($scope.escenariosBase != null) {
            for (var i = 0; i < $scope.escenariosBase.length; i++) {
                if ($scope.escenariosBase[i].id == newValue) {
                    $scope.selecteditem.fechaInicio = $scope.escenariosBase[i].fechaIni;
                    $scope.selecteditem.semanaInicio = $scope.escenariosBase[i].semanaIni;
                    $scope.selecteditem.fechaFin = $scope.escenariosBase[i].fechaFin;
                    $scope.selecteditem.semanaFin = $scope.escenariosBase[i].semanaFin;
                    break;
                }
            }
        }
    });

    $scope.getPlugins = function () {
        ajax.get("Escenario", "GetPlugins", null, function (data) {
            $scope.plugins = data;
            $scope.onApply();
        }, $scope.onDefaultReturnFault);
        $scope.onApply();
    };
    $scope.getPlugins();

    //
    //grafico
    //
    $scope.onBuscarTipos = function () {
        $scope.show = false;
        $scope.selected = null;

        ajax.get("Grafico", "BuscarTipos", null, function (data) {
            $scope.tipos = data;
            $scope.onApply();
        }, $scope.onDefaultReturnFault);

        $scope.onApply();
    };
    $scope.onBuscarTipos();

    $scope.showActive = null;
   
    
    $scope.onChangeGrafico = function (id) {
        $scope.show = true;
        //limpiar template
        //$scope.GetGrafico();
    };

    $scope.GetGrafico = function (item) {   
        ajax.get("Grafico", "GetGrafico", item.id, function (data) {
            //data trae template, script, csharp
            $scope.filterObject.selected = data;
            $scope.onApply();
        }, $scope.onDefaultReturnFault);
    }

    

    $scope.showButtonSave = false;
    //showFiltros
    $scope.ChangeWidth =  function () {
        $scope.colapsar = !$scope.colapsar;
    };

    $scope.colapsarSupuesto = true;
    $scope.ChangeWidthSupuestos = function () {
        if (!$scope.colapsarMod) {
            $scope.colapsarMod = true;
        }
        $scope.colapsarSupuesto = !$scope.colapsarSupuesto;       
    };

    $scope.colapsarMod = true;
    $scope.ChangeWidthMod = function () {
        if (!$scope.colapsarSupuesto) {
            $scope.colapsarSupuesto = true;
        }
        $scope.colapsarMod = !$scope.colapsarMod;
    };

    //Graficos    

    $scope.onClickTipo = function (tipo) {
        $scope.showActive = tipo;
        $scope.onBuscarGraficos(tipo);
    };

    $scope.onBuscarGraficos = function (tipo) {
        $scope.tipo = tipo;
        ajax.get("Grafico", "BuscarGraficosPorTipo", tipo, function (data) {
            $scope.graficos = data;                 
            $scope.onChangeGrafico();
            $scope.onApply();
        }, $scope.onDefaultReturnFault);
    };
  

    //
    //Supuestos
    //
    $scope.IsShowSupuestos = function () {
        try {
            return (($scope.modo == $scope.MODO_EDITAR) && ($scope.selecteditem.estado == 'M' || $scope.selecteditem.estado == 'E'));
        }
        catch (error) {

        }
        return false;
    }

    $scope.onBuscarSupuestos = function () {
        //$scope.mods = null;
        if ($scope.selecteditem.baseDatos !== null && $scope.filterObject.GrupoId != null) {
            ajax.get("Supuesto", "BuscarSupuestos", [$scope.selecteditem.baseDatos, $scope.filterObject.GrupoId], function (data) {
                $scope.supuestos = data;
                $scope.onApply();
            }, $scope.onDefaultReturnFault);
        }
    }    

    $scope.selectdIsSup = true;
    $scope.onClickSupuesto = function (idSupuesto) {        
        $scope.selectdIsSup = true;
        $scope.GetSupuesto(idSupuesto);
    }  

    $scope.GetSupuesto = function (idSupuesto) {
        //limpia la grilla de valores        
        $scope.selectedSupuesto = null;
        $scope.script = null;
        $scope.template = null;
        $scope.codeSharp = null;
        $scope.showLoad = true;
        $scope.showCargando = true;
        ajax.get("Supuesto", "GetSupuesto", idSupuesto, function (data) {
            $scope.selectedSupuesto = data;
            $scope.selectedSupuesto.schema = $scope.selecteditem.baseDatos;
            $scope.selectedSupuesto.selectdIsSup = $scope.selectdIsSup;
            $scope.script = $scope.selectedSupuesto.scritp;
            $scope.template = $scope.selectedSupuesto.template;
            $scope.codeSharp = $scope.selectedSupuesto.codeSharp;

            $scope.showSupuesto = true;
            $scope.showLoad = false;
            $scope.showCargando = false;
        }, $scope.onDefaultReturnFault);
    }   

    $scope.onBuscarAfter = function () {
        $scope.showActive = null;
        $scope.filterObject.selected = null;
        $scope.graficos = null;               
        $scope.showSupuesto = false;
        $scope.showLoad = false;
        $scope.showCargando = false;
        $scope.show = false;
               
        $rootScope.$broadcast('UpdateGrupoInfo', null);
    }

    $scope.ActualizarSupuesto = function () {       
        $scope.onClickMod($scope.selectedSupuesto.id);
       
    };

    $scope.onClickMod = function (idSupuesto) {
        $scope.selectdIsSup = false;
        $scope.GetSupuesto(idSupuesto);       
    }

    $scope.showEdit = function (isAdmin, row) {
        return $scope.showTableButtonEdit && ((isAdmin && row.publico == "Publico") || (!isAdmin && row.publico == "Privado"))
    }


    //
    //Log
    //
    function LogModalController($scope, $uibModalInstance, $sce, data, ajax) {
        $scope.data = data;
        $scope.log = { data: [] };
        $scope.deliberatelyTrustDangerousSnippet = function (htmlText) {
            return $sce.trustAsHtml(htmlText);
        };

        $scope.onOk = function () {
            $scope.actualizar = function () { };
            $uibModalInstance.close($scope.data);
            $rootScope.$broadcast('OnEventBuscar',null);
        };

        $scope.onCancel = function () {
            $scope.isClonar = false; 
            $scope.actualizar = function () { };
            $uibModalInstance.dismiss('cancel');
        };

        $scope.getLog = function (returnfunction) {
            ajax.get("Escenario", "GetLog", $scope.data.extraData, function (data) {


                $scope.log.data = {};
                $scope.log.data = data;

                if (returnfunction != undefined && returnfunction != null)
                    returnfunction();

            }, $scope.onDefaultReturnFault);
        }

        $scope.actualizar = function () {
            setTimeout(function () {
                $scope.getLog($scope.actualizar);
            },5000);
        };

        $scope.getLog($scope.actualizar);
    }

    $scope.onShowLog = function (item) {
        $scope.onOpenDialog("Log", "", null, "xlg", "modalLog.html", item.id, LogModalController, null,".modal.in .modal-dialog .log");                    
    }

    setTimeout(function () {
        $rootScope.$broadcast('showLoadingPage', false);
    }, 500);
});
registerController('AngularJSApp', 'appController');