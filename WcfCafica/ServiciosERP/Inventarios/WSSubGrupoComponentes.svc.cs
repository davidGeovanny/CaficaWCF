using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSSubGrupoComponentes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSSubGrupoComponentes.svc or WSSubGrupoComponentes.svc.cs at the Solution Explorer and start debugging.
    public class WSSubGrupoComponentes : WsBase, IWSSubGrupoComponentes
    {
        public string[] CamposBloqueados = { "GrupoComponentesId" };

        public List<SubgruposComponentes> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var subgrupocomponentes = db.SubgruposComponentes.Include("GruposComponentes").ToList();

                return subgrupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public SubgruposComponentes get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                SubgruposComponentes subgrupocomponentes = db.SubgruposComponentes.Find(ID);
                return subgrupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public SubgruposComponentes add(SubgruposComponentes subgrupocomponentes)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.SubgruposComponentes.Add(subgrupocomponentes);
                db.SaveChanges();
                return subgrupocomponentes;

            }
            catch (Exception ex)
            {
                Error(ex, "El subgrupo ");
                return null;
            }
        }

        public SubgruposComponentes update(SubgruposComponentes subgrupocomponente)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                ValidarCamposBloqueados<SubgruposComponentes,EmpresaContext>(subgrupocomponente, CamposBloqueados, "Subgrupos de Componentes");

                db.SubgruposComponentes.Attach(subgrupocomponente);
                db.Entry(subgrupocomponente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return subgrupocomponente;
            }
            catch (Exception ex)
            {
                Error(ex, "El subgrupo ");
                return subgrupocomponente;
            }
        }

        public SubgruposComponentes delete(SubgruposComponentes subgrupocomponente)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                SubgruposComponentes Subgrupocomponente = db.SubgruposComponentes.Find(subgrupocomponente.Id);

                db.SubgruposComponentes.Attach(Subgrupocomponente);
                db.SubgruposComponentes.Remove(Subgrupocomponente);
                db.SaveChanges();
                return Subgrupocomponente;
            }
            catch (Exception ex)
            {
                Error(ex, "El subgrupo ");
                return null;
            }
        }

        public List<SubgruposComponentes> getSubGruposComponentesXGrupo(int grupoid)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var subgrupocomponentes = (from s in db.SubgruposComponentes
                                           where s.GrupoComponentesId == grupoid
                                           select s
                                           ).ToList();
                return subgrupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
