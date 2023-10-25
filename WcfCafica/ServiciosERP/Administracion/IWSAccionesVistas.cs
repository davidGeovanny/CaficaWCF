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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IWSAccionesVistas" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IWSAccionesVistas : IWSBase<AccionesVistas>
    {

    }
}
