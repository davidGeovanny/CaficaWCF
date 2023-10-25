using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts.Reportes
{
    public class outputResource
    {
        public string contentType { get; set; }
        public string fileName { get; set; }
        public Boolean outputFinal { get; set; }
    }
}