app.controller("MovimientosController", ['$scope','$window','myApp', function($scope,$window,myApp) { 
    $scope.savingfrm = false;
    $scope.SociosMonedero = [];
    $scope.Movimientos = [];
    $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[1];
    $scope.altInputFormats = ['M!/d!/yyyy'];
    $scope.IndexMovimientos=0;
    $scope.CargandoMovimientos=false;
    $scope.placement = {
    options: [
      'top',
      'top-left',
      'top-right',
      'bottom',
      'bottom-left',
      'bottom-right',
      'left',
      'left-top',
      'left-bottom',
      'right',
      'right-top',
      'right-bottom'
    ],
    selected: 'top'
  };
    var slides = $scope.slides = [];


    $scope.popup1 = {
        opened: false
    };
    $scope.getTotal = function(){
        var total = 0;
        for(var i = 0; i < $scope.Movimientos.length; i++){
            var movimiento = $scope.Movimientos[i];
            total += (movimiento.Abono);
        }
        return total;
    }
    $scope.init = function() {
        myApp.getSaldo();
        
        myApp.WsPeticion(null,"acceso","ServiciosERP/Ventas/WSSociosMonedero.svc/getSociosOGA",function(response){
            if(response.headers("Error")==null)
            {
                $scope.SociosMonedero=response.data
            }
            else
            {
                myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
            }
            $scope.savingfrm = false;
        });

         slides.push({
                  image: 'http://172.16.5.72:8080/MonederoElectronico/img/ticketbrissaagua.jpg',
                  text: 'Ticket Agua',
                  id: 0
                });

          slides.push({
                  image: 'http://172.16.5.72:8080/MonederoElectronico/img/ticketbrissahielo.jpg',
                  text: 'Ticket Hielo',
                  id: 1
                });

        //$scope.getMovimientosxUsuario();
    };
    $scope.getMovimientosxUsuario=function()
    {
       // console.log($scope.CargandoMovimientos);
        if ($scope.CargandoMovimientos) return;
                $scope.CargandoMovimientos = true;

        myApp.WsPeticion($scope.Movimientos.length,"index","ServiciosERP/Ventas/WSMovimientosMonedero.svc/getMovimientosxUsuario",function(response){
            if(response.headers("Error")==null)
            {
                //$scope.Movimientos = response.data;
                //$scope.Movimientos.push(response.data);
                var items=response.data;
                if(items.length==0) 
                {
                    $scope.CargandoMovimientos=true;
                    return;
                }
                for (var i = 0; i < items.length; i++) {
                     $scope.Movimientos.push(items[i]);
                }
            }
            else
            {

                myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
            }
            $scope.savingfrm = false;
            $scope.CargandoMovimientos=false;
        });
    }
    $scope.Guardar = function() {
        var data=$scope.Movimiento;
        //Valida sin las informacion del formulario es correcta
        if($scope.FrmMovimientosOga.$valid==false)
        {
            $scope.msgbox("Error","danger","Existen valores pendientes de capturar");
            angular.element('#myModalAlert').modal();
            return;
        }
        $scope.savingfrm = true;
        data.FechaHora = myApp.convertToJSONDate(data.FechaHora);
		
		//Esta condición revisa si el monto tiene el símbolo $ para eliminarlo
		if(data.MontoTicket.substr(0,1) == "$"){
		   var longitud = data.MontoTicket.length-1;
			data.MontoTicket = data.MontoTicket.substr(1,longitud);
		}
		
        myApp.WsPeticion(data,"item","ServiciosERP/Ventas/WSMovimientosMonedero.svc/addOga",function(response){
            if(response.headers("Error")==null)
            {
                $scope.Movimiento={};
                $scope.FrmMovimientosOga.$setUntouched();
                $scope.FrmMovimientosOga.$setPristine();
                //angular.element('#myModal').modal('hide');
                angular.element('#myModalMovimientosOga').modal('hide');
                myApp.ShowMsgbox("Ticket registrado con exito...!!!","success");
            }
            else
            {
                myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
            }
            $scope.savingfrm = false;
            /////por mientras//////////////////
           myApp.getSaldo();
             $scope.CargandoMovimientos = false;
            $scope.getMovimientosxUsuario();
        });
    };
       $scope.GuardarBrissa = function() {
        var data=$scope.Movimiento;
        //Valida sin las informacion del formulario es correcta
        if($scope.FrmMovimientosBrissa.$valid==false)
        {
            $scope.msgbox("Error","danger","Existen valores pendientes de capturar");
            angular.element('#myModalAlert').modal();
            return;
        }
        $scope.savingfrm = true;
        data.FechaHora = myApp.convertToJSONDate(data.FechaHora);
		   
		//Esta condición revisa si el monto tiene el símbolo $ para eliminarlo
		if(data.MontoTicket.substr(0,1) == "$"){
		   var longitud = data.MontoTicket.length-1;
			data.MontoTicket = data.MontoTicket.substr(1,longitud);
		}
		   
        myApp.WsPeticion(data,"item","ServiciosERP/Ventas/WSMovimientosMonedero.svc/addHielera",function(response){
            if(response.headers("Error")==null)
            {
                $scope.Movimiento={};
                $scope.FrmMovimientosBrissa.$setUntouched();
                $scope.FrmMovimientosBrissa.$setPristine();
                //angular.element('#myModal').modal('hide');
                angular.element('#myModalMovimientosBrissa').modal('hide');
                myApp.ShowMsgbox("Ticket registrado con exito...!!!","success");
            }
            else
            {
                myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
            }
            $scope.savingfrm = false;
            /////por mientras//////////////////
           myApp.getSaldo();
             $scope.CargandoMovimientos = false;
            $scope.getMovimientosxUsuario();
        });
    };
     $scope.GuardarViaRapida = function() {
        var data=$scope.Movimiento;
        //Valida sin las informacion del formulario es correcta
        if($scope.FrmMovimientosViaRapida.$valid==false)
        {
            $scope.msgbox("Error","danger","Existen valores pendientes de capturar");
            angular.element('#myModalAlert').modal();
            return;
        }
        $scope.savingfrm = true;
        data.FechaHora = myApp.convertToJSONDate(data.FechaHora);
		
		//Esta condición revisa si el monto tiene el símbolo $ para eliminarlo
		if(data.MontoTicket.substr(0,1) == "$"){
		   var longitud = data.MontoTicket.length-1;
			data.MontoTicket = data.MontoTicket.substr(1,longitud);
		}
		
        myApp.WsPeticion(data,"item","ServiciosERP/Ventas/WSMovimientosMonedero.svc/addViaRapida",function(response){
            if(response.headers("Error")==null)
            {
                $scope.Movimiento={};
                $scope.FrmMovimientosViaRapida.$setUntouched();
                $scope.FrmMovimientosViaRapida.$setPristine();
                //angular.element('#myModal').modal('hide');
                angular.element('#myModalMovimientosViaRapida').modal('hide');
                myApp.ShowMsgbox("Ticket registrado con exito...!!!","success");
            }
            else
            {
                myApp.ShowMsgbox(myApp.codigoError(response.headers("Error")),"danger");
            }
            $scope.savingfrm = false;
            /////por mientras//////////////////
             myApp.getSaldo();
             $scope.CargandoMovimientos = false;
             $scope.getMovimientosxUsuario();
        });
    };
    $scope.ShowRegistroOga=function(){
        angular.element('#myModalMovimientosOga').modal('show');
        $scope.Movimiento={};
  
        $scope.FrmMovimientosOga.$setUntouched();
        $scope.FrmMovimientosOga.$setPristine();
    };
    $scope.open1 = function() {
        $scope.popup1.opened = true;
    };
    $scope.ShowRegistroBrissa=function(){
        angular.element('#myModalMovimientosBrissa').modal('show');
        $scope.Movimiento={};
        
        $scope.FrmMovimientosBrissa.$setUntouched();
        $scope.FrmMovimientosBrissa.$setPristine();
    };$scope.ShowRegistroViaRapida=function(){
        angular.element('#myModalMovimientosViaRapida').modal('show');
        $scope.Movimiento={};
      
        $scope.FrmMovimientosViaRapida.$setUntouched();
        $scope.FrmMovimientosViaRapida.$setPristine();
    };
    $scope.getMasMovimientos=function(){
      //  console.log("final");
    };

}]);