using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;
using WcfCafica.Contexts.Reportes;

namespace WcfCafica.ServiciosERP
{
    public class WsBase
    {
        ValidarToken Token = new ValidarToken();
        public DateTime FechaInicio,FechaFin;
        public bool DesactivarPermisos { get; set; }

        public WsBase()
        {
            OperationContext currentContext = OperationContext.Current;
            HttpRequestMessageProperty reqMsg = currentContext.IncomingMessageProperties["httpRequest"] as HttpRequestMessageProperty;
            FechaInicio = Convert.ToDateTime(reqMsg.Headers["FechaInicio"]);
            FechaFin = Convert.ToDateTime(reqMsg.Headers["FechaFin"]);
        }
        public void Error(Exception ex, String nombrevista)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            string error = null;
           
           

            if (ex is DbEntityValidationException)

                error = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors.ToList()[0].ValidationErrors.ToList()[0].ErrorMessage;

            else if (ex is DbUpdateException) {
                var sqlException = ex.InnerException.InnerException as MySql.Data.MySqlClient.MySqlException;

                if (sqlException.Number==1451)  //NO SE PUEDE ELIMINAR POR QUE TIENE LLAVES FORANEAS
                     {
                error = string.Format("No es posible eliminar {0} porque esta en uso en el sistema", nombrevista);
                     }
                else if(sqlException.Number == 1062)  //YA EXISTE EL ITEM EN EL SISTEMA
                    error = string.Format("{0} ya existe en el sistema", nombrevista);
                else
                error = ex.InnerException.InnerException.Message;
            }
            else if (ex is EntityCommandExecutionException)
                error = ex.InnerException.Message;
            else
            error = ex.Message;

            response.Headers.Add("Error", error);
            
        }

