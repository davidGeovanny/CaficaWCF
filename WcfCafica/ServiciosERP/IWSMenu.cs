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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSMenu" in both code and config file together.
    [ServiceContract]
    public interface IWSMenu
    {
       [OperationContract]
       [WebInvoke(UriTemplate = "getMenuNavBar",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
       List<ItemNavBar> getMenuNavBar(int moduloid);

        [OperationContract]
        [WebInvoke(UriTemplate = "getPermisosFormulario",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         Method = "GET")]
        List<Vistas> getPermisosFormulario();

        [OperationContract]
        [WebInvoke(UriTemplate = "getDatosLogin",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           Method = "POST")]
        string[] getDatosLogin();
    }

}
