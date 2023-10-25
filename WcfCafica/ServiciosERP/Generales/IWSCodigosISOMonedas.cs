using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSCodigosISOMonedas" in both code and config file together.
    [ServiceContract]
    public interface IWSCodigosISOMonedas : IWSBase<CodigosISOMonedas>
    {
    
    }
}
