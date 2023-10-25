using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Inventarios;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSSubGrupoComponentes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSSubGrupoComponentes.svc or WSSubGrupoComponentes.svc.cs at the Solution Explorer and start debugging.
    public class WSSubGrupoComponentes : WsBase, IWSSubGrupoComponentes
    {
        public List<SubgruposComponentes> getSubGruposComponentes()
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
                var subgrupocomponentes = db.SubgruposComponentes.ToList();

                return subgrupocomponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public SubgruposComponentes getSubGrupoComponente(int ID)
        {
            try
            {
                Validar();
                InventarioContext db = new InventarioContext();
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

        public SubgruposComponentes addSubGrupoComponente(SubgruposComponentes subgrupocomponentes)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                InventarioContext db = new InventarioContext();
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

        public SubgruposComponentes updateSubGrupoComponente(SubgruposComponentes subgrupocomponente)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                InventarioContext db = new InventarioContext();
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

        public SubgruposComponentes deleteSubGrupoComponente(int ID)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                InventarioContext db = new InventarioContext();
                SubgruposComponentes subgrupocomponente = db.SubgruposComponentes.Find(ID);

                db.SubgruposComponentes.Attach(subgrupocomponente);
                db.SubgruposComponentes.Remove(subgrupocomponente);
                db.SaveChanges();
                return subgrupocomponente;
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
                InventarioContext db = new InventarioContext();
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
