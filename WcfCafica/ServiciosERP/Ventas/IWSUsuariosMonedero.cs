using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSUsuariosMonedero" in both code and config file together.
    [ServiceContract]
    public interface IWSUsuariosMonedero : IWSBase<UsuariosMonedero>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getUsuarioWeb",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        UsuariosMonedero getUsuarioWeb();

        [OperationContract]
        [WebInvoke(UriTemplate = "getVerificarRegistro",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        bool getVerificarRegistro();
    }
}
  