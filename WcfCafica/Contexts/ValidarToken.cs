using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.Contexts
{
    public class ValidarToken
    {
        //"Token" Token de autentifiacion del usuario, "TokenEmpresa" Token de los datos de la empresa conectada
        public string getKey(string key, string Token)
        {
            try
            {
                OperationContext currentContext = OperationContext.Current;
                HttpRequestMessageProperty reqMsg = currentContext.IncomingMessageProperties["httpRequest"] as HttpRequestMessageProperty;
                string authToken = reqMsg.Headers[Token];
                string value;
                if (authToken != null && authToken != "")
                {
                    var payload = JWT.JsonWebToken.DecodeToObject(authToken, "pwjrnew") as IDictionary<string, object>;
                    value = payload.ContainsKey(key) ? payload[key].ToString() : "";
                }
                else
                {
                    value = "";
                }
                return value;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //Valda token contra la base de datos
        public void Validar()
        {
            UsuariosContext db = new UsuariosContext();

            OperationContext currentContext = OperationContext.Current;
            HttpRequestMessageProperty reqMsg = currentContext.IncomingMessageProperties["httpRequest"] as HttpRequestMessageProperty;
            string authToken = reqMsg.Headers["Token"];

            //Busca el token en la tabla
            Tokens token = db.Tokens.Include("Versiones").Where(b => b.Token == authToken).FirstOrDefault();

            if (token == null)
                throw new Exception("Usuario no auntetificado.Tu sesion ha expirado.");

            if(token.Versiones.Version != reqMsg.Headers["Version"] )
                throw new Exception("Cuentas con una versión desactualizada del sistema, favor de cerrar y abrir el sistema para aplicar la actualización.");

            //Update para que se actualize la fecha
            db.Database.ExecuteSqlCommand(
                 "UPDATE Tokens SET FechaUltimaModificacion=NOW() WHERE Id=" + token.Id +";");
            db.SaveChanges();

            
        }
        //Valida token contra la base de datos para monedero brissa
        public void ValidarTokenWeb()
        {
            EmpresaContext db = new EmpresaContext();

            OperationContext currentContext = OperationContext.Current;
            HttpRequestMessageProperty reqMsg = currentContext.IncomingMessageProperties["httpRequest"] as HttpRequestMessageProperty;
            string authToken = reqMsg.Headers["Token"];

            //Busca el token en la tabla
            TokensMonedero token = db.TokensMonedero.Where(b => b.Token == authToken).FirstOrDefault();

            if (token == null)
                throw new Exception("Usuario no auntetificado.Tu sesion ha expirado.");

            //Update para que se actualize la fecha
            db.Database.ExecuteSqlCommand(
                 "UPDATE TokensMonedero SET FechaUltimaModificacion=NOW() WHERE Id=" + token.Id + ";");
            db.SaveChanges();


        }
        private long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }
    }
}