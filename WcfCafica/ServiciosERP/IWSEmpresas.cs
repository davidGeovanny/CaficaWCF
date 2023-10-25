using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IWSEmpresas" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IWSEmpresas
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getEmpresas",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<BDEmpresas> getEmpresas();

        [OperationContract]
        [WebInvoke(UriTemplate = "getTokenEmpresas",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        string getTokenEmpresas(BDEmpresas empresa);


    }
}
