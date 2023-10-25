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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSSubGrupoComponentes" in both code and config file together.
    [ServiceContract]
    public interface IWSSubGrupoComponentes
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getSubGruposComponentes",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           Method = "POST")]
        List<SubgruposComponentes> getSubGruposComponentes();

        [OperationContract]
        [WebInvoke(UriTemplate = "getSubGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        SubgruposComponentes getSubGrupoComponente(int ID);

        [OperationContract]
        [WebInvoke(UriTemplate = "addSubGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        SubgruposComponentes addSubGrupoComponente(SubgruposComponentes subgrupocomponente);

        [OperationContract]
        [WebInvoke(UriTemplate = "updateSubGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        SubgruposComponentes updateSubGrupoComponente(SubgruposComponentes subgrupocomponente);



        [OperationContract]
        [WebInvoke(UriTemplate = "deleteSubGrupoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        SubgruposComponentes deleteSubGrupoComponente(int ID);

        [OperationContract]
        [WebInvoke(UriTemplate = "getSubGruposComponentesXGrupo",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         Method = "POST")]
        List<SubgruposComponentes> getSubGruposComponentesXGrupo(int grupoid);
    }
}
