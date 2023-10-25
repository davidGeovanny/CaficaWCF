using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Administracion;

namespace WcfCafica.ServiciosERP
{
     public class WSMenu : WsBase, IWSMenu
    {
        public List<ItemNavBar> getMenuNavBar(int moduloid)
        {
            try
            {
                List<ItemNavBar> menus = new List<ItemNavBar>();

                //Obtenemos el id del usuario  desde el token
                long UsuarioID = Convert.ToInt64(getKey("id","Token")); 

                //Obtenemos el modulo que presione el usuario               
                var ModuloID = moduloid;
                UsuariosContext db = new UsuariosContext();

                //Obtiene los roles a los que pertenece el usuario
                var roles = db.UsuariosRoles.Where(usr => usr.UsuarioId == UsuarioID).ToList();

                //Verifica todos las vistas que pertenecen al modulo seleccionado
                var modulos = db.VistasGruposVistas.Where(vst => vst.ModuloId == ModuloID).ToList();

                //Recorre tods las vista del modulo
                foreach (VistasGruposVistas vistagrupo in modulos)
                {
                    bool permiso=false;
                    //Checa si algun de los roles a los que pertencen tiene persmiso para la vista actual
                    foreach (UsuariosRoles rol in roles)
                    { 
                        var acciones= (from av in db.AccionesVistas
                                  join r in db.RolesAcciones on av.Id equals r.AccionId
                                  where av.VistaId == vistagrupo.VistaId
                                  where r.RolId == rol.RolId
                                  select
                                     av
                       ).ToList();
                        permiso = acciones.ToList().Count > 0 ? true : false; 
                        //Si detecta true en algunos de los roles rompe el ciclo
                        if(permiso==true)
                                break;
                    }
                    //Si tiene permiso para la vista la agrega al objeto de retorno
                    if (permiso)
                    {
                        var vista = db.Vistas.Find(vistagrupo.VistaId);

                        ItemNavBar menu = new ItemNavBar();
                        menu.Nombre = vista.Nombre;
                        menu.Grupo = db.GruposVistas.Find(vistagrupo.GrupoVistaId).Nombre;
                        menu.ImagenMenu = vista.ImagenMenu;
                        menu.Parametros = vista.Id +"," + vista.Tipo + "," + vista.Parametros;
                        menus.Add(menu);
                    }
                }

                //Consulta que retorna todas las vistas a las que tiene permiso el usuario
                /*var menu = (from ur in db.UsuariosRoles
                            join ra in db.RolesAcciones on ur.RolId equals ra.RolId
                            join av in db.AccionesVistas on ra.AccionId equals av.Id 
                            join v in db.Vistas on av.VistaId equals v.Id
                            join gv in db.GruposVistas on v.GrupoVistaId equals gv.Id
                            where ur.UsuarioId == UsuarioID
                            where gv.ModuloId == ModuloID
                         /*   where ur.BanEliminar==0
                            where ra.BanEliminar==0
                            where av.BanEliminar==0
                            where v.BanEliminar==0
                            where gv.BanEliminar==0*/
                            /*group av by new { Id = v.Id, Nombre = v.Nombre,Grupo=gv.Nombre,ImagenMenu=v.ImagenMenu,Parametros=v.Parametros,GrupoOrden=gv.Orden,VistaOrden=v.Orden} into t
                            orderby t.Key.GrupoOrden, t.Key.VistaOrden
                            select new ItemNavBar
                            {
                                Id=t.Key.Id,
                                Nombre = t.Key.Nombre,
                                Grupo = t.Key.Grupo,
                                ImagenMenu = t.Key.ImagenMenu,
                                Parametros = t.Key.Parametros,
                           
                            }
                       ).ToList();*/
                return menus;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

        public string[] getDatosLogin()
        {
            try
            {
                
                long UsuarioID = Convert.ToInt64(getKey("id", "Token"));
                long EmpresaID = Convert.ToInt64(getKey("id", "TokenEmpresa"));
                UsuariosContext db = new UsuariosContext();
                string[] datos = new string[3];
                //Obtener  usuario
                var usuario = db.Usuarios.Find(UsuarioID);

                //Obtener  empresa
                var empresa = db.BDEmpresas.Find(EmpresaID);
                  

                datos[0] = usuario.Nombre;
                datos[1] = usuario.NombreCompleto;
                datos[2] = empresa.RazonSocial;

                return datos;
           }
           catch (Exception ex)
           {
                Error(ex);
                return null;
           }
    }
       
        public List<Vistas> getPermisosFormulario()
        {

            try
            {
                long UsuarioID =Convert.ToInt64(getKey("id", "Token"));
                UsuariosContext db = new UsuariosContext();
                List<Vistas> lstVistas = new List<Vistas>();

                //Consulta que retorna las vistas a las que tiene permiso los roles de un usuario
                var vistasPermitidas = (from v in db.Vistas
                                        join av in db.AccionesVistas on v.Id equals av.VistaId
                                        join ra in db.RolesAcciones on av.Id equals ra.AccionId
                                        join ur in db.UsuariosRoles on ra.RolId equals ur.RolId
                                        //  where ra.BanEliminar == 0
                                        //  where ur.BanEliminar == 0
                                        //  where av.BanEliminar == 0
                                        where ur.UsuarioId == UsuarioID
                                        select
                                           v
                       ).ToList();

                //Ciclo para formar la lista de las vistas con las acciones permitidas
                foreach (Vistas vista in vistasPermitidas)
                {
                    //Consulta que retorna los ids de las acciones permitidas
                    var accionesPermitidas = (from v in db.Vistas
                                              join av in db.AccionesVistas on v.Id equals av.VistaId
                                              join ra in db.RolesAcciones on av.Id equals ra.AccionId
                                              join ur in db.UsuariosRoles on ra.RolId equals ur.RolId
                                              /* where ra.BanEliminar == 0
                                               where ur.BanEliminar == 0
                                               where av.BanEliminar == 0*/
                                              where ur.UsuarioId == UsuarioID
                                              select
                                                 av.Id
                  );
                    //Se obtienen las acciones relacionadas con la vista y que estan permitidas
                    /*  db.Entry(vista)
                        .Collection(x => x.AccionesVistas)
                        .Query()
                        .Where(y => accionesPermitidas.Contains(y.Id))
                        .Load();
                      //Se agregar a la lista que retornara el metodo
                      lstVistas.Add(vista);*/
                }

                return lstVistas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
          
        }
    }
}
