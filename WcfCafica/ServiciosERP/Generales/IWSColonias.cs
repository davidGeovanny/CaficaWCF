using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSColonias" in both code and config file together.
    [ServiceContract]
    public interface IWSColonias : IWSBase<Colonias>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getColoniasCiudades",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json,
          Method = "POST")]
        List<Colonias> getColoniasCiudades(int id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getColoniasCP",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json,
          Method = "POST")]
        List<Colonias> getColoniasCP(int id);
    }
}
