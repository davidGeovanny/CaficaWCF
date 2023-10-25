using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Compras
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSGrupoProveedores" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSGrupoProveedores.svc or WSGrupoProveedores.svc.cs at the Solution Explorer and start debugging.
    public class WSGrupoProveedores :WsBase,IWSGrupoProveedores
    {
        public List<GrupoProveedores> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los paises
                var listagrupos = db.GrupoProveedores.ToList();
                return listagrupos;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public GrupoProveedores get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                GrupoProveedores grupo = db.GrupoProveedores.Find(ID);
                return grupo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public GrupoProveedores add(GrupoProveedores grupo)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.GrupoProveedores.Add(grupo);
                db.SaveChanges();
                return grupo;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }



        public GrupoProveedores update(GrupoProveedores grupo)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.GrupoProveedores.Attach(grupo);
                db.Entry(grupo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return grupo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return grupo;
            }
        }

        public GrupoProveedores delete(GrupoProveedores grupo)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                GrupoProveedores grupobuscar = db.GrupoProveedores.Find(grupo.Id);

                db.GrupoProveedores.Attach(grupobuscar);
                db.GrupoProveedores.Remove(grupobuscar);
                db.SaveChanges();
                return grupobuscar;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
