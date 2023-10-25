using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGrupoComponentes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGrupoComponentes.svc or WSGrupoComponentes.svc.cs at the Solution Explorer and start debugging.
    public class WSGrupoComponentes : WsBase,IWSGrupoComponentes
    {
        public List<GruposComponentes> getGruposComponentes()
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                var grupocomponentes = db.GruposComponentes.ToList();

                return grupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public GruposComponentes getGrupoComponente(int ID)
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                GruposComponentes grupocomponentes = db.GruposComponentes.Find(ID);
                return grupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public GruposComponentes addGrupoComponente(GruposComponentes grupocomponente)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                InventarioContext db = new InventarioContext();
                db.GruposComponentes.Add(grupocomponente);
                db.SaveChanges();
                return grupocomponente;

            }
            catch (Exception ex)
            {
                Error(ex, "El grupo ");
                return null;
            }
        }

        public GruposComponentes updateGrupoComponente(GruposComponentes grupocomponente)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                InventarioContext db = new InventarioContext();
                db.GruposComponentes.Attach(grupocomponente);
                db.Entry(grupocomponente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return grupocomponente;
            }
            catch (Exception ex)
            {
                Error(ex, "El grupo ");
                return grupocomponente;
            }
        }

        public GruposComponentes deleteGrupoComponente(int ID)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                InventarioContext db = new InventarioContext();
                GruposComponentes grupocomponente = db.GruposComponentes.Find(ID);

                db.GruposComponentes.Attach(grupocomponente);
                db.GruposComponentes.Remove(grupocomponente);
                db.SaveChanges();
                return grupocomponente;
            }
            catch (Exception ex)
            {
                Error(ex, "El grupo ");
                return null;
            }
        }

  
        public List<GruposComponentes> getGruposComponentesXTipo(int tipo)
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                var grupocomponentes = (from g in db.GruposComponentes
                                       where g.TipoComponenteId==tipo
                                       select g)
                                        .ToList();

                return grupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
