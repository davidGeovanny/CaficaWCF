using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts.Reportes
{
    public class ReporteModel
    {
        public string reportUnitUri { get; set; }
        public string outputFormat { get; set; }
        public Boolean freshData { get; set; }
        public Boolean saveDataSnapshot { get; set; }
        public Boolean interactive { get; set; }
        public Boolean allowInlineScripts { get; set; }
        public Boolean ignorePagination { get; set; }
        public string pages { get; set; }
        public Boolean async { get; set; }
        public string transformerKey { get; set; }
        public string attachmentsPrefix { get; set; }
        public string baseURL { get; set; }
        public reportParameters parameters { get; set; }
    }
}