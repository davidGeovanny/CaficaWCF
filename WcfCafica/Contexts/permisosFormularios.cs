using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts
{
    public class PermisosFormularios
    {
        public long IdAccion { get; set; }
        public string NombreAccion { get; set; }
        public bool Permitida { get; set; }
    }
}