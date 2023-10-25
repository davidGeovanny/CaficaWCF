using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Compras
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSOrdendeCompra" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSOrdendeCompra.svc or WSOrdendeCompra.svc.cs at the Solution Explorer and start debugging.
    public class WSOrdendeCompra : WsBase,IWSOrdendeCompra
    {
        ComprasDocsImpuestos impuestos = new ComprasDocsImpuestos();
        Impuestos porcentaje = new Impuestos();
        double? subtotalcompra = 0;

        public List<ComprasDocs> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var compras = db.ComprasDocs.ToList();

                return compras;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public ComprasDocs get(int ID)
        {

            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                ComprasDocs compra = db.ComprasDocs.Include("ComprasDocsDetalles").Where(i => i.Id == ID).SingleOrDefault();
                compra.ComprasDocsDetalles = compra.ComprasDocsDetalles.OrderBy(u => u.Id).ToList();// se ordena la lista 

                return compra;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }
        public ComprasDocs add(ComprasDocs compra)
        {
            try
            {

                
                Validar();
                EmpresaContext db = new EmpresaContext();
                ComprasDocsImpuestos compraimpuesto = new ComprasDocsImpuestos();

                compra.ValidarModel();

                double? TotalDocumento=0;
                long impuestocomponente;
                

           //     LlenarTablaComprasDocsImpuestos(compra, db);

                foreach (ComprasDocsDetalles detalle in compra.ComprasDocsDetalles)
                    {
                      
                      TotalDocumento += detalle.Importe;
                      detalle.Componentes = null;

                    /////////////////////////LLENAR TABLA ComprasDocsImpuestos////////////////////////////
                    /*      compraimpuesto.ComprasDocsId = compra.Id;
                          compraimpuesto.ImpuestoId=

      ComprasDocsIdbigint
      ImpuestoIdbigint
      SubtotalCompradouble              
      Otrosimpuestosdouble
      Porcentajedouble 
      Importe*/
                }
                compra.Total = TotalDocumento ;

                db.ComprasDocs.Add(compra);
                
                db.SaveChanges();
                return compra;
            }
            catch (Exception ex)
            {
                Error(ex, "La compra ");
                return null;
            }

        }
        public ComprasDocs update(ComprasDocs compra)
        {

            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex, "El impuesto ");
                return null;
            }
        }



        public ComprasDocs delete(ComprasDocs compra)
        {

            try
            {
                return null;

            }
            catch (Exception ex)
            {
                Error(ex, "El impuesto ");
                return null;
            }


        }
        /*   public ComprasDocsImpuestos LlenarTablaComprasImpuestos(ComprasDocs compras, ComprasDocsDetalles Detalle, EmpresaContext db)
           {


                   impuestoarticulo = db.ComponentesImpuestos.Where(id => id.ComponenteId == Detalle.ComponenteId).SingleOrDefault();
                   ImpuestoActual.ImpuestoId = impuestoarticulo.ImpuestoId;
                   ImpuestoActual.SubtotalCompra += Detalle.Importe;

                   Impu = db.Impuestos.Where(id => id.Id == impuestoarticulo.Id).SingleOrDefault();
                   Impuesto.ComprasDocsId = compra.Id;
                   Impuesto.Porcentaje = (double)porcentaje.Porcentaje;
                   Impuesto.Importe


           } */
      /*  public void LlenarTablaComprasDocsImpuestos(ComprasDocs compra, EmpresaContext db)
        {
             List < ComprasDocsImpuestos > (compra.ComprasDocsDetalles.Select(a => a.Componentes.ComponentesImpuestos));
            //////////////////////////////////////SENTENCIAS CONDICIONALES//////////////////////////////////////////////////
            if (compra.ComprasDocsDetalles.ToList().GroupBy(a=>a.Componentes.ComponentesImpuestos) == null) throw new Exception("El articulo "+ detalles.Componentes.Nombre+" debe tener al menos un impuesto");
            //////////////////////////////////////SENTENCIAS CONDICIONALES//////////////////////////////////////////////////

            impuestos.ImpuestoId=detalles.Componentes.ComponentesImpuestos.FirstOrDefault().ImpuestoId;
            impuestos.ImpuestoId=detalles.Importe

            //GUARDAR LOS DATOS TABLAS:COMPRASDOCSIMPUESTOS
            db.ComprasDocsImpuestos.Add(impuestos);
             
        }*/

    }
}
