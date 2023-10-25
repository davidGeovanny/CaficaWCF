using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP.Administracion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSUsuarios" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSUsuarios.svc or WSUsuarios.svc.cs at the Solution Explorer and start debugging.
    public class WSUsuarios : WsBase, IWSUsuarios
    {
        public List<Usuarios> getall()
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                //Consulta que retorna todos los usuarios
                //var listaUsuarios = db.Usuarios.Where(u => u.BanEliminar==0).ToList();
                var listaUsuarios = db.Usuarios.ToList();
                return listaUsuarios;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public Usuarios get(int ID)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                /*var usuario = db.Usuarios.Include("UsuariosRoles").SingleOrDefault(usr => usr.Id==ID);
                Usuarios usuario = db.Usuarios.Include("UsuariosRoles").Where(usr => usr.UsuariosRoles.Where(r => r.BanEliminar ==0));
                            repo.GetSubForms.Include("Classes").Where(sf => sf.Classes.Where(c => c.TermId == termId));*/
                Usuarios usuario = db.Usuarios.Find(ID);
                //  db.Entry(usuario).Collection(usr => usr.UsuariosRoles).Query().Where(r => r.BanEliminar == 0).ToList();
                db.Entry(usuario).Collection(usr => usr.UsuariosRoles).Query().ToList();
                usuario.Contrasena = null;
                return usuario;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public Usuarios update(Usuarios usuario)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                PasswordEncrypt encriptacion = new PasswordEncrypt();
                AdminBD dbadmin = new AdminBD();

                db.UsuariosRoles.Remove(db.UsuariosRoles.Where(usr => usr.UsuarioId == usuario.Id).First());
                db.UsuariosRoles.Add(usuario.UsuariosRoles.ToList()[0]);

                dbadmin.Database.ExecuteSqlCommand("GRANT " + db.Roles.Find(usuario.UsuariosRoles.ToList()[0].RolId).Nombre + " TO " + "'" + usuario.Nombre + "'");

     /***********************************CODIGO COMENTADO CUANDO TENIA UN LOOKUPEDIT EN VEZ DE COMBO, LA INTENCION ERA HACERLO MULTIROL*******************************/
                //Se Obtiene la lista de los roles actuales del usuario, que se encuentran en la base de datos
                //List<UsuariosRoles> RolesActuales = db.UsuariosRoles.Where(usr => usr.UsuarioId == usuario.Id).Where(b => b.BanEliminar == 0).ToList();
                /*   List<UsuariosRoles> RolesActuales = db.UsuariosRoles.Where(usr => usr.UsuarioId == usuario.Id).ToList();
                   List<UsuariosRoles> RolesNuevos = usuario.UsuariosRoles.ToList();

                   List<UsuariosRoles> RolesEliminados = RolesActuales.Where(n => !RolesNuevos.Select(n1 => n1.RolId).Contains(n.RolId)).ToList();
                   List<UsuariosRoles> NuevosRoles = RolesNuevos.Where(n => !RolesActuales.Select(n1 => n1.RolId).Contains(n.RolId)).ToList();

                   db.UsuariosRoles.AddRange(NuevosRoles);
                   db.UsuariosRoles.RemoveRange(RolesEliminados);*/


                /*  RolesEliminados.ForEach(r =>{ 
                                         //     r.BanEliminar = 1;
                                         db.Customers.DeleteObjects(customersToDelete);
                                          });*/


                //   usuario.UsuariosRoles = null;
                //se le asigna el rol correspondiente con la consulta "GRANT Juridico TO 'TESTROLES'"
                //   dbadmin.Database.ExecuteSqlCommand("GRANT " + db.Roles.Find(NuevosRoles.ToList()[0].RolId).Nombre + " TO " + "'" + usuario.Nombre + "'");


                //Quita todos los
                /****     db.Database.ExecuteSqlCommand(
                     "REVOKE ALL PRIVILEGES, GRANT OPTION FROM  '" + usuario.Nombre + "'"  + ";");
                     db.Usuarios.Attach(usuario);****/

                //Verifica los roles del usuario
                /***     foreach (UsuariosRoles rol in usuario.UsuariosRoles)
                     {
                         //Consulta a que empresas tiene derecho un rol
                         var empresas = db.BDEmpresasRoles.Where(usr => usr.RolId == rol.RolId).ToList();
                         foreach (BDEmpresasRoles empresarol in empresas)
                         {
                             var nombrebd = db.BDEmpresas.Find(empresarol.BDEmpresaId);
                             //Le da permiso al usuario para la empresa selcccionada
                             /****  db.Database.ExecuteSqlCommand(
                                   "GRANT SELECT ON " + nombrebd.RFC + ".* TO" + "'" + usuario.Nombre + "'");
                             db.Database.ExecuteSqlCommand("GRANT"+ usuario.UsuariosRoles+ "TO"   + "'" + usuario.Nombre + "'");
                           //  GRANT SYSDBA TO 'EGUERRA10'@'%'
                         }
                     }***/

                //Se dan permisos por default a la bd usuarios
                /****    db.Database.ExecuteSqlCommand(
                            "GRANT SELECT ON Usuarios.* TO" + "'" + usuario.Nombre + "'");

                    db.Database.ExecuteSqlCommand(
                            "GRANT INSERT,UPDATE ON Usuarios.Tokens TO" + "'" + usuario.Nombre + "'");****/


                db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                if (usuario.Contrasena == null)  //checa si es  modificacion de la contraseña del usuario
                {
                    db.Entry(usuario).Property(p => p.Contrasena).IsModified = false; // si la contraseña no fue modificada, no le hace ningun cambio

                }
                else
                {
                    // Si se realizo actualizacion de la contraseña
                    //ACTUALIZA CONTRASEÑA EN BD MYSQL EJ: SET PASSWORD FOR 'PACIFICO' = PASSWORD('000000')
                    dbadmin.Database.ExecuteSqlCommand("SET PASSWORD FOR " + "'" + usuario.Nombre + "'" + " = PASSWORD('" + usuario.Contrasena + "')");
                    usuario.Contrasena = encriptacion.EncryptText(usuario.Contrasena);

                }
                db.SaveChanges();

                return usuario;

            }
            catch (Exception ex)
            {
                Error(ex, "El usuario");
                return usuario;
            }
        }




        public Usuarios add(Usuarios usuario)
        {
            try
            {
                Validar();
                UsuariosContext db = new UsuariosContext();
                AdminBD dbadmin = new AdminBD(); //SE CREA EL OBJETO PARA UTILIZAR EL CONTEX ADMINBD QUE SIRVE PARA EJECUTAR CONSULTA CON UN MISMO USUARIO, PARA NO CREAR CONFLICTOS
                PasswordEncrypt encriptacion = new PasswordEncrypt();

                //Crea el usuario en la base datos CREATE USER IF NOT EXISTS 'foo'@'localhost' IDENTIFIED BY 'bar';

                dbadmin.Database.ExecuteSqlCommand(
                    "CREATE USER IF NOT EXISTS '" + usuario.Nombre + "'" + "@'%' IDENTIFIED BY" + "'" + usuario.Contrasena + "'" + ";");

                usuario.Contrasena = encriptacion.EncryptText(usuario.Contrasena);
                db.Usuarios.Add(usuario);

                db.UsuariosRoles.Where(usr => usr.UsuarioId == usuario.Id).ToList();
                //se le asigna el rol correspondiente con la consulta "GRANT Juridico TO 'TESTROLES'"
                dbadmin.Database.ExecuteSqlCommand("GRANT " + db.Roles.Find(usuario.UsuariosRoles.ToList()[0].RolId).Nombre + " TO " + "'" + usuario.Nombre + "'");
            //    UPDATE `user` set default_role = 'ROLTELEFONO' where `User`= 'tablet'
             //  db.Database.ExecuteSqlCommand("UPDATE `mysql`.`user` set default_role = '" + db.Roles.Find(usuario.UsuariosRoles.ToList()[0].RolId).Nombre + "'" + " WHERE `User`= "+  "'" + usuario.Nombre + "'");

                //Verifica los roles del usuario
                /*    foreach (UsuariosRoles rol in usuario.UsuariosRoles)
                    {
                        //Consulta a que empresas tiene derecho un rol
                        var empresas=db.BDEmpresasRoles.Where(usr => usr.RolId == rol.RolId).ToList();
                        foreach(BDEmpresasRoles empresarol in empresas)
                        {
                            var nombrebd = db.BDEmpresas.Find(empresarol.BDEmpresaId);
                            //Le da permiso al usuario para la empresa selcccionada en la bd mysql.user
                            db.Database.ExecuteSqlCommand(
                                "GRANT SELECT ON " + nombrebd.RFC + ".* TO" + "'" + usuario.Nombre + "'");
                        }
                    }

                    //Se dan permisos por default a la bd usuarios ylos aplica en la bd mysql.user
                    db.Database.ExecuteSqlCommand(
                            "GRANT SELECT ON Usuarios.* TO" + "'" + usuario.Nombre + "'");

                    db.Database.ExecuteSqlCommand(
                            "GRANT INSERT,UPDATE ON Usuarios.Tokens TO" + "'" + usuario.Nombre + "'");*/

                db.SaveChanges();
                usuario.Contrasena = null;
                return usuario;
            }
            catch (Exception ex)
            {
                Error(ex,"El usuario ");
                return usuario;
            }
        }

        public Usuarios delete(Usuarios usuario)
        {
            try
            {
                Validar();
 
                UsuariosContext db = new UsuariosContext();
                db.Usuarios.Attach(usuario);
                db.Usuarios.Remove(usuario); //BORRA EL USUARIO DE LA  BD USUARIOS
                db.SaveChanges();
                AdminBD dbadmin = new AdminBD();
                dbadmin.Database.ExecuteSqlCommand("DROP USER " + "'" + usuario.Nombre + "'"); //BORRA EL USUARIO DE LA TABLA BD mysql 
                return usuario;
            }
            catch (Exception ex)
            {
                Error(ex,"usuario ");
                return null;
            }
        }
    }

}
