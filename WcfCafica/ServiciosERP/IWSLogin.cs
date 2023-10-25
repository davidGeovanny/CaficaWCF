using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSLogin" in both code and config file together.
    [ServiceContract]
    public interface IWSLogin
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Login",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        string Login(Login acceso);

        [OperationContract]
        [WebInvoke(UriTemplate = "LoginMonedero",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        string LoginMonedero(Login acceso);

        //Servicio para dar respueta a als peticion OPTIONS que viene de CORS
        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void GetOptions();
    }
}
