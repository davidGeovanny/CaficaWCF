using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{ 
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSUsuariosMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSUsuariosMonedero.svc or WSUsuariosMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSUsuariosMonedero : WsBase, IWSUsuariosMonedero
    {
        public List<UsuariosMonedero> getall()
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }

        public UsuariosMonedero add(UsuariosMonedero usuario)
        {
            try
            {
                PasswordEncrypt encriptacion = new PasswordEncrypt();
                OperationContext currentContext = OperationContext.Current;
                HttpRequestMessageProperty reqMsg = currentContext.IncomingMessageProperties["httpRequest"] as HttpRequestMessageProperty;
                string bd = reqMsg.Headers["udn"];

                //UsuariosContext setrol = new UsuariosContext("");
                EmpresaContext setrol = new EmpresaContext("root", "Ml3qB4FCz9IMkrlWx8OKBQ==", bd);
                //Aplica el rol que le corresponde al usuario monedero
                setrol.Database.ExecuteSqlCommand("SET role rol_monedero");
                setrol.Database.ExecuteSqlCommand("SET DEFAULT role rol_monedero");
                setrol.Database.ExecuteSqlCommand("FLUSH PRIVILEGES");

                //Metodo para Agregar una empresa
                string StrPassMonedero = System.Diagnostics.Debugger.IsAttached ? "nIo8gMeaM2zq3SaKVx/38g==" : "ZAItKxgK/lf606z9zhnhzw==";
                EmpresaContext db = new EmpresaContext("MONEDERO", StrPassMonedero, bd);

                usuario.Activo = "NO";
                usuario.Contrasena = encriptacion.EncryptText(usuario.Contrasena);
                db.UsuariosMonedero.Add(usuario);
                db.SaveChanges();

                string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/correo.html");

                using (StreamReader sr = new StreamReader(strFilePath2))
                {
                    // Read the stream to a string, and write the string to the console.
                    String mail = sr.ReadToEnd();
                    mail = mail.Replace("@nombre", usuario.Nombre);
                    string IpServidor = System.Diagnostics.Debugger.IsAttached ? "localhost:54363" : "conemzt.dyndns.org:5556/WcfCafica";
                    mail = mail.Replace("@link", "http://"+ IpServidor  + "/ServiciosERP/Ventas/WSActivacionesCorreos.svc/registrobrissa/" + GenerarToken(usuario.Id.ToString(),usuario.Email,bd)); 
                    SendEmail(usuario.Email, "Monedero Brissa", mail);
                }

                return usuario;
            }
            catch (Exception ex)
            {
                Error(ex, "El usuario");
                return null;
            }
        }
        public UsuariosMonedero get(int id)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public UsuariosMonedero update(UsuariosMonedero usuario)
        {
            try
            {
                ValidarWebMonedero();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));

                UsuariosMonedero UsuarioActual = db.UsuariosMonedero.Single(u => u.Id==UsuarioMonederoId);
                usuario.Contrasena = UsuarioActual.Contrasena;
                usuario.Email = UsuarioActual.Email;
                usuario.PaisId = 1;
                usuario.RegistroCompletado = "SI";
                db.Entry(UsuarioActual).CurrentValues.SetValues(usuario);

                db.SaveChanges();
                return usuario;
            }
            catch (Exception ex)
            {

                Error(ex, "El usuarios monedero");
                return null;
            }
        }
        public UsuariosMonedero delete(UsuariosMonedero almacen)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El usuario monedero");
                return null;
            }
        }
        public static string GenerarToken(string usuarioid, string correo,string bd)
        {
            try
            {
                var payload = new Dictionary<string, object>()
                {
                    { "usuarioid", usuarioid },
                    { "correo", correo },
                    { "bd", bd },
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
        public UsuariosMonedero getUsuarioWeb()
        {
            try
            {
                ValidarWebMonedero();

                EmpresaContext db = new EmpresaContext();

                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));

                UsuariosMonedero usuario = db.UsuariosMonedero.Find(UsuarioMonederoId);

                usuario.Contrasena = null;

                return usuario;
            }
            catch (Exception ex)
            {
                Error(ex, "El usuario");
                return null;
            }
        }
        public bool getVerificarRegistro()
        {
            try
            {
                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));

                EmpresaContext db = new EmpresaContext();

                return db.UsuariosMonedero.Find(UsuarioMonederoId).RegistroCompletado == "SI" ? true : false;
            }
            catch (Exception ex)
            {

                Error(ex, "El usuario");
                return false;
            }
        }
    }
}
