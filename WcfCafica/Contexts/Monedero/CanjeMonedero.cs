using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts.Monedero
{
    public class CanjeMonedero
    {
        public double subTotal { get; set; }
        public double totalCost { get; set; }
        public double? taxRate { get; set; }
        public double? tax { get; set; }
        public List<CanjeMonederoDetalle> items { get; set; }

        public CanjeMonedero()
        {
            items = new List<CanjeMonederoDetalle>();
        }
    }
}