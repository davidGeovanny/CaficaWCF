using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSCodigosPostales" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSCodigosPostales.svc or WSCodigosPostales.svc.cs at the Solution Explorer and start debugging.
    public class WSCodigosPostales : WsBase, IWSCodigosPostales
    {
        public List<CodigosPostales> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades
                var listaCodigosPostales = db.CodigosPostales.Include("Ciudades").Include("Municipios").Include("Estados").Include("Paises").ToList();

                return listaCodigosPostales;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public List<CodigosPostales> getCodigosPostalesCiudad(int IDCiudad)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades
                var listaCodigosPostales = db.CodigosPostales.Where(x => x.CiudadId == IDCiudad).ToList();

                return listaCodigosPostales;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }


        public CodigosPostales get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                CodigosPostales codigopostal = db.CodigosPostales.Find(ID);
                return codigopostal;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public CodigosPostales add(CodigosPostales codigopostal)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.CodigosPostales.Add(codigopostal);
                db.SaveChanges();
                return codigopostal;

            }
            catch (Exception ex)
            {
                Error(ex,"codigo postal");
                return null;
            }
        }



        public CodigosPostales update(CodigosPostales codigopostal)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.CodigosPostales.Attach(codigopostal);
                db.Entry(codigopostal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return codigopostal;
            }
            catch (Exception ex)
            {
                Error(ex);
                return codigopostal;
            }
        }

        public CodigosPostales delete(CodigosPostales codigopostalsel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                CodigosPostales codigopostal = db.CodigosPostales.Find(codigopostalsel.Id);
                db.CodigosPostales.Attach(codigopostal);
                db.CodigosPostales.Remove(codigopostal);
                db.SaveChanges();
                return codigopostal;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
