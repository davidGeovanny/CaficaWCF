using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Compras
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSProveedores" in both code and config file together.
    [ServiceContract]
    public interface IWSProveedores : IWSBase<Proveedores>
    {
      
    }
}
