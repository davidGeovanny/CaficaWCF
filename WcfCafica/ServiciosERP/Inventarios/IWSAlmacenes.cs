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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSAlmacenes" in both code and config file together.
    [ServiceContract]
    public interface IWSAlmacenes : IWSBase<Almacenes>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getAlmacenesXGrupoComponentes",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Almacenes> getAlmacenesXGrupoComponentes(int id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getGruposComponentesXAlmacen",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
       List<GruposComponentes> getGruposComponentesXAlmacen(int id);

       [OperationContract]
       [WebInvoke(UriTemplate = "getAlmacenesxTipo",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
       List<Almacenes> getAlmacenesxTipo(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getAlmacenesResguardo",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Almacenes> getAlmacenesResguardo();

    }
}
