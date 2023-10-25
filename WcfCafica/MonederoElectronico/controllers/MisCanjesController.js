app.controller("MisCanjesController", ['$scope','$window','myApp', function($scope,$window,myApp) { 
	$scope.savingfrm = false;

	$scope.init = function() {
        myApp.getSaldo();
        
        myApp.WsPeticion(null,null,"ServiciosERP/Ventas/WSSolicitudesCanjeMonedero.svc/getSolicitudesWebXUsuario",function(response){
            if(response.headers("Error")==null)
            {
                $scope.LstCanjes=response.data
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
            $scope.savingfrm = false;
        });
    };
    $scope.AbrirCanje=function(canje){
        if(canje.status === undefined)
            return;
        if(canje.status.isFirstOpen==true && canje.canje.SolicitudesCanjeMonederoDetalles.length == 0)
        {
            //console.log(canje.status.isFirstOpen);
            myApp.WsPeticion(canje.canje.Id,"id","ServiciosERP/Ventas/WSSolicitudesCanjeMonedero.svc/getSolicitudesDetallesWebXUsuario",function(response){
                if(response.headers("Error")==null)
                {
                    //$scope.LstCanjes.filter(x => x.Id === canje.canje.Id).map(x => x.SolicitudesCanjeMonederoDetalles)=response.data;
                    //console.log(response.data);
                    $scope.LstCanjes[canje.$index].SolicitudesCanjeMonederoDetalles=response.data;
                }
                else
                {
                    myApp.ShowMsgbox(response.headers("Error"),"danger");
                }
                $scope.savingfrm = false;
            });
        }
    };

}]);