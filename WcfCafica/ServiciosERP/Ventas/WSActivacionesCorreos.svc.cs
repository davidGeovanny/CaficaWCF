using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSActivacionesCorreos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSActivacionesCorreos.svc or WSActivacionesCorreos.svc.cs at the Solution Explorer and start debugging.
    public class WSActivacionesCorreos : WsBase,IWSActivacionesCorreos
    {
        public string getKeyToken(string key, string Token)
        {
            try
            {
                var payload = JWT.JsonWebToken.DecodeToObject(Token, "pwjrnew") as IDictionary<string, object>;
                string value = payload.ContainsKey(key) ? payload[key].ToString() : "";

                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public System.IO.Stream registrobrissa(string token)
        {
            try
            {
                string bd = getKeyToken("bd", token);
                string correo = getKeyToken("correo", token);
                long usuarioid = Convert.ToInt64(getKeyToken("usuarioid", token));

                string StrPassMonedero = System.Diagnostics.Debugger.IsAttached ? "nIo8gMeaM2zq3SaKVx/38g==" : "ZAItKxgK/lf606z9zhnhzw==";
                EmpresaContext db = new EmpresaContext("MONEDERO", StrPassMonedero, bd);

                UsuariosMonedero usuario = db.UsuariosMonedero.Where(c => c.Id == usuarioid && c.Email == correo).SingleOrDefault();

                if (usuario == null)
                    throw new Exception("Token invalido");

                usuario.Activo = "SI";
                db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/respuesta.html");

                string result;
                byte[] resultBytes;

                using (StreamReader sr = new StreamReader(strFilePath2))
                {
                    //result = "<a href='someLingk' >Some link</a>";
                    result = sr.ReadToEnd();
                    result = result.Replace("@nombre", usuario.Nombre);
                    result = result.Replace("@link", "http://localhost:54363/MonederoElectronico/");
                    resultBytes = Encoding.UTF8.GetBytes(result);
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                }

                return new MemoryStream(resultBytes);
            }
            catch (Exception ex)
            {

                Error(ex, "El token");
                return null;
            }
        }
    }
}
