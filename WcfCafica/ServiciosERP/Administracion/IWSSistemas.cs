using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSSistemas" in both code and config file together.
    [ServiceContract]
    public interface IWSSistemas : IWSBase<Sistemas>
    {
     
    }
}
