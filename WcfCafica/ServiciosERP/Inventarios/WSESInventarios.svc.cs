using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;
using System.Data.Entity;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSESInventarios" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSESInventarios.svc or WSESInventarios.svc.cs at the Solution Explorer and start debugging.
    public class WSESInventarios :WsBase, IWSESInventarios
    {

        public List<InventariosES> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var inventarioses = db.InventariosES.Include("Almacenes").Include("ConceptosES").Where(f => f.Fecha >= FechaInicio && f.Fecha <= FechaFin).ToList();

                return inventarioses;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<InventariosES> getallxnaturaleza(string Naturaleza)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var inventarioses = db.InventariosES.Include("Almacenes").Include("ConceptosES").Where(n => n.Naturaleza==Naturaleza).Where(f => f.Fecha >= FechaInicio && f.Fecha <= FechaFin).ToList();

                return inventarioses;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public InventariosES get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                InventariosES documentoES = db.InventariosES.Where(inv => inv.Id == ID).
                                     Include(p => p.ConceptosES).
                                     Include(p => p.Almacenes).
                                     Include(p => p.InventariosESDetalles.Select(cm => cm.Componentes)).
                                     Include(p => p.InventariosESDetalles.Select(cm => cm.Componentes.Unidades)).
                                     Include(p => p.InventariosESDetalles.Select(islt => islt.InventariosESLotesSeries.Select(lt => lt.LotesSeries))).SingleOrDefault();

                /*InventariosES documentoES = db.InventariosES.Find(ID);
                ConceptosES conceptoES = db.ConceptosES.Find(documentoES.ConceptoId);
                documentoES.ConceptosES = conceptoES;
                Almacenes almacenes = db.Almacenes.Find(documentoES.AlmacenId);
                documentoES.Almacenes = almacenes;
                db.Entry(documentoES).Collection("InventariosESDetalles").Load();

                foreach (InventariosESDetalles detalle in documentoES.InventariosESDetalles)
                {
                    db.Entry(detalle).Collection("InventariosESLotesSeries").Load();
                    db.Entry(detalle).Reference("Componentes").Load();
                    db.Entry(detalle.Componentes).Reference("Unidades").Load();
                    foreach (InventariosESLotesSeries LoteSerie in detalle.InventariosESLotesSeries)
                    {
                        db.Entry(LoteSerie).Reference("LotesSeries").Load();
                    }
                }*/

                return documentoES;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public InventariosES cancel(InventariosES conceptoes)
        {
            try
            {

                //   EmpresaContext db = new EmpresaContext();
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        Inventarios cancelacion = new Inventarios();
                        conceptoes.InventariosESDetalles = null;
                        cancelacion.cancelar(conceptoes, db);
                        transaccion.Commit();
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
        public InventariosES add(InventariosES inventarioentradaosalida)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                //EmpresaContext db = new EmpresaContext();
        

                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        Inventarios documento = new Inventarios();
                        documento.add(inventarioentradaosalida, db);
                        transaccion.Commit();
                    }
                }

                return inventarioentradaosalida;
            }
            catch (Exception ex)
            {
       
                    Error(ex, "El folio ");
                    return null;
             }
        }
  
        
        public InventariosES update(InventariosES documento)
        {
            try
            {
                Validar();

                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        Inventarios modif = new Inventarios();
                        modif.update(documento, db);
                        transaccion.Commit();
                    }
                }

                return documento;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }

       public InventariosES delete(InventariosES conceptoes)
        {
            try
            {

             //   EmpresaContext db = new EmpresaContext();
                using (EmpresaContext db = new EmpresaContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        Inventarios cancelacion = new Inventarios();
                        cancelacion.cancelar(conceptoes, db);
                        transaccion.Commit();
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
        private InventariosES addentrada()
        {

            return null;
        }
        public string getEmpresaVariacionCosto() //retorna el enum MetodoCosteo de la tabla BDEmpresas, de la empresa que se esta trabjando
        {
            try
            {
                UsuariosContext db = new UsuariosContext();
                ValidarToken Token = new ValidarToken();
                string rfc = Token.getKey("bd", "TokenEmpresa");
                // return db.BDEmpresas.Where(s => s.RFC == rfc).Where(s => s.ValidaVariacionCosto == "SI").FirstOrDefault().PorcentajeVariacionCosto;
                return db.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().ValidaVariacionCosto;
            }
            catch (Exception ex)
            {
                string error = null;
                error = ex.Message;
                return null;

            }
        }
       

        public void CalcularVariacionCosto(InventariosESDetalles inventarioentradaosalida)
        {
          try
          {
            EmpresaContext db = new EmpresaContext();
            UsuariosContext udb = new UsuariosContext();
            ValidarToken Token = new ValidarToken();
            string rfc = Token.getKey("bd", "TokenEmpresa");
            string empresavariacionsino= udb.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().ValidaVariacionCosto;
//ASI ESTABA ANTES (LENTO CON MUCHAS ENTRADAS) var Lstcostoxcomponente = (from co in db.InventariosESDetalles where co.ComponenteId == inventarioentradaosalida.ComponenteId select  co.CostoUnitario).ToList();
            long? ultimocostoid = db.InventariosESDetalles.Where(co => co.ComponenteId == inventarioentradaosalida.ComponenteId).Max(i=> (long?)i.Id);
            if (ultimocostoid == null || empresavariacionsino == "NO") return; //VALIDA SI TIENE VARIACION LA EMPRESA, Y SI ES LA PRIMERA ENTRADA 
             double? ultimocosto = db.InventariosESDetalles.Where(id => (long?)id.Id == ultimocostoid).FirstOrDefault().CostoUnitario;
 //           double ultimocostounitario=0;

                if (ultimocosto > 0)//EL COSTO debe ES MAYOR QUE CERO PARA CALCULAR
                {
                
                double? porcentajeempresa = udb.BDEmpresas.Where(s => s.RFC == rfc).Where(s => s.ValidaVariacionCosto == "SI").FirstOrDefault().PorcentajeVariacionCosto;
                    /*  ultimocostounitario = Lstcostoxcomponente.ToList().OrderByDescending(p => p).ToList().First();
                      ultimocostounitario=((inventarioentradaosalida.CostoUnitario - ultimocostounitario) / ultimocostounitario) * 100;*/
                    ultimocosto= (((inventarioentradaosalida.CostoUnitario - ultimocosto) / ultimocosto) * 100);


                    if (ultimocosto> porcentajeempresa)
                        {
                        throw new Exception("Costo cambia en "+ Math.Round((double)ultimocosto, 2) +"% verifique si es correcto");
                        }

                          }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        
        public void ValidarMaximo(InventariosESDetalles inventarioentradaosalida)
        {
            try
            {
                EmpresaContext db = new EmpresaContext();
                double existenciaactual =(double)ExistenciaActualComponente(inventarioentradaosalida);
                var maximo = db.ComponentesAlmacenes.Where(s => s.ComponenteId == inventarioentradaosalida.ComponenteId).Where(a => a.AlmacenId == inventarioentradaosalida.AlmacenId).FirstOrDefault();
                if (!(maximo == null)) //CUANDO NO TENGA CONFIGURADO MAXIMO EN EL ALMACEN O EL ARTICULO NO TENGA MOVIMIENTOS NO HACE NADA
                    if (maximo.Maximo>0 && (existenciaactual + inventarioentradaosalida.Cantidad > maximo.Maximo)) //si el maximo
                {
                        throw new Exception("Se esta superando el maximo configurado para este componente");
                   
                }
            }
            catch (Exception ex )
            {
                Error(ex);
                
            }
        }

        public List<LotesSeries> getSerielotexcomponente(InventariosESDetalles inventarioentradaosalida)
        {
            try
            {
                Validar();
                List<LotesSeries> lstUnidades = new List<LotesSeries>();
                EmpresaContext db = new EmpresaContext();
                var existencialotesseries = (from g in db.ExistenciasLotesSeries
                                    where g.ComponenteId == inventarioentradaosalida.ComponenteId
                                             where g.Existencia>0 where g.AlmacenId==inventarioentradaosalida.AlmacenId
                                    select g)
                               .ToList();


                foreach (ExistenciasLotesSeries els in existencialotesseries)
                {
                    var loteserie = db.LotesSeries.Find(els.LotesSeriesId);
                    if(!(loteserie.Lote=="SIN LOTE" || loteserie.NumeroSerie=="SIN SERIE"))
                    lstUnidades.Add(loteserie);
                }

                return lstUnidades;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<InventariosES> getTranspasosPendientes(long almacenid)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                List<InventariosES> Lista = db.InventariosES.Where(inv => inv.ConceptoId==12 && inv.AlmacenDestinoId==almacenid && inv.SalidaTraspasoAplicada=="NO" && inv.Cancelado=="NO").
                                            Include(a => a.Almacenes).Include(invd=>invd.InventariosESDetalles.Select(c=>c.Componentes)).
                                            Include(invls => invls.InventariosESDetalles.Select(c => c.InventariosESLotesSeries.Select(lt => lt.LotesSeries))).ToList();

                return Lista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<InventariosES> getAbrirTranspasoPendiente(long id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                List<InventariosES> Lista = db.InventariosES.Where(inv=>inv.Id==id).ToList();

                return Lista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }


    }
    }


