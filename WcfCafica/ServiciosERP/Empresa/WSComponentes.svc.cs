using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSProductoTerminado" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSProductoTerminado.svc or WSProductoTerminado.svc.cs at the Solution Explorer and start debugging.
    public class WSComponentes : WsBase,IWSComponentes
    {


        public List<Componentes> getProductosTerminados()
        {
            try
            {
                var lstProductosTerminados = getComponentes(5);
                return lstProductosTerminados;
            }
            catch 
                (Exception ex)
            {

                Error(ex);
                return null;
            }

        }

        private List<Componentes> getComponentes(int tipo)
        {

            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                //Consulta que retorna todos los ciudades
               var lstComponentes = db.Componentes.Include("GruposComponentes").Include("SubgruposComponentes").Include("GruposUnidades").
                    Include("UnidadInventario").Include("UnidadCompra").Include("UnidadVenta").Include("TiposComponentes").ToList();
               /*var lstComponentes = (from s in db.Componentes
                                      where s.TipoComponenteId == tipo
                                      select s).ToList();*/
                if (tipo==0) //Si el tipo es igual a 0 retorna todos los componentes
                {
                    return lstComponentes;
                }   
                else // de lo contrario rotarnara los que coincida con el filtro
                {
                    return lstComponentes.FindAll(ls => ls.TipoComponenteId == tipo);
                }                                                                   
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Componentes getComponente(int ID)
        {

            return null;
        }
       
        public Componentes updateComponente(Componentes componente)
        {

            return null;
        }

        public Componentes addComponente(Componentes componente)
        {

            return null;
        }
        public Componentes deleteComponente(int ID)
        {

            return null;
        }
    }
}
