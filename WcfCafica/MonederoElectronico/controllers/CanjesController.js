app.controller("CanjesController", ['$scope','$window','myApp', function($scope,$window,myApp) { 
	$scope.savingfrm = false;
	$scope.Premios= [];
	$scope.CargandoPremios = false;
	
	$scope.init = function() {
        
		if ($scope.CargandoPremios) return;
        
		$scope.CargandoPremios = true;
		
		myApp.WsPeticion($scope.Premios.length,"index","ServiciosERP/Ventas/WSPremiosMonedero.svc/premiosVigentes",function(response){
        	if(response.headers("Error")==null)
        	{
        		myApp.getSaldo();
				var items=response.data;
                if(items.length==0) 
                {
					$scope.CargandoPremios=true;
                    return;
                }
                for (var i = 0; i < items.length; i++) {
                     $scope.Premios.push(items[i]);
                }
        	}
        	else
        	{
                myApp.ShowMsgbox(response.headers("Error"),"danger");
        	}
			$scope.savingfrm = false;
			$scope.CargandoPremios=false;
    	});
    };
}]);