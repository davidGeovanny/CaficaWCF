using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSSociosMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSSociosMonedero.svc or WSSociosMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSSociosMonedero : WsBase, IWSSociosMonedero
    {
        public List<SociosMonedero> getall()
        {
            try
            {
                ValidarWebMonedero();

                EmpresaContext db = new EmpresaContext();
                var socios = db.SociosMonedero.ToList();

                return socios;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public List<SociosMonedero> getSociosOGA()
        {
            try
            {
                ValidarWebMonedero();

                EmpresaContext db = new EmpresaContext();
                var socios = db.SociosMonedero.Where(x=>x.GrupoSocio=="OGA").ToList();

                return socios;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }


        public SociosMonedero add(SociosMonedero movimiento)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.SociosMonedero.Add(movimiento);
                db.SaveChanges();
                return movimiento;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public SociosMonedero get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                SociosMonedero socio = db.SociosMonedero.Find(id);
                return socio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public SociosMonedero update(SociosMonedero movimiento)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.SociosMonedero.Attach(movimiento);
                db.Entry(movimiento).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return movimiento;
            }
            catch (Exception ex)
            {
                Error(ex);
                return movimiento;
            }
        }
        public SociosMonedero delete(SociosMonedero movimiento)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                SociosMonedero socio = db.SociosMonedero.Find(movimiento.Id);

                db.SociosMonedero.Attach(movimiento);
                db.SociosMonedero.Remove(movimiento);
                db.SaveChanges();
                return socio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
