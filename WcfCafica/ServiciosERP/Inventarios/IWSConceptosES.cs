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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSConceptosES" in both code and config file together.
    [ServiceContract]
    public interface IWSConceptosES : IWSBase<ConceptosES>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getConceptosEntradaSalida",
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
      ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json,
      Method = "POST")]
        List<ConceptosES> getConceptosEntradaSalida(string Naturaleza);
    }
}
