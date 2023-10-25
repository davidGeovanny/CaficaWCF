using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;


namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSCondicionesPago" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSCondicionesPago.svc or WSCondicionesPago.svc.cs at the Solution Explorer and start debugging.
    public class WSCondicionesPago : WsBase, IWSCondicionesPago
    {
        public List<CondicionesPago> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var condiciones = db.CondicionesPago.ToList();

                return condiciones;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public CondicionesPago get(int ID)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                CondicionesPago condicion = db.CondicionesPago.Include("CondicionesPagoPlazos").Where(i => i.Id == ID).SingleOrDefault();
                condicion.CondicionesPagoPlazos = condicion.CondicionesPagoPlazos.OrderBy(u => u.Id).ToList();// se ordena la lista 

                return condicion;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }
        public CondicionesPago add(CondicionesPago condicion)
        {
            try
            {


                Validar();
                EmpresaContext db = new EmpresaContext();


                if (condicion.Predeterminado == "SI")
                    if (HayCondicionPredeterminado(db, condicion.Id))
                        throw new Exception("Ya existe un condicion predeterminada.");

                db.CondicionesPago.Add(condicion);
                db.SaveChanges();
                return condicion;
            }
            catch (Exception ex)
            {
                Error(ex, "El impuesto ");
                return null;
            }

        }
        public CondicionesPago update(CondicionesPago condicion)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();


                if (condicion.Predeterminado == "SI")
                    if (HayCondicionPredeterminado(db, condicion.Id))
                        throw new Exception("Ya existe un condicion predeterminada.");
                //Se Obtiene la lista de los impuestos gravados
                List<CondicionesPagoPlazos> PagoPlazosExistentes = db.CondicionesPagoPlazos.Where(c => c.CondicionesPagoId == condicion.Id).ToList();
                List<CondicionesPagoPlazos> PagoPlazosAgregados = condicion.CondicionesPagoPlazos.Where(n => n.Id == 0).ToList();
                List<CondicionesPagoPlazos> PagoPlazosModificados = condicion.CondicionesPagoPlazos.Where(n => n.Id != 0).ToList();
                List<CondicionesPagoPlazos> PagoPlazosEliminados = PagoPlazosExistentes.Where(n => !PagoPlazosModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.CondicionesPagoPlazos.AddRange(PagoPlazosAgregados);
                db.CondicionesPagoPlazos.RemoveRange(PagoPlazosEliminados);
                foreach (CondicionesPagoPlazos cb in PagoPlazosModificados)
                {
                    var local = db.Set<CondicionesPagoPlazos>().Local.FirstOrDefault(l => l.Id == cb.Id);
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(cb).State = System.Data.Entity.EntityState.Modified;
                }

                condicion.CondicionesPagoPlazos = null;

                db.CondicionesPago.Attach(condicion);
                db.Entry(condicion).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return condicion;
            }
            catch (Exception ex)
            {

                Error(ex, "El impuesto ");
                return null;
            }
        }



        public CondicionesPago delete(CondicionesPago condicion)
        {

            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();

                condicion = db.CondicionesPago.Find(condicion.Id);
                db.CondicionesPago.Attach(condicion);
                db.CondicionesPago.Remove(condicion);
                db.SaveChanges();

                return condicion;

            }
            catch (Exception ex)
            {
                Error(ex, "El impuesto ");
                return null;
            }


        }
        public bool HayCondicionPredeterminado(EmpresaContext db, long id)
        {
            long CondicionPred;
            if (id == 0)
                CondicionPred = db.CondicionesPago.Where(x => x.Predeterminado == "SI").Count();
            else
                CondicionPred = db.CondicionesPago.Where(x => x.Predeterminado == "SI").Where(z => z.Id != id).Count();

            if (CondicionPred > 0)
                return true;
            else
                return false;
        }
    }
}
