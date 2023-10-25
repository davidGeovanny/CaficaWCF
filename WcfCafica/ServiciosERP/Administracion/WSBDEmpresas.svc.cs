using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "WSBDEmpresas" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione WSBDEmpresas.svc o WSBDEmpresas.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class WSBDEmpresas : WsBase, IWSBDEmpresas
    {
        public List<BDEmpresas> getall()
        {
            try
            {
                Validar();
                //Metodo para Cargar las empresas al grid(Crea una lista)
                UsuariosContext db = new UsuariosContext();
                // var bdempresas = db.BDEmpresas.Where(x => x.BanEliminar==0).ToList();
               var bdempresas = db.BDEmpresas.ToList();
                return bdempresas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public BDEmpresas add(BDEmpresas empresas)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                UsuariosContext db = new UsuariosContext();
                db.BDEmpresas.Add(empresas);
                db.SaveChanges();
                //Crea la base de datos
                AdminBD dbAdmin = new AdminBD();
                dbAdmin.Database.ExecuteSqlCommand("CREATE DATABASE " + empresas.RFC);
                return empresas;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public BDEmpresas get(int ID)
        {
            try
            {
                Validar();
                //Metodo para cargar la empresa seleccionada del grid.
                UsuariosContext db = new UsuariosContext();
                BDEmpresas empresa = db.BDEmpresas.Find(ID);
                return empresa;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public BDEmpresas update(BDEmpresas empresas)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                UsuariosContext db = new UsuariosContext();
                db.BDEmpresas.Attach(empresas);
                db.Entry(empresas).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return empresas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return empresas;
            }
        }

        public BDEmpresas delete(BDEmpresas empresasel)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                UsuariosContext db = new UsuariosContext();
                BDEmpresas empresa = db.BDEmpresas.Find(empresasel.Id);

                db.BDEmpresas.Attach(empresa);
                db.BDEmpresas.Remove(empresa);
                db.SaveChanges();
                return empresa;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
