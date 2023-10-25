using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSLogin" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSLogin.svc or WSLogin.svc.cs at the Solution Explorer and start debugging.
    public class WSLogin : IWSLogin
    {
        public string Login(Login acceso)
        {
            try
            {
                UsuariosContext db = new UsuariosContext();
                PasswordEncrypt encriptacion = new PasswordEncrypt();
                acceso.Contrasena = encriptacion.EncryptText(acceso.Contrasena);
                var login = db.Usuarios.Where(u => u.Nombre == acceso.Usuario).Where(u => u.Contrasena == acceso.Contrasena).ToList();
                if (login.ToList().Count > 0)
                {
                    Tokens Token = new Tokens();
                    Token.VersionId = 1;

                    long usuarioid = login.ToList()[0].Id;
                    var rolmysql = db.UsuariosRoles.FirstOrDefault(i => i.UsuarioId == usuarioid);

                    Token.Token = GenerarToken(login.ToList()[0].Id.ToString(), login.ToList()[0].Nombre, login.ToList()[0].Contrasena, db.Roles.Find(rolmysql.RolId).Nombre,rolmysql.RolId.ToString());

                    db.Tokens.Add(Token);
                    db.SaveChanges();

                    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                    response.Headers.Add("FechaActual",DateTime.Today.ToShortDateString());
                    
                    return Token.Token;
                }
                else
                    throw new Exception("101");
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
        public string LoginMonedero(Login acceso)
        {
            try
            {
                OperationContext currentContext = OperationContext.Current;
                HttpRequestMessageProperty reqMsg = currentContext.IncomingMessageProperties["httpRequest"] as HttpRequestMessageProperty;
                string bd = reqMsg.Headers["udn"];

                //Aplica el rol que le corresponde al usuario monedero
                string StrPassMonedero= System.Diagnostics.Debugger.IsAttached ? "nIo8gMeaM2zq3SaKVx/38g==" : "ZAItKxgK/lf606z9zhnhzw==";
                EmpresaContext setrol = new EmpresaContext("MONEDERO", StrPassMonedero, bd);
                setrol.Database.ExecuteSqlCommand("SET role rol_monedero");
                setrol.Database.ExecuteSqlCommand("SET DEFAULT role rol_monedero");
                setrol.Database.ExecuteSqlCommand("FLUSH PRIVILEGES");

                EmpresaContext db = new EmpresaContext("MONEDERO", StrPassMonedero, bd);

                PasswordEncrypt encriptacion = new PasswordEncrypt();

                acceso.Contrasena = encriptacion.EncryptText(acceso.Contrasena);
                var login = db.UsuariosMonedero.Where(u => u.Email == acceso.Usuario &&  u.Contrasena == acceso.Contrasena).ToList();

                if (login.ToList().Count > 0)
                {

                    if (login[0].Activo == "NO")
                        throw new Exception("102");

                    TokensMonedero Token = new TokensMonedero();
                    Token.Token = GenerarTokenMonedero(login.ToList()[0].Id.ToString(),"MONEDERO", StrPassMonedero, bd);

                    db.TokensMonedero.Add(Token);
                    db.SaveChanges();

                    return Token.Token;
                }
                else
                    throw new Exception("101");
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
        public static string GenerarToken(string id, string user,string password,string rol,string rolid)
        {
            try
            {
                var payload = new Dictionary<string, object>()
                {
                    { "id", id },
                    { "user", user },
                    { "password", password },
                    { "rol",rol},
                    { "rolid",rolid},
                    { "Fecha",DateTime.Now}
                };
                var secretKey = "pwjrnew";
                return JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string GenerarTokenMonedero(string id, string user, string password,string bd)
        {
            try
            {
                var payload = new Dictionary<string, object>()
                {
                    { "id", id },
                    { "user", user },
                    { "password", password },
                    { "bd", password },
                    { "Fecha",DateTime.Now}
                };
                var secretKey = "pwjrnew";
                return JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void GetOptions()
        {
        }
    }
}
