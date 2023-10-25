using System;
using System.Collections.Generic;
using System.Linq;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "WSAccionesVistas" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione WSAccionesVistas.svc o WSAccionesVistas.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class WSAccionesVistas : WsBase, IWSAccionesVistas
    {
        public List<AccionesVistas> getall()
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todas las AccionesVistas
                //  var listaVistas = db.Vistas.Where(v => v.BanEliminar == 0).ToList();
                var listaAccionesVistas = db.AccionesVistas.Include("Vistas").ToList();
                return listaAccionesVistas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;

            }

        }

        public AccionesVistas get(int ID)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                AccionesVistas Accionesvistas = db.AccionesVistas.Find(ID);
                return Accionesvistas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public AccionesVistas add(AccionesVistas accionesVistas)
        {
            try
            {
                Validar();
                //Metodooo para añadir una AccionesVistas
                UsuariosContext db = new UsuariosContext();
                db.AccionesVistas.Add(accionesVistas);
                db.SaveChanges();
                return accionesVistas;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public AccionesVistas update(AccionesVistas accionesVistas)
        {
            try
            {
                Validar();
                //Metodo que Actualiza una AccionesVistas
                UsuariosContext db = new UsuariosContext();
                db.AccionesVistas.Attach(accionesVistas);
                db.Entry(accionesVistas).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return accionesVistas;

            }
            catch (Exception ex)
            {
                Error(ex);
                return accionesVistas;       
            }

        }

        public AccionesVistas delete(AccionesVistas accionVista)
        {
            try
            {
                Validar();
                // Metodo para eliminar accionesVistas  
                UsuariosContext db = new UsuariosContext();
                AccionesVistas accionesVistas = db.AccionesVistas.Find(accionVista.Id);

                db.AccionesVistas.Attach(accionesVistas);
                db.AccionesVistas.Remove(accionesVistas);
                db.SaveChanges();
                return accionesVistas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
