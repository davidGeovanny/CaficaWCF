using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGiros" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGiros.svc or WSGiros.svc.cs at the Solution Explorer and start debugging.
    public class WSGiros : WsBase, IWSGiros
    {
        public Giros add(Giros giro)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                db.Giros.Add(giro);
                db.SaveChanges();
                return giro;

            }
            catch (Exception ex)
            {
                Error(ex, "El giro ");
                return null;
            }
        }

        public Giros delete(Giros giro)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                giro = db.Giros.Find(giro.Id);

                db.Giros.Attach(giro);
                db.Giros.Remove(giro);
                db.SaveChanges();
                return giro;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Giros get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Giros giro = db.Giros.Find(id);
                return giro;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<Giros> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var giros = db.Giros.ToList();

                return giros;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Giros update(Giros giro)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Giros.Attach(giro);
                db.Entry(giro).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return giro;
            }
            catch (Exception ex)
            {
                Error(ex);
                return giro;
            }
        }
    }
}
