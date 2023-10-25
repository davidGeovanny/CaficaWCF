
using System.ServiceModel;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSCanjes" in both code and config file together.
    [ServiceContract]
    public interface IWSCanjes : IWSBase<MovimientosMonedero>
    {

    }
}
