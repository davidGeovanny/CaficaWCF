using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSDepartamentos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSDepartamentos.svc or WSDepartamentos.svc.cs at the Solution Explorer and start debugging.
    public class WSDepartamentos : WsBase, IWSDepartamentos
    {
        public List<Departamentos> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var departamentos = db.Departamentos.ToList();

                return departamentos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Departamentos add(Departamentos departamento)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Departamentos.Add(departamento);
                db.SaveChanges();
                return departamento;

            }
            catch (Exception ex)
            {
                Error(ex, "El departamento ");
                return null;
            }
        }
        public Departamentos get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Departamentos departamento = db.Departamentos.Find(id);
                return departamento;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Departamentos update(Departamentos departamento)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                db.Departamentos.Attach(departamento);
                db.Entry(departamento).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return departamento;
            }
            catch (Exception ex)
            {
                Error(ex, "El departamento ");
                return departamento;
            }
        }
        public Departamentos delete(Departamentos departamentol)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Departamentos departamento = db.Departamentos.Find(departamentol.Id);

                db.Departamentos.Attach(departamento);
                db.Departamentos.Remove(departamento);
                db.SaveChanges();
                return departamento;
            }
            catch (Exception ex)
            {
                Error(ex, "El departamento ");
                return null;
            }
        }
    }
}
