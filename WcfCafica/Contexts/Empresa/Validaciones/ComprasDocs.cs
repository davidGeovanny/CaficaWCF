using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace WcfCafica.Contexts.Empresa
{
    public partial class ComprasDocs 
    {
        public  void ValidarModel()
        {
            try
            {
                if (ComprasDocsDetalles.Count == 0)
                    throw new Exception("No ha realizado ningun movimiento, no es posible guardar");
                if (ComprasDocsDetalles.Where(c => c.CantidadCompra == 0).Count() > 0)
                    throw new Exception("No se permite guardar con componentes en cantidad cero");

               

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}