var app = angular.module("myApp", ['ui.mask','compareField','ngRoute','ngAnimate', 'ngSanitize','ui.bootstrap','ui.grid','ngCart','infinite-scroll']);

app.config(function($routeProvider,$httpProvider) {
    $routeProvider
    .when("/", {
        templateUrl : "index.html"
    })
    .when("/main", {    
        templateUrl : "main.html"
    })
    .when("/movimientos", {
        templateUrl : "movimientos.html",
        controller : "MovimientosController"
    })
    .when("/canjes", {
        templateUrl : "canjes.html",
        controller : "CanjesController"
    })
    .when("/carrito", {
        templateUrl : "carrito.html",
        controller : "CarritoController"
    })
    .when("/miscanjes", {
        templateUrl : "miscanjes.html",
        controller : "MisCanjesController"
    });
});

app.provider('myApp', function () {
        this.$get = function () {
        };
});

app.run(function($rootScope,$http,ngCart) {
    $rootScope.saldo = 0.0;

    //Bandera para identificar si el usuario ya completo su registro
    $rootScope.RegistroCompletado = false;

    $rootScope.alerts = [
            { type: 'danger', msg: 'Oh snap! Change a few things up and try submitting again.' },
            { type: 'success', msg: 'Well done! You successfully read this important alert message.' }
    ];

    $rootScope.MsgAlert;
});

app.service('myApp', function($rootScope,$http,$uibModal) {
    var that = this;
    //DFS for fixing JSON references
    var elements = {}
    this.init = function(){
        
    };

    this.ShowMsgbox =function (mensaje,tipo){       
        //$rootScope.alerts.push({msg: 'Another alert!',type:'danger'});
        var $ctrl = this;

        /*console.log(mensaje);
        console.log(tipo);*/

        $rootScope.MsgAlert= $uibModal.open({
            animation: true,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'template/dialog/alert.html',
            controller: 'AlterController',
            controllerAs: '$ctrl',
            windowTopClass: '{border:0px,z-index:1200}',
            windowClass : '{border:0px,z-index:1200}',
            resolve: {
                mensaje: function(){
                    return mensaje;
                },
                tipo: function(){
                    return tipo;
                }
            }
        });
    };
    this.getSaldo = function () {
        this.WsPeticion(null,"acceso","ServiciosERP/Ventas/WSSaldosMonedero.svc/saldoUsuario",function(response){
            if(response.headers("Error")==null)
            {
                $rootScope.saldo=response.data;
            }
            else
            {
                /*$scope.msgbox("Error","danger",response.headers("Error"));
                angular.element('#myModalAlert').modal();*/
            }
            //$scope.savingfrm = false;
        });
        //$rootScope.$broadcast('myApp:getSaldo', {});
    };
    this.getVerificarRegistro = function () {
        this.WsPeticion(null,"acceso","ServiciosERP/Ventas/WSUsuariosMonedero.svc/getVerificarRegistro",function(response){
            if(response.headers("Error")==null)
            {
                $rootScope.RegistroCompletado=response.data;
            }
            else
            {
                /*$scope.msgbox("Error","danger",response.headers("Error"));
                angular.element('#myModalAlert').modal();*/
            }
        });
    };
    this.WsPeticion = function(data,parametro,url,callback) {
        //alert('{"' + parametro + '":' + JSON.stringify(data) + '}');

        var token=sessionStorage.getItem('token');

        this.ShowModalCargando();
        $http({
            method : "POST",
            data: '{"' + parametro + '":' + JSON.stringify(data) + '}',
            headers: {
               'Content-Type': 'application/json',
               'udn' : '3fe7Uj2bqhpewYMVCjFNRg==',
               'Token' : token==null ? "" : token,
               'TokenEmpresa' : "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJiZCI6IkNKTzk4MDgxMjM5MyJ9.bDTLZi_iImnFhDUatoMu_Pdm4gWNJC4YntNBO9wnVOc"
            },
            url : "http://localhost:54363/" + url
            //url : "http://172.16.5.72:8080/" + url
        }).then(function mySuccess(response) {
            if(response.status==200 )
            {
               response.data=that.parseAndResolve(JSON.stringify(response.data));
               callback(response);
               that.CloseModalCargando();
            }
        }, function myError(response) {
            alert(response.statusText);
            that.CloseModalCargando();
        });  
        //$rootScope.$broadcast('myApp:getSaldo', {});
    };
     this.convertToJSONDate=function(strDate){
        var dt = new Date(strDate);
        var newDate = new Date(Date.UTC(dt.getFullYear(), dt.getMonth(), dt.getDate(), dt.getHours(), dt.getMinutes(), dt.getSeconds(), dt.getMilliseconds()));
        return '/Date(' + newDate.getTime() + ')/';
    };
    this.parseAndResolve=function(json) {
        var refMap = {};
            return JSON.parse(json, function (key, value) {
                if (key === '$id') { 
                    refMap[value] = this;
                    // return undefined so that the property is deleted
                    return void(0);
                }

                if (value && value.$ref) { return refMap[value.$ref]; }

                return value; 
            });
    };
    this.ShowModalCargando=function(){
        if(angular.element('#ModalLoader').is(':visible')==true)
        {
            return;
        }
        angular.element('#ModalLoader').modal('show');
    };
    this.CloseModalCargando=function(){
        angular.element('#ModalLoader').modal('hide');    
    };
	
	//FUNCIÓN PARA MANDAR MENSAJES DE ERROR
    this.codigoError=function(error){
        var mensaje;
        console.log("entre");
        if (error == "101"){
            mensaje = "Usuario o contraseña inválidos";
        }
        else if(error == "102"){
            mensaje = "El usuario con el que intentas entrar no se encuentra activo, favor de ir a tu correo y continuar con el proceso de activación";
        }
        else if(error == "201"){
            mensaje = "El ticket no es válido";
        }
        else{
            return error;
        }
        return mensaje;   
    };
});

app
// allow you to format a text input field.
// <input type="text" ng-model="test" format="number" />
// <input type="text" ng-model="test" format="currency" />
.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });

            elem.bind('blur', function(event) {
                ///[^\d|\-+|\.+]/g --acepta numero negativos
                var plainNumber = elem.val().replace(/[^\d|\+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber));
            });
        }
    };
}]);
function convertToJSONDate(input){
    /*var dt = new Date(strDate);
    var newDate = new Date(Date.UTC(dt.getFullYear(), dt.getMonth(), dt.getDate(), dt.getHours(), dt.getMinutes(), dt.getSeconds(), dt.getMilliseconds()));
    return '/Date(' + newDate.getTime() + ')/';*/
    //if (typeof input !== "object") return input;

    for (var key in input) {
        alert(input[key]);
        /*if (!input.hasOwnProperty(key)) continue;
        /*var value = input[key];
        var match;
        // Check for string properties which look like dates.
        if (typeof value === "string" && (match = value.match(regexIso8601))) {
            var milliseconds = Date.parse(match[0])
            if (!isNaN(milliseconds)) {
                input[key] = new Date(milliseconds);
            }
        } else if (typeof value === "object") {
            // Recurse into object
            convertDateStringsToDates(value);
        }*/
    }
}

angular.module('myApp').controller('AlterController', function ($scope,$uibModalInstance, mensaje,tipo,$rootScope) {
    var $ctrl = this;
    $ctrl.mensaje = mensaje;
    $ctrl.tipo = tipo;
    $scope.closeAlert = function() {
        $rootScope.MsgAlert.close('a');
    };
});

app.filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});