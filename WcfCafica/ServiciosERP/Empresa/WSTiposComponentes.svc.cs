using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSTiposComponentes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSTiposComponentes.svc or WSTiposComponentes.svc.cs at the Solution Explorer and start debugging.
    public class WSTiposComponentes : WsBase, IWSTiposComponentes
    {
        public List<TiposComponentes> getTiposComponentes()
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                var tiposcomponentes = db.TiposComponentes.ToList();

                return tiposcomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public TiposComponentes getTipoComponente(int ID)
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                TiposComponentes tiposcomponentes = db.TiposComponentes.Find(ID);
                return tiposcomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
}
}
