using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSAreas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSAreas.svc or WSAreas.svc.cs at the Solution Explorer and start debugging.
    public class WSAreas : WsBase, IWSAreas
    {
        public List<Areas> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var areas = db.Areas.ToList();

                return areas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Areas add(Areas area)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Areas.Add(area);
                db.SaveChanges();
                return area;

            }
            catch (Exception ex)
            {
                Error(ex, "El area ");
                return null;
            }
        }
        public Areas get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Areas area = db.Areas.Find(id);
                return area;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Areas update(Areas area)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Areas.Attach(area);
                db.Entry(area).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return area;
            }
            catch (Exception ex)
            {
                Error(ex, "El area ");
                return area;
            }
        }
        public Areas delete(Areas areal)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Areas area = db.Areas.Find(areal.Id);

                db.Areas.Attach(area);
                db.Areas.Remove(area);
                db.SaveChanges();
                return area;
            }
            catch (Exception ex)
            {
                Error(ex, "El area ");
                return null;
            }
        }
    }
}
