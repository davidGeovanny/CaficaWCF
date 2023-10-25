using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSTablas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSTablas.svc or WSTablas.svc.cs at the Solution Explorer and start debugging.
    public class WSTablas : WsBase, IWSTablas
    {
        public List<Tablas> getall()
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todas las tablas
                var listaTablas = db.Tablas.ToList();

                return listaTablas;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public Tablas get(int ID)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                Tablas tabla = db.Tablas.Find(ID);
                return tabla;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Tablas add(Tablas tabla)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                UsuariosContext db = new UsuariosContext();
                db.Tablas.Add(tabla);
                db.SaveChanges();
                return tabla;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

      

        public Tablas update(Tablas tabla)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                UsuariosContext db = new UsuariosContext();
                db.Tablas.Attach(tabla);
                db.Entry(tabla).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return tabla;
            }
            catch (Exception ex)
            {
                Error(ex);
                return tabla;
            }
        }

        public Tablas delete(Tablas tablasel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                UsuariosContext db = new UsuariosContext();
                Tablas tabla = db.Tablas.Find(tablasel.Id);

                db.Tablas.Attach(tabla);
                db.Tablas.Remove(tabla);
                db.SaveChanges();
                return tabla;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

    }
}
