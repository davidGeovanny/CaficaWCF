using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace WcfCafica.ServiciosERP
{
        [ServiceContract]
        public interface IWSBaseLike<TEntity> : IWSBase<TEntity>
        {
            [OperationContract]
            [WebInvoke(UriTemplate = "buscar",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            Method = "POST")]
            List<TEntity> buscar(string item);
        }
}