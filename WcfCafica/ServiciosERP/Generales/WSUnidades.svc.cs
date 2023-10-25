using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSUnidades" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSUnidades.svc or WSUnidades.svc.cs at the Solution Explorer and start debugging.
    public class WSUnidades : WsBase, IWSUnidades
    {
        public List<Unidades> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var unidades = db.Unidades.ToList();

                return unidades;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Unidades get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                Unidades unidad = db.Unidades.Find(ID);
                return unidad;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Unidades add(Unidades unidad)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.Unidades.Add(unidad);
                db.SaveChanges();
                return unidad;

            }
            catch (Exception ex)
            {
                Error(ex, "La unidad ");
                return null;
            }
        }

        public Unidades update(Unidades unidad)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Unidades.Attach(unidad);
                db.Entry(unidad).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return unidad;
            }
            catch (Exception ex)
            {
                Error(ex, "La unidad ");
                return unidad;
            }
        }

        public Unidades delete(Unidades unidadsel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Unidades unidad = db.Unidades.Find(unidadsel.Id);

                db.Unidades.Attach(unidad);
                db.Unidades.Remove(unidad);
                db.SaveChanges();
                return unidad;
            }
            catch (Exception ex)
            {
                Error(ex,"La unidad ");
                return null;
            }
        }

        public List<Unidades> getUnidadesXGrupo(int grupoid)
        {
            try
            {
                Validar();
                List<Unidades> lstUnidades = new List<Unidades>();
                EmpresaContext db = new EmpresaContext();
                var detallegrupo = (from g in db.GruposUnidadesDetalle
                               where g.GrupoUnidadesId== grupoid
                               select g)
                               .ToList();

               
                foreach (GruposUnidadesDetalle dg in detallegrupo)
                {
                    var unidad = db.Unidades.Find(dg.UnidadEquivalenteId);

                    lstUnidades.Add(unidad);
                }

                return lstUnidades;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
