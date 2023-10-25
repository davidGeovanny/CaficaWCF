using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSAlmacenes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSAlmacenes.svc or WSAlmacenes.svc.cs at the Solution Explorer and start debugging.
    public class WSAlmacenes : WsBase,IWSAlmacenes
    {
        public List<Almacenes> getAlmacenes()
        {
            try
            {
                Validar();

                InventarioContext db = new InventarioContext();
                var almacenes = db.Almacenes.ToList();

                return almacenes;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
    }
}
