using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "WSEmpresas" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione WSEmpresas.svc o WSEmpresas.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class WSEmpresas : IWSEmpresas
    {

        public string[] CamposBloqueados = { "GrupoCompomentesId", "SubgruposComponentesId" };

        public List<BDEmpresas> getEmpresas()
        { 
            //Retorna las Empresas que tiene permiso el Usuario
            try
            {
                UsuariosContext db = new UsuariosContext();
                ValidarToken Token = new ValidarToken();

                long usuarioID = Convert.ToInt64(Token.getKey("id", "Token"));

                UsuariosContext setrol = new UsuariosContext("");
                //Buscar el rol que le corresponde al usuarios
                setrol.Database.ExecuteSqlCommand("SET role " + Token.getKey("rol", "Token"));
                setrol.Database.ExecuteSqlCommand("SET DEFAULT role " + Token.getKey("rol", "Token"));
                setrol.Database.ExecuteSqlCommand("FLUSH PRIVILEGES");

                Token.Validar();

                var empresas = (from ur in db.UsuariosRoles
                                join bder in db.BDEmpresasRoles on ur.RolId equals bder.RolId
                                join bde in db.BDEmpresas on bder.BDEmpresaId equals bde.Id
                                where ur.UsuarioId == usuarioID
                                select bde
                                ).ToList();
                return empresas;
            }
            catch (Exception ex)
            {

                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                string error = null;
                if (ex is DbEntityValidationException)
                    error = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors.ToList()[0].ValidationErrors.ToList()[0].ErrorMessage;
                else if (ex is DbUpdateException)
                    error = ex.InnerException.InnerException.Message;
                else if (ex is EntityCommandExecutionException)
                    error = ex.InnerException.Message;
                else
                    error = ex.Message;
                response.Headers.Add("Error", error);
                return null;
            }
        }
        public string getTokenEmpresas(BDEmpresas empresa)
        {
            try
            {
                ValidarToken Token = new ValidarToken();
                Token.Validar();
                return GenerarToken(empresa.Id.ToString(),empresa.RFC,"",empresa.ContrasenaReportes,empresa.RazonSocial);
            }
            catch (Exception ex)
            {

                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                string error = null;
                if (ex is DbEntityValidationException)
                    error = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors.ToList()[0].ValidationErrors.ToList()[0].ErrorMessage;
                else if (ex is DbUpdateException)
                    error = ex.InnerException.InnerException.Message;
                else if (ex is EntityCommandExecutionException)
                    error = ex.InnerException.Message;
                else
                    error = ex.Message;
                response.Headers.Add("Error", error);
                return null;
            }
        }
        public static string GenerarToken(string id, string bd, string servidor,string password_reportes,string razonsocial)
        {
            var payload = new Dictionary<string, object>()
                {
                    { "id", id },
                    { "bd", bd },
                    { "servidor", servidor },
                    { "password_reportes",password_reportes},
                    { "razonsocial",razonsocial},
                    { "Fecha",DateTime.Now}
                };
            var secretKey = "pwjrnew";
            return JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256);
        }

    }
}
