using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSMonedas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSMonedas.svc or WSMonedas.svc.cs at the Solution Explorer and start debugging.
    public class WSMonedas : WsBase,IWSMonedas
    {
        public List<Monedas> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var monedas = db.Monedas.ToList();

                return monedas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Monedas get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                Monedas moneda = db.Monedas.Find(ID);
                return moneda;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Monedas add(Monedas moneda)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                moneda.MonedaLocal = "NO";
                db.Monedas.Add(moneda);
                db.SaveChanges();
                return moneda;

            }
            catch (Exception ex)
            {
                Error(ex, "La moneda ");
                return null;
            }
        }
        public Monedas update(Monedas moneda)
        {
               try
               {
                   Validar();
                   //Metodo para Actualizar los campos de las empresas
                   EmpresaContext db = new EmpresaContext();
                   db.Monedas.Attach(moneda);
                   db.Entry(moneda).State = System.Data.Entity.EntityState.Modified;
                   db.SaveChanges();
                   return moneda;
               }
               catch (Exception ex)
               {
                   Error(ex, "La moneda ");
                   return moneda;
               }
        
        }

        public Monedas delete(Monedas moneda)
        {
           

            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Monedas buscarmoneda = db.Monedas.Find(moneda.Id);
                if (buscarmoneda.MonedaLocal == "SI")
                    throw new Exception("No se puede eliminar una moneda local");
                else
                {
                    db.Monedas.Attach(buscarmoneda);
                    db.Monedas.Remove(buscarmoneda);
                    db.SaveChanges();
                }
                return moneda;

            }
            catch (Exception ex)
            {
                Error(ex, "La moneda ");
                return null;
            }
            
        }
    }
}
