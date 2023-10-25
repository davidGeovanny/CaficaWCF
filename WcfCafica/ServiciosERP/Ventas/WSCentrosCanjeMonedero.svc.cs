using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;
using System.Data.Entity;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSCentrosCanjeMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSCentrosCanjeMonedero.svc or WSCentrosCanjeMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSCentrosCanjeMonedero : WsBase, IWSCentrosCanjeMonedero
    {
        public List<CentrosCanjeMonedero> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var centros = db.CentrosCanjeMonedero.Include("Almacenes").ToList();

                return centros;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public CentrosCanjeMonedero add(CentrosCanjeMonedero centro)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.CentrosCanjeMonedero.Add(centro);
                db.SaveChanges();
                return centro;
            }
            catch (Exception ex)
            {
                Error(ex, "El centro de canje");
                return null;
            }
        }
        public CentrosCanjeMonedero get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                CentrosCanjeMonedero centro = db.CentrosCanjeMonedero.Find(id);
                return centro;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public CentrosCanjeMonedero update(CentrosCanjeMonedero centro)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                db.CentrosCanjeMonedero.Attach(centro);
                db.Entry(centro).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return centro;
            }
            catch (Exception ex)
            {

                Error(ex, "El centro de canje");
                return null;
            }
        }
        public CentrosCanjeMonedero delete(CentrosCanjeMonedero cetroel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                CentrosCanjeMonedero centro = db.CentrosCanjeMonedero.Find(cetroel.Id);

                db.CentrosCanjeMonedero.Attach(centro);
                db.CentrosCanjeMonedero.Remove(centro);
                db.SaveChanges();
                return centro;
            }
            catch (Exception ex)
            {
                Error(ex, "El centro de canje");
                return null;
            }
        }
        //Retonar el id del del Centro del Canje al que tiene acceso el usuario
        public long getCentrosdeCanjexUsuario()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                //Obtiene el id del usuario
                long UsuarioId = int.Parse(getKey("id", "Token"));

                CentrosCanjeUsuariosMonedero centro = db.CentrosCanjeUsuariosMonedero.Where(c => c.UsuarioId==UsuarioId).SingleOrDefault();

                return centro.CentroCanjeMonederoId;

            }
            catch (Exception ex)
            {

                Error(ex, "El centro de canje");
                return 0;
            }
        }
    }
}
