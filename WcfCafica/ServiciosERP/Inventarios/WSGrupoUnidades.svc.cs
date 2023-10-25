using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGrupoUnidades" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGrupoUnidades.svc or WSGrupoUnidades.svc.cs at the Solution Explorer and start debugging.
    public class WSGrupoUnidades : WsBase, IWSGrupoUnidades
    {
        public List<GruposUnidades> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var grupounidades = db.GruposUnidades.ToList();

                return grupounidades;

            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public GruposUnidades add(GruposUnidades grupounidad)
        {
            try
            {
                Validar();
                //Metodo para Agregar una Grupo Unidad
                EmpresaContext db = new EmpresaContext();
                db.GruposUnidades.Add(grupounidad);
                db.SaveChanges();
                return grupounidad;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public GruposUnidades get(int id)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                GruposUnidades grupounidad = db.GruposUnidades.Find(id);
                db.Entry(grupounidad).Collection(x => x.GruposUnidadesDetalle).Load();


                return grupounidad;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public GruposUnidades update(GruposUnidades grupounidad)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de Grupos Unidades

                EmpresaContext db = new EmpresaContext();

                //Se Obtiene la lista de los roles actuales del usuario, que se encuentran en la base de datos
                List<GruposUnidadesDetalle> GruposUnidadesDetallesActuales = db.GruposUnidadesDetalle.Where(e => e.GrupoUnidadesId == grupounidad.Id).ToList();
                List<GruposUnidadesDetalle> GruposUnidadesDetallesNuevos = grupounidad.GruposUnidadesDetalle.ToList();

                List<GruposUnidadesDetalle> GruposUnidadesDetallesEliminadas = GruposUnidadesDetallesActuales.Where(n => !GruposUnidadesDetallesNuevos.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                List<GruposUnidadesDetalle> GruposUnidadesDetallesNuevas = GruposUnidadesDetallesNuevos.Where(n => !GruposUnidadesDetallesActuales.Select(n1 => n1.Id).Contains(n.Id)).ToList();

                db.GruposUnidadesDetalle.AddRange(GruposUnidadesDetallesNuevas);
                db.GruposUnidadesDetalle.RemoveRange(GruposUnidadesDetallesEliminadas);

                //Modidifica los valores
                foreach (GruposUnidadesDetalle gp in grupounidad.GruposUnidadesDetalle.Where(item => item.Id != 0).ToList())
                {
                    //Obtiene el objeto de las base de datos
                    var item =db.GruposUnidadesDetalle.Find(gp.Id);

                    if(item != null)
                    {
                        //Asigna los valores que viene desde la aplicacion contra los de las base de datos
                        var detalle = db.Entry(item);
                        detalle.CurrentValues.SetValues(gp);
                    }
                }

                grupounidad.GruposUnidadesDetalle = null;

                db.GruposUnidades.Attach(grupounidad);
                db.Entry(grupounidad).State = System.Data.Entity.EntityState.Modified;


                db.SaveChanges();

                return grupounidad;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public GruposUnidades delete(GruposUnidades grupounidad)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                GruposUnidades Grupounidad = db.GruposUnidades.Find(grupounidad.Id);

                db.GruposUnidades.Remove(Grupounidad);
                db.SaveChanges();

                return Grupounidad;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
