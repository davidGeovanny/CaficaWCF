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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSModulos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSModulos.svc or WSModulos.svc.cs at the Solution Explorer and start debugging.
    public class WSModulos : WsBase, IWSModulos
    {
    /*    public List<Modulos> getModulos(Sistemas sistema)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todas las vistas
                var listaModulos = (from m in db.Modulos
                                 //  where m.BanEliminar == 0
                                   where m.SistemaId == sistema.Id
                                   select m).ToList();
                               
                return listaModulos;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }*/

        public List<Modulos> getall()
        {
            try
            {
                Validar();
                //Consulta que retorna todos los Modulos
                UsuariosContext db = new UsuariosContext();
                //var gpv = db.GruposVistas.Include("Sistemas").Where(x => x.BanEliminar == 0).ToList();
                var mod = db.Modulos.Include("Sistemas").ToList();
                return mod;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public Modulos get(int ID)
        {
            try
            {
                Validar();
                //Consulta que retorna el Modulo usando como parametro el ID
                UsuariosContext db = new UsuariosContext();
                Modulos Modulo = db.Modulos.Find(ID);
                return Modulo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Modulos add(Modulos modulo)
        {
            try
            {
                Validar();
                //Metodo para crear un registro nuevo de Modulos.
                UsuariosContext db = new UsuariosContext();
                db.Modulos.Add(modulo);
                db.SaveChanges();
                return modulo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Modulos update(Modulos modulo)
        {
            try
            {
                Validar();
                //Metodo para actualizar un Modulo / parametro vista
                UsuariosContext db = new UsuariosContext();
                db.Modulos.Attach(modulo);
                db.Entry(modulo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return modulo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Modulos delete(Modulos modulosel)
        {
            try
            {
                Validar();
                //Metodo para Eliminar un Modulo / parametro vista    
                UsuariosContext db = new UsuariosContext();
                Modulos modulo = db.Modulos.Find(modulosel.Id);
                db.Modulos.Remove(modulo);
                db.SaveChanges();
                return modulo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
