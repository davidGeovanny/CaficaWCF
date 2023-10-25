using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSMunicipios" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSMunicipios.svc or WSMunicipios.svc.cs at the Solution Explorer and start debugging.
    public class WSMunicipios : WsBase, IWSMunicipios
    {
        public List<Municipios> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los municipios
                var listaMunicipios = db.Municipios.Include("Estados").Include("Paises").ToList();

                return listaMunicipios;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }
        public List<Municipios> getMunicipiosEstado(int IDEstado)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los municipios
                var listaMunicipios = db.Municipios.Where(x => x.EstadoId == IDEstado).ToList();

                return listaMunicipios;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public Municipios get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                Municipios municipio = db.Municipios.Find(ID);
                return municipio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Municipios add(Municipios municipio)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.Municipios.Add(municipio);
                db.SaveChanges();
                return municipio;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Municipios update(Municipios municipio)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Municipios.Attach(municipio);
                db.Entry(municipio).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return municipio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return municipio;
            }
        }

        public Municipios delete(Municipios municipiosel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Municipios municipio = db.Municipios.Find(municipiosel.Id);

                db.Municipios.Attach(municipio);
                db.Municipios.Remove(municipio);
                db.SaveChanges();
                return municipio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}

