using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSTiposCambio" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSTiposCambio.svc or WSTiposCambio.svc.cs at the Solution Explorer and start debugging.
    public class WSTiposCambio : WsBase, IWSTiposCambio
    {
        public List<TiposCambio> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var tiposcambio = db.TiposCambio.ToList();

                return tiposcambio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public TiposCambio get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                TiposCambio tipocambio = db.TiposCambio.Find(ID);
                return tipocambio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public TiposCambio add(TiposCambio tipocambio)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.TiposCambio.Add(tipocambio);
                db.SaveChanges();
                return tipocambio;

            }
            catch (Exception ex)
            {
                Error(ex, "El tipo de cambio ");
                return null;
            }
        }
        public TiposCambio update(TiposCambio tipocambio)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.TiposCambio.Attach(tipocambio);
                db.Entry(tipocambio).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return tipocambio;
            }
            catch (Exception ex)
            {
                Error(ex, "El tipo de cambio ");
                return tipocambio;
            }

        }

        public TiposCambio delete(TiposCambio tipocambio)
        {


            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                TiposCambio buscartipocambio = db.TiposCambio.Find(tipocambio.Id);

                db.TiposCambio.Attach(buscartipocambio);
                db.TiposCambio.Remove(buscartipocambio);
                db.SaveChanges();

                return buscartipocambio;

            }
            catch (Exception ex)
            {
                Error(ex, "El tipo de cambio ");
                return null;
            }

        }
    }
}