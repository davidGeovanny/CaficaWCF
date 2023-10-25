using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSCanjes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSCanjes.svc or WSCanjes.svc.cs at the Solution Explorer and start debugging.
    public class WSCanjes : WsBase, IWSCanjes
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
        public MovimientosMonedero get(int Id)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();

                MovimientosMonedero Canje = db.MovimientosMonedero.Where(c => c.Id == Id).Include(u => u.UsuariosMonedero)
                                                .Include(d => d.MovimientosMonederoDetalles.Select(p => p.PremiosMonedero)).SingleOrDefault();

                return Canje;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public MovimientosMonedero add(MovimientosMonedero mov)
        {
            try
            {
                Validar();

                //Metodo para Agregar un almacen
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        string CodigoCanje="";
                        

                        mov.FechaHora = DateTime.Now;
                        mov.Canje = mov.MovimientosMonederoDetalles.Sum(x => x.Puntos);

                        //Armo el documento que va afectar al inventario
                        InventariosES documentosalida = new InventariosES();
                        documentosalida.ConceptoId = 17; //Concepto de entrada por toma de inventatario fisico
                        documentosalida.AlmacenId = mov.CentrosCanjeMonedero.AlmacenId;
                        documentosalida.Fecha = DateTime.Now;
                        documentosalida.ModuloOrigen = "VT";
                        documentosalida.Descripcion = "SALIDA POR CANJE,GENERADA AUTOMATICAMENTE POR EL SISTEMA";
                        documentosalida.Cancelado = "NO";
                        documentosalida.Naturaleza = "SALIDA";
                        
                        foreach(MovimientosMonederoDetalles detalle in mov.MovimientosMonederoDetalles)
                        {
                            //Busco los componente del Premio seleccionado
                            List<PremiosMonederoDetalles> LstPremiosMonederosDetalle = db.PremiosMonederoDetalles.Include(c => c.Componentes).Where(x => x.PremioMonederoId == detalle.PremioId).ToList();
                            foreach (PremiosMonederoDetalles detallePremiod in LstPremiosMonederosDetalle)
                            {

                                //Agrega el componente al documento del inventario
                                documentosalida.InventariosESDetalles.Add(new InventariosESDetalles
                                {
                                    ConceptoId  = documentosalida.ConceptoId,
                                    Naturaleza  = documentosalida.Naturaleza,
                                    AlmacenId   = documentosalida.AlmacenId,
                                    Fecha       = documentosalida.Fecha,

                                    ComponenteId           = detallePremiod.ComponenteId,
                                    Cantidad               = detallePremiod.CantidadReal,
                                    TipoSeguimiento        = detallePremiod.Componentes.TipoSeguimiento,
                                    ComponentesAlmacenesId = db.ComponentesAlmacenes.Where(c => c.ComponenteId == detallePremiod.ComponenteId).Where(a => a.AlmacenId == documentosalida.AlmacenId).Single().Id,
                                    Componentes            = detallePremiod.Componentes
                                });
                            }
                            
                        }

                        //Afecta el inventario
                        Inventarios.Inventarios Afectacion = new Inventarios.Inventarios();
                        Afectacion.DesactivarPermisos = false;
                        Afectacion.add(documentosalida, db);


                        mov.UsuariosMonedero = null;
                        mov.CentrosCanjeMonedero = null;
                        mov.MovimientosMonederoDetalles.ToList().ForEach(x => x.PremiosMonedero = null);

                        //Variable para que la solicitud de canje solo se busque una vez
                        bool BuscarSolicitud = true;

                        //Pongo en el estado REALIZADO lo movimientos canjeados  Nota:Solo de los premios que tienen existencia
                        foreach (MovimientosMonederoDetalles detalle in mov.MovimientosMonederoDetalles)
                        {
                            SolicitudesCanjeMonederoDetalles SolicitudDetalle = db.SolicitudesCanjeMonederoDetalles.Where(c => c.Id==detalle.SolicitudesCanjeMonederoDetallesId).SingleOrDefault();
                            SolicitudDetalle.Estado = "REALIZADO";

                            if(BuscarSolicitud==true)
                            { 
                                //Busca la solicitutud de canje para ser modificada solo la primera vuelta
                                db.Entry(SolicitudDetalle).Reference(c => c.SolicitudesCanjeMonedero).Load();
                                SolicitudDetalle.SolicitudesCanjeMonedero.Estado = "REALIZADO";

                                BuscarSolicitud = false;
                            }

                            db.SolicitudesCanjeMonederoDetalles.Attach(SolicitudDetalle);
                            db.Entry(SolicitudDetalle).State= System.Data.Entity.EntityState.Modified;
                        }

                        //Actualiza saldo del usuario monedero
                        CargosAbonos Cargo = new CargosAbonos();
                                         //UsuariomonederoId
                        Cargo.Cargo(mov.UsuarioMonederoId,mov.Canje ?? 0, db);

                        db.MovimientosMonedero.Add(mov);
                        db.SaveChanges();

                        //Envia correo de canje
                        string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/canje.html");

                        using (StreamReader sr = new StreamReader(strFilePath2))
                        {
                            UsuariosMonedero usuario = db.UsuariosMonedero.Find(mov.UsuarioMonederoId);

                            // Read the stream to a string, and write the string to the console.
                            String mail = sr.ReadToEnd();
                            mail = mail.Replace("@nombre", usuario.Nombre);
                            mail = mail.Replace("@TotalCanje",mov.Canje.ToString());
                            SendEmail(usuario.Email, "Monedero Brissa", mail);
                        }

                        transaction.Commit();

                    }
                }

                return mov;
            }
            catch (Exception ex)
            {
                Error(ex, "El Canje ");
                return null;
            }
        }

        public MovimientosMonedero update(MovimientosMonedero concepto)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El Canje ");
                //return concepto;
                return null;
            }
        }
        public MovimientosMonedero delete(MovimientosMonedero conceptoes)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El Canje ");
                return null;
            }
        }
    }
}
