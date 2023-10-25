app.controller("RegistroController", ['$scope','myApp','$uibModalInstance', function($scope,myApp,$uibModalInstance) { 
	$scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[1];
    $scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

	$scope.init = function() {  

        myApp.WsPeticion(1,"id","ServiciosERP/Generales/WSEstados.svc/getEstadosPais",function(response){
            if(response.headers("Error")==null)
            {
                $scope.Colonias = [];
                $scope.Estados = response.data;
                myApp.WsPeticion(null,null,"ServiciosERP/Ventas/WSUsuariosMonedero.svc/getUsuarioWeb",function(response){
                        if(response.headers("Error")==null)
                        {
                            $scope.usuario = response.data;
                            $scope.CargarMunicipios();
                        }
                        else
                        {
                            myApp.ShowMsgbox(response.headers("Error"),"danger");
                        }
                        
                    });
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
    
        });
    };
    $scope.cerrar=function(){
        $uibModalInstance.dismiss('cancel');
    }
    $scope.CargarMunicipios = function(){
        $scope.Ciudades = [];
        if($scope.usuario.EstadoId==null)
            return;
        myApp.WsPeticion($scope.usuario.EstadoId,"id","ServiciosERP/Generales/WSMunicipios.svc/getMunicipiosEstado",function(response){
            if(response.headers("Error")==null)
            {
                console.log("inicio");
                $scope.Municipios = response.data;
                $scope.CargarLocalidades();
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
    
        });
    };

    $scope.CargarLocalidades = function(){
        $scope.CodigosPostales = [];
        if($scope.usuario.MunicipioId==null)
            return;
        myApp.WsPeticion($scope.usuario.MunicipioId,"id","ServiciosERP/Generales/WSCiudades.svc/getCiudadesMunicipio",function(response){
            if(response.headers("Error")==null)
            {
                $scope.Ciudades = response.data;
                $scope.CargarCodigosPostales();
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
    
        });          
    };

    $scope.CargarCodigosPostales = function(){
        $scope.Colonias = [];
        if($scope.usuario.CiudadId==null)
            return;
        myApp.WsPeticion($scope.usuario.CiudadId,"id","ServiciosERP/Generales/WSCodigosPostales.svc/getCodigosPostalesCiudad",function(response){
            if(response.headers("Error")==null)
            {
                $scope.CodigosPostales = response.data;
                $scope.CargarColonias();
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
    
        });           
    };

    $scope.CargarColonias = function(){
        if($scope.usuario.CodigoPostalId==null)
            return;
        myApp.WsPeticion($scope.usuario.CodigoPostalId,"id","ServiciosERP/Generales/WSColonias.svc/getColoniasCP",function(response){
            if(response.headers("Error")==null)
            {
                $scope.Colonias = response.data;
                console.log("fin");
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
    
        });   
    };

    $scope.guardar = function(){
        var data=$scope.usuario;
        //Valida sin las informacion del formulario es correcta
        if($scope.FrmRegistro.$valid==false)
        {
            $scope.msgbox("Error","danger","Existen valores pendientes de capturar");
            return;
        }
        $scope.savingfrm = true;
        data.FechaNacimiento = myApp.convertToJSONDate(data.FechaNacimiento);
        myApp.WsPeticion(data,"item","ServiciosERP/Ventas/WSUsuariosMonedero.svc/update",function(response){
            if(response.headers("Error")==null)
            {
                //RegistroCompletado=true;
                $scope.usuario={};
                $scope.FrmRegistro.$setUntouched();
                $scope.FrmRegistro.$setPristine();
                //$uibModalInstance.dismiss('cancel');
                //Refresh provisional
                window.location.reload();
                //angular.element('#myModal').modal('hide');
            }
            else
            {
                myApp.ShowMsgbox(response.headers("Error"),"danger");
            }
            $scope.savingfrm = false;
        });
    };
    $scope.open1 = function() {
        $scope.popup1.opened = true;
    };

}]);