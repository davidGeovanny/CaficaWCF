using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts.Reportes
{
    public class reportParameters
    {
        public List<reportParameter> reportParameter { get; set; }
        public reportParameters()
        {
            reportParameter = new List<reportParameter>();
        }
    }
}