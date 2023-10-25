using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts.Administracion
{
    public class PermisosModel
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public bool IsChecked { get; set; }
        public bool HasChild { get; set; }
        public string Tipo { get; set; }
        public string Index { get; set; }
        public string ParentId { get; set; }

        public Modulos Modulo { get; set; }
        public Vistas Vista { get; set; }
        public AccionesVistas AccionesVista { get; set; }
    }
}