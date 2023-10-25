using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Compras
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSProveedores" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSProveedores.svc or WSProveedores.svc.cs at the Solution Explorer and start debugging.
    public class WSProveedores : WsBase,IWSProveedores
    {
        public Proveedores add(Proveedores proveedor)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                foreach(ProveedoresDirecciones pd in proveedor.ProveedoresDirecciones )
                {
                    pd.Colonias = null;
                    pd.CodigosPostales = null;
                    pd.Ciudades = null;
                    pd.Municipios = null;
                    pd.Estados = null;
                    pd.Paises = null;
                }
                foreach (ProveedoresContactos pc in proveedor.ProveedoresContactos)
                {
                    pc.Colonias = null;
                    pc.CodigosPostales = null;
                    pc.Ciudades = null;
                    pc.Municipios = null;
                    pc.Estados = null;
                    pc.Paises = null;
                }
                db.Proveedores.Add(proveedor);
                db.SaveChanges();
                return proveedor;

            }
            catch (Exception ex)
            {
                Error(ex, "El proveedor ");
                return null;
            }
        }

        public Proveedores delete(Proveedores proveedor)
        {
            try
            {
                Validar();
              
                EmpresaContext db = new EmpresaContext();

                proveedor = db.Proveedores.Find(proveedor.Id);
                db.Proveedores.Attach(proveedor);
                db.Proveedores.Remove(proveedor);
                db.SaveChanges();

                return proveedor;

            }
            catch (Exception ex)
            {
                Error(ex, "El proveedor ");
                return null;
            }
        }


        public Proveedores get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Proveedores proveedor = db.Proveedores.Include("ProveedoresDirecciones").Include("ProveedoresDirecciones.Colonias")
                    .Include("ProveedoresDirecciones.CodigosPostales").Include("ProveedoresDirecciones.Ciudades")
                    .Include("ProveedoresDirecciones.Municipios").Include("ProveedoresDirecciones.Estados")
                    .Include("ProveedoresDirecciones.Paises")
                    .Include("ProveedoresContactos")
                    .Where(i => i.Id == id).SingleOrDefault();
                return proveedor;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<Proveedores> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var proveedores = db.Proveedores.ToList();

                return proveedores;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Proveedores update(Proveedores proveedor)
        {
            try
            {
                Validar();

                foreach (ProveedoresDirecciones pd in proveedor.ProveedoresDirecciones)
                {
                    pd.Colonias = null;
                    pd.CodigosPostales = null;
                    pd.Ciudades = null;
                    pd.Municipios = null;
                    pd.Estados = null;
                    pd.Paises = null;
                }
                foreach (ProveedoresContactos pc in proveedor.ProveedoresContactos)
                {
                    pc.Colonias = null;
                    pc.CodigosPostales = null;
                    pc.Ciudades = null;
                    pc.Municipios = null;
                    pc.Estados = null;
                    pc.Paises = null;
                }

                EmpresaContext db = new EmpresaContext();
           
                //Se Obtiene la lista de las direcciones
                List<ProveedoresDirecciones> DireccionesExistentes = db.ProveedoresDirecciones.Where(c => c.ProveedorId == proveedor.Id).ToList();
                List<ProveedoresDirecciones> DireccionesAgregadas = proveedor.ProveedoresDirecciones.Where(n => n.Id == 0).ToList();
                List<ProveedoresDirecciones> DireccionesModificadas = proveedor.ProveedoresDirecciones.Where(n => n.Id != 0).ToList();
                List<ProveedoresDirecciones> DireccionesEliminadas = DireccionesExistentes.Where(n => !DireccionesModificadas.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.ProveedoresDirecciones.AddRange(DireccionesAgregadas);
                db.ProveedoresDirecciones.RemoveRange(DireccionesEliminadas);

                foreach (ProveedoresDirecciones pd in DireccionesModificadas)
                {
                    var local = db.Set<ProveedoresDirecciones>().Local.FirstOrDefault(l => l.Id == pd.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(pd).State = System.Data.Entity.EntityState.Modified;
                }

                //Se Obtiene la lista de las contactos
                List<ProveedoresContactos> ContactosExistentes = db.ProveedoresContactos.Where(c => c.ProveedorId == proveedor.Id).ToList();
                List<ProveedoresContactos> ContactosAgregados = proveedor.ProveedoresContactos.Where(n => n.Id == 0).ToList();
                List<ProveedoresContactos> ContactosModificados = proveedor.ProveedoresContactos.Where(n => n.Id != 0).ToList();
                List<ProveedoresContactos> ContactosEliminados = ContactosExistentes.Where(n => !ContactosModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.ProveedoresContactos.AddRange(ContactosAgregados);
                db.ProveedoresContactos.RemoveRange(ContactosEliminados);

                foreach (ProveedoresContactos pc in ContactosModificados)
                {
                    var local = db.Set<ProveedoresContactos>().Local.FirstOrDefault(l => l.Id == pc.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                }



                proveedor.ProveedoresDirecciones = null;
                proveedor.ProveedoresContactos = null;

                db.Proveedores.Attach(proveedor);
                db.Entry(proveedor).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return proveedor;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
    }
}
