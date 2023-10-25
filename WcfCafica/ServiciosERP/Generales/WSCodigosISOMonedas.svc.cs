using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Generales
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSCodigosISOMonedas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSCodigosISOMonedas.svc or WSCodigosISOMonedas.svc.cs at the Solution Explorer and start debugging.
    public class WSCodigosISOMonedas : WsBase, IWSCodigosISOMonedas
    {
        public List<CodigosISOMonedas> getall()
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                var codigoisomonedas = db.CodigosISOMonedas.ToList();

                return codigoisomonedas;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public CodigosISOMonedas get(int ID)
        {
            try
            {
                Validar();
                EmpresaContext db = new EmpresaContext();
                //Consulta que retorna una AccionVista usando como parametro el ID
                CodigosISOMonedas codigoisomoneda = db.CodigosISOMonedas.Find(ID);
                return codigoisomoneda;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }
        public CodigosISOMonedas add(CodigosISOMonedas tipodocumento)
        {
            return null;
        }
        public CodigosISOMonedas update(CodigosISOMonedas tipodocumento)
        {
            return null;
        }
        public CodigosISOMonedas delete(CodigosISOMonedas tipodocumento)
        {
            return null;
        }
    }
}
