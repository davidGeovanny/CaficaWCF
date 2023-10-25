using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSMarcasComponentes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSMarcasComponentes.svc or WSMarcasComponentes.svc.cs at the Solution Explorer and start debugging.
    public class WSMarcasComponentes : WsBase,IWSMarcasComponentes
    {
        public MarcasComponentes add(MarcasComponentes item)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.MarcasComponentes.Add(item);
                db.SaveChanges();
                return item;

            }
            catch (Exception ex)
            {
                Error(ex, "La marca ");
                return null;
            }
        }

        public MarcasComponentes delete(MarcasComponentes item)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                MarcasComponentes marca = db.MarcasComponentes.Find(item.Id);
                db.MarcasComponentes.Attach(marca);
                db.MarcasComponentes.Remove(marca);
                db.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                Error(ex, "La marca ");
                return null;
            }
        }

        public MarcasComponentes get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el objeto solicitado usando como parametro el ID
                MarcasComponentes marca = db.MarcasComponentes.Find(id);
                return marca;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<MarcasComponentes> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var lstMarcas = db.MarcasComponentes.ToList();

                return lstMarcas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public MarcasComponentes update(MarcasComponentes item)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.MarcasComponentes.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                Error(ex, "La marca ");
                return item;
            }
        }
    }
}
