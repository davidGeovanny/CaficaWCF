using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSRoles" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSRoles.svc or WSRoles.svc.cs at the Solution Explorer and start debugging.
    public class WSRoles : WsBase, IWSRoles
    {
        public List<Roles> getall()
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todos los usuarios
                // var listaRoles = db.Roles.Where(u => u.BanEliminar == 0).ToList();
                var listaRoles = db.Roles.ToList();
                return listaRoles;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }
        public Roles get(int id)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                Roles rol = db.Roles.Find(id);
                db.Entry(rol).Collection(x => x.BDEmpresasRoles).Load();
                db.Entry(rol).Collection(x => x.RolesAcciones).Load();

                return rol;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public Roles add(Roles Rol)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                AdminBD dbAdmin = new AdminBD();
                //Si el rol se va agregar por primera vez
                db.Roles.Add(Rol);
                //Crea el rol en la base de datos
                dbAdmin.Database.ExecuteSqlCommand("DROP role IF EXISTS  " + Rol.Nombre);
                dbAdmin.Database.ExecuteSqlCommand("CREATE role " + Rol.Nombre);

                SetPermisos(Rol);

                db.SaveChanges();
                return Rol;
            }
            catch (Exception ex)
            {

                Error(ex, "rol ");
                return null;
            }
        }
        //Metodo para asignar los permisos
        private void SetPermisos(Roles Rol)
        {
            try
            {
                UsuariosContext db = new UsuariosContext();
                AdminBD dbAdmin = new AdminBD();

                dbAdmin.Database.ExecuteSqlCommand("GRANT SELECT ON Usuarios.* TO  " + Rol.Nombre);
                dbAdmin.Database.ExecuteSqlCommand("GRANT RELOAD ON *.* TO  " + Rol.Nombre);
                dbAdmin.Database.ExecuteSqlCommand("Grant select, insert, update on Usuarios.Tokens to " + Rol.Nombre);


                //Permisos relacionados a cuestiones de administracion de base de datos
                /*    if (Rol.Administrador == "Si")
                    {
                        dbAdmin.Database.ExecuteSqlCommand("GRANT CREATE USER,CREATE,DROP,GRANT OPTION ON *.* TO  " + Rol.Nombre);
                    }
                    */
                foreach (RolesAcciones roles in Rol.RolesAcciones)
                {
                    //Seleccion la accion para se procesada
                    var accion = db.AccionesVistas.FirstOrDefault(a => a.Id == roles.AccionId);

                    //Selecciono todas las tablas que tiene relacion con la vista
                    var tablas = db.VistasTablas.ToList().Where(vi => vi.VistaId == accion.VistaId);

                    foreach (BDEmpresasRoles bdrol in Rol.BDEmpresasRoles)
                    {
                        string bd = db.BDEmpresas.FirstOrDefault(id => id.Id == bdrol.BDEmpresaId).RFC;

                        //Da permiso de lectura a todos las tablas
                        dbAdmin.Database.ExecuteSqlCommand("GRANT SELECT ON " + bd + ".* TO  " + Rol.Nombre);

                        foreach (VistasTablas tabla in tablas)
                        {
                            var teable = db.Tablas.Find(tabla.TablaId);
                            if (teable.EsMaestra == "SI")//da permisos a las tablas que son maestras,ejemplo: en inventarios la tablas maestra son: inventariosaldos, inventarioscostos etc.
                            {
                                dbAdmin.Database.ExecuteSqlCommand("GRANT SELECT,INSERT,UPDATE,DELETE ON " + bd + "." + teable.Nombre + " TO " + Rol.Nombre);
                            }
                            //Busca el nombre de la tabla
                            string Nombretabla = db.Tablas.Find(tabla.TablaId).Nombre;

                            //Buscar si la tabl a la cual se le asignan permisos se encuentra en la bd de Usuarios
                            var buscartabla = db.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='Usuarios' AND TABLE_NAME='" + Nombretabla + "'").SingleOrDefault();

                            if (accion.Tipo != "ESPECIAL")
                            {
                                if (buscartabla == null && teable.EsMaestra == "NO")
                                    dbAdmin.Database.ExecuteSqlCommand("GRANT " + accion.Tipo + " ON " + bd + "." + Nombretabla + " TO " + Rol.Nombre);
                                else if (teable.EsMaestra == "NO")
                                    //GRANT SELECT  ON *.* TO  compras;
                                    dbAdmin.Database.ExecuteSqlCommand("GRANT " + accion.Tipo + " ON Usuarios." + Nombretabla + " TO " + Rol.Nombre);
                                if (teable.Nombre == "InventariosES")
                                {
                                    dbAdmin.Database.ExecuteSqlCommand("GRANT UPDATE ON " + bd + ".InventariosES TO " + Rol.Nombre);
                                }

                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public Roles update(Roles Rol)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                AdminBD dbAdmin = new AdminBD();

                //Elminiar todos los permisos que tiene el usuario
                dbAdmin.Database.ExecuteSqlCommand("REVOKE ALL PRIVILEGES, GRANT OPTION FROM  '" + Rol.Nombre + "'" + ";");

                SetPermisos(Rol);

                //Se Obtiene la lista de los roles actuales del usuario, que se encuentran en la base de datos
                List<BDEmpresasRoles> EmpresasActuales = db.BDEmpresasRoles.Where(e => e.RolId == Rol.Id).ToList();
                List<BDEmpresasRoles> EmpresasNuevos = Rol.BDEmpresasRoles.ToList();

                List<BDEmpresasRoles> EmpresasEliminadas = EmpresasActuales.Where(n => !EmpresasNuevos.Select(n1 => n1.BDEmpresaId).Contains(n.BDEmpresaId)).ToList();
                List<BDEmpresasRoles> EmpresasNuevas = EmpresasNuevos.Where(n => !EmpresasActuales.Select(n1 => n1.BDEmpresaId).Contains(n.BDEmpresaId)).ToList();

                db.BDEmpresasRoles.AddRange(EmpresasNuevas);
                db.BDEmpresasRoles.RemoveRange(EmpresasEliminadas);

                List<RolesAcciones> AccionesActuales = db.RolesAcciones.Where(e => e.RolId == Rol.Id).ToList();
                List<RolesAcciones> AccionesNuevos = Rol.RolesAcciones.ToList();

                List<RolesAcciones> AccionesEliminadas = AccionesActuales.Where(n => !AccionesNuevos.Select(n1 => n1.AccionId).Contains(n.AccionId)).ToList();
                List<RolesAcciones> AccionesNuevas = AccionesNuevos.Where(n => !AccionesActuales.Select(n1 => n1.AccionId).Contains(n.AccionId)).ToList();

                db.RolesAcciones.AddRange(AccionesNuevas);
                db.RolesAcciones.RemoveRange(AccionesEliminadas);

                //Si se va modificar el rol
                Rol.RolesAcciones = null;
                Rol.BDEmpresasRoles = null;
                Rol.SistemasRoles = null;

                db.Roles.Attach(Rol);
                db.Entry(Rol).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return Rol;
            }
            catch (Exception ex)
            {
                Error(ex, "rol ");
                return null;
            }
        }
        public Roles delete(Roles rol)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                AdminBD dbadmin = new AdminBD();
                Roles Rol = db.Roles.Find(rol.Id);

                db.Roles.Attach(Rol);
                db.Roles.Remove(Rol);
                db.SaveChanges();

                dbadmin.Database.ExecuteSqlCommand("DROP ROLE IF EXISTS " + "'" + rol.Nombre + "'"); ;

                return rol;
            }
            catch (Exception ex)
            {

                Error(ex, "este rol ");
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
                var modulos = db.Modulos.ToList();

                foreach (Modulos modulo in modulos)
                {
                    PermisosModel pmodulo = new PermisosModel();
                    pmodulo.Id = modulo.Id;
                    pmodulo.Nombre = modulo.Nombre;
                    pmodulo.Tipo = "Modulo";
                    //Toma el nombre de modulo y lo pasa minusculas
                    pmodulo.Index = modulo.Nombre.ToLower();
                    Permisos.Add(pmodulo);

                    var vistas = db.VistasGruposVistas.ToList().Where(v => v.ModuloId == pmodulo.Id);
                    foreach (VistasGruposVistas gvista in vistas)
                    {
                        var vista = db.Vistas.Find(gvista.VistaId);
                        PermisosModel pvista = new PermisosModel();
                        pvista.Id = vista.Id;
                        pvista.Nombre = vista.Nombre;
                        pvista.Tipo = "Vista";
                        pvista.Index = modulo.Id.ToString() + "," + gvista.Id.ToString() + "-" + vista.Nombre.ToLower();
                        pvista.ParentId = pmodulo.Index;
                        Permisos.Add(pvista);

                        var acciones = db.AccionesVistas.ToList().Where(a => a.VistaId == pvista.Id);
                        foreach (AccionesVistas accion in acciones)
                        {
                            PermisosModel paccion = new PermisosModel();
                            paccion.Id = accion.Id;
                            paccion.Nombre = accion.Nombre;
                            paccion.Tipo = "Accion";
                            paccion.Index = modulo.Id.ToString() + "," + gvista.Id.ToString() + "," + accion.Id.ToString() + "-" + accion.Nombre.ToLower();
                            paccion.ParentId = pvista.Index;

                            Permisos.Add(paccion);
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
