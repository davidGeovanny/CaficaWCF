using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSProductoTerminado" in both code and config file together.
    [ServiceContract]
    public interface IWSComponentes
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getProductosTerminados",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getProductosTerminados();

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        Componentes getComponente(int ID);

        [OperationContract]
        [WebInvoke(UriTemplate = "updateComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        Componentes updateComponente(Componentes componente);

        [OperationContract]
        [WebInvoke(UriTemplate = "addComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        Componentes addComponente(Componentes componente);


        [OperationContract]
        [WebInvoke(UriTemplate = "deleteProductoTerminado",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        Componentes deleteComponente(int ID);
    }
}
