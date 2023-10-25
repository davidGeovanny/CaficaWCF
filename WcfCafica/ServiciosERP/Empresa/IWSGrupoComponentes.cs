using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSGrupoComponentes" in both code and config file together.
    [ServiceContract]
    public interface IWSGrupoComponentes
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getGruposComponentes", 
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json,
          Method = "POST")]
        List<GruposComponentes> getGruposComponentes();

        [OperationContract]
        [WebInvoke(UriTemplate = "getGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposComponentes getGrupoComponente(int ID);

        [OperationContract]
        [WebInvoke(UriTemplate = "addGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposComponentes addGrupoComponente(GruposComponentes grupocomponente);

        [OperationContract]
        [WebInvoke(UriTemplate = "updateGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposComponentes updateGrupoComponente(GruposComponentes grupocomponente);



        [OperationContract]
        [WebInvoke(UriTemplate = "deleteGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposComponentes deleteGrupoComponente(int ID);

        [OperationContract]
        [WebInvoke(UriTemplate = "getGruposComponentesXTipo",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         Method = "POST")]
        List<GruposComponentes> getGruposComponentesXTipo(int tipo);
    }
}
