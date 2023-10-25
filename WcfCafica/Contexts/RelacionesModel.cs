using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCafica.Contexts
{
    public class RelacionesModel
    {
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string REFERENCED_TABLE_NAME { get; set; }
        public string REFERENCED_COLUMN_NAME { get; set; }
    }
}