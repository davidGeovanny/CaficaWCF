using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using WcfCafica.Contexts.Reportes;
using WcfCafica.Contexts;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSResguardos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSResguardos.svc or WSResguardos.svc.cs at the Solution Explorer and start debugging.
    public class WSResguardos : WsBase, IWSResguardos
    {
        public List<Resguardos> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var resguardos = db.Resguardos.Include("Responsables").Where(f => f.Fecha >= FechaInicio && f.Fecha <= FechaFin && f.TipoDocumento== "RESGUARDO").ToList();

                return resguardos;
            }
            catch (Exception ex)
            {


                Error(ex);
                return null;
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
                        Inventarios documento = new Inventarios();
                        documento.DesactivarPermisos = true;

                        InventariosES SalidaResguardo = new InventariosES();
                        SalidaResguardo.ConceptoId = 15;
                        SalidaResguardo.AlmacenId = docresguardo.AlmacenId;
                        SalidaResguardo.Fecha = DateTime.Now;
                        SalidaResguardo.Descripcion = "Salida por resguardo generada automáticamente por el sistema";
                        SalidaResguardo.Naturaleza = "SALIDA";
                        SalidaResguardo.Cancelado = "NO";

                        foreach (ResguardosDetalles detalle in docresguardo.ResguardosDetalles)
                        {

                            detalle.ResponsableId = docresguardo.ResponsableId;
                            detalle.TipoDocumento = docresguardo.TipoDocumento;
                            detalle.ComponenteId = detalle.Componentes.Id;
                            detalle.ComponentesAlmacenesId = db.ComponentesAlmacenes.Where(c => c.ComponenteId == detalle.ComponenteId).Where(a => a.AlmacenId == docresguardo.AlmacenId).Single().Id; 
                            detalle.AlmacenId = docresguardo.AlmacenId;

                            InventariosESDetalles DetalleSalida = new InventariosESDetalles();
                            DetalleSalida.ConceptoId = SalidaResguardo.ConceptoId;
                            DetalleSalida.Naturaleza = SalidaResguardo.Naturaleza;
                            DetalleSalida.AlmacenId = SalidaResguardo.AlmacenId;
                            DetalleSalida.ComponenteId = detalle.Componentes.Id;
                            DetalleSalida.Fecha = SalidaResguardo.Fecha;
                            DetalleSalida.Cantidad = detalle.Cantidad;
                            DetalleSalida.ComponentesAlmacenesId = detalle.ComponentesAlmacenesId;
                            DetalleSalida.TipoSeguimiento = detalle.Componentes.TipoSeguimiento;
                            

                            if (detalle.Cantidad > ExistenciaActualComponente((long)detalle.AlmacenId, detalle.ComponenteId))
                                throw new Exception("Existencia insuficiente para surtir el componente " + detalle.Componentes.Nombre);

                             detalle.Componentes = null;

                            foreach (ResguardosLotesSeries detalleseries in detalle.ResguardosLotesSeries)
                            {
                                //if((detalleseries.LotesSeries != null || detalleseries.LotesSeries.NumeroSerie != "") && db.ResguardosLotesSeries.Where(c => c.LotesSeries.NumeroSerie==detalleseries.LotesSeries.NumeroSerie && c.TipoDocumento== "RESGUARDO"))

                                if(detalleseries.LotesSeries==null || detalleseries.LotesSeries.NumeroSerie=="")
                                {
                                    detalleseries.LotesSeries = db.LotesSeries.Where(c => c.ComponenteId == detalle.ComponenteId && c.NumeroSerie == "SIN SERIE").Single();
                                }

                                detalleseries.ComponenteId = detalle.ComponenteId;
                                detalleseries.LotesSeriesId = detalleseries.LotesSeries.Id;
                                detalleseries.ResponsableId = docresguardo.ResponsableId;
                                detalleseries.TipoDocumento = docresguardo.TipoDocumento;
                          

                                InventariosESLotesSeries DetalleSerie = new InventariosESLotesSeries();
                                DetalleSerie.Cantidad = 1;
                                DetalleSerie.LotesSeriesId = (long)detalleseries.LotesSeriesId;
                                DetalleSerie.LotesSeries = detalleseries.LotesSeries;

                                DetalleSalida.InventariosESLotesSeries.Add(DetalleSerie);

                                
                                detalleseries.LotesSeries = null;
                            }

                            if (detalle.ResguardosLotesSeries.Count == 0)
                            {
                                detalle.ResguardosLotesSeries = new List<ResguardosLotesSeries>();
                                for (int i = 1; i <= detalle.Cantidad; i++)
                                {
                                    detalle.ResguardosLotesSeries.Add(new ResguardosLotesSeries
                                    {
                                        ComponenteId = detalle.ComponenteId,
                                        ResponsableId = detalle.ResponsableId,
                                        TipoDocumento = detalle.TipoDocumento,
                                        Cancelado = "NO"
                                    });
                                }
                            }
                            
                            SalidaResguardo.InventariosESDetalles.Add(DetalleSalida);
                        }

                        documento.add(SalidaResguardo, db);

                        docresguardo.InventariosESId = SalidaResguardo.Id;
                        db.Resguardos.Add(docresguardo);
                        db.SaveChanges();
                        transaccion.Commit();

                        //Checa si el reponsable cuenta con correo para poder enviarle un aviso
                        Responsables responsable = db.Responsables.Find(docresguardo.ResponsableId);
                        if(responsable.CorreoElectronico != null)
                        { 
                            string strFilePath2 = System.Web.HttpContext.Current.Server.MapPath("/Resguardo.html");

                            using (StreamReader sr = new StreamReader(strFilePath2))
                            {
                                // Read the stream to a string, and write the string to the console.
                                String mail = sr.ReadToEnd();
                                mail = mail.Replace("@responsable", responsable.Nombre + " " + responsable.Paterno + " " + responsable.Materno);
                                mail = mail.Replace("@folio", docresguardo.Id.ToString());
                                SendEmail(responsable.CorreoElectronico, "Resguardo CAFICAERP", mail,docresguardo.Id.ToString(),"/ERP/Inventarios/Resguardos/Resguardo");
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
                documentoResguardo.ResguardosDetalles.Where(w => w.Componentes.TipoSeguimiento == "NORMAL").ToList().ForEach(s => s.ResguardosLotesSeries.Clear());

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
                            throw new Exception("Esta resguardo ya se encuentra cancelada");
                        Inventarios Afectacion = new Inventarios();
                        Afectacion.DesactivarPermisos = true;

                       
                        foreach(ResguardosDetalles detalle in resguardo.ResguardosDetalles)
                        {
                            if (detalle.Componentes.TipoSeguimiento == "NORMAL")
                                detalle.ResguardosLotesSeries = db.ResguardosLotesSeries.Where(c => c.ResguardosDetalleId == detalle.Id).ToList();

                            foreach (ResguardosLotesSeries detalleserie in detalle.ResguardosLotesSeries)
                            {
                                if (detalleserie.ResguardoLotesSeriesOrigenId != null)
                                    throw new Exception("No es posible cancelar este resguardo por que ya tiene una devolución ligada.");

                                detalleserie.Cancelado="SI";
                                detalleserie.LotesSeries = null;
                                detalleserie.Componentes = null;
                                db.ResguardosLotesSeries.Attach(detalleserie);
                                db.Entry(detalleserie).State = EntityState.Modified;
                            }
                            detalle.Cancelado = "SI";
                            detalle.Componentes = null;
                            detalle.ResguardosLotesSeries = null;
                            db.ResguardosDetalles.Attach(detalle);
                            db.Entry(detalle).State = EntityState.Modified;
                        }


                        InventariosES documentoentrada = getEntradaSalida((int)resguardo.InventariosESId);
                        documentoentrada.ConceptosES = db.ConceptosES.Find(documentoentrada.ConceptoId);
                        Afectacion.cancelar(documentoentrada, db);

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
                                mail = mail.Replace("@documento", "resguardo");
                                SendEmail(responsable.CorreoElectronico, "Cancelación resguardo CAFICAERP", mail, resguardo.Id.ToString(),"/ERP/Inventarios/Resguardos/Resguardo");
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
    }
}
