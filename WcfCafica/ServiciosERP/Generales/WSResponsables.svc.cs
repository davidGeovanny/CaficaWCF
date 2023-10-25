using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSResponsables" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSResponsables.svc or WSResponsables.svc.cs at the Solution Explorer and start debugging.
    public class WSResponsables : WsBase,IWSResponsables 
    {
        public List<Responsables> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var responsables = db.Responsables.ToList();

                return responsables;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Responsables add(Responsables responsable)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Responsables.Add(responsable);
                db.SaveChanges();
                return responsable;

            }
            catch (Exception ex)
            {
                Error(ex, "El responsable ");
                return null;
            }
        }
        public Responsables get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Responsables responsable = db.Responsables.Find(id);
                return responsable;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Responsables update(Responsables responsable)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Responsables.Attach(responsable);
                db.Entry(responsable).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return responsable;
            }
            catch (Exception ex)
            {
                Error(ex, "El responsable ");
                return responsable;
            }
        }
        public Responsables delete(Responsables responsablesel)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Responsables responsable = db.Responsables.Find(responsablesel.Id);

                db.Responsables.Attach(responsable);
                db.Responsables.Remove(responsable);
                db.SaveChanges();
                return responsable;
            }
            catch (Exception ex)
            {
                Error(ex, "El responsable ");
                return null;
            }
        }
    }
}
