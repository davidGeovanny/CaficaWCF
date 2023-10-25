using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSActivacionesCorreos" in both code and config file together.
    [ServiceContract]
    public interface IWSActivacionesCorreos
    {
        [OperationContract]
        [WebGet(UriTemplate = "registrobrissa/{token}", BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream registrobrissa(string token);
    }
}
