using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSSistemas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSSistemas.svc or WSSistemas.svc.cs at the Solution Explorer and start debugging.
    public class WSSistemas : WsBase ,IWSSistemas
    {
        public List<Sistemas> getall()
        {
            try
            {
                Validar();
                //Consulta que retorna todos los Sistemas
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todas las vistas
                //var listaSistemas = db.Sistemas.Where(s => s.BanEliminar == 0).ToList();
                var listaSistemas = db.Sistemas.ToList();
                return listaSistemas;
               
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public Sistemas get(int ID)
        {
            try
            {
                Validar();
                //Consulta que retorna el Sistema usando como parametro el ID
                UsuariosContext db = new UsuariosContext();
                Sistemas Sistema = db.Sistemas.Find(ID);
                return Sistema;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Sistemas add(Sistemas sistema)
        {
            try
            {
                Validar();
                //Metodo para crear un registro nuevo de Sistemas.
                UsuariosContext db = new UsuariosContext();
                db.Sistemas.Add(sistema);
                db.SaveChanges();
                return sistema;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Sistemas update(Sistemas sistema)
        {
            try
            {
                Validar();
                //Metodo para actualizar un Sistema / parametro vista
                UsuariosContext db = new UsuariosContext();
                db.Sistemas.Attach(sistema);
                db.Entry(sistema).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return sistema;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Sistemas delete(Sistemas sistemasel)
        {
            try
            {
                Validar();
                //Metodo para Eliminar un Sistema / parametro vista    
                UsuariosContext db = new UsuariosContext();
                Sistemas sistema = db.Sistemas.Find(sistemasel.Id);
                
                db.Sistemas.Remove(sistema);
                db.SaveChanges();
                return sistema;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
