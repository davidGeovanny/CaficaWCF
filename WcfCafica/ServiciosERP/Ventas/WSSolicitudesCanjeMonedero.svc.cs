using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Entity;
using WcfCafica.Contexts.Empresa;
using WcfCafica.Contexts.Monedero;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSSolicitudesCanjeMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSSolicitudesCanjeMonedero.svc or WSSolicitudesCanjeMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSSolicitudesCanjeMonedero : WsBase, IWSSolicitudesCanjeMonedero
    {
        public List<SolicitudesCanjeMonedero> getall()
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
        public SolicitudesCanjeMonedero add(SolicitudesCanjeMonedero solicitud)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "La solicitud");
                return null;
            }
        }
        public string addSolicitudWeb(CanjeMonedero data)
        {
            try
            {
                //Variable para obtener total real de los productos desde la bd
                int total=0;

                //Variable 
                string LstPremiosCorreo="";

                ValidarWebMonedero();
                long UsuarioMonederoId= int.Parse(getKey("id", "Token"));
                EmpresaContext db = new EmpresaContext();

                //Configuracion de la empresa
                ConfiguracionMonedero Config = db.ConfiguracionMonedero.Find(1);

                SolicitudesCanjeMonedero solicitud = new SolicitudesCanjeMonedero();
                solicitud.UsuarioMonederoId = UsuarioMonederoId;
                solicitud.CodigoCanje = CodigoCanje(db);
                solicitud.VigenciaAl = DateTime.Now.AddDays((double)Config.VigenciaCanje);
                solicitud.Estado = "PENDIENTE";

                solicitud.FechaHora = DateTime.Now;

                foreach (CanjeMonederoDetalle canje in data.items)
                {
                    PremiosMonedero premio = db.PremiosMonedero.Find(long.Parse(canje.id));
                     
                    for (int i = 0; i < canje.quantity ; i++)
                    {
                        SolicitudesCanjeMonederoDetalles solicitud_canje = new SolicitudesCanjeMonederoDetalles
                        {
                            UsuarioMonederoId = UsuarioMonederoId,
                            PremioId = premio.Id,
                            PuntosCanje = premio.PuntosCanje,
                            Estado = "PENDIENTE",
                            VigenciaAl = DateTime.Now.AddDays((double)Config.VigenciaCanje)
                        };
                        solicitud.SolicitudesCanjeMonederoDetalles.Add(solicitud_canje);

                        total += solicitud_canje.PuntosCanje;
                    }

                    LstPremiosCorreo += "<li>" + canje.quantity.ToString() + " " + premio.Nombre + "</li>";
                }

                //Coloca la suma del canje
                solicitud.TotalPuntosCanje = total;

                //Verifica que se tenga saldo suficiente para realizar el canje
                CargosAbonos saldo = new CargosAbonos();
                if (total > saldo.Saldo(UsuarioMonederoId, db))
                    throw new Exception("Saldo insuficiente para realizar el canje");

                db.SolicitudesCanjeMonedero.Add(solicitud);
                db.SaveChanges();

                //Envia correo de canje
                string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/codigo.html");

                using (StreamReader sr = new StreamReader(strFilePath2))
                {
                    UsuariosMonedero usuario = db.UsuariosMonedero.Find(solicitud.UsuarioMonederoId);

                    // Read the stream to a string, and write the string to the console.
                    String mail = sr.ReadToEnd();
                    mail = mail.Replace("@nombre", usuario.Nombre);
                    mail = mail.Replace("@premios", LstPremiosCorreo);
                    mail = mail.Replace("@codigo", solicitud.CodigoCanje);
                    SendEmail(usuario.Email, "Monedero Brissa", mail);
                }


                return solicitud.CodigoCanje;
            }
            catch (Exception ex)
            {
                Error(ex, "La solicitud");
                return null;
            }   
        }
        public string CodigoCanje(EmpresaContext db)
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 100000000);

                string codigo=randomNumber.ToString().Length < 8 ? randomNumber.ToString().PadLeft(8,'0') : randomNumber.ToString();
                //string codigo = "93922045";

                return db.SolicitudesCanjeMonedero.Where(c => c.CodigoCanje == codigo).SingleOrDefault() ==null ? codigo : CodigoCanje(db);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public SolicitudesCanjeMonedero get(int id)
        {
            try
            {
                ValidarWebMonedero();
                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));

                EmpresaContext db = new EmpresaContext();

                return null;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public SolicitudesCanjeMonedero update(SolicitudesCanjeMonedero solcitud)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex, "La solicitud");
                return null;
            }
        }
        public SolicitudesCanjeMonedero delete(SolicitudesCanjeMonedero solicitud)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "La solicitud");
                return null;
            }
        }
        public List<SolicitudesCanjeMonedero> getSolicitudesWebXUsuario()
        {
            try
            {
                ValidarWebMonedero();
                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));
                EmpresaContext db = new EmpresaContext();

                List<SolicitudesCanjeMonedero> LstSolicitudes = db.SolicitudesCanjeMonedero.Where(c => c.UsuarioMonederoId == UsuarioMonederoId && c.Estado == "PENDIENTE").OrderByDescending( f => f.FechaHora).ToList();
                /*List<SolicitudesCanjeMonedero> LstSolicitudes = db.SolicitudesCanjeMonedero.Where(c => c.UsuarioMonederoId == UsuarioMonederoId && c.Estado=="PENDIENTE")
                                                                .Include(s => s.SolicitudesCanjeMonederoDetalles.Select(p => p.PremiosMonedero)).ToList();*/

                return LstSolicitudes;
            }
            catch (Exception ex)
            {
                Error(ex, "La solicitud");
                return null;
            }
        }
        public List<SolicitudesCanjeMonederoDetalles> getSolicitudesDetallesWebXUsuario(long id)
        {
            try
            {
                ValidarWebMonedero();
                
                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));
                EmpresaContext db = new EmpresaContext();

                List<SolicitudesCanjeMonederoDetalles> LstSolicidutedesDetalles = db.SolicitudesCanjeMonederoDetalles.Where(c=> c.SolicitudCanjeMonederoId == id && c.UsuarioMonederoId==UsuarioMonederoId).Include(p=> p.PremiosMonedero).ToList();

                return LstSolicidutedesDetalles;
            }
            catch (Exception ex)
            {

                Error(ex, "La solicitud");
                return null;
            }
        }
        public SolicitudesCanjeMonedero getSolicitudxCodigo(string codigo)
        {
            try
            {
                ValidarWebMonedero();

                EmpresaContext db = new EmpresaContext();

                SolicitudesCanjeMonedero solicitud = db.SolicitudesCanjeMonedero.Include(u => u.UsuariosMonedero).Include(d => d.SolicitudesCanjeMonederoDetalles.Select(p => p.PremiosMonedero))
                                                                                                                .Where(s => s.CodigoCanje == codigo && s.Estado=="PENDIENTE").SingleOrDefault();
                if (solicitud == null)
                    throw new Exception("El código de canje no se encuentra registrado o ya fue canjeado");

                return solicitud;
            }
            catch (Exception ex)
            {

                Error(ex, "La solicitud");
                return null;
            }
        }
    }
}
