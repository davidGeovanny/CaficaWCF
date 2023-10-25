using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSConceptosES" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSConceptosES.svc or WSConceptosES.svc.cs at the Solution Explorer and start debugging.
    public class WSConceptosES : WsBase, IWSConceptosES
    {
        public string[] CamposBloqueados = { "Naturaleza", "TipoDocumentoId" };

        public List<ConceptosES> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var conceptos = db.ConceptosES.Include("TiposDocumentos").Where(id => id.Id !=13 && id.Id!=14 ).ToList();

                return conceptos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public ConceptosES get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                ConceptosES concepto = db.ConceptosES.Find(ID);
                return concepto;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        public ConceptosES add(ConceptosES concepto)
        {
            try
            {
                Validar();
                //Metodo para Agregar una empresa
                EmpresaContext db = new EmpresaContext();
                db.ConceptosES.Add(concepto);
                concepto.Predefinido = "NO";
                db.SaveChanges();
                return concepto;

            }
            catch (Exception ex)
            {
                Error(ex, "El concepto ");
                return null;
            }
        }

        public ConceptosES update(ConceptosES concepto)
        {
            try
            {
                Validar();
                //Metodo para Actualizar los campos de las empresas
                EmpresaContext db = new EmpresaContext();
                ValidarCamposBloqueados<ConceptosES, EmpresaContext>(concepto, CamposBloqueados, "Conceptos E/S");
                if (concepto.Predefinido == "SI")
                    throw new Exception("No se puede modificar un concepto predefinido");

                db.ConceptosES.Attach(concepto);
                db.Entry(concepto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return concepto;
            }
            catch (Exception ex)
            {
                Error(ex, "El concepto ");
                return concepto;
            }
        }

        public ConceptosES delete(ConceptosES conceptoes)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
           
            try
            {
                Validar();
                //Metodo para cambiar el BanEliminar una Empresa / parametro Empresa    
                EmpresaContext db = new EmpresaContext();
                ConceptosES concepto = db.ConceptosES.Find(conceptoes.Id);
                if (conceptoes.Predefinido == "SI")
                    throw new Exception("No se puede eliminar un concepto predefinido");
                else
                {
                    db.ConceptosES.Attach(concepto);
                    db.ConceptosES.Remove(concepto);
                    db.SaveChanges();
                }
                return concepto;
                
            }
            catch (Exception ex)
            {
                Error(ex, "El concepto ");
                return null;
            }
        }
        public List<ConceptosES> getConceptosEntradaSalida(string Naturaleza) //Obtine todos los conceptos con NAturaleza ENTRADA o salida
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var conceptosentrada = (from s in db.ConceptosES
                                           where s.Naturaleza == Naturaleza where s.Id!=13 where s.Id!=14
                                        select s
                                           ).ToList();
                return conceptosentrada;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }


    }
}
