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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSUnidades" in both code and config file together.
    [ServiceContract]
    public interface IWSUnidades :IWSBase<Unidades>
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "getUnidadesXGrupo",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         Method = "POST")]
        List<Unidades> getUnidadesXGrupo(int id);

    }
}
