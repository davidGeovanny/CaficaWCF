using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSESInventarioFisico" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSESInventarioFisico.svc or WSESInventarioFisico.svc.cs at the Solution Explorer and start debugging.
    public class WSESInventarioFisico : WsBase, IWSESInventarioFisico
    {
        public List<InventariosFisicos> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var inventariosfisicos = db.InventariosFisicos.Include("Almacenes").Where(f => f.Fecha >= FechaInicio && f.Fecha <= FechaFin).ToList();

                return inventariosfisicos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public InventariosFisicos add(InventariosFisicos inventariofisico)
        {
            try
            {
                Validar();

                CalcularPeriodo(inventariofisico.Fecha);

                //Metodo para Agregar un almacen
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    { 
                        //Todas las tomas de inventario fisica nuevas y las guarda como pendientes forzosamente
                        inventariofisico.Estado = "PENDIENTE";

                        foreach(InventariosFisicosDetalles detalle in inventariofisico.InventariosFisicosDetalles)
                        {
                            //seleccion el componentealmacendesid de la tabla componentes almacenes
                            detalle.Componentes = null;
                            detalle.ComponentesAlmacenesId = db.ComponentesAlmacenes.Where(c => c.ComponenteId == detalle.ComponenteId).Where(a => a.AlmacenId == inventariofisico.AlmacenId).Single().Id;
                            string tipo = db.Componentes.Find(detalle.ComponenteId).TipoSeguimiento == "LOTES" ? "Lote" : "NumeroSerie";

                            foreach (InventariosFisicosLotesSeries item in detalle.InventariosFisicosLotesSeries)
                            {
                                BuscarLoteSerie(tipo, item, detalle.ComponenteId, db, inventariofisico.AlmacenId);
                            }
                        }

                        db.InventariosFisicos.Add(inventariofisico);

                        db.SaveChanges();
                        transaction.Commit();
                    }
                }
                return this.get((int)inventariofisico.Id);
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public void BuscarLoteSerie(string Tipo, InventariosFisicosLotesSeries item, long ComponenteId, EmpresaContext db,long AlmacenId)
        {
            try
            {
                //Busca el lote serie para poder empatar los id
                LotesSeries LoteSerie;

                //Sin los campos viene vacios les coloca la leyenda dependiendo el tipo de seguimiento
                if (item.LotesSeries.Lote == null && Tipo == "Lote") item.LotesSeries.Lote = "SIN LOTE";
                if (item.LotesSeries.NumeroSerie == null && Tipo == "NumeroSerie") item.LotesSeries.NumeroSerie = "SIN SERIE";

                if (Tipo == "Lote")
                    LoteSerie = db.LotesSeries.Where(l => l.Lote == item.LotesSeries.Lote).Where(c => c.ComponenteId == ComponenteId).SingleOrDefault();
                else
                    LoteSerie = db.LotesSeries.Where(l => l.NumeroSerie == item.LotesSeries.NumeroSerie).Where(c => c.ComponenteId == ComponenteId).SingleOrDefault();

                if (LoteSerie != null)
                {
                    item.LotesSeriesId = LoteSerie.Id;
                    item.LotesSeries = null;
                }
                else
                {
                    item.LotesSeries.Componentes = null;
                    db.LotesSeries.Add(item.LotesSeries);
                    db.SaveChanges();

                    item.LotesSeriesId = item.LotesSeries.Id;
                    item.LotesSeries = null;
                }
                //Busca ya tiene un registro en la tabla ExistenciasLotesSeries en el almacen seleccionado
                ExistenciasLotesSeries existencia = db.ExistenciasLotesSeries.Where(c => c.ComponenteId == ComponenteId && c.AlmacenId== AlmacenId && c.LotesSeriesId==item.LotesSeriesId).SingleOrDefault();
                if(existencia == null)
                {
                    ExistenciasLotesSeries IniciarExistencia = new ExistenciasLotesSeries();
                    IniciarExistencia.LotesSeriesId = item.LotesSeriesId;
                    IniciarExistencia.ComponenteId = ComponenteId;
                    IniciarExistencia.AlmacenId = AlmacenId;
                    IniciarExistencia.Existencia = 0;
                    db.ExistenciasLotesSeries.Add(IniciarExistencia);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public InventariosFisicos get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();


                InventariosFisicos documentoItF = db.InventariosFisicos.Where(inv => inv.Id == id).
                                                Include(p => p.InventariosFisicosDetalles.Select(islt => islt.InventariosFisicosLotesSeries.Select(lt => lt.LotesSeries))).
                                                Include(p => p.InventariosFisicosDetalles.Select(cm => cm.Componentes)).SingleOrDefault();

                /*InventariosFisicos documentoItF = db.InventariosFisicos.Find(id);
                db.Entry(documentoItF).Collection("InventariosFisicosDetalles").Load();

                foreach(InventariosFisicosDetalles detalle in documentoItF.InventariosFisicosDetalles)
                {
                    db.Entry(detalle).Reference("Componentes").Load();
                    db.Entry(detalle).Collection("InventariosFisicosLotesSeries").Load();
                    foreach(InventariosFisicosLotesSeries LoteSerie in detalle.InventariosFisicosLotesSeries)
                    {
                        db.Entry(LoteSerie).Reference("LotesSeries").Load();
                    }
                }*/

                return documentoItF;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public InventariosFisicos update(InventariosFisicos inventariofisico)
        {
            try
            {
                Validar();
                CalcularPeriodo(inventariofisico.Fecha);

                if (inventariofisico.Estado != "PENDIENTE")
                    throw new Exception("Este inventario ya fue aplicado o se encuentra cancelado");

                //Metodo para Agregar un almacen
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //foreach(InventariosFisicosDetalles detalle in inventariofisico.InventariosFisicosDetalles.Where(d => d.Lotes_Series_IF.Count > 0).ToList())
                        foreach (InventariosFisicosDetalles detalle in inventariofisico.InventariosFisicosDetalles)
                        {
                            string tipo = db.Componentes.Find(detalle.ComponenteId).TipoSeguimiento == "LOTES" ? "Lote" : "NumeroSerie";
                            foreach (InventariosFisicosLotesSeries LoteSerie in detalle.InventariosFisicosLotesSeries)
                            {
                                BuscarLoteSerie(tipo, LoteSerie, detalle.ComponenteId, db,inventariofisico.AlmacenId);
                                //db.Entry(LoteSerie).State = System.Data.Entity.EntityState.Modified;
                            }

                            UpdateLista<InventariosFisicosLotesSeries>(db.InventariosFisicosLotesSeries.Where(e => e.InventariosFisicosDetalleId == detalle.Id).ToList(), detalle.InventariosFisicosLotesSeries.ToList(), db);
                            detalle.InventariosFisicosLotesSeries = null;
                            detalle.Componentes = null;
                            db.InventariosFisicosDetalles.Attach(detalle);
                            db.Entry(detalle).State = EntityState.Modified;
                        }

                        inventariofisico.InventariosFisicosDetalles = null;
                        db.InventariosFisicos.Attach(inventariofisico);
                        db.Entry(inventariofisico).State = EntityState.Modified;
                        //db.Entry(inventariofisico).Property(p => p.Id).IsModified = false;
                        db.Entry(inventariofisico).Property(p => p.AlmacenId).IsModified = false;

                        db.SaveChanges();
                        transaction.Commit();
                    }
                }
               return this.get((int)inventariofisico.Id);
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public InventariosFisicos delete(InventariosFisicos doc)
        {
            try
            {
                Validar();

                if (doc.Estado != "PENDIENTE")
                    throw new Exception("Solo los documento en estado PENDIENTE pueden ser eliminados");

                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();

                InventariosFisicos documento = db.InventariosFisicos.Find(doc.Id);


                //db.InventariosFisicos.Attach(doc);
                db.InventariosFisicos.Remove(documento);
                db.SaveChanges();

                return doc;
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
                    /*foreach (InventariosESLotesSeries LoteSerie in detalle.InventariosESLotesSeries)
                    {
                        db.Entry(LoteSerie).Reference("LotesSeries").Load();
                    }*/
                }

                return documentoES;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public InventariosFisicos cancel(InventariosFisicos Inventario)
        {
            try
            {
                Validar();
                using (EntityConnection conn = new EntityConnection(EmpresaContext.GetConnectionString()))
                {
                    conn.Open();
                    using (EmpresaContext db = new EmpresaContext(conn, false))
                    {
                        using (var transaccion = conn.BeginTransaction())
                        {
                            if (Inventario.Estado == "PENDIENTE")
                                throw new Exception("No se puede cancelar una toma de inventario sin antes haber sido aplicada");

                            if (Inventario.Estado == "CANCELADO")
                                throw new Exception("Esta toma de inventario ya se encuentra cancelada");

                            Inventarios Afectacion = new Inventarios();
                            Afectacion.DesactivarPermisos = true;

                            //Cancela las entrada
                            if (Inventario.InventariosESEntradaId != null)
                            {
                                using (EmpresaContext DocEntrada = new EmpresaContext(conn, false))
                                {
                                    InventariosES documentoentrada = getEntradaSalida((int)Inventario.InventariosESEntradaId);
                                    documentoentrada.ConceptosES = db.ConceptosES.Find(documentoentrada.ConceptoId);
                                    Afectacion.cancelar(documentoentrada, DocEntrada);
                                }
                            }
                            //Cancela la salida
                            if (Inventario.InventariosESSalidaId != null)
                            {
                                using (EmpresaContext DocSalida = new EmpresaContext(conn, false))
                                {
                                    InventariosES documentosalida = getEntradaSalida((int)Inventario.InventariosESSalidaId);
                                    documentosalida.ConceptosES = db.ConceptosES.Find(documentosalida.ConceptoId);
                                    Afectacion.cancelar(documentosalida, DocSalida);
                                }
                            }

                            ValidarToken Token = new ValidarToken();

                            Inventario.Estado = "CANCELADO";
                            Inventario.UsuarioCancelo = Token.getKey("user", "Token");
                            Inventario.FechaHoraCancelacion = DateTime.Now;
                            Inventario.InventariosFisicosDetalles = null;
                            db.InventariosFisicos.Attach(Inventario);
                            db.Entry(Inventario).State = EntityState.Modified;
                            db.SaveChanges();
                            transaccion.Commit();
                        }
                    }
                }

                return this.get((int)Inventario.Id);
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public bool aplicarInventario(int id)
        {
            try
            {
                Validar();
                ValidarToken Token = new ValidarToken();

                using (EntityConnection conn =new EntityConnection(EmpresaContext.GetConnectionString()))
                {
                    conn.Open();
                    using (var transaccion = conn.BeginTransaction())
                    {
                        using (EmpresaContext db = new EmpresaContext(conn, false))
                        {
                            InventariosFisicos documentoItF = this.get(id);

                            if (documentoItF.Estado == "APLICADO")
                                throw new Exception("Esta toma de inventario ya se encuentra aplicada,no es posible continuar");

                            if (documentoItF.Estado == "CANCELADO")
                                throw new Exception("Esta toma de inventario ya se encuentra cancelada,no es posible continuar");

                            //db.Entry(documentoItF).Collection("InventariosFisicosDetalles").Load();


                            InventariosES documentoentrada = new InventariosES();
                            documentoentrada.ConceptoId = 10; //Concepto de entrada por toma de inventatario fisico
                            documentoentrada.AlmacenId = documentoItF.AlmacenId;
                            documentoentrada.Fecha = documentoItF.Fecha;
                            documentoentrada.Descripcion = documentoItF.Descripcion;
                            documentoentrada.ModuloOrigen = "IF";
                            documentoentrada.Cancelado = "NO";
                            documentoentrada.Naturaleza = "ENTRADA";

                            InventariosES documentosalida = new InventariosES();
                            documentosalida.ConceptoId = 11; //Concepto de entrada por toma de inventatario fisico
                            documentosalida.AlmacenId = documentoItF.AlmacenId;
                            documentosalida.Fecha = documentoItF.Fecha;
                            documentosalida.ModuloOrigen = "IF";
                            documentosalida.Descripcion = documentoItF.Descripcion;
                            documentosalida.Cancelado = "NO";
                            documentosalida.Naturaleza = "SALIDA";

                            foreach (InventariosFisicosDetalles detalle in documentoItF.InventariosFisicosDetalles)
                            {
                                //db.Entry(detalle).Reference("Componentes").Load();
                                //db.Entry(detalle).Collection("Lotes_Series_IF").Load();
                                detalle.ExistenciaTeorica = ExistenciaActualComponente(documentoItF.AlmacenId, detalle.ComponenteId);
                                detalle.Diferencia = detalle.ExistenciaTeorica - detalle.ExistenciaFisica;

                                if (detalle.Componentes.TipoSeguimiento == "NORMAL")
                                {
                                    InventarioNormal(detalle, documentoentrada, documentosalida, db);
                                }
                                if (detalle.Componentes.TipoSeguimiento == "NÚMERO DE SERIE")
                                {
                                    InventarioNumeroSerie(detalle, documentoentrada, documentosalida, db);
                                }
                                if (detalle.Componentes.TipoSeguimiento == "LOTES")
                                {
                                    InventarioLotes(detalle, documentoentrada, documentosalida, db);
                                }

                                /*db.InventariosFisicosDetalles.Attach(detalle);
                                db.Entry(detalle).State = EntityState.Modified;*/
                            }

                            using (EmpresaContext DocEntrada = new EmpresaContext(conn, false))
                            {
                                //context1.Database.UseTransaction(transaccion);
                                Inventarios Afectacion = new Inventarios();
                                Afectacion.DesactivarPermisos = true;

                                if (documentoentrada.InventariosESDetalles.Count > 0)
                                {
                                    Afectacion.add(documentoentrada, DocEntrada);
                                    documentoItF.InventariosESEntradaId = documentoentrada.Id;
                                    //Actualiza el id que se genero en la tabla de Entradas y Salidas
                                    foreach (InventariosESDetalles InvEntradaDetalle in documentoentrada.InventariosESDetalles)
                                    {
                                        InventariosFisicosDetalles InvDetalleFiscio = documentoItF.InventariosFisicosDetalles.Where(c => c.ComponenteId == InvEntradaDetalle.ComponenteId && c.InventariosFisicosId == documentoItF.Id).Single();
                                        InvDetalleFiscio.InventariosESDetalleId = InvEntradaDetalle.Id;

                                        InvDetalleFiscio.InventariosFisicosLotesSeries = null;
                                        InvDetalleFiscio.Componentes = null;
                                        db.InventariosFisicosDetalles.Attach(InvDetalleFiscio);
                                        db.Entry(InvDetalleFiscio).State = EntityState.Modified;
                                    }
                                }
                            }
                            using (EmpresaContext DocSalida = new EmpresaContext(conn, false))
                            {
                                //context2.Database.UseTransaction(transaccion);

                                Inventarios Afectacion = new Inventarios();
                                Afectacion.DesactivarPermisos = true;

                                if (documentosalida.InventariosESDetalles.Count > 0)
                                {
                                    Afectacion.add(documentosalida, DocSalida);
                                    documentoItF.InventariosESSalidaId = documentosalida.Id;
                                    foreach (InventariosESDetalles InvSalidaDetalle in documentosalida.InventariosESDetalles)
                                    {
                                        InventariosFisicosDetalles InvDetalleFiscio = documentoItF.InventariosFisicosDetalles.Where(c => c.ComponenteId == InvSalidaDetalle.ComponenteId && c.InventariosFisicosId == documentoItF.Id).Single();
                                        InvDetalleFiscio.InventariosESDetalleId = InvSalidaDetalle.Id;

                                        InvDetalleFiscio.InventariosFisicosLotesSeries = null;
                                        InvDetalleFiscio.Componentes = null;
                                        db.InventariosFisicosDetalles.Attach(InvDetalleFiscio);
                                        db.Entry(InvDetalleFiscio).State = EntityState.Modified;
                                    }
                                }
                            }

                            // ValidarToken Token = new ValidarToken();

                            documentoItF.Estado = "APLICADO";
                            documentoItF.UsuarioAplico = Token.getKey("user", "Token");
                            documentoItF.FechaHoraAplicacion = DateTime.Now;
                            documentoItF.InventariosFisicosDetalles = null;
                            db.InventariosFisicos.Attach(documentoItF);
                            db.Entry(documentoItF).State = EntityState.Modified;
                            db.SaveChanges();
                            transaccion.Commit();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }
        public void InventarioNormal(InventariosFisicosDetalles detalle,InventariosES documentoentrada,InventariosES documentosalida,EmpresaContext db)
        {
            try
            {
                    if (detalle.Diferencia < 0.0) //Entrada
                    {
                        documentoentrada.InventariosESDetalles.Add(new InventariosESDetalles
                        {
                            ConceptoId = documentoentrada.ConceptoId,
                            Naturaleza = documentoentrada.Naturaleza,
                            AlmacenId = documentoentrada.AlmacenId,
                            ComponenteId = detalle.ComponenteId,
                            Fecha = documentoentrada.Fecha,
                            Cantidad = Math.Abs((double)detalle.Diferencia),
                            ComponentesAlmacenesId = detalle.ComponentesAlmacenesId,
                            Componentes = detalle.Componentes
                        });
                    }
                    else if (detalle.Diferencia > 0.0) //Salida
                    { 
                        documentosalida.InventariosESDetalles.Add(new InventariosESDetalles
                        {
                            ConceptoId = documentosalida.ConceptoId,
                            Naturaleza = documentosalida.Naturaleza,
                            AlmacenId = documentosalida.AlmacenId,
                            ComponenteId = detalle.ComponenteId,
                            Fecha = documentosalida.Fecha,
                            Cantidad = Math.Abs((double)detalle.Diferencia),
                            ComponentesAlmacenesId = detalle.ComponentesAlmacenesId,
                            Componentes=detalle.Componentes
                        });
                    }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void InventarioLotes(InventariosFisicosDetalles detalle, InventariosES documentoentrada, InventariosES documentosalida, EmpresaContext db)
        {
            try
            {
                //Obtengo la existencia actual de la del del componente actual
                var ExistenciaLote = db.ExistenciasLotesSeries.Where(c => ( c.ComponenteId == detalle.ComponenteId && c.AlmacenId == documentoentrada.AlmacenId && c.LotesSeries.Lote == "SIN LOTE") || (c.ComponenteId == detalle.ComponenteId && c.AlmacenId == documentoentrada.AlmacenId && c.Existencia != 0)).Include(e => e.LotesSeries).ToList();

                //Entrada
                InventariosESDetalles InventarioDetalleEntrada  = new InventariosESDetalles();
                InventarioDetalleEntrada.ConceptoId             = documentoentrada.ConceptoId;
                InventarioDetalleEntrada.Naturaleza             = documentoentrada.Naturaleza;
                InventarioDetalleEntrada.AlmacenId              = documentoentrada.AlmacenId;
                InventarioDetalleEntrada.ComponenteId           = detalle.ComponenteId;
                InventarioDetalleEntrada.Fecha                  = documentoentrada.Fecha;
                InventarioDetalleEntrada.ComponentesAlmacenesId = detalle.ComponentesAlmacenesId;
                InventarioDetalleEntrada.Componentes            = detalle.Componentes;


                //Variable para sacar el Id de la serie que contiene el valor SIN LOTE
                long IdSinLote = 0;

                //Salida
                InventariosESDetalles InventarioDetalleSalida  = new InventariosESDetalles();
                InventarioDetalleSalida.ConceptoId             = documentosalida.ConceptoId;
                InventarioDetalleSalida.Naturaleza             = documentosalida.Naturaleza;
                InventarioDetalleSalida.AlmacenId              = documentosalida.AlmacenId;
                InventarioDetalleSalida.ComponenteId           = detalle.ComponenteId;
                InventarioDetalleSalida.Fecha                  = documentosalida.Fecha;
                InventarioDetalleSalida.ComponentesAlmacenesId = detalle.ComponentesAlmacenesId;
                InventarioDetalleSalida.Componentes             = detalle.Componentes;  

                double TotalSinLote = 0.0;


                foreach (InventariosFisicosLotesSeries InventarioLoteSerie in detalle.InventariosFisicosLotesSeries)
                {
                    if (InventarioLoteSerie.LotesSeries.Lote != "SIN LOTE")
                    {
                        if (!ExistenciaLote.Any(c => c.LotesSeriesId == InventarioLoteSerie.LotesSeriesId))
                            InventarioDetalleEntrada.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                            {
                                InventariosESDetalleId = 0,
                                Cantidad = InventarioLoteSerie.Cantidad,
                                LotesSeriesId = InventarioLoteSerie.LotesSeriesId,
                                LotesSeries= InventarioLoteSerie.LotesSeries
                            });
                        else
                        {
                            double DiferenciaLotes = ExistenciaLote.Where(c => c.LotesSeries.Lote == InventarioLoteSerie.LotesSeries.Lote).Single().Existencia - InventarioLoteSerie.Cantidad;
                            //Salida
                            if(DiferenciaLotes > 0.0)
                            InventarioDetalleSalida.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                            {
                                InventariosESDetalleId = 0,
                                Cantidad = Math.Abs((double)DiferenciaLotes),
                                LotesSeriesId = InventarioLoteSerie.LotesSeriesId,
                                LotesSeries = InventarioLoteSerie.LotesSeries
                            });
                            //Entrada
                            else if(DiferenciaLotes < 0.0)
                            InventarioDetalleEntrada.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                            {
                                InventariosESDetalleId = 0,
                                Cantidad = Math.Abs((double)DiferenciaLotes),
                                LotesSeriesId = InventarioLoteSerie.LotesSeriesId,
                                LotesSeries = InventarioLoteSerie.LotesSeries
                            });
                        }

                    }
                    else
                    {
                        TotalSinLote += InventarioLoteSerie.Cantidad;
                        IdSinLote = InventarioLoteSerie.LotesSeriesId;
                    }
                }


                foreach (ExistenciasLotesSeries ExistenciaLoteSerie in ExistenciaLote.ToList())
                {
                    if (ExistenciaLoteSerie.LotesSeries.Lote != "SIN LOTE")
                    {
                        if (!detalle.InventariosFisicosLotesSeries.Any(c => c.LotesSeriesId == ExistenciaLoteSerie.LotesSeriesId))
                            InventarioDetalleSalida.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                            {
                                InventariosESDetalleId = 0,
                                Cantidad = ExistenciaLoteSerie.Existencia,
                                LotesSeriesId = ExistenciaLoteSerie.LotesSeriesId,
                                LotesSeries=ExistenciaLoteSerie.LotesSeries
                            });
                    }
                    else
                        IdSinLote = ExistenciaLoteSerie.LotesSeriesId;
                }

                //Verifica los lotes que tienen la leyenda SIN LOTE
                var LoteTotalSinLoteBD = ExistenciaLote.Where(c => c.LotesSeries.Lote == "SIN LOTE" && c.ComponenteId == detalle.ComponenteId).SingleOrDefault();
                if (TotalSinLote > 0 || LoteTotalSinLoteBD != null)
                {
                    double TotalSinLoteBD = LoteTotalSinLoteBD == null ? 0.0 : LoteTotalSinLoteBD.Existencia;

                    double DirefenciaSinLote =(double)TotalSinLoteBD - TotalSinLote;

                    //Salida
                    if (DirefenciaSinLote > 0.0)
                        InventarioDetalleSalida.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                        {
                            InventariosESDetalleId = 0,
                            Cantidad = Math.Abs((double)DirefenciaSinLote),
                            LotesSeriesId = IdSinLote,
                            LotesSeries= LoteTotalSinLoteBD.LotesSeries
                        });
                    //Entrada
                    else if (DirefenciaSinLote < 0.0)
                        InventarioDetalleEntrada.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                        {
                            InventariosESDetalleId = 0,
                            Cantidad = Math.Abs((double)DirefenciaSinLote),
                            LotesSeriesId = IdSinLote,
                            LotesSeries = LoteTotalSinLoteBD.LotesSeries
                        });
                }

                if (InventarioDetalleEntrada.InventariosESLotesSeries.Count() > 0)
                {
                    InventarioDetalleEntrada.Cantidad = InventarioDetalleEntrada.InventariosESLotesSeries.Sum(l => l.Cantidad);
                    documentoentrada.InventariosESDetalles.Add(InventarioDetalleEntrada);
                }
                if (InventarioDetalleSalida.InventariosESLotesSeries.Count() > 0)
                {
                    InventarioDetalleSalida.Cantidad = InventarioDetalleSalida.InventariosESLotesSeries.Sum(l => l.Cantidad);
                    documentosalida.InventariosESDetalles.Add(InventarioDetalleSalida);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void InventarioNumeroSerie(InventariosFisicosDetalles detalle, InventariosES documentoentrada, InventariosES documentosalida, EmpresaContext db)
        {
            try
            {
                //Obtengo la existencia actual de la series del componente actual
                var ExistenciaSerie = db.ExistenciasLotesSeries.Where(c => (c.ComponenteId == detalle.ComponenteId && c.AlmacenId == documentoentrada.AlmacenId && c.LotesSeries.NumeroSerie == "SIN SERIE") || (c.ComponenteId == detalle.ComponenteId && c.AlmacenId== documentoentrada.AlmacenId && c.Existencia != 0)).Include(e => e.LotesSeries).ToList();

                //Variable para sacar el Id de la serie que contiene el valor SIN SERIE
                long IdSinSerie=0;

                //Entrada
                InventariosESDetalles InventarioDetalleEntrada = new InventariosESDetalles();
                InventarioDetalleEntrada.ConceptoId = documentoentrada.ConceptoId;
                InventarioDetalleEntrada.Naturaleza = documentoentrada.Naturaleza;
                InventarioDetalleEntrada.AlmacenId = documentoentrada.AlmacenId;
                InventarioDetalleEntrada.ComponenteId = detalle.ComponenteId;
                InventarioDetalleEntrada.Fecha = documentoentrada.Fecha;
                InventarioDetalleEntrada.ComponentesAlmacenesId = detalle.ComponentesAlmacenesId;
                InventarioDetalleEntrada.Componentes = detalle.Componentes;

                int TotalSinSerie=0;

                foreach (InventariosFisicosLotesSeries InventarioLoteSerie in detalle.InventariosFisicosLotesSeries)
                {
                    if (InventarioLoteSerie.LotesSeries.NumeroSerie != "SIN SERIE")
                    { 
                        if (!ExistenciaSerie.Any(c => c.LotesSeriesId == InventarioLoteSerie.LotesSeriesId))
                            InventarioDetalleEntrada.InventariosESLotesSeries.Add(new InventariosESLotesSeries {
                                InventariosESDetalleId = 0,
                                Cantidad=1,
                                LotesSeriesId= InventarioLoteSerie.LotesSeriesId,
                                LotesSeries= InventarioLoteSerie.LotesSeries
                            });
                    }
                    else
                    { 
                        TotalSinSerie++;
                        IdSinSerie = InventarioLoteSerie.LotesSeriesId;
                    }
                }


                //Salida
                InventariosESDetalles InventarioDetalleSalida = new InventariosESDetalles();
                InventarioDetalleSalida.ConceptoId = documentosalida.ConceptoId;
                InventarioDetalleSalida.Naturaleza = documentosalida.Naturaleza;
                InventarioDetalleSalida.AlmacenId = documentosalida.AlmacenId;
                InventarioDetalleSalida.ComponenteId = detalle.ComponenteId;
                InventarioDetalleSalida.Fecha = documentosalida.Fecha;
                InventarioDetalleSalida.ComponentesAlmacenesId = detalle.ComponentesAlmacenesId;
                InventarioDetalleSalida.Componentes = detalle.Componentes;

                foreach (ExistenciasLotesSeries ExistenciaLoteSerie in ExistenciaSerie.ToList())
                {
                    if (ExistenciaLoteSerie.LotesSeries.NumeroSerie != "SIN SERIE")
                    {
                        if (!detalle.InventariosFisicosLotesSeries.Any(c => c.LotesSeriesId == ExistenciaLoteSerie.LotesSeriesId))
                            InventarioDetalleSalida.InventariosESLotesSeries.Add(new InventariosESLotesSeries{
                                InventariosESDetalleId = 0,
                                Cantidad = 1,
                                LotesSeriesId = ExistenciaLoteSerie.LotesSeriesId,
                                LotesSeries = ExistenciaLoteSerie.LotesSeries
                            });
                    }
                    else
                        IdSinSerie = ExistenciaLoteSerie.LotesSeriesId;
                }

                var NumeroSerieTotalSinSerieBD = ExistenciaSerie.Where(c => c.LotesSeries.NumeroSerie == "SIN SERIE" && c.ComponenteId == detalle.ComponenteId).SingleOrDefault();
                if (TotalSinSerie > 0 || NumeroSerieTotalSinSerieBD != null)
                {
                    int TotalSinSerieBD = NumeroSerieTotalSinSerieBD==null ? 0: (int)NumeroSerieTotalSinSerieBD.Existencia;

                    int? NumerodeRegistros =TotalSinSerieBD - TotalSinSerie;

                    for(int i=1;i<= Math.Abs((int)NumerodeRegistros);i++)
                    {
                        //Salida
                        if(NumerodeRegistros>0)
                            InventarioDetalleSalida.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                            {
                                InventariosESDetalleId = 0,
                                Cantidad = 1,
                                LotesSeriesId = IdSinSerie,
                                LotesSeries= NumeroSerieTotalSinSerieBD.LotesSeries
                            });
                        //Entrada
                        else if (NumerodeRegistros < 0)
                            InventarioDetalleEntrada.InventariosESLotesSeries.Add(new InventariosESLotesSeries
                            {
                                InventariosESDetalleId = 0,
                                Cantidad = 1,
                                LotesSeriesId = IdSinSerie,
                                LotesSeries = NumeroSerieTotalSinSerieBD.LotesSeries
                            });
                    }
                }

                if (InventarioDetalleEntrada.InventariosESLotesSeries.Count() >0 )
                {
                    //Suma la cantidad de los hijos
                    InventarioDetalleEntrada.Cantidad = InventarioDetalleEntrada.InventariosESLotesSeries.Sum(l => l.Cantidad);
                    documentoentrada.InventariosESDetalles.Add(InventarioDetalleEntrada);
                }

                if (InventarioDetalleSalida.InventariosESLotesSeries.Count() > 0)
                {
                    //Suma la cantidad de los hijos
                    InventarioDetalleSalida.Cantidad = InventarioDetalleSalida.InventariosESLotesSeries.Sum(l => l.Cantidad);
                    documentosalida.InventariosESDetalles.Add(InventarioDetalleSalida);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
