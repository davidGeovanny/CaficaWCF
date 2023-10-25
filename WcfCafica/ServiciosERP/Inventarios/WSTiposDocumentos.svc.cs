using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSTiposDocumentos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSTiposDocumentos.svc or WSTiposDocumentos.svc.cs at the Solution Explorer and start debugging.
    public class WSTiposDocumentos : WsBase, IWSTiposDocumentos
    {
        public List<TiposDocumentos> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var tiposdocumentos = db.TiposDocumentos.ToList();

                return tiposdocumentos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public TiposDocumentos get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                TiposDocumentos tiposdocumentos = db.TiposDocumentos.Find(ID);
                return tiposdocumentos;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public TiposDocumentos add(TiposDocumentos tipodocumento)
        {
            return null;
        }
        public TiposDocumentos update(TiposDocumentos tipodocumento)
        {
            return null;
        }
        public TiposDocumentos delete(TiposDocumentos tipodocumento)
        {
            return null;
        }
    }
}
