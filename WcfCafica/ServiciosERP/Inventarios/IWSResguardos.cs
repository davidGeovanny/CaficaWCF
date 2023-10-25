﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSResguardos" in both code and config file together.
    [ServiceContract]
    public interface IWSResguardos : IWSBaseDocumento<Resguardos>
    {

    }
}
