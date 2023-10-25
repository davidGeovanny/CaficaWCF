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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSGrupoUnidades" in both code and config file together.
    [ServiceContract]
    public interface IWSGrupoUnidades
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getGrupoUnidades",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<GruposUnidades> getGrupoUnidades();

       [OperationContract]
       [WebInvoke(UriTemplate = "addGrupoUnidades",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
       GruposUnidades addGrupoUnidades(GruposUnidades grupounidad);

        [OperationContract]
        [WebInvoke(UriTemplate = "getGrupoUnidad",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposUnidades getGrupoUnidad(int id);

        [OperationContract]
        [WebInvoke(UriTemplate = "updateGrupoUnidad",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposUnidades updateGrupoUnidad(GruposUnidades grupounidad);

        [OperationContract]
        [WebInvoke(UriTemplate = "deleteGrupoUnidad",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        GruposUnidades deleteGrupoUnidad(int ID);
    }
}
