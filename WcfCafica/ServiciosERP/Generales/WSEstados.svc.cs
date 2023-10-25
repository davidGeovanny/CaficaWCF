using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSEstados" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSEstados.svc or WSEstados.svc.cs at the Solution Explorer and start debugging.
    public class WSEstados : WsBase, IWSEstados
    {
        public List<Estados> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todas los estados 
                var listaEstados = db.Estados.Include("Paises").ToList();

                return listaEstados;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }

        public List<Estados>  getEstadosPais(int IDPais)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todas los estados 
                var listaEstados = db.Estados.Where(x=> x.PaisId==IDPais).ToList();

                return listaEstados;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }


        }



        public Estados get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                Estados estado = db.Estados.Find(ID);
                return estado;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Estados add(Estados estado)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.Estados.Add(estado);
                db.SaveChanges();
                return estado;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }



        public Estados update(Estados estado)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.Estados.Attach(estado);
                db.Entry(estado).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return estado;
            }
            catch (Exception ex)
            {
                Error(ex);
                return estado;
            }
        }

        public Estados delete(Estados estadosel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Estados estado = db.Estados.Find(estadosel.Id);

                db.Estados.Attach(estado);
                db.Estados.Remove(estado);
                db.SaveChanges();
                return estado;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

    }
}
