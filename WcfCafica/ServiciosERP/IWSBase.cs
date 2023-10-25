using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfCafica.ServiciosERP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSBase" in both code and config file together.
    [ServiceContract]
    public interface IWSBase<TEntity>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getall",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<TEntity> getall();

        [OperationContract]
        [WebInvoke(UriTemplate = "add",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        TEntity add(TEntity item);

        [OperationContract]
        [WebInvoke(UriTemplate = "get",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        TEntity get(int id);

        [OperationContract]
        [WebInvoke(UriTemplate = "update",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        TEntity update(TEntity item);

        [OperationContract]
        [WebInvoke(UriTemplate = "delete",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        TEntity delete(TEntity item);

        //Servicio para dar respueta a als peticion OPTIONS que viene de CORS
        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void GetOptions();
    }
}
