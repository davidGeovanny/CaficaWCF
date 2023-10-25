using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace WcfCafica.ServiciosERP
{
    [ServiceContract]
    public interface IWSBaseDocumento<TEntity>: IWSBase<TEntity>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "cancel",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        TEntity cancel(TEntity item);
    }
}