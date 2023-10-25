using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    public class Inventarios : WsBase
    {
        double CostoTotalDocumento = 0;
        //estado  1=Modificar 0=Agrega 
        public void BuscarLoteSerie(string Tipo, InventariosESLotesSeries item, long ComponenteId, long AlmacenId, EmpresaContext db, sbyte estado, string naturaleza,long? conceptoid)
        {
            try
             {
                //Busca el lote serie para poder empatar los id
                LotesSeries LoteSerie;
                LotesSeries oldvalueesloteserie = db.LotesSeries.Where(id => id.Id == item.LotesSeriesId).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(item.LotesSeries.Lote) && Tipo == "Lote") item.LotesSeries.Lote = "SIN LOTE";
                if (string.IsNullOrWhiteSpace(item.LotesSeries.NumeroSerie) && Tipo == "NumeroSerie") item.LotesSeries.NumeroSerie = "SIN SERIE";
                if (Tipo == "Lote")
                    LoteSerie = db.LotesSeries.Where(l => l.Lote == item.LotesSeries.Lote).Where(c => c.ComponenteId == ComponenteId).SingleOrDefault();
                else
                    LoteSerie = db.LotesSeries.Where(l => l.NumeroSerie == item.LotesSeries.NumeroSerie).Where(c => c.ComponenteId == ComponenteId).SingleOrDefault();
                

                if (LoteSerie != null && (estado == 0 || (oldvalueesloteserie.NumeroSerie == "SIN SERIE" || oldvalueesloteserie.Lote == "SIN LOTE")))
             { 
                

                    if (naturaleza == "ENTRADA" && Tipo == "NumeroSerie" && item.LotesSeries.NumeroSerie != "SIN SERIE" && db.ExistenciasLotesSeries.Where(v => v.LotesSeriesId == LoteSerie.Id).Sum(e=>e.Existencia) >= 1 )
                    {
                        throw new Exception("La serie '"+ item.LotesSeries.NumeroSerie+"' ya se encuentra registrada");
                    }
                    else
                    {
                        ExistenciasLotesSeries existenciaserie = db.ExistenciasLotesSeries.Where(v => v.LotesSeriesId == LoteSerie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                        if (existenciaserie != null)
                        {
                            if (naturaleza == "SALIDA" && existenciaserie.Existencia != 1 && item.LotesSeries.NumeroSerie != "SIN SERIE" && Tipo != "Lote" && conceptoid!=14  )
                                throw new Exception("No se encuentra suficiente existencia de la serie" + " ' " + existenciaserie.LotesSeries.NumeroSerie + " ' " + " existencia: " + existenciaserie.Existencia);
                            if (estado == 0)
                            {
                                if (naturaleza == "SALIDA" && item.LotesSeries.NumeroSerie != "SIN SERIE" && Tipo != "Lote")
                                    existenciaserie.Existencia -= 1;
                                else if (naturaleza == "ENTRADA" && item.LotesSeries.NumeroSerie != "SIN SERIE" && Tipo != "Lote")
                                    existenciaserie.Existencia += 1;
                            }
                            else if (estado == 1)
                            {

                                if (item.LotesSeries.NumeroSerie != null || item.LotesSeries.Lote == "SIN LOTE") item.LotesSeries.FechaCaducidad = null;
                                if (naturaleza == "SALIDA" && item.LotesSeries.NumeroSerie != "SIN SERIE" && Tipo != "Lote" && oldvalueesloteserie.NumeroSerie == "SIN SERIE") { 
                                    existenciaserie.Existencia -= 1;
                                    ExistenciasLotesSeries sinserie = db.ExistenciasLotesSeries.Where(ls=>ls.LotesSeriesId==oldvalueesloteserie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                                    sinserie.Existencia += 1;
                                }
                                else if (naturaleza == "ENTRADA" && item.LotesSeries.NumeroSerie != "SIN SERIE" && Tipo != "Lote" && oldvalueesloteserie.NumeroSerie == "SIN SERIE") { 
                                    existenciaserie.Existencia += 1;
                                    ExistenciasLotesSeries sinserie = db.ExistenciasLotesSeries.Where(ls => ls.LotesSeriesId == oldvalueesloteserie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                                    sinserie.Existencia -= 1;
                                }
                            }
                        }
                    }
                    if (item.LotesSeries.NumeroSerie == "SIN SERIE" || Tipo == "Lote")
                    {
                        ExistenciasLotesSeries existencia = db.ExistenciasLotesSeries.Where(v => v.LotesSeriesId == LoteSerie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                        if (existencia != null)
                        {
                            if (estado == 0 || Tipo == "Lote")
                                if (naturaleza == "ENTRADA") { 
                                    existencia.Existencia += item.Cantidad;
                               /* else if (existencia.Existencia < item.Cantidad && item.LotesSeries.Lote != "SIN LOTE" && Tipo != "NumeroSerie")
                                    throw new Exception("No se encuentra suficiente existencia del lote: " + " ' " + existencia.LotesSeries.Lote + " ' " + " existencia: " + existencia.Existencia);*/
                                    
                                    if(oldvalueesloteserie!=null && oldvalueesloteserie.Lote=="SIN LOTE")
                                       {
                                         ExistenciasLotesSeries sinlote = db.ExistenciasLotesSeries.Where(ls => ls.LotesSeriesId == oldvalueesloteserie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                                         sinlote.Existencia -= item.Cantidad;
                                       }
                                }
                            if (naturaleza == "SALIDA") { 
                                existencia.Existencia -= item.Cantidad;
                                if (oldvalueesloteserie != null && oldvalueesloteserie.Lote == "SIN LOTE")
                                {
                                    ExistenciasLotesSeries sinlote = db.ExistenciasLotesSeries.Where(ls => ls.LotesSeriesId == oldvalueesloteserie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                                    sinlote.Existencia += item.Cantidad;
                                }
                            }
                        }

                    }
            //       
                  //  else 
                  //  {
                  ////////////////////////////////PARA UN NUEVO REGISTRO CUANDO NO EXISTA EN LA TABLA EXISTENCIA LOTES SERIES ES POR ARTICULO Y ALMACEN///////////////////////////
                        ExistenciasLotesSeries existencia1 = db.ExistenciasLotesSeries.Where(v => v.LotesSeriesId == LoteSerie.Id).Where(a => a.AlmacenId == AlmacenId).SingleOrDefault();
                        if (existencia1 == null) { 
                        //EmpresaContext dbauxiliar = new EmpresaContext();
                        ExistenciasLotesSeries existencianueva = new ExistenciasLotesSeries();
                        existencianueva.LotesSeriesId = LoteSerie.Id;
                        existencianueva.ComponenteId = ComponenteId;
                        if (naturaleza == "ENTRADA")
                            existencianueva.Existencia = item.Cantidad;
                        else
                            existencia1.Existencia -= item.Cantidad;
                        existencianueva.AlmacenId = AlmacenId;
                        db.ExistenciasLotesSeries.Add(existencianueva);
                        db.SaveChanges();
                        }
                  //  }

                    item.LotesSeriesId = LoteSerie.Id;
                    item.LotesSeries = null;
                
            }
                else if(LoteSerie==null)
                {

                    //Se genera un context nuevo para poder obtener el id real en a base de datos de otra manera no encuentra el objeto
                    // que se acaba de agregar en la tabla lotes series
                    db.LotesSeries.Add(item.LotesSeries);
                    db.SaveChanges();
                    item.LotesSeriesId = item.LotesSeries.Id;

                    ExistenciasLotesSeries existencianueva = new ExistenciasLotesSeries();
                    existencianueva.LotesSeriesId = item.LotesSeries.Id;
                    existencianueva.ComponenteId = ComponenteId;
                    if (naturaleza == "ENTRADA")
                        existencianueva.Existencia = item.Cantidad;
                    else
                        existencianueva.Existencia -= item.Cantidad;
                    existencianueva.AlmacenId = AlmacenId;

                    item.LotesSeries.ExistenciasLotesSeries = null;
                    item.LotesSeries.InventariosESLotesSeries = null;
                    db.ExistenciasLotesSeries.Add(existencianueva);
                    db.SaveChanges();


                    //Resta existencia a SIN SERIE o SIN LOTE
                    if (estado == 1)
                    {
                        ExistenciasLotesSeries existencia;
                        if (Tipo == "Lote")
                        {
                            long sinloteid = db.LotesSeries.Where(id => id.Lote == "SIN LOTE").Where(c => c.ComponenteId == ComponenteId).Single().Id;
                            existencia = db.ExistenciasLotesSeries.Where(c => c.ComponenteId == ComponenteId).Where(lt => lt.LotesSeriesId == sinloteid).Where(a => a.AlmacenId == AlmacenId).Single();
                        }
                        else
                        {
                            long sinserieid = db.LotesSeries.Where(id => id.NumeroSerie == "SIN SERIE").Where(c => c.ComponenteId == ComponenteId).Single().Id;
                            existencia = db.ExistenciasLotesSeries.Where(c => c.ComponenteId == ComponenteId).Where(lt => lt.LotesSeriesId == sinserieid).Where(a => a.AlmacenId == AlmacenId).Single();
                        }
                        existencia.Existencia -= existencianueva.Existencia;
                    }


                    //item.LotesSeries = null;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void CalcularVariacionCostoAlGuardar(double componenteid, double costounitario,EmpresaContext db,UsuariosContext udb,ValidarToken Token)
        {
            try
            {

                string rfc = Token.getKey("bd", "TokenEmpresa");
                string empresavariacionsino = udb.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().ValidaVariacionCosto;
                long? ultimocostoid = db.InventariosESDetalles.Where(co => co.ComponenteId == componenteid).Max(i => (long?)i.Id);
                if (ultimocostoid == null || empresavariacionsino == "NO") return; //VALIDA SI TIENE VARIACION LA EMPRESA, Y SI ES LA PRIMERA ENTRADA 
                double? ultimocosto = db.InventariosESDetalles.Where(id => (long?)id.Id == ultimocostoid).FirstOrDefault().CostoUnitario;
                /*   var Lstcostoxcomponente = (from co in db.InventariosESDetalles where co.ComponenteId == componenteid select co.CostoUnitario).ToList();
                   double ultimocostounitario = 0;*/

                if (ultimocosto > 0)
                {

                    double? porcentajeempresa = udb.BDEmpresas.Where(s => s.RFC == rfc).Where(s => s.ValidaVariacionCosto == "SI").FirstOrDefault().PorcentajeVariacionCosto;
                    /*   ultimocostounitario = Lstcostoxcomponente.ToList().OrderByDescending(p => p).ToList().First();
                       ultimocostounitario = ((costounitario - ultimocostounitario) / ultimocostounitario) * 100;*/
                    ultimocosto = ((costounitario - ultimocosto) / ultimocosto) * 100;
                    if (ultimocosto > porcentajeempresa && ultimocosto > 0)
                    {
                        throw new Exception("No cuentas con permiso para guardar documentos con cambio en variacion de precio de ");
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RevisarlotesAlGuardar(InventariosESDetalles inventarioentradaosalida,EmpresaContext db,UsuariosContext udb)
        {
            try
            {
          

                if (inventarioentradaosalida.InventariosESLotesSeries.Sum(p => p.Cantidad) < inventarioentradaosalida.Cantidad)
                    throw new Exception("Te hacen falta lotes por capturar en " + inventarioentradaosalida.Componentes.Nombre);
                // inventarioentradaosalida.InventariosESLotesSeries.Except(inventarioentradaosalida.InventariosESLotesSeries.ToList().Where(a => a.Cantidad == 0));
                //inventarioentradaosalida.InventariosESLotesSeries.ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GeneraFolio(long conceptoESId)
        {
            try
            {
                //Metodo para obtener una nueva clave
                EmpresaContext db = new EmpresaContext();


                // Se obtiene la clabe del concepto que le corresponde
                string prefijo = (from t in db.ConceptosES
                                  where t.Id == conceptoESId
                                  select t.Clave
                              ).ToList().First().ToString();
                //Se obitiene el ultimo folio usado del tipo de componente
                // para ello primero se obtiene una lista de los folios(Clave sin prefijo) de los componentes del tyipo que le corresponde

                var LstFolios = (from c in db.InventariosES
                                 where c.ConceptoId == conceptoESId
                                 where c.FolioAutomatico == "SI"
                                 select c.Folio.Substring(prefijo.Length)
                                  ).ToList();
                //Se convierte la lista a long para despues ordenarla y obtener el primer registro el cual seria el ultimo folio
                long ultimofolio = 0;
                if (LstFolios.Count > 0)
                {
                    ultimofolio = LstFolios.ConvertAll(s => Int64.Parse(s)).ToList().OrderByDescending(p => p).ToList().First();
                }


                //Se calcula el proximo consecutivo
                var consecutivo = ultimofolio + 1;
                //Se arma la nueva clave
                string nuevaclave = prefijo + consecutivo.ToString();

                return nuevaclave;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void EspermisoCapturaDiaria(long accionid, DateTime fechacaptura,UsuariosContext udb, ValidarToken ValidarToken)
        {
            try
            {
                if (TienePermisoEspecial(accionid,udb, ValidarToken))
                {
                    if (fechacaptura.ToShortDateString() != DateTime.Today.ToShortDateString())
                        throw new Exception("No cuentas con permiso para realizar movimientos con fecha diferente a la de hoy");

                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public string getEmpresaCosteo() //retorna el enum MetodoCosteo de la tabla BDEmpresas, de la empresa que se esta trabjando
        {
            try
            {
                UsuariosContext db = new UsuariosContext();
                ValidarToken Token = new ValidarToken();
                string rfc = Token.getKey("bd", "TokenEmpresa");
                return db.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().MetodoCosteo;

            }
            catch (Exception)
            {
                throw;
                /*string error = null;
                error = ex.Message;
                return null;*/
            }
        }
        public class CostosSaldosLista
        {
            public List<InventariosSaldos> ListaSaldos { get; set; }
            public List<InventariosCostos> ListaCostos { get; set; }
        }
        public InventariosES add(InventariosES inventarioentradaosalida, EmpresaContext db)
        {
            UsuariosContext udb = new UsuariosContext();
            ValidarToken Token = new ValidarToken();
            string rfc = Token.getKey("bd", "TokenEmpresa");
            string claveconcepto;
            bool yaexiste;
            string MetodoCosteoEmpresa = getEmpresaCosteo();
            string empresavariacionsino = udb.BDEmpresas.Where(s => s.RFC == rfc).FirstOrDefault().ValidaVariacionCosto;
            string escostoautomatico= escostoautomatico = db.ConceptosES.Find(inventarioentradaosalida.InventariosESDetalles.FirstOrDefault().ConceptoId).CostoAutomatico;
            bool permisoagregararticulosinloteentrada = TienePermisoEspecial(159,udb,Token);
            bool permisoagregararticulosinlotesalida = TienePermisoEspecial(181, udb, Token);
            bool permisoregistrarentradavariacioncosto = TienePermisoEspecial(154, udb, Token);
            bool permisoagregararticulosinserieentrada = TienePermisoEspecial(160, udb, Token);
            bool permisoagregararticulosinseriesalida = TienePermisoEspecial(182, udb, Token);


            try
            {
                if (!TienePermisoEspecial(117, udb, Token) && inventarioentradaosalida.Naturaleza == "ENTRADA") throw new Exception("No cuentas con permiso para crear documentos de entradas");
                if (!TienePermisoEspecial(162, udb, Token) && inventarioentradaosalida.Naturaleza == "SALIDA") throw new Exception("No cuentas con permiso para crear documentos de salidas");
                
                if (inventarioentradaosalida.Naturaleza=="ENTRADA" && !TienePermisoEspecial(125, udb, Token))//SENTENCIA QUE COMPARA SI EL USUARIO TIENE PERMISO PERMITIR CAPTURAR FUERA DEL PERIODO CONFIGURADO EN LA EMPRESA. SI NO LO TIENE CONSULTA OTRAS DOS SENTENCIAS DENTRO DEL IF
                {
                    EspermisoCapturaDiaria(124, inventarioentradaosalida.Fecha,udb, Token);//SENTENCIA QUE COMPARA SI EL USUARIO EL DE TIPO "OBLIGAR CAPTURA DIARIA" SE MANDA COMO PARAMETRO LA ACCIONID Y LA FECHA DEL MOVIMIENTO, SI HACE MOVIMIENTO FUERA DEL DIA ACTUAL, SE MANDA EXCEPCION Y NO CONTINUA EL PROCESO
                    CalcularPeriodo(inventarioentradaosalida.Fecha);//SENTENCIA QUE COMPARA SI EL MOVIEMIENTO ESTA DENTRO DEL RANGO DEL PERIODO CONFIGURADO EN LA EMPRESA
                }
                else
                {
                if (inventarioentradaosalida.Naturaleza == "SALIDA" && !TienePermisoEspecial(169, udb, Token))
                    {
                        EspermisoCapturaDiaria(179, inventarioentradaosalida.Fecha, udb, Token);//SENTENCIA QUE COMPARA SI EL USUARIO EL DE TIPO "OBLIGAR CAPTURA DIARIA" SE MANDA COMO PARAMETRO LA ACCIONID Y LA FECHA DEL MOVIMIENTO, SI HACE MOVIMIENTO FUERA DEL DIA ACTUAL, SE MANDA EXCEPCION Y NO CONTINUA EL PROCESO
                        CalcularPeriodo(inventarioentradaosalida.Fecha);//SENTENCIA QUE COMPARA SI EL MOVIEMIENTO ESTA DENTRO DEL RANGO DEL PERIODO CONFIGURADO EN LA EMPRESA
                    }
                }

                if ((from t in db.ConceptosES
                     where t.Id == inventarioentradaosalida.ConceptoId
                     select t.FolioAutomatico).FirstOrDefault().ToString() == "SI") //SENTENCIA QUE COMPARA SI EL CONCEPTO DE LA ENTRADA O SALIDA TIENE ACTUALMENTE FOLIO AUTOMATICO O NO
                {
                    inventarioentradaosalida.Folio = GeneraFolio(inventarioentradaosalida.ConceptoId); //GENERA EL FOLIO DE LA ENTRADA O SALIDA CUANDO ES ATUMATICO
                    inventarioentradaosalida.FolioAutomatico = "SI"; // AÑADIR EL "SI" EN LA TABLA InventariosES ya que es folio automatico.
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(inventarioentradaosalida.Folio)) //SENTENCIA QUE COMPARA SI EL CONCEPTO DE LA ENTRADA O SALIDA VIENE VACIO O CON ESPACIOS
                        throw new Exception("El campo folio es obligatorio");
                    inventarioentradaosalida.FolioAutomatico = "NO"; // AÑADIR EL "NO" EN LA TABLA InventariosES ya que NO es folio automatico.
                    claveconcepto = (from o in db.ConceptosES where o.Id == inventarioentradaosalida.ConceptoId where o.FolioAutomatico == "NO" select o.Clave).FirstOrDefault().ToString();//OBTIENE LA CLAVE DEL CONCEPTO DE LA TABLA ConceptosES
                    yaexiste = inventarioentradaosalida.Folio.StartsWith(claveconcepto);
                    if (yaexiste) //SENTENCIA QUE COMPARA SI EL FOLIO ESCRITO POR EL USUARIO CONTIENE LA MISMA CLAVE DEL CONCEPTO, PARA MANDARLE UNA EXCEPCION DE QUE NO ES POSIBLE PONER EN EL FOLIO LA CLAVE DEL CONCEPTO
                        throw new Exception("El folio escrito no puede iniciar con la clave del concepto");
                }

                if (inventarioentradaosalida.InventariosESDetalles.Count == 0)//SENTENCIA QUE COMPARA SI NO TRAE NINGUN DETALLE
                    throw new Exception("Los detalles no han sido indicados");

                var nRegCantidadCero = inventarioentradaosalida.InventariosESDetalles.Where(z => z.Cantidad == 0).Count();
                if(nRegCantidadCero>0)
                    throw new Exception("No se puede dejar en cero la cantidad de un componente");

                foreach (InventariosESDetalles detalle in inventarioentradaosalida.InventariosESDetalles)//FOR PARA PONERLE EL METODO DE COSTEO Y FECHA A CADA DETALLE EN LA TABLA INVENTARIOESDETALLES
                {

                    detalle.MetodoCosteo = MetodoCosteoEmpresa;
                    detalle.ComponentesAlmacenesId = db.ComponentesAlmacenes.Where(c => c.ComponenteId == detalle.ComponenteId).Where(a => a.AlmacenId == inventarioentradaosalida.AlmacenId).Single().Id;

                           
                        
                              
                           if (inventarioentradaosalida.Naturaleza=="SALIDA")EmpresaSalidasinExistencia(detalle,db,udb,Token); //CHECA SI LA PREFERENCIA DE LA EMPRESA SPERMITIR SALIDAS SIN EXISTENCIA Y MANDA UNA EXCEPCION SI NO TIENE SUFICIENTE EXISTENCIA
                           
                           if (detalle.ConceptoId == 12) //CHECA SI ES TRANSPASO DE SALIDA Y PROCEDE A CHECAR QUE SEAN COMPATIBLES LOS ALMACENES (DEL MISMO TIPO Y QUE MANEJEN EL MISMO GRUPO COMPONENTE)
                           {
                       
                        var almacengrupo = db.ComponentesAlmacenes.Where(g => g.ComponenteId == detalle.ComponenteId).Where(a => a.AlmacenId == inventarioentradaosalida.AlmacenDestinoId).SingleOrDefault();
                        if (almacengrupo == null)
                                   throw new Exception("Este almacen no lleva el contro del  componente " + detalle.Componentes.Nombre + " agrega su grupo en la configuracion del almacen");
                               if (inventarioentradaosalida.AlmacenDestinoId == inventarioentradaosalida.AlmacenId)
                                   throw new Exception("No es posible hacer un transpaso al mismo almacen");
                               inventarioentradaosalida.AlmacenDestinoId = inventarioentradaosalida.AlmacenDestinoId;
                           }


                           if (detalle.TipoSeguimiento == "LOTES")
                           {
                         
                               RevisarlotesAlGuardar(detalle,db,udb);
                           }

                           if (empresavariacionsino == "SI")
                           {
                               if (!permisoregistrarentradavariacioncosto)
                                   CalcularVariacionCostoAlGuardar(detalle.ComponenteId, detalle.CostoUnitario,db,udb,Token);
                           }


                         

                        foreach (InventariosESLotesSeries detalleloteserie in detalle.InventariosESLotesSeries)//FOR PARA RECORRER LOS DETALLES DE LOS COMPONENTES CON SERIE O LOTE Y PODER REVISAR SI VIENEN VACIOS Y CORREOBORAR SI TIENE PERMISO ESPECIAL
                           {

                               if (detalle.TipoSeguimiento == "NÚMERO DE SERIE" && (detalleloteserie.LotesSeries.NumeroSerie == null || string.IsNullOrWhiteSpace(detalleloteserie.LotesSeries.NumeroSerie)))
                               {

                                   if (inventarioentradaosalida.Naturaleza=="ENTRADA" && !permisoagregararticulosinserieentrada)
                                       throw new Exception("No cuentas con permiso para guardar documentos sin serializar los componentes");
                                   else if (inventarioentradaosalida.Naturaleza == "SALIDA" && !permisoagregararticulosinseriesalida)
                                      throw new Exception("No cuentas con permiso para guardar documentos sin serializar los componentes");
                        }
                               else if (detalle.TipoSeguimiento == "LOTES" && (detalleloteserie.LotesSeries.Lote == null || string.IsNullOrWhiteSpace(detalleloteserie.LotesSeries.Lote)))
                               {
                                   //   detalleloteserie.Lote = null;
                                   if (inventarioentradaosalida.Naturaleza == "ENTRADA" && !permisoagregararticulosinloteentrada)
                                       throw new Exception("No cuentas con permiso para guardar documentos sin lotificar los componentes");
                                   else if (inventarioentradaosalida.Naturaleza == "SALIDA" && !permisoagregararticulosinlotesalida)
                                       throw new Exception("No cuentas con permiso para guardar documentos sin lotificar los componentes");
                        }
                               if (detalle.TipoSeguimiento == "NÚMERO DE SERIE" || detalleloteserie.LotesSeries.Lote=="SIN LOTE") detalleloteserie.LotesSeries.FechaCaducidad = null;
                               string tipo = detalle.TipoSeguimiento == "LOTES" ? "Lote" : "NumeroSerie";
                               if (detalle.TipoSeguimiento == "LOTES" && detalleloteserie.LotesSeries.FechaCaducidad == null && !(detalleloteserie.LotesSeries.Lote == null || detalleloteserie.LotesSeries.Lote == "SIN LOTE" || string.IsNullOrWhiteSpace(detalleloteserie.LotesSeries.Lote)))
                                       throw new Exception("La fecha de los lotes es obligatoria");
                               BuscarLoteSerie(tipo, detalleloteserie, detalle.ComponenteId, detalle.AlmacenId, db, 0, inventarioentradaosalida.Naturaleza,inventarioentradaosalida.ConceptoId);
                           }

                            InventariosCostos Costo = LlenarObjetoInventariosCostos(inventarioentradaosalida, detalle, MetodoCosteoEmpresa, escostoautomatico, db);
                            InventariosSaldos Saldo = LlenarObjetoInventartiosSaldos(inventarioentradaosalida, detalle, db);

                            CostoTotalDocumento += detalle.CostoTotal;
                            //Se actualiza tabla inventarios costos
                            if ((Costo != null))
                            {

                                if (Costo.Id == 0)
                                {
                                    db.InventariosCostos.Add(Costo);
                                }
                                else
                                {
                                    db.InventariosCostos.Attach(Costo);
                                    db.Entry(Costo).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                            //SE ACTUALIZA TABLA InventariosSaldos
                            if (Saldo.Id == 0)
                            {
                                db.InventariosSaldos.Add(Saldo);

                            }
                            else
                            {
                                db.InventariosSaldos.Attach(Saldo);
                                db.Entry(Saldo).State = System.Data.Entity.EntityState.Modified;

                            }
                            detalle.Componentes = null;
                            //detalle.InventariosFisicosDetalles = null;
                }

              inventarioentradaosalida.CostoTotal = CostoTotalDocumento;
          
                if (inventarioentradaosalida.ConceptoId == 2)
                {
                    var salida = db.InventariosES.Find(inventarioentradaosalida.SalidaTraspasoId);
                    salida.SalidaTraspasoAplicada = "SI";
                    db.Entry(salida).State = System.Data.Entity.EntityState.Modified;
                    inventarioentradaosalida.SalidaTraspasoAplicada = "NO";
                }

                else
                   inventarioentradaosalida.SalidaTraspasoAplicada = "NO";
                inventarioentradaosalida.InventariosFisicos = null;
                inventarioentradaosalida.InventariosFisicos1 = null;

               
                db.InventariosES.Add(inventarioentradaosalida);
                db.SaveChanges();

                return inventarioentradaosalida;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public InventariosCostos LlenarObjetoInventariosCostos(InventariosES inventario,InventariosESDetalles Detalle,string metododecosteo, string escostoautomatico, EmpresaContext db)
        {
            try
            {
               
               
                    InventariosCostos Costo = new InventariosCostos();
                    InventariosCostos CostoActual = new InventariosCostos();

                     CostoActual = db.InventariosCostos.Where(s => s.ComponenteId == Detalle.ComponenteId && s.AlmacenId == inventario.AlmacenId).FirstOrDefault();


                    
                    ///////////////////////////////PARA LA TABLA INVENTARIOSCOSTOS//////////////////////////////
                  /*  if (metododecosteo == "PEPS") //SI ES PEPS
                    {
                        if (CostoActual == null) // SI NO HAY NINGUN MOVIMIENTO EN LA TABLA INVENTARIOCOSTO  X ARTICULO  Y ALMACEN 
                        {

                            if (inventario.Naturaleza == "ENTRADA") // SI ES LA PRIMERA ENTRADA X ARTICULO  Y ALMACEN 
                            {
                                //         CostoActual = db.InventariosCostos.Where(s => s.ComponenteId == Detalle.ComponenteId).FirstOrDefault(); //SE BUSCA SOLO POR EL ARTICULO(YA QUE NO SE ENCONTRO POR ARTICULO Y ALMACEN) PARA PONER ESE COSTO
                                long? ultimocostopepsid = db.InventariosCostos.Where(co => co.ComponenteId == Detalle.ComponenteId).OrderBy(f => f.Fecha).Max(i => (long?)i.Id);
                                if (escostoautomatico == "SI")// SI ES COSTO AUTOMATICO 
                                {


                                    var ultimocosto = db.InventariosCostos.Find(ultimocostopepsid);
                                    if (ultimocosto == null) //SIGNIFICA QUE NO HAY NINGUN REGISTRO EN LA TABLA INVENTARIOSCOSTOS PARA EL ARTICULO DADO
                                    {
                                        Costo.ComponenteId = Detalle.ComponenteId;
                                        Costo.AlmacenId = inventario.AlmacenId;
                                        Costo.Fecha = Detalle.Fecha;
                                        Costo.Activo = "SI";
                                        Costo.Existencia = Detalle.Cantidad;
                                        Costo.CostoTotal = Detalle.CostoTotal;
                                    }
                                    else
                                    {
                                        Costo.Existencia = Detalle.Cantidad;
                                        Costo.CostoTotal = (ultimocosto.CostoTotal / ultimocosto.Existencia) * Detalle.Cantidad;
                                        Costo.ComponenteId = Detalle.ComponenteId;
                                        Costo.AlmacenId = inventario.AlmacenId;
                                        Costo.Fecha = Detalle.Fecha;
                                        Costo.Activo = "SI";
                                        Detalle.CostoTotal = Costo.CostoTotal;
                                        Detalle.CostoUnitario = (ultimocosto.CostoTotal / ultimocosto.Existencia);
                                    }
                                }
                                else // SI NO ES COSTO AUTOMATICO
                                {
                                    Costo.ComponenteId = Detalle.ComponenteId;
                                    Costo.AlmacenId = inventario.AlmacenId;
                                    Costo.Fecha = Detalle.Fecha;
                                    Costo.Activo = "SI";
                                    Costo.Existencia = Detalle.Cantidad;
                                    Costo.CostoTotal = Detalle.CostoTotal;
                                }
                            }
                        //   CostoTotalDocumento += Detalle.CostoTotal;
                        //list1.Add(Costo);
                        return Costo;
                        }
                        else
                        {
                            if (inventario.Naturaleza == "ENTRADA") // SI ES LA PRIMERA ENTRADA X ARTICULO  Y ALMACEN 
                            {
                                //         CostoActual = db.InventariosCostos.Where(s => s.ComponenteId == Detalle.ComponenteId).FirstOrDefault(); //SE BUSCA SOLO POR EL ARTICULO(YA QUE NO SE ENCONTRO POR ARTICULO Y ALMACEN) PARA PONER ESE COSTO
                                long? ultimocostopepsid = db.InventariosCostos.Where(co => co.ComponenteId == Detalle.ComponenteId).Where(al => al.AlmacenId == Detalle.AlmacenId).OrderBy(f => f.Fecha).Max(i => (long?)i.Id);
                                if (escostoautomatico == "SI")// SI ES COSTO AUTOMATICO 
                                {

                                    var ultimocosto = db.InventariosCostos.Find(ultimocostopepsid);
                                    Costo.Existencia = Detalle.Cantidad;
                                    Costo.CostoTotal = (ultimocosto.CostoTotal / ultimocosto.Existencia) * Detalle.Cantidad;
                                    Costo.ComponenteId = Detalle.ComponenteId;
                                    Costo.AlmacenId = inventario.AlmacenId;
                                    Costo.Fecha = Detalle.Fecha;
                                    Costo.Activo = "SI";
                                    Detalle.CostoTotal = Costo.CostoTotal;
                                    Detalle.CostoUnitario = (ultimocosto.CostoTotal / ultimocosto.Existencia);

                                }
                                else // SI NO ES COSTO AUTOMATICO
                                {
                                    Costo.ComponenteId = Detalle.ComponenteId;
                                    Costo.AlmacenId = inventario.AlmacenId;
                                    Costo.Fecha = Detalle.Fecha;
                                    Costo.Activo = "SI";
                                    Costo.Existencia = Detalle.Cantidad;
                                    Costo.CostoTotal = Detalle.CostoTotal;
                                }
                            //  CostoTotalDocumento += Detalle.CostoTotal;
                            // list1.Add(Costo);
                            return Costo;
                            }

                            else //SI ES SALIDA
                            {
                                CostoActual = db.InventariosCostos.Where(s => s.ComponenteId == Detalle.ComponenteId).Where(s => s.AlmacenId == inventario.AlmacenId).Where(c => c.Activo == "SI").FirstOrDefault();
                                if (CostoActual != null) // 
                                {
                                    if (Detalle.Cantidad <= CostoActual.Existencia && CostoActual.Activo == "SI")
                                    {
                                        Detalle.CostoTotal = (CostoActual.CostoTotal / CostoActual.Existencia) * Detalle.Cantidad;
                                        Detalle.CostoUnitario = (CostoActual.CostoTotal / CostoActual.Existencia);
                                        CostoActual.Existencia = CostoActual.Existencia - Detalle.Cantidad;
                                        CostoActual.Activo = CostoActual.Existencia == 0 ? "NO" : "SI";
                                        CostoActual.CostoTotal = CostoActual.CostoTotal - Detalle.CostoTotal;
                                    }
                                //   CostoTotalDocumento += Detalle.CostoTotal;
                                //list1.Add(CostoActual);
                                return CostoActual;
                                }

                            }

                        }

                    }*/

                    //--------------------------------------------------------------M E T O D O   P R O M E D I O   P O N D E R A D O   -----------------------------------------------------------------------------------------------------------
                     if (metododecosteo == "PROMEDIO PONDERADO")
                    {
                        if (CostoActual == null) // SI NO HAY NINGUN MOVIMIENTO EN LA TABLA INVENTARIOCOSTO  X ARTICULO  Y ALMACEN 
                        {

                            if (inventario.Naturaleza == "ENTRADA") // SI ES LA PRIMERA ENTRADA X ARTICULO  Y ALMACEN 
                            {
                           
                               var ultimocosto = db.InventariosCostos.Where(co => co.ComponenteId == Detalle.ComponenteId && co.AlmacenId == Detalle.AlmacenId).SingleOrDefault();

                                    if (ultimocosto == null || escostoautomatico == "NO") // SI NO ES COSTO AUTOMATICO O  el ultimo costo es null SIGNIFICA QUE NO HAY NINGUN REGISTRO EN LA TABLA INVENTARIOSCOSTOS PARA EL ARTICULO DADO
                                {
                                        Costo.ComponenteId = Detalle.ComponenteId;
                                        Costo.AlmacenId = inventario.AlmacenId;
                                        Costo.Fecha = Detalle.Fecha;
                                        Costo.Activo = "SI";
                                        Costo.Existencia = Detalle.Cantidad;
                                        Costo.CostoTotal = Detalle.CostoTotal;
                                    }
                                    else
                                    {
                                        Costo.Existencia = Detalle.Cantidad;
                                        Costo.CostoTotal = (ultimocosto.CostoTotal / ultimocosto.Existencia) * Detalle.Cantidad;
                                        Costo.ComponenteId = Detalle.ComponenteId;
                                        Costo.AlmacenId = inventario.AlmacenId;
                                        Costo.Fecha = Detalle.Fecha;
                                        Costo.Activo = "SI";
                                        Detalle.CostoTotal = Costo.CostoTotal;
                                        Detalle.CostoUnitario = (ultimocosto.CostoTotal / ultimocosto.Existencia);
                                    }
                               
                            }

                        return Costo;
                        }
                        else
                        {
                            if (inventario.Naturaleza == "ENTRADA")
                            {

                                if (escostoautomatico == "SI" && CostoActual.Existencia != 0)// SI ES COSTO AUTOMATICO 
                                {
                                        Detalle.CostoTotal = (CostoActual.CostoTotal / CostoActual.Existencia) * Detalle.Cantidad;
                                        Detalle.CostoUnitario = (CostoActual.CostoTotal / CostoActual.Existencia);
                                   
                                }
                                
                            CostoActual.Existencia = CostoActual.Existencia + Detalle.Cantidad;
                            CostoActual.CostoTotal = CostoActual.CostoTotal + Detalle.CostoTotal;
                            CostoActual.Activo = CostoActual.Existencia == 0 ? "NO" : "SI";

                            return CostoActual;
                            }

                            else //SI ES SALIDA
                            {
                                if (Detalle.Cantidad <= CostoActual.Existencia && CostoActual.Activo == "SI")
                                {
                                    Detalle.CostoTotal = (CostoActual.CostoTotal / CostoActual.Existencia) * Detalle.Cantidad;
                                    Detalle.CostoUnitario = (CostoActual.CostoTotal / CostoActual.Existencia);
                                    CostoActual.CostoTotal -= Detalle.CostoTotal;
                                    CostoActual.Existencia -= Detalle.Cantidad;
                                    CostoActual.Activo = CostoActual.Existencia == 0 ? "NO" : "SI";
                              
                                return CostoActual;
                                }

                            }

                        }
                    }

                return null;
            }
            catch (Exception ex)
            {
                /*string error = null;
                error = ex.Message;
                return null;*/
                throw;
            }
        }
        public InventariosSaldos LlenarObjetoInventartiosSaldos(InventariosES inventario, InventariosESDetalles Detalle, EmpresaContext db)
        {
            try
            {
               
                InventariosSaldos Saldo = new InventariosSaldos();
                InventariosSaldos SaldoActual = new InventariosSaldos();
             

                SaldoActual = db.InventariosSaldos.Where(s => s.ComponenteId == Detalle.ComponenteId && s.AlmacenId == inventario.AlmacenId && s.Anio == Detalle.Fecha.Year && s.Mes == Detalle.Fecha.Month).FirstOrDefault();
              

                ////////////////////////////////////PARA LA TABLA SALDOS//////////////////////////////////////////////////////
                      if (SaldoActual == null)
                       {//SI ES UN NUEVO  REGITRO EN LA TABLA INVENTARIOS SALDOS, ES UNO POR CADA MES DEL AÑO
                           Saldo.ComponenteId = Detalle.ComponenteId;
                           Saldo.AlmacenId = inventario.AlmacenId;
                           Saldo.Anio = Detalle.Fecha.Year;
                           Saldo.Mes = Detalle.Fecha.Month;
                           Saldo.UltimoDia = Detalle.Fecha.Day;

                           if (inventario.Naturaleza == "ENTRADA")
                           {
                               Saldo.EntradasUnidades = Detalle.Cantidad;
                               Saldo.EntradasCosto = Detalle.CostoUnitario * Detalle.Cantidad;
                           }
                           else
                           {
                               Saldo.SalidasUnidades = Detalle.Cantidad;
                               Saldo.SalidasCosto = Detalle.CostoUnitario * Detalle.Cantidad;
                           }

                          return Saldo;

                       }
                       else
                       {

                           if (Detalle.Fecha.Day > SaldoActual.UltimoDia)
                               SaldoActual.UltimoDia = Detalle.Fecha.Day;



                           if (inventario.Naturaleza == "ENTRADA")
                           {
                               SaldoActual.EntradasUnidades += Detalle.Cantidad;
                               SaldoActual.EntradasCosto += Detalle.CostoUnitario * Detalle.Cantidad;
                           }
                           else
                           {
                               SaldoActual.SalidasUnidades += Detalle.Cantidad;
                               SaldoActual.SalidasCosto += Detalle.CostoUnitario * Detalle.Cantidad;
                           }
                            return SaldoActual;
                       }
            }
            catch (Exception ex)
            {
                /*string error = null;
                error = ex.Message;
                return null;*/
                throw;
            }
        }
        public InventariosES get(int ID)
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
                    db.Entry(detalle).Reference("Componentes").Load();
                    db.Entry(detalle.Componentes).Reference("Unidades").Load();
                    foreach (InventariosESLotesSeries LoteSerie in detalle.InventariosESLotesSeries)
                    {
                        db.Entry(LoteSerie).Reference("LotesSeries").Load();
                    }
                }

                return documentoES;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public InventariosES update(InventariosES documento,EmpresaContext db)
        {
            try
            {
                UsuariosContext udb = new UsuariosContext();
                ValidarToken Token = new ValidarToken();
                if (!TienePermisoEspecial(118,udb,Token) && documento.Naturaleza == "ENTRADA") throw new Exception("No cuentas con permiso para modificar documentos de entrada");
                if (!TienePermisoEspecial(163, udb, Token) && documento.Naturaleza == "SALIDA") throw new Exception("No cuentas con permiso para modificar documentos de  salida");
                if (documento.Cancelado == "SI") throw new Exception("No se puede modificar un documento cancelado");
                foreach (InventariosESDetalles detalle in documento.InventariosESDetalles)
                {
                    //
                    string tipo = db.Componentes.Find(detalle.ComponenteId).TipoSeguimiento == "LOTES" ? "Lote" : "NumeroSerie";
                    foreach (InventariosESLotesSeries LoteSerie in detalle.InventariosESLotesSeries)
                    {

                        var item = db.LotesSeries.Find(LoteSerie.LotesSeriesId);
                        if ((item.Lote != "SIN LOTE" && (item.Lote != LoteSerie.LotesSeries.Lote)) || (item.NumeroSerie != "SIN SERIE" && (item.NumeroSerie != LoteSerie.LotesSeries.NumeroSerie)))
                            throw new Exception("No se puede modificar un lote/serie de un documento ya guardado ");
                        if ((item.Lote != LoteSerie.LotesSeries.Lote && !string.IsNullOrWhiteSpace(LoteSerie.LotesSeries.Lote)) || (item.Lote != LoteSerie.LotesSeries.NumeroSerie && !string.IsNullOrWhiteSpace(LoteSerie.LotesSeries.NumeroSerie)))
                        {
                          
                                BuscarLoteSerie(tipo, LoteSerie, detalle.ComponenteId, detalle.AlmacenId, db, 1, documento.Naturaleza,null);

                            if (item.Id != LoteSerie.LotesSeriesId)
                            {
                                /* if (item.NumeroSerie != null || item.Lote == "SIN LOTE")*/
                            //    if (LoteSerie.LotesSeries.NumeroSerie != null || LoteSerie.LotesSeries.Lote == "SIN LOTE") LoteSerie.LotesSeries.FechaCaducidad = null;
                                db.Entry(LoteSerie).State = System.Data.Entity.EntityState.Modified;
                                
                            }
                        }
                    }
                    //
                    UpdateLista<InventariosESLotesSeries>(db.InventariosESLotesSeries.Where(e => e.InventariosESDetalleId == detalle.Id).ToList(), detalle.InventariosESLotesSeries.ToList(), db);
                    detalle.InventariosESLotesSeries = null;
                }

                documento.InventariosESDetalles = null;
                db.InventariosES.Attach(documento);
                db.Entry(documento).State = System.Data.Entity.EntityState.Modified;
                db.Entry(documento).Property(p => p.Descripcion).IsModified = false;
                db.Entry(documento).Property(p => p.FolioAutomatico).IsModified = false;
                db.Entry(documento).Property(p => p.SalidaTraspasoAplicada).IsModified = false;

                db.SaveChanges();

                return documento;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public InventariosES cancelar(InventariosES conceptoes,EmpresaContext db)
        {
            InventariosES salidatranspaso;
            try
            {
                UsuariosContext udb = new UsuariosContext();
                ValidarToken Token = new ValidarToken();
                if (!TienePermisoEspecial(119,udb, Token) && conceptoes.Naturaleza == "ENTRADA") throw new Exception("No tienes permisos para cancelar documentos de entrada");
                if (!TienePermisoEspecial(164, udb, Token) && conceptoes.Naturaleza == "SALIDA") throw new Exception("No tienes permisos para cancelar documentos de salida");
                if (DesactivarPermisos == false && conceptoes.ModuloOrigen!="IN") throw new Exception("No se puede cancelar un documento creado en otro modulo");
                if (conceptoes.ConceptoId == 13 || conceptoes.ConceptoId == 14) throw new Exception("No se puede cancelar un documento generado automaticamente por el sistema");
                if (conceptoes.Cancelado == "SI") throw new Exception("No se puede cancelar un documento ya cancelado");
                if (conceptoes.SalidaTraspasoAplicada == "SI" && conceptoes.Naturaleza == "SALIDA") throw new Exception("No se puede cancelar un transpaso ya aplicado");
                if (conceptoes.Naturaleza == "ENTRADA" && conceptoes.ConceptoId == 2)
                {
                    salidatranspaso = db.InventariosES.Find(conceptoes.SalidaTraspasoId);
                    salidatranspaso.SalidaTraspasoAplicada = "NO";
                    salidatranspaso.SalidaTraspasoId = null;
                    salidatranspaso.SalidaTraspasoId = null;
                    db.Entry(salidatranspaso).State = System.Data.Entity.EntityState.Modified;
                }

                InventariosES DocCancelar = this.get((int)conceptoes.Id); 
                conceptoes.Cancelado = "SI";
                DocCancelar.Id = 0;
                DocCancelar.Fecha = conceptoes.Fecha;
                DocCancelar.Naturaleza = conceptoes.Naturaleza == "SALIDA" ? "ENTRADA" : "SALIDA";
                DocCancelar.ConceptoId = conceptoes.Naturaleza == "SALIDA" ? 13 : 14;
                foreach (InventariosESDetalles detalle in DocCancelar.InventariosESDetalles)
                {
                    detalle.Naturaleza = DocCancelar.Naturaleza;
                    detalle.ConceptoId = DocCancelar.ConceptoId;
                }
                DocCancelar.InventariosESDetalles.ToList().ForEach(s => s.Naturaleza = DocCancelar.Naturaleza);
                DocCancelar.Descripcion = DocCancelar.Naturaleza + " GENERADA AUTOMATICAMENTE POR EL SISTEMA, POR CANCELACION DEL FOLIO: " + conceptoes.Folio + "(" + conceptoes.ConceptosES.Nombre + ")";

                this.add(DocCancelar,db);
                db.InventariosES.Attach(conceptoes);
                db.Entry(conceptoes).State = System.Data.Entity.EntityState.Modified;
                db.Entry(conceptoes).Property(p => p.Descripcion).IsModified = false;
                db.Entry(conceptoes).Property(p => p.FolioAutomatico).IsModified = false;
                db.Entry(conceptoes).Property(p => p.SalidaTraspasoAplicada).IsModified = false;
                db.Entry(conceptoes).Property(p => p.ModuloOrigen).IsModified = false;

                db.SaveChanges();

                return conceptoes;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void EmpresaSalidasinExistencia(InventariosESDetalles inventarioentradaosalida,EmpresaContext db,UsuariosContext db1,ValidarToken Token)
        {
            try
            {
                string rfc = Token.getKey("bd", "TokenEmpresa");
                double existenciaactual = (double)ExistenciaActualComponente(inventarioentradaosalida);
                var salidasinexistencia = db1.BDEmpresas.Where(e => e.RFC ==rfc).FirstOrDefault();
                if (salidasinexistencia != null && salidasinexistencia.SalidasSinExistencia=="NO") 
                    if (existenciaactual - inventarioentradaosalida.Cantidad < 0) //si el maximo
                    {
                        throw new Exception(inventarioentradaosalida.Componentes.Nombre+" no tiene suficiente existencia se rechaza la salida");

                    }
            }
            catch (Exception)
            {
                throw;

            }
        }
    }
}