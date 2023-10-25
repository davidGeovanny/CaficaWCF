using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSCiudades" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSCiudades.svc or WSCiudades.svc.cs at the Solution Explorer and start debugging.
    public class WSCiudades : WsBase, IWSCiudades
    {
        public List<Ciudades> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades
                var listaCiudades = db.Ciudades.Include("Municipios").Include("Estados").Include("Paises").ToList();

                return listaCiudades;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }
        public List<Ciudades> getCiudadesMunicipio(int IDMunicipio)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades de un municipio
                var listaCiudades = db.Ciudades.Where(x => x.MunicipioId == IDMunicipio).ToList();

                return listaCiudades;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public Ciudades get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                Ciudades ciudad = db.Ciudades.Find(ID);
                return ciudad;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Ciudades add(Ciudades ciudad)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.Ciudades.Add(ciudad);
                db.SaveChanges();
                return ciudad;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }



        public Ciudades update(Ciudades ciudad)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Ciudades.Attach(ciudad);
                db.Entry(ciudad).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return ciudad;
            }
            catch (Exception ex)
            {
                Error(ex);
                return ciudad;
            }
        }

        public Ciudades delete(Ciudades ciudadsel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Ciudades ciudad = db.Ciudades.Find(ciudadsel.Id);
                db.Ciudades.Attach(ciudad);
                db.Ciudades.Remove(ciudad);
                db.SaveChanges();
                return ciudad;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

    }
}

