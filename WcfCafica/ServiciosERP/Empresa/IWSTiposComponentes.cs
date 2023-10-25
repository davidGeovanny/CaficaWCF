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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSTiposComponentes" in both code and config file together.
    [ServiceContract]
    public interface IWSTiposComponentes
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getTiposComponentes",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
        List<TiposComponentes> getTiposComponentes();

        [OperationContract]
        [WebInvoke(UriTemplate = "getTipoComponente",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        TiposComponentes getTipoComponente(int ID);
    }
}
