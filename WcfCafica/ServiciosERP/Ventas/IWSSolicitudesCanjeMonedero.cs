using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;
using WcfCafica.Contexts.Monedero;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSSolicitudesCanjeMonedero" in both code and config file together.
    [ServiceContract]
    public interface IWSSolicitudesCanjeMonedero : IWSBase<SolicitudesCanjeMonedero>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "addSolicitudWeb",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        string addSolicitudWeb(CanjeMonedero data);

        [OperationContract]
        [WebInvoke(UriTemplate = "getSolicitudesWebXUsuario",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<SolicitudesCanjeMonedero> getSolicitudesWebXUsuario();

        [OperationContract]
        [WebInvoke(UriTemplate = "getSolicitudesDetallesWebXUsuario",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<SolicitudesCanjeMonederoDetalles> getSolicitudesDetallesWebXUsuario(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getSolicitudxCodigo",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        SolicitudesCanjeMonedero getSolicitudxCodigo(string codigo);
    }
}
