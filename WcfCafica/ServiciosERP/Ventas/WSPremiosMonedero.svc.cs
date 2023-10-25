using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSPremiosMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSPremiosMonedero.svc or WSPremiosMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSPremiosMonedero : WsBase, IWSPremiosMonedero
    {
        public List<PremiosMonedero> getall()
        {
            try
            {
                ValidarWebMonedero();

                EmpresaContext db = new EmpresaContext();
                DateTime DiaActual = DateTime.Now;
                List<PremiosMonedero> Premios = db.PremiosMonedero.ToList();

                return Premios;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public List<PremiosMonedero> premiosVigentes(int index)
        {
            try
            {
                ValidarWebMonedero();

                EmpresaContext db = new EmpresaContext();
                DateTime DiaActual = DateTime.Now;
                List<PremiosMonedero> Premios = db.PremiosMonedero.Where(c => c.FechaInicia <= DiaActual && c.FechaTermina >= DiaActual).OrderBy(o => o.Id).Skip(index).Take(20).ToList();

                return Premios;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public PremiosMonedero add(PremiosMonedero premio)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                foreach (PremiosMonederoDetalles pd in premio.PremiosMonederoDetalles)
                {
                    var equivalencia = db.GruposUnidadesDetalle.Where(x => x.GrupoUnidadesId == pd.Componentes.GrupoUnidadesId && x.UnidadEquivalenteId == pd.UnidadId).SingleOrDefault();
                    pd.CantidadReal = equivalencia.CantidadBase*pd.Cantidad;
                    pd.UnidadRealId = equivalencia.UnidadBaseId;
                    pd.Componentes = null;
                    
                }
               
                db.PremiosMonedero.Add(premio);
                db.SaveChanges();
                return premio;
            }
            catch (Exception ex)
            {
                Error(ex, "El premio");
                return null;
            }
        }
        public PremiosMonedero get(int id)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                PremiosMonedero premio = db.PremiosMonedero.Include("PremiosMonederoDetalles").Include("PremiosMonederoDetalles.Componentes")
                    .Where(i => i.Id == id).SingleOrDefault();
                return premio;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public PremiosMonedero update(PremiosMonedero premio)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();

                foreach (PremiosMonederoDetalles pd in premio.PremiosMonederoDetalles)
                {
                 //   pd.Unidades = null;
                 //   pd.Unidades1 = null;
                   // pd.PremiosMonedero = null;
                    if(pd.Id==0)
                    {
                        var equivalencia = db.GruposUnidadesDetalle.Where(x => x.GrupoUnidadesId == pd.Componentes.GrupoUnidadesId && x.UnidadEquivalenteId == pd.UnidadId).SingleOrDefault();
                        pd.CantidadReal = equivalencia.CantidadBase * pd.Cantidad;
                        pd.UnidadRealId = equivalencia.UnidadBaseId;
                    }

                    pd.Componentes = null;
                }

                //Se Obtiene la lista de los detalles
                List<PremiosMonederoDetalles> DetallesExistentes = db.PremiosMonederoDetalles.Where(c => c.PremioMonederoId == premio.Id).ToList();
                List<PremiosMonederoDetalles> DetallesAgregados = premio.PremiosMonederoDetalles.Where(n => n.Id == 0).ToList();
                List<PremiosMonederoDetalles> DetallesModificados = premio.PremiosMonederoDetalles.Where(n => n.Id != 0).ToList();
                List<PremiosMonederoDetalles> DetallesEliminados = DetallesExistentes.Where(n => !DetallesModificados.Select(n1 => n1.Id).Contains(n.Id)).ToList();
                //Se agregan y eliminan los rangos correspondientes

                db.PremiosMonederoDetalles.AddRange(DetallesAgregados);
                db.PremiosMonederoDetalles.RemoveRange(DetallesEliminados);

                foreach (PremiosMonederoDetalles pd in DetallesModificados)
                {
                    var local = db.Set<PremiosMonederoDetalles>().Local.FirstOrDefault(l => l.Id == pd.Id);
                    pd.PremiosMonedero = null;
                    if (local != null)
                        db.Entry(local).State = System.Data.Entity.EntityState.Detached;
                    db.Entry(pd).State = System.Data.Entity.EntityState.Modified;
                }

               
                premio.PremiosMonederoDetalles = null;

                db.PremiosMonedero.Attach(premio);
                db.Entry(premio).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return premio;
            }
            catch (Exception ex)
            {

                Error(ex,"El premio");
                return null;
            }
        }
        public PremiosMonedero delete(PremiosMonedero premio)
        {
            try
            {
                Validar();

                EmpresaContext db = new EmpresaContext();

                premio = db.PremiosMonedero.Find(premio.Id);
                db.PremiosMonedero.Attach(premio);
                db.PremiosMonedero.Remove(premio);
                db.SaveChanges();

                return premio;

            }
            catch (Exception ex)
            {
                Error(ex, "El premo ");
                return null;
            }
        }
    }
}
