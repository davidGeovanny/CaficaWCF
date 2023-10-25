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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSMunicipios" in both code and config file together.
    [ServiceContract]
    public interface IWSMunicipios : IWSBase<Municipios>
    {
   
        [OperationContract]
        [WebInvoke(UriTemplate = "getMunicipiosEstado",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Municipios> getMunicipiosEstado(int id);

    }
}
