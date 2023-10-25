using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGrupoUnidades" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGrupoUnidades.svc or WSGrupoUnidades.svc.cs at the Solution Explorer and start debugging.
    public class WSGrupoUnidades : WsBase, IWSGrupoUnidades
    {
        public List<GruposUnidades> getGrupoUnidades()
        {
            try
            {
                Validar();

                InventarioContext db = new InventarioContext();
                var grupounidades = db.GruposUnidades.ToList();

                return grupounidades;

            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public GruposUnidades addGrupoUnidades(GruposUnidades grupounidad)
        {
            try
            {
                Validar();
                //Metodo para Agregar una Grupo Unidad
                InventarioContext db = new InventarioContext();
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
        public GruposUnidades getGrupoUnidad(int id)
        {
            try
            {
                Validar();

                InventarioContext db = new InventarioContext();
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
        public GruposUnidades updateGrupoUnidad(GruposUnidades grupounidad)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de Grupos Unidades

                InventarioContext db = new InventarioContext();

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
        public GruposUnidades  deleteGrupoUnidad(int ID)
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                GruposUnidades grupounidad = db.GruposUnidades.Find(ID);

                db.GruposUnidades.Remove(grupounidad);
                db.SaveChanges();

                return grupounidad;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
    }
}
