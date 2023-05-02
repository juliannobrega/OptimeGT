app.controller('appController', function ($scope, ajax, $uibModal) { 

    $scope.showLoginBox = true;
    try {
        var es_chrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
        if (!es_chrome) {
            $scope.showLoginBox = false;
            throw new Error("Navegador no soportado. Utilice Google Chrome.");
        }
    } catch (e) {
        alert("Navegador no soportado. Utilice Google Chrome.");
    }

    if (es_chrome) {
        BaseController($scope, $uibModal);

        $scope.data = { UsuarioNombre: "", UsuarioClave: "", ClaveNueva: "", ErrorShow: false };

        $scope.showLogin = true;
        $scope.showErrorMsj = false;
        $scope.showBuscando = false;

        $scope.onLogin = function () {
            $scope.data.ErrorShow = false;
            $scope.showErrorMsj = false;
            //check claves
            if ($scope.showLogin == false) {
                if (!$scope.data.ClaveNueva) {
                    $scope.data.ErrorShow = true;
                    return $scope.ErrorMsj = "La clave no puede ser nula";
                }
                if (!$scope.data.ClaveNuevaChequed) {
                    $scope.data.ErrorShow = true;
                    return $scope.ErrorMsj = "Reescribir Clave no puede ser nula";
                }
                if ($scope.data.ClaveNueva != $scope.data.ClaveNuevaChequed) {
                    $scope.data.ErrorShow = true;
                    return $scope.ErrorMsj = "Las claves deben coincidir";
                }
                var formato = $scope.data.ClaveNueva.match(/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])([a-zA-Z0-9]{8,})$/);
                if (!formato) {
                    $scope.data.ErrorShow = true;
                    return $scope.showErrorMsj = true;
                }
            }
            $scope.ErrorMsj = "Usuario o Contraseña incorrecta. Verifique sus credenciales";
            $scope.data.ErrorShow = false;//Con esto, en la carga inicial se sigue viendo y luego desaparece
            var result = ajax.post("Login", "Login", { usuario: $scope.data.UsuarioNombre, clave: $scope.data.UsuarioClave, claveNew: $scope.data.ClaveNueva },
                function (data) {
                    if (data != null) {
                        if (data == "clave") {
                            //clave nueva
                            return $scope.showLogin = false;
                        } else if (data == "perfil") {
                            //no tiene perfil activo
                            $scope.data.ErrorShow = true;
                            return $scope.ErrorMsj = "Perfil de Acceso Inactivo. Comuníquese con el Administrador.";
                        } else {
                            $("body").addClass("hidden");
                            window.location.href = config.baseURL;
                        }
                    }
                    else {
                        $scope.ErrorMsj = "Usuario o Contraseña incorrecta. Verifique sus credenciales";
                        $scope.data.ErrorShow = true;
                    }
                },
                function (data) {
                    $scope.ErrorMsj = "Usuario o Contraseña incorrecta. Verifique sus credenciales";
                    $scope.data.ErrorShow = true;
                });
        }

        $scope.recuperar = { UsuarioEmail: "" };
        $scope.onRecuperarClave = function () {
            $scope.onOpenDialog("", "", function () {
                $scope.showBuscando = true;
                var result = ajax.get("Login", "RecuperarClave", $scope.recuperar.UsuarioEmail,
                    function (data) {
                        if (data == true) {
                            $scope.onOpenDialog("Envio correcto", "Revise su correo", function () {
                            }, "sm", "modalOk.html");
                            $scope.showBuscando = false;
                        }
                        else {
                            $scope.onOpenDialog("Envio Incorrecto", "Revise los datos ingresados", function () {
                            }, "sm", "modalOk.html");
                            $scope.showBuscando = false;
                        }
                    },
                    function (data) {
                        $scope.onOpenDialog("Envio Incorrecto", "Revise los datos ingresados", function () {
                        }, "sm", "modalOk.html");
                        $scope.showBuscando = false;
                    });

            }, "sm", "modalRecuperarClave.html", $scope.recuperar, null, null, null);
            $scope.showBuscando = false;
        }
    }
    
});