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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSMovimientosMonedero" in both code and config file together.
    [ServiceContract]
    public interface IWSMovimientosMonedero : IWSBase<MovimientosMonedero>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "addOga",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        MovimientosMonedero addOga(MovimientosMonedero item);

        [OperationContract]
        [WebInvoke(UriTemplate = "getMovimientosxUsuario",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
        List<MovimientosMonedero> getMovimientosxUsuario(int index);


        [OperationContract]
        [WebInvoke(UriTemplate = "getCanjexSocioMonedero",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json, 
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<MovimientosMonedero> getCanjexSocioMonedero();

        [OperationContract]
        [WebInvoke(UriTemplate = "addHielera",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        MovimientosMonedero addHielera(MovimientosMonedero item);

        [OperationContract]
        [WebInvoke(UriTemplate = "addViaRapida",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
        MovimientosMonedero addViaRapida(MovimientosMonedero item);


    }
}
