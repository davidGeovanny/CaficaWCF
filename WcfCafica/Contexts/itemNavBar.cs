using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts
{
    using System;
    using System.Collections.Generic;

    public class ItemNavBar
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Grupo { get; set; }
        public string ImagenMenu { get; set; }
        public string Parametros { get; set; }
       
    }
}