        public void Error(Exception ex)
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

        }


        public void Validar()
        {
            Token.Validar(); 
        }
        public void ValidarTokenWeb()
        {
            Token.ValidarTokenWeb();
        }
        public string getKey(string key,string token)
        {
            try
            {
                return Token.getKey(key,token);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void SendEmail(string toAddress, string subject, string body, string Id,string urlreporte)
        {
            //string result = "Message Sent Successfully..!!";
            string senderID = "soporte@cafica.com.mx";// use sender’s email id here..
            const string senderPassword = "Tampaola_1986"; // sender password here…

            reportParameter parametro = new reportParameter();
            parametro.name = "Id";
            parametro.value.Add(Id); 
            VisorReporteViewModel VmReporte = new VisorReporteViewModel(urlreporte, parametro, "pdf");
            string NombrePdf = VmReporte.GenerarReporte();

            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(senderID, senderPassword),
                    Timeout = 30000,
                };
                MailMessage message = new MailMessage(senderID, toAddress, subject, body);

                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Application.Octet;
                contentType.Name = NombrePdf;

                ValidarToken Token = new ValidarToken();
                string BdEmpresa = Token.getKey("bd", "TokenEmpresa");
                message.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("/" + NombrePdf), contentType));
                message.IsBodyHtml = true;
                //smtp.Send(message);
                smtp.SendAsync(message, message);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SendEmail(string toAddress, string subject, string body)
        {
            //string result = "Message Sent Successfully..!!";
            string senderID = "soporte@cafica.com.mx";// use sender’s email id here..
            const string senderPassword = "Tampaola_1986"; // sender password here…

            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(senderID, senderPassword),
                    Timeout = 30000,
                };
                MailMessage message = new MailMessage(senderID, toAddress, subject, body);
                message.IsBodyHtml = true;
        
                smtp.SendAsync(message, message);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ValidarCamposBloqueados<Tipo, Context>(Tipo ItemNuevo, string[] CamposBloqueados,string Vista) where Tipo : class
                                                                                            where Context : DbContext
        {
            try
            {
                ValidarToken Token = new ValidarToken();
                Context db = Activator.CreateInstance<Context>();
                UsuariosContext UContext = new UsuariosContext();

                //Query para obtener las tablas con las que esta relacionado el objeto
                var tablas = db.Database.SqlQuery<RelacionesModel>("SELECT TABLE_NAME,COLUMN_NAME,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE CONSTRAINT_SCHEMA = '" + Token.getKey("bd", "TokenEmpresa") + "' AND " +
                                           "REFERENCED_TABLE_SCHEMA IS NOT NULL AND REFERENCED_TABLE_NAME IS NOT NULL AND REFERENCED_COLUMN_NAME " +
                                           "IS NOT NULL AND REFERENCED_TABLE_NAME = '" + ItemNuevo.GetType().Name + "'").ToList();

                foreach (string campo in CamposBloqueados)
                {
                    foreach (RelacionesModel tabla in tablas)
                    {
                        //Buscas las tablas que pertencen a la vista en ejecucion para exluirlas.
                        Vistas vista = UContext.Vistas.Where(v => v.Nombre == Vista).SingleOrDefault();
                        if (UContext.VistasTablas.Include("Tablas").Where(v => v.VistaId == vista.Id).Where(vt => vt.Tablas.Nombre == tabla.TABLE_NAME).Count() < 1)
                        {
                            //Buscas dinamicamente el estado acutal de la entidad
                            var ValorActual = db.Set<Tipo>().Find(ItemNuevo.GetType().GetProperty(tabla.REFERENCED_COLUMN_NAME).GetValue(ItemNuevo, null));

                            //Compara el valor actual con el valor nuevo
                            if (ValorActual.GetType().GetProperty(campo).GetValue(ValorActual, null).ToString() != ItemNuevo.GetType().GetProperty(campo).GetValue(ItemNuevo, null).ToString())
                            {

                                //db.Set<GruposComponentes>().Where(m => IsYourPredicateSatisfied(m));
                                int total = db.Database.SqlQuery<string>("SELECT Id FROM " + tabla.TABLE_NAME + " WHERE " + tabla.COLUMN_NAME + "=" + ItemNuevo.GetType().GetProperty(tabla.REFERENCED_COLUMN_NAME).GetValue(ItemNuevo, null).ToString()).Count();

                                if (total > 0)
                                    throw new Exception("Este registro no puede ser modificado, por que ya tiene movimientos en algun documento o catalogo");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool TienePermisoEspecial(long accionid, UsuariosContext udb, ValidarToken Token)
        {
            try
            {
                if (DesactivarPermisos == true)
                    return true;

           /*     UsuariosContext udb = new UsuariosContext();
                ValidarToken Token = new ValidarToken();*/
                string rol = Token.getKey("rolid", "Token");
                long rolid = long.Parse(rol);
                var query = udb.RolesAcciones.Where(r => r.RolId == rolid).Where(a => a.AccionId == accionid).FirstOrDefault();
                return query == null ? false : true;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CalcularPeriodo(DateTime fechacaptura)
        {
            try
            {
                UsuariosContext udb = new UsuariosContext();
                ValidarToken Token = new ValidarToken();
                string rfc = Token.getKey("bd", "TokenEmpresa");
                // obetener fecha inicio que tiene configurado la empresa
                DateTime fechainicio =(DateTime)udb.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().InicioPeriodo;
                // obetener fecha fin que tiene configurado la empresa
                DateTime fechafin = (DateTime)udb.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().FinPeriodo;
                if (!((fechacaptura.Date >= fechainicio.Date) && (fechacaptura.Date <= fechafin.Date)))//SE PONE SIGNO DE CONTRARIO EN  EL IF PARA EVITAR PONER EL ELSE
                    throw new Exception("La fecha esta incorrecta." + "  Debe ser una fecha del periodo de captura." + "  Periodo: " + fechainicio.ToShortDateString() + " al " + fechafin.ToShortDateString());//NO ESTA EN EL PERIODO
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double ExistenciaActualComponente(InventariosESDetalles inventarioentradaosalida)
        {
            try
            {
                EmpresaContext db = new EmpresaContext();
                double existencia=0.0;
                var eys = db.InventariosSaldos.Where(s => s.ComponenteId == inventarioentradaosalida.ComponenteId).Where(a => a.AlmacenId == inventarioentradaosalida.AlmacenId).ToList();
                if (!(eys.Count == 0))
                    existencia = (eys.Sum(a => a.EntradasUnidades)) - (eys.Sum(a => a.SalidasUnidades));

                return existencia;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0.0;

            }
        }
        public double ExistenciaActualComponente(long almacenid,long componenteid)
        {
            try
            {
                EmpresaContext db = new EmpresaContext();
                double existencia = 0.0;
                var eys = db.InventariosSaldos.Where(s => s.ComponenteId == componenteid).Where(a => a.AlmacenId == almacenid).ToList();
                if (!(eys.Count == 0))
                    existencia = (eys.Sum(a => a.EntradasUnidades)) - (eys.Sum(a => a.SalidasUnidades));

                return existencia;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0.0;

            }
        }
        //Valida conexion web
        public void ValidarWebMonedero()
        {
            try
            {
                ValidarTokenWeb();

            }
            catch (Exception ex)
            {
                try
                {

                    Validar();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public void UpdateLista<T>(List<T> ListaDetallesActuales, List<T> ListaDetallesNuevos, EmpresaContext db) where T : class
        {
            //Busca los item que seran elimninado y los nuevos sacando una diferencia entre cada lista controlado por el id
            List<T> ListaDetallesEliminadas = ListaDetallesActuales.Where(n => !ListaDetallesNuevos.Select(n1 => n1.GetType().GetProperty("Id").GetValue(n1, null)).Contains(n.GetType().GetProperty("Id").GetValue(n, null))).ToList();
            List<T> ListaDetallesNuevas = ListaDetallesNuevos.Where(n => !ListaDetallesActuales.Select(n1 => n1.GetType().GetProperty("Id").GetValue(n1, null)).Contains(n.GetType().GetProperty("Id").GetValue(n, null))).ToList();

            //Agrege e elimina en base al resultado
            db.Set<T>().AddRange(ListaDetallesNuevas);
            db.Set<T>().RemoveRange(ListaDetallesEliminadas);

            //Modidifica los valores
            foreach (T lt in ListaDetallesNuevos.Where(item => item.GetType().GetProperty("Id").GetValue(item, null).ToString() != "0").ToList())
            {
                //Obtiene el objeto de las base de datos
                var item = db.Set<T>().Find(lt.GetType().GetProperty("Id").GetValue(lt, null));

                if (item != null)
                {
                    //Asigna los valores que viene desde la aplicacion contra los de las base de datos
                    var detalle = db.Entry(item);
                    detalle.CurrentValues.SetValues(lt);
                }
            }
        }
        //Metodo para dar respuesta las peticiones OPTION CORS
        public void GetOptions()
        {
        }
    }
}