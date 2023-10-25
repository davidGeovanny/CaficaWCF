app.controller("LoginController", ['$scope','$window','myApp', function($scope,$window,myApp) { 
	$scope.savingfrm = false;

	$scope.Login = function() {
		$scope.savingfrm = true;
		var data=$scope.acceso;
		//Valida sin las informacion del formulario es correcta
		if($scope.FrmLogin.$valid==false)
		{
			//$scope.alert="Existen valores pendientes de capturar";
			$scope.msgbox("Error","danger","Existen valores pendientes de capturar");
			angular.element('#myModalAlert').modal();
			$scope.savingfrm = false;
			return;
		}
		$scope.savingfrm = true;
		myApp.WsPeticion(data,"acceso","ServiciosERP/WSLogin.svc/LoginMonedero",function(response){
        	if(response.headers("Error")==null)
        	{
        		console.log(response.data);
        		sessionStorage.setItem('token',response.data);
				//$window.location.href = '/MonederoElectronico/main.html';
				$window.location.href = '/MonederoElectronico/main.html#!/movimientos';	
        	}
        	else
        	{
        		myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
        	}
			$scope.savingfrm = false;
    	});
    };
    $scope.Registro = function() {
		var data=$scope.usuario;
		//Valida sin las informacion del formulario es correcta
		if($scope.FrmRegistro.$valid==false)
		{
			$scope.msgbox("Error","danger","Existen valores pendientes de capturar");
			angular.element('#myModalAlert').modal();
			return;
		}
		$scope.savingfrm = true;
		myApp.WsPeticion(data,"item","ServiciosERP/Ventas/WSUsuariosMonedero.svc/add",function(response){
        	if(response.headers("Error")==null)
        	{
				angular.element('#myModalAlertSuccess').modal();
				$scope.usuario={};
				$scope.FrmRegistro.$setUntouched();
				$scope.FrmRegistro.$setPristine();
				angular.element('#myModal').modal('hide');
        	}
        	else
        	{
        		myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
        	}
			$scope.savingfrm = false;
    	});
    };
    $scope.ShowRegistro=function(){
    	angular.element('#myModal').modal('show');
    	$scope.usuario={};
		$scope.FrmRegistro.$setUntouched();
		$scope.FrmRegistro.$setPristine();
    };
    $scope.msgbox=function(tipo,type,msj){
    	//danger success error warning
    	$scope.mensaje=msj;
    	$scope.tipo=tipo;
    	$scope.typeAlerta=type;
    }
}]);