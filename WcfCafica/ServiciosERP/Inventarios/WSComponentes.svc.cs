using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Administracion;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSProductoTerminado" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSProductoTerminado.svc or WSProductoTerminado.svc.cs at the Solution Explorer and start debugging.
    public class WSComponentes : WsBase,IWSComponentes
    {

        public string[] CamposBloqueados;

        public List<Componentes> getall()
        {
            return getAllOfType(0);
        }

        public List<Componentes> getAllOfType(int tipo)
        {
            //Metodo que obtiene los componentes de un tipo determinado o de todos los tipos(al recibir 0 en el parametro)
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna todos los ciudades
               var lstComponentes = db.Componentes.Include("GruposComponentes").Include("SubgruposComponentes").Include("GruposUnidades").
                    Include("Unidades").Include("Unidades1").Include("Unidades2").Include("TiposComponentes").Include("MarcasComponentes").ToList();

                if (tipo==0) //Si el tipo es igual a 0 retorna todos los componentes
                {
                    return lstComponentes;
                }   
                else // de lo contrario rotarnara los que coincida con el filtro
                {
                    return lstComponentes.FindAll(ls => ls.TipoComponenteId == tipo);
                }                                                                   
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Componentes get(int ID)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna el estado buscado
                Componentes componente = db.Componentes.Include("ComponentesImagenes").Include("ComponentesCodigosBarras").Include("Unidades").Include("ComponentesEquivalenciasPartes").
                                        Include("ComponentesAlmacenes").Include("ComponentesAlmacenes.Almacenes").Include("ComponentesImpuestos").
                                        Single(x => x.Id==ID);
                //Include("ComponentesFormula").Include("ComponentesFormulaDetalles").
                db.Entry(componente).Collection(x => x.ComponentesFormula).Query().Where(z => z.Estado == "A").ToList() ;
                db.Entry(componente).Collection(x => x.ComponentesFormulaDetalles).Query().Where(z => z.Estado == "A").ToList();
                return componente;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
       
        public Componentes update(Componentes componente)
        {

            try
            {
                Validar();
                if (!ValidarMaximosMinimos(componente))
                    throw new Exception("La configuracion de maximos, reorden y minimos es incorrecta.");
                if(!ValidarImpuestosRepetidos(componente.ComponentesImpuestos.ToList()))
                    throw new Exception("Existen impuestos repetidos.");

                EmpresaContext db = new EmpresaContext();

                if(componente.TipoComponenteId==4) //Si es servicio
                {
                    CamposBloqueados = new string[] { "GrupoComponentesId", "SubgrupoComponentesId" };
                }
                else
                {
                    CamposBloqueados = new string[] { "GrupoComponentesId", "SubgrupoComponentesId", "GrupoUnidadesId", "UnidadInventarioId" };
                }

                ValidarCamposBloqueados<Componentes, EmpresaContext>(componente, CamposBloqueados, "Producto Terminado");
                //----------------------------------------------------------------------------------------------------------------------
                //Códigos de barras
                // Se obtienen elementos existentes en la bd, los agregados,los eliminados y los modificados
                List<ComponentesCodigosBarras> CodigosBarrasExistentes = db.ComponentesCodigosBarras.Where(c => c.ComponenteId == componente.Id).ToList();
                List<ComponentesCodigosBarras> CodigosBarrasAgregados = componente.ComponentesCodigosBarras.Where(n => n.Id == 0).ToList();              
                List<ComponentesCodigosBarras> CodigosBarrasModificados= componente.ComponentesCodigosBarras.Where(n => n.Id != 0).ToList();
                List<ComponentesCodigosBarras> CodigosBarrasEliminados = CodigosBarrasExistentes.Where(n => !CodigosBarrasModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.ComponentesCodigosBarras.AddRange(CodigosBarrasAgregados);
                db.ComponentesCodigosBarras.RemoveRange(CodigosBarrasEliminados);
                //Se actualizan los objetos modificados
                foreach (ComponentesCodigosBarras cb in CodigosBarrasModificados)
                {
                    var local = db.Set<ComponentesCodigosBarras>().Local.FirstOrDefault(l =>l.Id == cb.Id);  
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(cb).State = System.Data.Entity.EntityState.Modified;
                }
                //------------------------------------------------------------------------------------------------------------------------
                //Repetimos los mismo para Almacenes
                // Se obtienen elementos existentes en la bd, los agregados,los eliminados y los modificados
                List<ComponentesAlmacenes> AlmacenesExistentes = db.ComponentesAlmacenes.Where(c => c.ComponenteId == componente.Id).ToList();
                List<ComponentesAlmacenes> AlmacenesAgregados = componente.ComponentesAlmacenes.Where(n => n.Id == 0).ToList();
                List<ComponentesAlmacenes> AlmacenesModificados = componente.ComponentesAlmacenes.Where(n => n.Id != 0).ToList();
                List<ComponentesAlmacenes> AlmacenesEliminados = AlmacenesExistentes.Where(n => !AlmacenesModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();

                //Se agregan y eliminan los rangos correspondientes
                db.ComponentesAlmacenes.AddRange(AlmacenesAgregados);
                db.ComponentesAlmacenes.RemoveRange(AlmacenesEliminados);
                //Se actualizan los objetos modificados
                foreach (ComponentesAlmacenes a in AlmacenesModificados)
                {
                    var local = db.Set<ComponentesAlmacenes>().Local.FirstOrDefault(l => l.Id == a.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(a).State = System.Data.Entity.EntityState.Modified;
                }
                //------------------------------------------------------------------------------------------------------------------------
                //Repetimos los mismo para la imagen
                //OJO:En las imagenes no hay modificacion ya que solo se agregan o eliminan
                // Se obtienen elementos existentes en la bd, los agregados,los eliminados y los modificados
                List<ComponentesImagenes> ImagenesExistentes = db.ComponentesImagenes.Where(c => c.ComponenteId == componente.Id).ToList();
                List<ComponentesImagenes> ImagenesAgregadas = componente.ComponentesImagenes.Where(n => n.Id == 0).ToList();
                List<ComponentesImagenes> ImagenesModificadas = componente.ComponentesImagenes.Where(n => n.Id != 0).ToList();
                List<ComponentesImagenes> ImagenesEliminadas = ImagenesExistentes.Where(n => !ImagenesModificadas.Select(n1 => n1.Id).Contains(n.Id)).ToList();

                //Se agregan y eliminan los rangos correspondientes
                db.ComponentesImagenes.AddRange(ImagenesAgregadas);
                db.ComponentesImagenes.RemoveRange(ImagenesEliminadas);

                //------------------------------------------------------------------------------------------------------------------------
                //FORMULA
                
                bool registrarFormula=false; //bandera para saber si se llevara acabo la accion de guardar la formula

                //Se obtiene la formula actual
                ComponentesFormula FormulaActual = db.ComponentesFormula.Where(c => c.ComponenteId == componente.Id).Where(d=>d.Estado=="A").FirstOrDefault();
                if (FormulaActual != null) //Si existe una formula actualmente se procede a realizar el historico en caso de que haya sufrido modificaciones
                {
                    //Si existe una formula actual  se obtienen sus detalles
                    List<ComponentesFormulaDetalles> FormulaDetallesActuales = db.ComponentesFormulaDetalles.Where(c => c.ComponenteFormulaId == FormulaActual.Id).ToList();
                    //Se obtienen los detalles que trae el objeto
                    List<ComponentesFormulaDetalles> FormulaDetallesEnEdicion = componente.ComponentesFormulaDetalles.ToList();

                    
                    if (FormulaDetallesActuales.Count != FormulaDetallesEnEdicion.Count)//Si el count de cada lista es diferente uno del otro , quiere decir que hubo modificaciones y se activa la bandera
                    {
                        registrarFormula = true;
                    }
                    else // Si son igualesse valida si hay nuevos registros
                    {
                        if(componente.ComponentesFormulaDetalles.Where(x=>x.Id==0).ToList().Count>0)//si hay registros con id = 0 , quiere decir que se agregaron recientemente y se activa la bandera
                        {
                            registrarFormula = true;
                        }
                        else // si no hay registros nuevos
                        {
                            //Se recorre el objeto de la formula editada
                            foreach (ComponentesFormulaDetalles cd in FormulaDetallesEnEdicion)
                            {
                                ComponentesFormulaDetalles existe = FormulaDetallesActuales.Where(z=>z.Id==cd.Id).SingleOrDefault();
                                if (existe == null) //sino existe quiere decir que se elimino de la formula y hay que activar la bandera
                                {
                                    registrarFormula = true;
                                    break;
                                }
                                else
                                {
                                    //si hay alguna modificacion en los campos especificados se activa la bandera
                                     if(existe.Cantidad!=cd.Cantidad || existe.UnidadId!=cd.UnidadId || existe.ComponenteHijoId!=cd.ComponenteHijoId)
                                    {
                                        registrarFormula = true;
                                        break;
                                    }
                                }
                            }
                        }
                        

                    }
                        
                        if(registrarFormula) //Si la bandera se activo hay que registrar el historial de la formula
                        {
                            //Se modifica el campo estado para que se marquen como historial
                            foreach (ComponentesFormulaDetalles cd in FormulaDetallesActuales)
                            {
                                cd.Estado = "H";
                                var local = db.Set<ComponentesFormula>().Local.FirstOrDefault(l => l.Id == cd.Id);
                                if (local != null)
                                    db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                                db.Entry(cd).State = System.Data.Entity.EntityState.Modified;
                            }
                            //Se marca la formula actual como historica
                            FormulaActual.Estado = "H";
                            var actual = db.Set<ComponentesFormula>().Local.FirstOrDefault(l => l.Id == FormulaActual.Id);
                            if (actual != null)
                                db.Entry(actual).State = System.Data.Entity.EntityState.Detached;
                            db.Entry(FormulaActual).State = System.Data.Entity.EntityState.Modified;
                        }
                }
                else
                {
                    registrarFormula = true;
                }

            

                if(registrarFormula)
                {
                    //Se da de alta la nueva formula
                    db.ComponentesFormula.AddRange(componente.ComponentesFormula);
                    db.ComponentesFormulaDetalles.AddRange(componente.ComponentesFormulaDetalles);
                }

                //----------------------------------------------------------------------------------------------------------------------
                //Equivalencias de partes
                // Se obtienen elementos existentes en la bd, los agregados,los eliminados y los modificados
                List<ComponentesEquivalenciasPartes> EquivalenciasExistentes = db.ComponentesEquivalenciasPartes.Where(c => c.ComponenteId == componente.Id).ToList();
                List<ComponentesEquivalenciasPartes> EquivalenciasAgregados = componente.ComponentesEquivalenciasPartes.Where(n => n.Id == 0).ToList();
                List<ComponentesEquivalenciasPartes> EquivalenciasModificados = componente.ComponentesEquivalenciasPartes.Where(n => n.Id != 0).ToList();
                List<ComponentesEquivalenciasPartes> EquivalenciasEliminados = EquivalenciasExistentes.Where(n => !EquivalenciasModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.ComponentesEquivalenciasPartes.AddRange(EquivalenciasAgregados);
                db.ComponentesEquivalenciasPartes.RemoveRange(EquivalenciasEliminados);
                //Se actualizan los objetos modificados
                foreach (ComponentesCodigosBarras cb in CodigosBarrasModificados)
                {
                    var local = db.Set<ComponentesCodigosBarras>().Local.FirstOrDefault(l => l.Id == cb.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(cb).State = System.Data.Entity.EntityState.Modified;
                }
                //------------------------------------------------------------------------------------------------------------------------
                //Impuestos
                // Se obtienen elementos existentes en la bd, los agregados,los eliminados y los modificados
                List<ComponentesImpuestos> ImpuestosExistentes = db.ComponentesImpuestos.Where(c => c.ComponenteId == componente.Id).ToList();
                List<ComponentesImpuestos> ImpuestosAgregados = componente.ComponentesImpuestos.Where(n => n.Id == 0).ToList();
                List<ComponentesImpuestos> ImpuestosModificados = componente.ComponentesImpuestos.Where(n => n.Id != 0).ToList();
                List<ComponentesImpuestos> ImpuestosEliminados = ImpuestosExistentes.Where(n => !ImpuestosModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.ComponentesImpuestos.AddRange(ImpuestosAgregados);
                db.ComponentesImpuestos.RemoveRange(ImpuestosEliminados);
                //Se actualizan los objetos modificados
              /*  foreach (ComponentesImpuestos ci in ImpuestosModificados)
                {
                    var local = db.Set<ComponentesImpuestos>().Local.FirstOrDefault(l => l.Id == ci.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(ci).State = System.Data.Entity.EntityState.Modified;
                }*/
                //------------------------------------------------------------------------------------------------------------------------

                //Se limpia el objeto
                componente.ComponentesCodigosBarras = null;
                componente.ComponentesAlmacenes = null;
                componente.ComponentesImagenes = null;
                componente.ComponentesFormulaDetalles = null;
                componente.ComponentesFormula = null;
                componente.ComponentesEquivalenciasPartes = null;
                componente.ComponentesImpuestos = null;
                componente.Unidades = null;
                componente.Unidades1 = null;
                componente.Unidades2 = null;

                db.Componentes.Attach(componente);
                db.Entry(componente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return componente;
            }
            catch (Exception ex)
            {

                Error(ex, "El componente");
                return null;
            }

        }

        public Componentes add(Componentes componente)
        {
        
            try
            {
                Validar();
                if(!ValidarMaximosMinimos(componente))
                   throw new Exception("La configuracion de maximos, reorden y minimos es incorrecta.");

                if (!ValidarImpuestosRepetidos(componente.ComponentesImpuestos.ToList()))
                    throw new Exception("Existen impuestos repetidos.");
                //Metodo para Agregar un componente
                EmpresaContext db = new EmpresaContext();
                componente.Clave = GenerarClave(componente.TipoComponenteId);
                db.Componentes.Add(componente);

                db.SaveChanges();
                componente = get((Int32)(componente.Id));
                return componente;
            }
            catch (Exception ex)
            {
                Error(ex, "El componente");
                return null;
            }
        }

        public string GenerarClave(long tipoComponenteId)
        {
            //Metodo para obtener una nueva clave
            EmpresaContext db = new EmpresaContext();
          
            // Se busca el prefijo que le corresponde
            string prefijo = (from t in db.TiposComponentes
                              where t.Id == tipoComponenteId
                              select t.Prefijo
                          ).ToList().First().ToString();
            //Se obitiene el ultimo folio usado del tipo de componente
            // para ello primero se obtiene una lista de los folios(Clave sin prefijo) de los componentes del tyipo que le corresponde
            var LstFolios = (from c in db.Componentes
                               where c.TipoComponenteId == tipoComponenteId
                               select c.Clave.Substring(3)
                              ).ToList();
            //Se convierte la lista a long para despues ordenarla y obtener el primer registro el cual seria el ultimo folio
            long ultimofolio = 0;
            if (LstFolios.Count>0){
                ultimofolio = LstFolios.ConvertAll(s => Int64.Parse(s)).ToList().OrderByDescending(p => p).ToList().First();
            }
            
          
            //Se calcula el proximo consecutivo
            var consecutivo = ultimofolio + 1;
            //Se arma la nueva clave
            string nuevaclave = prefijo + consecutivo.ToString();
            return nuevaclave;
        }

        public bool ValidarMaximosMinimos(Componentes componentevalidar)
        {
            bool valido;
            valido = true;
            foreach (ComponentesAlmacenes ca in componentevalidar.ComponentesAlmacenes)
            {
                //Si el maximo es menor que el reorden o el minimo. O el reorden es menor que el minimo
                if (ca.Maximo < ca.Reorden || ca.Maximo < ca.Minimo || ca.Reorden<ca.Minimo)
                {
                    valido = false;
                    break;
                }                    
            }
            return valido;
        }

        public Componentes delete(Componentes componente)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                componente = db.Componentes.Find(componente.Id);

                db.Componentes.Attach(componente);
                db.Componentes.Remove(componente);
                db.SaveChanges();
                return componente;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<Componentes> getComponentesFormula(Componentes componente)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                if (componente.TipoComponenteId == 5 || componente.TipoComponenteId == 6) // formula de productos terminados y semielaborados
                {

                    //Consulta que retorna todos los componentes que pueden formar parte de la formula de un producto terminado
                    var lstComponentes = db.Componentes.Include("GruposComponentes").Include("SubgruposComponentes").Include("Unidades").
                        Include("TiposComponentes").ToList();

                    //Regresa solamente los compoentens de tipo materia prima y producto semielaborado
                    if (componente.Id == 0)
                    {
                        return lstComponentes.FindAll(ls => ls.TipoComponenteId == 1).Union(lstComponentes.FindAll(ls => ls.TipoComponenteId == 6)).ToList();
                    }
                    else
                    {
                        //descarta el producto semielaborado que se esta editando
                        return lstComponentes.FindAll(ls => ls.TipoComponenteId == 1).Union(lstComponentes.FindAll(ls => ls.TipoComponenteId == 6)).ToList().Where(z=>z.Id!=componente.Id).ToList();
                    }
                     

                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<Componentes> getComponentesAlmacen(long almacenid)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                List<Componentes> lstcomponentes = (from c in db.Componentes 
                                             join ca in db.ComponentesAlmacenes on c.Id equals ca.ComponenteId
                                             where ca.AlmacenId ==almacenid where c.Activo=="SI"
                                             where c.Inventariable=="SI"
                                             select c).ToList();

           /*   foreach (Componentes c in lstcomponentes)
                {

                    db.Entry(c).Reference(x => x.Unidades).Load();
                    db.Entry(c).Reference(x => x.TiposComponentes).Load();
                    db.Entry(c).Reference(x => x.GruposComponentes).Load();
                    db.Entry(c).Reference(x => x.SubgruposComponentes).Load();

                }*/

               // Regresa solamente los compoentens de tipo materia prima y producto semielaborado
             //    return lstComponentes.FindAll(ls => ls.TipoComponenteId == 1).Union(lstComponentes.FindAll(ls => ls.TipoComponenteId == 6)).ToList();
                return lstcomponentes.ToList();


            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<Componentes> getComponentesAlmacenConImagen(long almacenid)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                List<Componentes> lstcomponentes = (from c in db.Componentes
                                                    join ca in db.ComponentesAlmacenes on c.Id equals ca.ComponenteId
                                                    where ca.AlmacenId == almacenid
                                                    where c.Activo == "SI"
                                                    where c.Inventariable == "SI"
                                                    select c).Include("ComponentesImagenes").ToList();

                /*   foreach (Componentes c in lstcomponentes)
                     {

                         db.Entry(c).Reference(x => x.Unidades).Load();
                         db.Entry(c).Reference(x => x.TiposComponentes).Load();
                         db.Entry(c).Reference(x => x.GruposComponentes).Load();
                         db.Entry(c).Reference(x => x.SubgruposComponentes).Load();

                     }*/

                // Regresa solamente los compoentens de tipo materia prima y producto semielaborado
                //    return lstComponentes.FindAll(ls => ls.TipoComponenteId == 1).Union(lstComponentes.FindAll(ls => ls.TipoComponenteId == 6)).ToList();
                return lstcomponentes.ToList();


            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<Componentes> getComponentesAlmacenConImagenImpuestos(long almacenid)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                List<Componentes> lstcomponentes = (from c in db.Componentes
                                                    join ca in db.ComponentesAlmacenes on c.Id equals ca.ComponenteId
                                                    where ca.AlmacenId == almacenid
                                                    where c.Activo == "SI"
                                                    where c.Inventariable == "SI"
                                                    select c).Include("ComponentesImagenes").Include(c=>c.ComponentesImpuestos.Select(a=>a.Impuestos)).ToList();

                /*   foreach (Componentes c in lstcomponentes)
                     {

                         db.Entry(c).Reference(x => x.Unidades).Load();
                         db.Entry(c).Reference(x => x.TiposComponentes).Load();
                         db.Entry(c).Reference(x => x.GruposComponentes).Load();
                         db.Entry(c).Reference(x => x.SubgruposComponentes).Load();

                     }*/

                // Regresa solamente los compoentens de tipo materia prima y producto semielaborado
                //    return lstComponentes.FindAll(ls => ls.TipoComponenteId == 1).Union(lstComponentes.FindAll(ls => ls.TipoComponenteId == 6)).ToList();
                return lstcomponentes.ToList();


            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<Componentes> getComponentesAlmacenConExistencia(long almacenid)
        {
            try
            {
                //Este metodo solo muestra los componentes con tipo de seguimiento numero de serie y normal
                Validar();
                EmpresaContext db = new EmpresaContext();

                //Obtengo los componentes qee tienen existencia mayor a 0
                var result = db.InventariosSaldos.Where(a => a.AlmacenId == almacenid).GroupBy(x => x.ComponenteId)
                  .Select(g => new { Id = g.Key, resultado = g.Sum(x => x.EntradasUnidades) - g.Sum(x => x.SalidasUnidades) })
                    .Where(x => x.resultado > 0).Select(x => x.Id).ToList();

                //Busco los componentes en su tabla basandome en la lista que me retorna la query anterior
                List<Componentes> LstComponentes = db.Componentes.Where(x => result.Contains(x.Id) && x.TipoSeguimiento != "LOTES").ToList();

                return LstComponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<Componentes> getComponentesAlmacenConExistenciaeImagen(long almacenid)
        {
            try
            {
                //Este metodo solo muestra los componentes con tipo de seguimiento numero de serie y normal
                Validar();
                EmpresaContext db = new EmpresaContext();

                //Obtengo los componentes que tienen existencia mayor a 0
                var result = db.InventariosSaldos.Where(a => a.AlmacenId == almacenid).GroupBy(x => x.ComponenteId)
                  .Select(g => new { Id = g.Key, resultado = g.Sum(x => x.EntradasUnidades) - g.Sum(x => x.SalidasUnidades) })
                    .Where(x => x.resultado > 0).Select(x => x.Id).ToList();

                //Busco los componentes en su tabla basandome en la lista que me retorna la query anterior
                List<Componentes> LstComponentes = db.Componentes.Where(x => result.Contains(x.Id) && x.TipoSeguimiento != "LOTES").Include("ComponentesImagenes").ToList();

                return LstComponentes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<ComponentesFormula> getHistorialFormula(long IdComponente)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna las formulas anteriores
                List<ComponentesFormula> lstformulas = (from cf in db.ComponentesFormula
                                                    where cf.ComponenteId == IdComponente
                                                        where cf.Estado == "H"
                                                    select cf).ToList();
                return lstformulas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<ComponentesFormulaDetalles> getHistorialFormulaDetalles(long IdComponente)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna las formulas anteriores
                List<ComponentesFormulaDetalles> lstformulasdetalles = (from cf in db.ComponentesFormulaDetalles
                                                                        where cf.ComponenteId == IdComponente
                                                        where cf.Estado == "H"
                                                        select cf).ToList();
                return lstformulasdetalles;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<LotesSeries> getNumerosSeries(long IdComponente)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna las formulas anteriores
                List<LotesSeries> lstNumerosSerie = (from ns in db.LotesSeries
                                                                        where ns.ComponenteId == IdComponente
                                                                        where ns.NumeroSerie!=null
                                                                        select ns).ToList();
                return lstNumerosSerie;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }


        public List<LotesSeries> getLotes(long IdComponente)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna las formulas anteriores
                List<LotesSeries> lstLotes = (from lt in db.LotesSeries
                                                     where lt.ComponenteId == IdComponente
                                                     where lt.Lote != null
                                                     select lt).ToList();
                return lstLotes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public ComponentesCodigosBarras getComponenteCodigodeBarra(string codigo)
        {
            try 
	        {
                Validar();
                EmpresaContext db = new EmpresaContext();

                ComponentesCodigosBarras ComponenteBarra = db.ComponentesCodigosBarras.Where(c => c.CodigoBarra == codigo && c.Activo=="SI").SingleOrDefault();
                
                return ComponenteBarra;
	        }
	        catch (Exception ex)
	        {
                Error(ex);
                return null;
	        }
        }
        public List<Componentes> getComponentesResguardo()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();

              
                    //Consulta que retorna todos los componentes que pueden ser asignados en un resguardo
                    var lstComponentes = db.Componentes.Include("GruposComponentes").Include("SubgruposComponentes").Include("Unidades").
                        Include("TiposComponentes").Where(x=>x.TipoComponenteId==7 || x.TipoComponenteId==3).Include("ComponentesImagenes").ToList();
                    
                        return lstComponentes;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public bool ValidarImpuestosRepetidos(List<ComponentesImpuestos> lstimpuestos)
        {

            var regdistintos = (from i in lstimpuestos
                     select i.ImpuestoId).Distinct().ToList().Count();
            var regtotal = lstimpuestos.Count();

            if (regdistintos != regtotal)
                return false;
            else
                return true;
        }

        public List<Componentes> getComponentesXTipoConImagen(long tipo)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();


                //Consulta que retorna todos los componentes del tipo especificado
                var lstComponentes = db.Componentes.Include("GruposComponentes").Include("SubgruposComponentes").Include("Unidades").
                    Include("TiposComponentes").Where(x => x.TipoComponenteId ==tipo).Include("ComponentesImagenes").ToList();

                return lstComponentes;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<Componentes> buscar(string palabra)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();

                List<Componentes> lstcomponentes = (from c in db.Componentes
                                                    join ca in db.ComponentesAlmacenes on c.Id equals ca.ComponenteId
                                                    where ca.AlmacenId == 39
                                                    where c.Activo == "SI"
                                                    where c.Inventariable == "SI"
                                                    select c).Include("ComponentesImagenes").Include("ComponentesImpuestos").Where(n=>n.Nombre.Contains(palabra)).ToList();

                return lstcomponentes;

            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

    }
}
