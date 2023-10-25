using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios       
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSESInventarios" in both code and config file together.
    [ServiceContract]
    public interface IWSESInventarios : IWSBaseDocumento<InventariosES>
    {
       [OperationContract]
       [WebInvoke(UriTemplate = "getEmpresaVariacionCosto",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
       string getEmpresaVariacionCosto();
        
       [OperationContract]
       [WebInvoke(UriTemplate = "CalcularVariacionCosto",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
       void  CalcularVariacionCosto(InventariosESDetalles inventarioentradaosalida);

       [OperationContract]
       [WebInvoke(UriTemplate = "ValidarMaximo",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json, 
       Method = "POST")]
       void ValidarMaximo(InventariosESDetalles inventarioentradaosalida);

        [OperationContract]
        [WebInvoke(UriTemplate = "getallxnaturaleza",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<InventariosES> getallxnaturaleza(string Naturaleza);


        [OperationContract]
        [WebInvoke(UriTemplate = "getSerielotexcomponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<LotesSeries> getSerielotexcomponente(InventariosESDetalles inventarioentradaosalida);

        [OperationContract]
        [WebInvoke(UriTemplate = "getTranspasosPendientes",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<InventariosES> getTranspasosPendientes(long almacenid);

        [OperationContract]
        [WebInvoke(UriTemplate = "getAbrirTranspasoPendiente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<InventariosES> getAbrirTranspasoPendiente(long id);

    }
}
