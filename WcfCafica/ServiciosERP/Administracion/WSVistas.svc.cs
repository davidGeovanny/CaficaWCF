using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSVistas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSVistas.svc or WSVistas.svc.cs at the Solution Explorer and start debugging.
    public class WSVistas : WsBase, IWSVistas
    {
        public List<Vistas> getall()
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todas las vistas
                //  var listaVistas = db.Vistas.Where(v => v.BanEliminar == 0).ToList();
                var listaVistas = db.Vistas.ToList();
                return listaVistas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }
        public Vistas get(int ID)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna la vista usando como parametro el ID
                Vistas vista = db.Vistas.Find(ID);
                db.Entry(vista).Collection(x => x.VistasTablas).Load();
                db.Entry(vista).Collection(x => x.VistasGruposVistas).Load();
                db.Entry(vista).Collection(x => x.VistasFiltrosReportes).Load();
                vista.VistasFiltrosReportes = vista.VistasFiltrosReportes.OrderBy(z => z.Orden).ToList();
                return vista;

            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }

        }
        public Vistas add(Vistas vista)
        {
            try
            {
                //Metodo para crear un registro nuevo de vistas.
                Validar();
                UsuariosContext db = new UsuariosContext();

                AccionesVistas lectura = new AccionesVistas { Nombre = "Lectura", Tipo = "SELECT", VistaId = vista.Id };
                db.AccionesVistas.Add(lectura);

                if (vista.Tipo=="FORMULARIO")
                {
                    AccionesVistas nuevo = new AccionesVistas { Nombre = "Nuevo", Tipo = "INSERT", VistaId = vista.Id };
                    db.AccionesVistas.Add(nuevo);
                    AccionesVistas modificar = new AccionesVistas { Nombre = "Modificar", Tipo = "UPDATE", VistaId = vista.Id };
                    db.AccionesVistas.Add(modificar);
                    AccionesVistas eliminar = new AccionesVistas { Nombre = "Eliminar", Tipo = "DELETE", VistaId = vista.Id };
                    db.AccionesVistas.Add(eliminar);
                }
                db.Vistas.Add(vista);
                db.SaveChanges();
                return vista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return vista;
                
            }
        }
        public Vistas update(Vistas vista)
        {
            try
            {
                //Metodo para actualizar una vista / parametro vista    
                Validar();
                UsuariosContext db = new UsuariosContext();

                //Se Obtiene la lista de tablas actuales dque pertenecen a la vista, que se encuentran en la base de datos
                List<VistasTablas> VistasActuales = db.VistasTablas.Where(e => e.VistaId == vista.Id).ToList();
                List<VistasTablas> VistasNuevos = vista.VistasTablas.ToList();

                List<VistasTablas> VistasEliminadas = VistasActuales.Where(n => !VistasNuevos.Select(n1 => n1.TablaId).Contains(n.TablaId)).ToList();
                List<VistasTablas> VistasNuevas = VistasNuevos.Where(n => !VistasActuales.Select(n1 => n1.TablaId).Contains(n.TablaId)).ToList();

                db.VistasTablas.AddRange(VistasNuevas);
                db.VistasTablas.RemoveRange(VistasEliminadas);


                List<VistasGruposVistas> VistasgVistasAcuales = db.VistasGruposVistas.Where(e => e.VistaId == vista.Id).ToList();
                List<VistasGruposVistas> VistasgVistasNuevos = vista.VistasGruposVistas.ToList();

                List<VistasGruposVistas> VistasgVistasEliminadas = VistasgVistasAcuales.Where(n => !VistasgVistasNuevos.Select(n1 => n1.GrupoVistaId).Contains(n.GrupoVistaId)).ToList();
                List<VistasGruposVistas> VistasgVistasaguardar = VistasgVistasNuevos.Where(n => !VistasgVistasAcuales.Select(n1 => n1.GrupoVistaId).Contains(n.GrupoVistaId)).ToList();


                db.VistasGruposVistas.AddRange(VistasgVistasaguardar);
                db.VistasGruposVistas.RemoveRange(VistasgVistasEliminadas);

                //----------------------------------------------------------------------------------------------------------------------
                //Filtros Reportes
                // Se obtienen elementos existentes en la bd, los agregados,los eliminados y los modificados
                List<VistasFiltrosReportes> FiltrosReportesExistentes = db.VistasFiltrosReportes.Where(f => f.VistaId == vista.Id).ToList();
                List<VistasFiltrosReportes> FiltrosReportesAgregados = vista.VistasFiltrosReportes.ToList();
                //  List<VistasFiltrosReportes> FiltrosReportesAgregados = vista.VistasFiltrosReportes.Where(n => n.Id == 0).ToList();
                // List<VistasFiltrosReportes> FiltrosReportesModificados = vista.VistasFiltrosReportes.Where(n => n.Id != 0).ToList();
                //   List<VistasFiltrosReportes> FiltrosReportesEliminados = FiltrosReportesExistentes.Where(n => !FiltrosReportesModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes

                db.VistasFiltrosReportes.RemoveRange(FiltrosReportesExistentes);
                //Se actualizan los objetos modificados
               /* foreach (VistasFiltrosReportes fr in FiltrosReportesModificados)
                {
                    var local = db.Set<VistasFiltrosReportes>().Local.FirstOrDefault(l => l.Id == fr.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(fr).State = System.Data.Entity.EntityState.Modified;
                }*/
                db.VistasFiltrosReportes.AddRange(FiltrosReportesAgregados);
                //------------------------------------------------------------------------------------------------------------------------
                //Si se va modificar la vista
                vista.VistasTablas = null;
                vista.VistasGruposVistas = null;
                vista.VistasFiltrosReportes = null;

                db.Vistas.Attach(vista);
                db.Entry(vista).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return vista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return vista;
            }
        }
        public Vistas delete(Vistas vistasel)
        {
            try
            {
                //Metodo para Eliminar una vista / parametro vista  
                Validar();
                UsuariosContext db = new UsuariosContext();
                Vistas vista = db.Vistas.Find(vistasel.Id);
              
                db.Vistas.Attach(vista);
                db.Vistas.Remove(vista);
                db.SaveChanges();
                return vista;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public List<PermisosModel> getPermisos()
        {
            try
            {
                Validar();
                List<PermisosModel> Permisos = new List<PermisosModel>();

                UsuariosContext db = new UsuariosContext();
                //var modulos = db.Modulos.ToList().Where(p=> p.Id==3);

                var sistemas = db.Sistemas.ToList();
                foreach (Sistemas sistema in sistemas)
                {
                    PermisosModel psistema = new PermisosModel();
                    psistema.Id = sistema.Id;
                    psistema.Nombre = sistema.Nombre;
                    psistema.Tipo = "Sistema";
                    psistema.Index = sistema.Nombre.ToLower();
                    Permisos.Add(psistema);

                    var modulos = db.Modulos.ToList().Where(m => m.SistemaId == psistema.Id);

                    foreach (Modulos modulo in modulos)
                {
                        PermisosModel pmodulo = new PermisosModel();
                        pmodulo.Id = modulo.Id;
                        pmodulo.Nombre = modulo.Nombre;
                        pmodulo.Tipo = "Modulo";
                        //Toma el nombre de modulo y lo pasa minusculas
                        pmodulo.Index = sistema.Id.ToString() + "," + modulo.Id.ToString() + "-" + modulo.Nombre.ToLower();
                        pmodulo.ParentId = psistema.Index;
                        Permisos.Add(pmodulo);

                        var vistas = db.GruposVistas.ToList().Where(v => v.ModuloId == pmodulo.Id);
                    foreach (GruposVistas gvista in vistas)
                    {
                     //   var vista = db.Vistas.Find(gvista.VistaId);
                        PermisosModel pvista = new PermisosModel();
                        pvista.Id = gvista.Id;
                        pvista.Nombre = gvista.Nombre;
                        pvista.Tipo = "Vista";
                        pvista.Index = sistema.Id.ToString()+"," + modulo.Id.ToString() + "," + gvista.Id.ToString() + "-" + gvista.Nombre.ToLower();
                        pvista.ParentId = pmodulo.Index;
                        Permisos.Add(pvista);

                    /*    var acciones = db.AccionesVistas.ToList().Where(a => a.VistaId == pvista.Id);
                        foreach (AccionesVistas accion in acciones)
                        {
                            PermisosModel paccion = new PermisosModel();
                            paccion.Id = accion.Id;
                            paccion.Nombre = accion.Nombre;
                            paccion.Tipo = "Accion";
                            paccion.Index = modulo.Id.ToString() + "," + gvista.Id.ToString() + "," + accion.Id.ToString() + "-" + accion.Nombre.ToLower();
                            paccion.ParentId = pvista.Index;

                            Permisos.Add(paccion);
                            */
                        }
                    }
                }

                return Permisos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

    }
}
