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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGruposVistas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGruposVistas.svc or WSGruposVistas.svc.cs at the Solution Explorer and start debugging.
    public class WSGruposVistas : WsBase ,IWSGruposVistas
    {
  /*      public List<GruposVistas> getall()
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todas las vistas
                var listaGruposVistas = (from m in db.GruposVistas
                                       //  where m.BanEliminar == 0
                                         where m.ModuloId == modulo.Id
                                         select m).ToList();

                return listaGruposVistas;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }*/
        public List<GruposVistas> getall()
        {
            try
            {
                Validar();
                //Consulta que retorna todas los Grupos Vistas
                UsuariosContext db = new UsuariosContext();
                //var gpv = db.GruposVistas.Include("Modulos").Include("Sistemas").Where(x => x.BanEliminar == 0).ToList();
                var gpv = db.GruposVistas.Include("Modulos").Include("Sistemas").ToList();
                return gpv;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public GruposVistas get(int ID)
        {
            try
            {
                Validar();
                //Consulta que retorna el Grupo Vista usando como parametro el ID
                UsuariosContext db = new UsuariosContext();
                GruposVistas GrupoVista = db.GruposVistas.Find(ID);
                return GrupoVista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public GruposVistas add(GruposVistas grupoVista)
        {
            try
            {
                Validar();
                //Metodo para crear un registro nuevo de Grupos Vistas.
                UsuariosContext db = new UsuariosContext();
                db.GruposVistas.Add(grupoVista);
                db.SaveChanges();
                return grupoVista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public GruposVistas update(GruposVistas grupoVista)
        {
            try
            {
                Validar();
                //Metodo para actualizar un Grupo Vista / parametro vista
                UsuariosContext db = new UsuariosContext();
                db.GruposVistas.Attach(grupoVista);
                db.Entry(grupoVista).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return grupoVista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public GruposVistas delete(GruposVistas grupoVista)
        {
            try
            {
                Validar();
                //Metodo para Eliminar un Grupo Vista / parametro vista   
                UsuariosContext db = new UsuariosContext();
                GruposVistas gruposVistas = db.GruposVistas.Find(grupoVista.Id);

                db.GruposVistas.Remove(gruposVistas);
                db.SaveChanges();
                
                return gruposVistas;
            }
            catch (Exception ex)
            {
                Error(ex, "este grupo vista");
                return null;
            }
        }
    }
}
