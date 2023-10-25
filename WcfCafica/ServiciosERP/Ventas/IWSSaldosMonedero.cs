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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSSaldosMonedero" in both code and config file together.
    [ServiceContract]
    public interface IWSSaldosMonedero : IWSBase<SaldosMonedero>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "saldoUsuario",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        double saldoUsuario();
    }
}
