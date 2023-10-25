app.controller("CarritoController", ['$scope','$window','ngCart','$uibModal','myApp', function($scope,$window,ngCart,$uibModal,myApp) { 
	//var $ctrl = this;
	$scope.savingfrm = false;
	$scope.ngCart = ngCart;
	$scope.FrmRegistro;

	$scope.init = function() {
       myApp.getSaldo();
       myApp.getVerificarRegistro();
    };

    $scope.showRegistro = function(){
    	$scope.FrmRegistro = $uibModal.open({
	    animation: true,
	    ariaLabelledBy: 'modal-title',
	    ariaDescribedBy: 'modal-body',
	    templateUrl: './registro.html',
	    controller: 'RegistroController',
	    //controllerAs: '$ctrl'
	  });	
    };

}]);