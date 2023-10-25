using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSColonias" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSColonias.svc or WSColonias.svc.cs at the Solution Explorer and start debugging.
    public class WSColonias : WsBase,IWSColonias
    {
        public List<Colonias> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades
                var listaCiudades = db.Colonias.Include("CodigosPostales").Include("Ciudades").Include("Municipios").Include("Estados").Include("Paises").ToList();

                return listaCiudades;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public List<Colonias> getColoniasCiudades(int id)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades de un municipio
                var listaColonias = db.Colonias.Where(x => x.CiudadId == id).ToList();

                return listaColonias;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<Colonias> getColoniasCP(int id)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();

                var listaCPs = db.Colonias.Where(c => c.CodigoPostalId == id).ToList();

                return listaCPs;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Colonias get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                Colonias colonia = db.Colonias.Find(ID);
                return colonia;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Colonias add(Colonias colonia)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.Colonias.Add(colonia);
                db.SaveChanges();
                return colonia;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }



        public Colonias update(Colonias colonia)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Colonias.Attach(colonia);
                db.Entry(colonia).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return colonia;
            }
            catch (Exception ex)
            {
                Error(ex);
                return colonia;
            }
        }

        public Colonias delete(Colonias coloniasel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Colonias colonia = db.Colonias.Find(coloniasel.Id);
                db.Colonias.Attach(colonia);
                db.Colonias.Remove(colonia);
                db.SaveChanges();
                return colonia;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
