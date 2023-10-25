using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.Contexts.Monedero
{
    public class CanjeMonederoDetalle
    {
        public string id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public double total { get; set; }
        public PremiosMonedero data { get; set; }
    }
}
