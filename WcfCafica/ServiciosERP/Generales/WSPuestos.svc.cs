using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSPuestos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSPuestos.svc or WSPuestos.svc.cs at the Solution Explorer and start debugging.
    public class WSPuestos : WsBase, IWSPuestos
    {
        public List<Puestos> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var puestos = db.Puestos.Include("Departamentos").ToList();

                return puestos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Puestos add(Puestos puesto)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Puestos.Add(puesto);
                db.SaveChanges();
                return puesto;

            }
            catch (Exception ex)
            {
                Error(ex, "El puesto ");
                return null;
            }
        }
        public Puestos get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Puestos puesto = db.Puestos.Find(id);
                return puesto;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Puestos update(Puestos puesto)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Puestos.Attach(puesto);
                db.Entry(puesto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return puesto;
            }
            catch (Exception ex)
            {
                Error(ex, "El puesto ");
                return puesto;
            }
        }
        public Puestos delete(Puestos puestol)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Puestos puesto = db.Puestos.Find(puestol.Id);

                db.Puestos.Attach(puesto);
                db.Puestos.Remove(puesto);
                db.SaveChanges();
                return puesto;
            }
            catch (Exception ex)
            {
                Error(ex, "El puesto ");
                return null;
            }
        }
        public List<Puestos> CargaPuestosxDepartamento(long IdDepartamento)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var puestos = db.Puestos.Where(c => c.DepartamentoId==IdDepartamento).ToList();

                return puestos;
            }
            catch (Exception ex)
            {
                Error(ex, "El puesto ");
                return null;
            }
        }
    }
}
