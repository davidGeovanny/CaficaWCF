using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;
using System.Data.Entity;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Globalization;
using FirebirdSql.Data.FirebirdClient;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSMovimientosMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSMovimientosMonedero.svc or WSMovimientosMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSMovimientosMonedero : WsBase, IWSMovimientosMonedero
    {
        public List<MovimientosMonedero> getall()
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
        //Controla los abonos
        public MovimientosMonedero add(MovimientosMonedero movimiento)
        {
            try
            {
                ValidarWebMonedero();

                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El usuario");
                return null;
            }
        }
        public MovimientosMonedero addViaRapida(MovimientosMonedero movimiento)
        {
            try
            {
                ValidarWebMonedero();
                //Verificar autenticidad del ticket
                EmpresaContext db = new EmpresaContext();
                movimiento.SocioMonederoId = db.SociosMonedero.Where(x => x.GrupoSocio == "VIA RAPIDA").FirstOrDefault().Id;

                if (VerificarTicketViaRapida(movimiento) == false)
                    throw new Exception("201");

                addTicket(movimiento);


                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El Ticket");
                return null;
            }
        }
        public MovimientosMonedero addOga(MovimientosMonedero movimiento)
        {
            try
            {
                ValidarWebMonedero();
                //Verificar autenticidad del ticket

                if (VerificarTicketOGA(movimiento) == false)
                    throw new Exception("201");

                addTicket(movimiento);


                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El Ticket");
                return null;
            }
        }


        public bool VerificarTicketOGA(MovimientosMonedero mov)
        {
            SqlConnection conn = null;

            try
            {
                bool existe;
                long totalReg;
                existe = false;
                totalReg = 0;

                EmpresaContext db = new EmpresaContext();
                SociosMonedero socio = db.SociosMonedero.Find(mov.SocioMonederoId);
                string password = "";
                if (socio.Servidor!="172.16.14.239")
                {
                    password = "Cistem32";
                }
                String ConnStr = @"Data Source = "+ socio.Servidor + "; Initial Catalog = RPCistemGas; User ID = Sa; Password = " + password + ";Trusted_Connection = False;";
                conn = new SqlConnection(ConnStr);
                conn.Open();

                string stm = "SELECT COUNT(*) FROM Trama where TramaID=@FolioTicket and Importe=@MontoTicket and CONVERT(VARCHAR(10), FechaCarga, 103)=@FechaTicket";
                SqlCommand cmd = new SqlCommand(stm, conn);

                cmd.Parameters.AddWithValue("@FolioTicket", mov.FolioTicket);
                cmd.Parameters.AddWithValue("@MontoTicket", mov.MontoTicket);
                cmd.Parameters.AddWithValue("@FechaTicket", mov.FechaHora.ToShortDateString());
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    totalReg = Convert.ToInt64(result);
                }

                if (totalReg > 0)
                    existe = true;

                return existe;

            }
            catch (Exception ex)
            {

                Error(ex, "El Ticket");
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }
        }
        public MovimientosMonedero addHielera(MovimientosMonedero movimiento)
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();
                movimiento.SocioMonederoId = db.SociosMonedero.Where(x => x.GrupoSocio == "BRISSA").FirstOrDefault().Id;
               
                //Verificar autenticidad del ticket
                if (VerificarTicketHielera(movimiento)== false)
                    throw new Exception("201");

               

               
                addTicket(movimiento);

                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El Ticket");
                return null;
            }
        }

        private void addTicket(MovimientosMonedero movimiento)
        {
            try
            {
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        movimiento.UsuarioMonederoId = int.Parse(getKey("id", "Token"));

                        int AntiguedadTicket;

                        //Configuracion del socio
                        SociosMonedero ConfigSocio = db.SociosMonedero.Find(movimiento.SocioMonederoId);
                        AntiguedadTicket = ConfigSocio.AntiguedadTicket == null ? 0 : (int)ConfigSocio.AntiguedadTicket;

                        //Configuracion de la empresa
                        ConfiguracionMonedero Config = db.ConfiguracionMonedero.Find(1);

                        //Si el socio no tiene configurado un antiguedad en el ticket los toma de la configuracion de la empresa
                        if (ConfigSocio.AntiguedadTicket == null)
                        {
                            if (Config == null || Config.AntiguedadTicket == null)
                                throw new Exception("No tiene configurada la antigüedad para esta empresa, asígnela desde el catálogo de empresas");

                            AntiguedadTicket = (int)Config.AntiguedadTicket;
                        }

                        TimeSpan ts = DateTime.Now - movimiento.FechaHora;

                        //Verifica que la fecha del ticket este dentro la fecha permitida
                        if (ts.Days > AntiguedadTicket || ts.Days < 0)
                            throw new Exception("Ticket caducado.No se permiten tickets con fecha mayor a "+ AntiguedadTicket + " dias");



                        //Si el socio no tiene cofigurado los factores de compra y puntos los toma de la configuracion de la empresa
                        double? FactorMonto = ConfigSocio.FactorMontoCompra == null ? Config.FactorMontoCompra : ConfigSocio.FactorMontoCompra;
                        int? FactorPuntos = ConfigSocio.FactorPuntos == null ? Config.FactorPuntos : ConfigSocio.FactorPuntos;

                        CargosAbonos saldo = new CargosAbonos();

                        //Calulca el abono a realizar
                        movimiento.Abono = saldo.Abono(FactorMonto, FactorPuntos, movimiento.MontoTicket, movimiento.UsuarioMonederoId, db);
                        db.MovimientosMonedero.Add(movimiento);
                        db.SaveChanges();

                        transaccion.Commit();
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Error(ex, "El Ticket");
          
            }

        }

        public bool VerificarTicketHielera(MovimientosMonedero mov)
        {
            MySqlConnection conn = null;
            
           
            try
            {


                bool existe;
                long totalReg;
                existe = false;
                totalReg = 0;

                EmpresaContext db = new EmpresaContext();
                SociosMonedero socio = db.SociosMonedero.Find(mov.SocioMonederoId);
                String cs = @"server=" + socio.Servidor + ";userid=hielera;password=hielo;database=Hielera_2";
                conn = new MySqlConnection(cs);
                conn.Open();

                string stm = "SELECT COUNT(Id_Remision) FROM Remisiones where FolioImpreso=@folio and Importe=@MontoTicket and Fecha=@FechaTicket";

                MySqlCommand cmd = new MySqlCommand(stm, conn);
                cmd.Parameters.AddWithValue("@folio", mov.FolioTicket);
                cmd.Parameters.AddWithValue("@MontoTicket", mov.MontoTicket);
                cmd.Parameters.AddWithValue("@FechaTicket", mov.FechaHora.ToString("yyyy-MM-dd"));
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    totalReg = Convert.ToInt64(result);   
                }

                if (totalReg > 0)
                    existe = true;

                return existe;

            }
            catch (Exception ex)
            {

                Error(ex, "El Ticket");
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }
        }


        public bool VerificarTicketViaRapida(MovimientosMonedero mov)
        {
            FbConnection conn = null;


            try
            {


                bool existe;
                long totalReg;
                existe = false;
                totalReg = 0;

                EmpresaContext db = new EmpresaContext();
                SociosMonedero socio = db.SociosMonedero.Find(mov.SocioMonederoId);
                string cs = "User=SYSDBA;" +
                        "Password=wf9twf;" +
                        "Database=C:\\Microsip datos\\OPC.FDB; " +
                        "DataSource=10.10.1.57;" +
                        "Port=3050;";
                conn = new FbConnection(cs);
                conn.Open();

                string stm = "SELECT COUNT(DOCTO_PV_ID) FROM DOCTOS_PV where FOLIO=@folio and (IMPORTE_NETO+TOTAL_IMPUESTOS)=@MontoTicket and FECHA=@FechaTicket ";

                FbCommand cmd = new FbCommand(stm, conn);
                cmd.Parameters.AddWithValue("@folio", mov.FolioTicket);
                cmd.Parameters.AddWithValue("@MontoTicket", mov.MontoTicket);
                cmd.Parameters.AddWithValue("@FechaTicket", mov.FechaHora.ToString("dd.MM.yyyy"));
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    totalReg = Convert.ToInt64(result);
                }

                if (totalReg > 0)
                    existe = true;

                return existe;

            }
            catch (Exception ex)
            {

                Error(ex, "El Ticket");
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }
        }

        public List<MovimientosMonedero> getMovimientosxUsuario(int index)
        {
            try
            {
                ValidarWebMonedero();

                //El id del usuario se obtiene del token de la aplicacion
                long UsuarioMonederoId = int.Parse(getKey("id", "Token"));

                EmpresaContext db = new EmpresaContext();
                //List<MovimientosMonedero> Movimientos = db.MovimientosMonedero.Include(s => s.SociosMonedero).Include(c => c.CentrosCanjeMonedero).Where(c => c.UsuarioMonederoId == UsuarioMonederoId).ToList();
                List<MovimientosMonedero> Movimientos = db.MovimientosMonedero.Include(s => s.SociosMonedero).Include(c => c.CentrosCanjeMonedero)
                                                       .Where(c => c.UsuarioMonederoId == UsuarioMonederoId).OrderByDescending(o => o.FechaCreacion).Skip(index).Take(30).ToList();


                return Movimientos;
            }
            catch (Exception ex)
            {

                Error(ex, "El Ticket");
                return null;
            }
        }
        public MovimientosMonedero get(int id)
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
        public MovimientosMonedero update(MovimientosMonedero movimiento)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex, "El usuarios monedero");
                return null;
            }
        }
        public MovimientosMonedero delete(MovimientosMonedero movimiento)
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
        //Si SocionMonderoId = 0 quiere decir que el usuario firmado en el sistema tiene derecho a ver todas las sucursales
        public List<MovimientosMonedero> getCanjexSocioMonedero()
        {
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();

                //Obtiene el id del usuario
                long UsuarioId = int.Parse(getKey("id", "Token"));

                CentrosCanjeUsuariosMonedero Usuario = db.CentrosCanjeUsuariosMonedero.Where(c => c.UsuarioId == UsuarioId).SingleOrDefault();

                if (Usuario == null)
                    throw new Exception("El usuario no tiene asignado ningun centro de canje");

                List<MovimientosMonedero> LstMovimientos = db.MovimientosMonedero.Include(c => c.UsuariosMonedero).Include(s => s.CentrosCanjeMonedero)
                                                                                            .Where(m => m.FechaHora >= FechaInicio && m.FechaHora <= FechaFin &&  m.CentroCanjeMonederoId == Usuario.CentroCanjeMonederoId && m.Canje != null ).ToList();

                return LstMovimientos;
            }
            catch (Exception ex)
            {
                Error(ex, "El movimiento");
                return null;
            }
        }
    }
}
