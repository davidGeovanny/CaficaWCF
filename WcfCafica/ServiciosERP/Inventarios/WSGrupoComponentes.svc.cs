using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGrupoComponentes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGrupoComponentes.svc or WSGrupoComponentes.svc.cs at the Solution Explorer and start debugging.
    public class WSGrupoComponentes : WsBase, IWSGrupoComponentes
    {
        public string[] CamposBloqueados = { "TipoComponenteId" };

        public List<GruposComponentes> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var grupocomponentes = db.GruposComponentes.Include("TiposComponentes").ToList();

                return grupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public GruposComponentes get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
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

        public GruposComponentes add(GruposComponentes grupocomponente)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
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
        public GruposComponentes update(GruposComponentes grupocomponente)
        {
            try
            {
                Validar();

                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();

                ValidarCamposBloqueados<GruposComponentes,EmpresaContext>(grupocomponente,CamposBloqueados, "Grupos de Componentes");

                db.GruposComponentes.Attach(grupocomponente);
                //var x=db.Entry(grupocomponente).OriginalValues;
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
        public GruposComponentes delete(GruposComponentes grupocomponente)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                GruposComponentes Grupocomponente = db.GruposComponentes.Find(grupocomponente.Id);

                db.GruposComponentes.Attach(Grupocomponente);
                db.GruposComponentes.Remove(Grupocomponente);
                db.SaveChanges();
                return Grupocomponente;
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
                EmpresaContext db = new EmpresaContext();
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
