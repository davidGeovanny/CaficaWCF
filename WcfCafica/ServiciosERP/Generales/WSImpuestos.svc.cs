using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSImpuestos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSImpuestos.svc or WSImpuestos.svc.cs at the Solution Explorer and start debugging.
    public class WSImpuestos :  WsBase, IWSImpuestos
    {
        public Impuestos add(Impuestos impuesto)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

               /* if (impuesto.Predeterminado == "SI")
                    if (HayImpuestoPredeterminado(db,impuesto.Id))
                        throw new Exception("Ya existe un impuesto predeterminado.");*/

                db.Impuestos.Add(impuesto);
                db.SaveChanges();
                return impuesto;

            }
            catch (Exception ex)
            {
                Error(ex, "El impuesto ");
                return null;
            }
        }

        public Impuestos delete(Impuestos impuesto)
        {
            try
            {
                Validar();
              
                EmpresaContext db = new EmpresaContext();

                impuesto = db.Impuestos.Find(impuesto.Id);
                db.Impuestos.Attach(impuesto);
                db.Impuestos.Remove(impuesto);
                db.SaveChanges();
                
                return impuesto;

            }
            catch (Exception ex)
            {
                Error(ex, "El impuesto ");
                return null;
            }
        }

        public Impuestos get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                Impuestos impuesto = db.Impuestos.Include("ImpuestosGravados1").Where(i=>i.Id==id).SingleOrDefault();
                return impuesto;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public List<Impuestos> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var impuestos = db.Impuestos.ToList();

                return impuestos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public Impuestos update(Impuestos impuesto)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos del impuesto

                EmpresaContext db = new EmpresaContext();
               /* if (impuesto.Predeterminado == "SI")
                    if (HayImpuestoPredeterminado(db,impuesto.Id))
                        throw new Exception("Ya existe un impuesto predeterminado.");*/

                //Se Obtiene la lista de los impuestos gravados
                List<ImpuestosGravados> ImpuestosGravadosExistentes = db.ImpuestosGravados.Where(c => c.ImpuestoId == impuesto.Id).ToList();
                List<ImpuestosGravados> ImpuestosGravadosAgregados = impuesto.ImpuestosGravados1.Where(n => n.Id == 0).ToList();
                List<ImpuestosGravados> ImpuestosGravadosModificados = impuesto.ImpuestosGravados1.Where(n => n.Id != 0).ToList();
                List<ImpuestosGravados> ImpuestosGravadosEliminados = ImpuestosGravadosExistentes.Where(n => !ImpuestosGravadosModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes
                db.ImpuestosGravados.AddRange(ImpuestosGravadosAgregados);
                db.ImpuestosGravados.RemoveRange(ImpuestosGravadosEliminados);

                impuesto.ImpuestosGravados1 = null;

                db.Impuestos.Attach(impuesto);
                db.Entry(impuesto).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return impuesto;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }

       /* public bool HayImpuestoPredeterminado(EmpresaContext db,long id)
        {
            long ImpuestoPred;
            if(id==0)
                ImpuestoPred = db.Impuestos.Where(x => x.Predeterminado == "SI").Count();
            else
                ImpuestoPred = db.Impuestos.Where(x => x.Predeterminado == "SI").Where(z=>z.Id!=id).Count();

            if (ImpuestoPred > 0)
                return true;
            else
                return false;
        }*/
    }
}
