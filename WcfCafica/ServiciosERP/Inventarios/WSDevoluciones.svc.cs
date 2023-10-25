using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;
using System.IO;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSDevoluciones" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSDevoluciones.svc or WSDevoluciones.svc.cs at the Solution Explorer and start debugging.
    public class WSDevoluciones : WsBase, IWSDevoluciones
    {
        public List<Resguardos> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var resguardos = db.Resguardos.Include("Responsables").Where(f => f.Fecha >= FechaInicio && f.Fecha <= FechaFin && f.TipoDocumento == "DEVOLUCION").ToList();

                return resguardos;
            }
            catch (Exception ex)
            {


                Error(ex);
                return null;
            }
        }
        public void ActulizarResguardo(List<ResguardosLotesSeries> DevolucionesDetalles, EmpresaContext db)
        {
            try
            {
                foreach(ResguardosLotesSeries detalle in DevolucionesDetalles)
                {

                    ResguardosLotesSeries resguardo = db.ResguardosLotesSeries.Find(detalle.ResguardoLotesSeriesOrigenId);
                    resguardo.ResguardoLotesSeriesOrigenId = detalle.Id;
                    /*db.InventariosFisicosDetalles.Attach(detalle);*/
                    db.Entry(resguardo).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Resguardos add(Resguardos docresguardo)
        {
            try
            {
                Validar();
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        List<ResguardosLotesSeries> DevolucionesDetalles = new List<ResguardosLotesSeries>();

                        Inventarios documento = new Inventarios();
                        documento.DesactivarPermisos = true;

                        InventariosES EntradaDevolucion = new InventariosES();
                        EntradaDevolucion.ConceptoId = 16;
                        EntradaDevolucion.AlmacenId = docresguardo.AlmacenId;
                        EntradaDevolucion.Fecha = DateTime.Now;
                        EntradaDevolucion.Descripcion = "Entrada por devolucion de resguardo, generada automáticamente por el sistema";
                        EntradaDevolucion.Naturaleza = "ENTRADA";
                        EntradaDevolucion.Cancelado = "NO";

                        foreach (ResguardosDetalles detalle in docresguardo.ResguardosDetalles)
                        {

                            detalle.ResponsableId = docresguardo.ResponsableId;
                            detalle.TipoDocumento = docresguardo.TipoDocumento;
                            detalle.ComponenteId = detalle.Componentes.Id;
                            detalle.ComponentesAlmacenesId = db.ComponentesAlmacenes.Where(c => c.ComponenteId == detalle.ComponenteId).Where(a => a.AlmacenId == docresguardo.AlmacenId).Single().Id;
                            detalle.AlmacenId = docresguardo.AlmacenId;

                            InventariosESDetalles DetalleSalida = new InventariosESDetalles();
                            DetalleSalida.ConceptoId = EntradaDevolucion.ConceptoId;
                            DetalleSalida.Naturaleza = EntradaDevolucion.Naturaleza;
                            DetalleSalida.AlmacenId = EntradaDevolucion.AlmacenId;
                            DetalleSalida.ComponenteId = detalle.Componentes.Id;
                            DetalleSalida.Fecha = EntradaDevolucion.Fecha;
                            DetalleSalida.Cantidad = detalle.Cantidad;
                            DetalleSalida.ComponentesAlmacenesId = detalle.ComponentesAlmacenesId;
                            DetalleSalida.TipoSeguimiento = detalle.Componentes.TipoSeguimiento;

                            detalle.Componentes = null;


                            //foreach (ResguardosLotesSeries detalleseries in detalle.ResguardosLotesSeries.Where(c => c.Componentes.TipoSeguimiento == "NÚMERO DE SERIE"))
                            foreach (ResguardosLotesSeries detalleseries in detalle.ResguardosLotesSeries)
                            {
                                /*if (detalleseries.LotesSeries == null || detalleseries.LotesSeries.NumeroSerie == "")
                                {
                                    detalleseries.LotesSeries = db.LotesSeries.Where(c => c.ComponenteId == detalle.ComponenteId && c.NumeroSerie == "SIN SERIE").Single();
                                }*/
                                if (detalleseries.Componentes.TipoSeguimiento== "NÚMERO DE SERIE")
                                { 
                                    detalleseries.ComponenteId = detalle.ComponenteId;
                                    detalleseries.LotesSeriesId = detalleseries.LotesSeries.Id;
                                    detalleseries.ResponsableId = docresguardo.ResponsableId;
                                    detalleseries.TipoDocumento = docresguardo.TipoDocumento;

                                    InventariosESLotesSeries DetalleSerie = new InventariosESLotesSeries();
                                    DetalleSerie.Cantidad = 1;
                                    DetalleSerie.LotesSeriesId = (long)detalleseries.LotesSeriesId;
                                    DetalleSerie.LotesSeries = detalleseries.LotesSeries;

                                    DetalleSalida.InventariosESLotesSeries.Add(DetalleSerie);
                                }

                                detalleseries.LotesSeries = null;
                                detalleseries.Componentes = null;

                                DevolucionesDetalles.Add(detalleseries);

                            }

                            EntradaDevolucion.InventariosESDetalles.Add(DetalleSalida);
                        }

                        documento.add(EntradaDevolucion, db);

                        docresguardo.InventariosESId = EntradaDevolucion.Id;
                        db.Resguardos.Add(docresguardo);
                        db.SaveChanges();

                        ActulizarResguardo(DevolucionesDetalles,db);

                        transaccion.Commit();

                        //Checa si el reponsable cuenta para con correo para poder enviarle un aviso
                        Responsables responsable = db.Responsables.Find(docresguardo.ResponsableId);
                        if (responsable.CorreoElectronico != null)
                        {
                            string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/Devolucion.html");

                            using (StreamReader sr = new StreamReader(strFilePath2))
                            {
                                // Read the stream to a string, and write the string to the console.
                                String mail = sr.ReadToEnd();
                                mail = mail.Replace("@responsable", responsable.Nombre + " " + responsable.Paterno + " " + responsable.Materno);
                                mail = mail.Replace("@folio", docresguardo.Id.ToString());
                                SendEmail(responsable.CorreoElectronico, "Devolucíon CAFICAERP", mail, docresguardo.Id.ToString(),"/ERP/Inventarios/Resguardos/Devolucion");
                            }
                        }
                    }
                }

                return docresguardo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Resguardos get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                Resguardos documentoResguardo = db.Resguardos.Where(r => r.Id == id).
                                                Include(p => p.ResguardosDetalles.Select(islt => islt.ResguardosLotesSeries.Select(lt => lt.LotesSeries))).
                                                Include(p => p.ResguardosDetalles.Select(cm => cm.Componentes)).SingleOrDefault();

                //Esta linea se usa para los componente de seguimiento normal que no incluya los detalles de lotesseries
                //documentoResguardo.ResguardosDetalles.Where(w => w.Componentes.TipoSeguimiento == "NORMAL").ToList().ForEach(s => s.ResguardosLotesSeries.Clear());

                return documentoResguardo;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Resguardos update(Resguardos inventariofisico)
        {
            try
            {
                throw new Exception("Los documentos de tipo resguardo no son modificables");
            }
            catch (Exception ex)
            {


                Error(ex);
                return null;
            }
        }
        public Resguardos delete(Resguardos doc)
        {
            try
            {
                throw new Exception("Los documentos de tipo resguardo no se permite eliminarlos");
            }
            catch (Exception ex)
            {


                Error(ex);
                return null;
            }

        }
        public Resguardos cancel(Resguardos resguardo)
        {
            try
            {
                Validar();
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        if (resguardo.Cancelado == "SI")
                            throw new Exception("Esta resguarrdo ya se encuentra cancelada");
                        Inventarios Afectacion = new Inventarios();
                        Afectacion.DesactivarPermisos = true;

                        InventariosES documentosalida= getEntradaSalida((int)resguardo.InventariosESId);
                        documentosalida.ConceptosES = db.ConceptosES.Find(documentosalida.ConceptoId);
                        Afectacion.cancelar(documentosalida, db);


                        foreach (ResguardosDetalles detalle in resguardo.ResguardosDetalles)
                        {
                            foreach (ResguardosLotesSeries detalleserie in detalle.ResguardosLotesSeries)
                            {
                                detalleserie.Cancelado = "SI";
                                detalleserie.LotesSeries = null;
                                detalleserie.Componentes = null;
                                db.ResguardosLotesSeries.Attach(detalleserie);
                                db.Entry(detalleserie).State = EntityState.Modified;

                                ResguardosLotesSeries ResguardoOrigen = db.ResguardosLotesSeries.Find(detalleserie.ResguardoLotesSeriesOrigenId);
                                ResguardoOrigen.ResguardoLotesSeriesOrigenId = null;
                                db.Entry(ResguardoOrigen).State = EntityState.Modified;
                            }
                            detalle.Cancelado = "SI";
                            detalle.Componentes = null;
                            detalle.ResguardosLotesSeries = null;
                            db.ResguardosDetalles.Attach(detalle);
                            db.Entry(detalle).State = EntityState.Modified;
                        }

                        resguardo.ResguardosDetalles = null;
                        resguardo.Cancelado = "SI";
                        db.Resguardos.Attach(resguardo);
                        db.Entry(resguardo).State = EntityState.Modified;
                        db.SaveChanges();
                        transaccion.Commit();

                        //Checa si el reponsable cuenta para con correo para poder enviarle un aviso
                        Responsables responsable = db.Responsables.Find(resguardo.ResponsableId);
                        if (responsable.CorreoElectronico != null)
                        {
                            string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/CancelacionResguardo.html");

                            using (StreamReader sr = new StreamReader(strFilePath2))
                            {
                                // Read the stream to a string, and write the string to the console.
                                String mail = sr.ReadToEnd();
                                mail = mail.Replace("@responsable", responsable.Nombre + " " + responsable.Paterno + " " + responsable.Materno);
                                mail = mail.Replace("@folio", resguardo.Id.ToString());
                                mail = mail.Replace("@documento", "devolución");
                                SendEmail(responsable.CorreoElectronico, "Cancelación devolucíon CAFICAERP", mail, resguardo.Id.ToString(), "/ERP/Inventarios/Resguardos/Devolucion");
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public InventariosES getEntradaSalida(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                InventariosES documentoES = db.InventariosES.Find(ID);

                db.Entry(documentoES).Collection("InventariosESDetalles").Load();

                foreach (InventariosESDetalles detalle in documentoES.InventariosESDetalles)
                {
                    db.Entry(detalle).Collection("InventariosESLotesSeries").Load();
                }

                return documentoES;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ResguardosLotesSeries> getDevolucionesPendientes(long ResponsableId)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                //List<Componentes> LstComponetes = db.ResguardosLotesSeries.Where(u => u.ResponsableId == ResponsableId && u.ResguardoLotesSeriesOrigenId == null).Select(s => s.Componentes).ToList();
                List<ResguardosLotesSeries> LstComponentesDevoluciones = db.ResguardosLotesSeries.Where(u => u.ResponsableId == ResponsableId && u.ResguardoLotesSeriesOrigenId == null && u.Cancelado=="NO")
                                            .Include(c => c.Componentes)
                                            .Include(r => r.LotesSeries).ToList();

                return LstComponentesDevoluciones;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
    }
}
