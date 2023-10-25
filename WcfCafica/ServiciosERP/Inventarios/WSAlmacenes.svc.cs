using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;
using WcfCafica.ServiciosERP;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSAlmacenes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSAlmacenes.svc or WSAlmacenes.svc.cs at the Solution Explorer and start debugging.
    public class WSAlmacenes : WsBase, IWSAlmacenes
    {
        public string[] CamposBloqueados = { "TipoAlmacen", "TipoComponenteId" };

        public List<Almacenes> getall()
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                var almacenes = db.Almacenes.Include("TiposComponentes").ToList();

                return almacenes;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }

        public Almacenes add(Almacenes almacen)
        {
            try
            {
                Validar();

                //Metodo para Agregar un almacen
                EmpresaContext db = new EmpresaContext();
                db.Almacenes.Add(almacen);
                //Agregar la relacion entre el almacen y componentes de los grupos que aplican para el almacen 
                List<ComponentesAlmacenes> LstComponentesAlmacenes = new List<ComponentesAlmacenes>();

                foreach (AlmacenesGruposComponentes a in almacen.AlmacenesGruposComponentes)
                {
                    var LstComponentes = (from c in db.Componentes
                                     where c.GrupoComponentesId == a.GrupoComponentesId
                                     select c).ToList();
                    foreach (Componentes c in LstComponentes)
                    {
                        var ca = new ComponentesAlmacenes
                        {
                            AlmacenesGruposComponentesId = a.Id,
                            ComponenteId = c.Id,
                            AlmacenId = almacen.Id
                        };

                        LstComponentesAlmacenes.Add(ca);
                    }
                }


                db.ComponentesAlmacenes.AddRange(LstComponentesAlmacenes);
                db.SaveChanges();

                return almacen;
            }
            catch (Exception ex)
            {
                Error(ex,"El almacen");
                return null;
            }
        }
        public Almacenes get(int id)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();
                Almacenes almacen = db.Almacenes.Find(id);
                db.Entry(almacen).Collection(x => x.AlmacenesGruposComponentes).Load();
                

                return almacen;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public Almacenes update(Almacenes almacen)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de Grupos Unidades

                EmpresaContext db = new EmpresaContext();
                ValidarCamposBloqueados<Almacenes, EmpresaContext>(almacen, CamposBloqueados, "Almacenes");

                //Se Obtiene la lista de los roles actuales del usuario, que se encuentran en la base de datos
                List<AlmacenesGruposComponentes> AlmacenesGruposComponentesDetallesActuales = db.AlmacenesGruposComponentes.Where(a => a.AlmacenId == almacen.Id).ToList();
                List<AlmacenesGruposComponentes> AlmacenesGruposComponentesDetallesNuevos = almacen.AlmacenesGruposComponentes.ToList();

                List<AlmacenesGruposComponentes> AlmacenesGruposComponentesDetallesEliminadas = AlmacenesGruposComponentesDetallesActuales.Where(n => !AlmacenesGruposComponentesDetallesNuevos.Select(n1 => n1.GrupoComponentesId).Contains(n.GrupoComponentesId)).ToList();
                List<AlmacenesGruposComponentes> AlmacenesGruposComponentesDetallesNuevas = AlmacenesGruposComponentesDetallesNuevos.Where(n => !AlmacenesGruposComponentesDetallesActuales.Select(n1 => n1.GrupoComponentesId).Contains(n.GrupoComponentesId)).ToList();

                db.AlmacenesGruposComponentes.AddRange(AlmacenesGruposComponentesDetallesNuevas);
                db.AlmacenesGruposComponentes.RemoveRange(AlmacenesGruposComponentesDetallesEliminadas);

                almacen.AlmacenesGruposComponentes= null;

                //Agregar la relacion entre el almacen y componentes de los grupos que aplican para el almacen 
                List<ComponentesAlmacenes> LstComponentesAlmacenes = new List<ComponentesAlmacenes>();

                foreach (AlmacenesGruposComponentes a in AlmacenesGruposComponentesDetallesNuevas)
                {
                    var LstComponentes = (from c in db.Componentes
                                          where c.GrupoComponentesId == a.GrupoComponentesId
                                          select c).ToList();
                    foreach (Componentes c in LstComponentes)
                    {
                        var ca = new ComponentesAlmacenes
                        {
                            AlmacenesGruposComponentesId = a.Id,
                            ComponenteId = c.Id,
                            AlmacenId = almacen.Id
                        };

                        LstComponentesAlmacenes.Add(ca);
                    }
                }

                db.ComponentesAlmacenes.AddRange(LstComponentesAlmacenes);

                db.Almacenes.Attach(almacen);
                db.Entry(almacen).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return almacen;
            }
            catch (Exception ex)
            {

                Error(ex, "El almacen");
                return null;
            }
        }
        public Almacenes delete(Almacenes almacen)
        {
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                Almacenes Almacen = db.Almacenes.Find(almacen.Id);

                db.Almacenes.Attach(Almacen);
                db.Almacenes.Remove(Almacen);
                db.SaveChanges();

                return Almacen;
            }
            catch (Exception ex)
            {
                Error(ex, "El Almacen");
                return null;
            }
        }

        public List<Almacenes> getAlmacenesXGrupoComponentes(int tipo)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                List<Almacenes> almacenes = (from a in db.Almacenes
                                 join ag in db.AlmacenesGruposComponentes on a.Id equals ag.AlmacenId
                                 where ag.GrupoComponentesId == tipo
                                 where a.Activo=="SI"
                                 select a).OrderBy(x=>x.Nombre).ToList();

                foreach(Almacenes a in almacenes)
                {

                    db.Entry(a).Collection(x => x.AlmacenesGruposComponentes).Load();
                    
                }


                return almacenes;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }

        }

        public List<GruposComponentes> getGruposComponentesXAlmacen(int id)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                List<GruposComponentes> grupos=(from ag in db.AlmacenesGruposComponentes 
                                                     join g in db.GruposComponentes on ag.GrupoComponentesId equals g.Id
                                                     where ag.AlmacenId==id
                                                     select g).ToList();

                return grupos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public List<Almacenes> getAlmacenesResguardo()
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                List<Almacenes> grupos = db.Almacenes.Where(c => c.TipoComponenteId == 3 || c.TipoComponenteId == 7).ToList();

                return grupos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public List<Almacenes> getAlmacenesxTipo(long id)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                List<Almacenes> almacenes = (from tc in db.Almacenes where tc.TipoComponenteId == id select tc).ToList();

                return almacenes;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

    }
}
