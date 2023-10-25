using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSTiposDocumentos" in both code and config file together.
    [ServiceContract]
    public interface IWSTiposDocumentos : IWSBase<TiposDocumentos>
    {
        
    }
}
