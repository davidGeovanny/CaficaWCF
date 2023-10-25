using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSPaises" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSPaises.svc or WSPaises.svc.cs at the Solution Explorer and start debugging.
    public class WSPaises : WsBase, IWSPaises
    {
        public List<Paises> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los paises
                var listaPaises = db.Paises.ToList();
                return listaPaises;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public Paises get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                Paises pais = db.Paises.Find(ID);
                return pais;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Paises add(Paises pais)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.Paises.Add(pais);
                db.SaveChanges();
                return pais;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }



        public Paises update(Paises pais)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Paises.Attach(pais);
                db.Entry(pais).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return pais;
            }
            catch (Exception ex)
            {
                Error(ex);
                return pais;
            }
        }

        public Paises delete(Paises paissel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Paises pais = db.Paises.Find(paissel.Id);

                db.Paises.Attach(pais);
                db.Paises.Remove(pais);
                db.SaveChanges();
                return pais;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        
    }
}
