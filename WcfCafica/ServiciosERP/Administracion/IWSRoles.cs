﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSRoles" in both code and config file together.
    [ServiceContract]
    public interface IWSRoles : IWSBase<Roles>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getPermisos",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<PermisosModel> getPermisos();
    }
}
