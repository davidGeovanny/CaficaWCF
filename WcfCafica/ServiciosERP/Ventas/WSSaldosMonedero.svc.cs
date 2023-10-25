using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WSSaldosMonedero" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WSSaldosMonedero.svc or WSSaldosMonedero.svc.cs at the Solution Explorer and start debugging.
    public class WSSaldosMonedero : WsBase, IWSSaldosMonedero
    {
        public List<SaldosMonedero> getall()
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }

        public SaldosMonedero add(SaldosMonedero saldo)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El usuario");
                return null;
            }
        }
        public SaldosMonedero get(int id)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex);
                return null;
            }
        }
        public double saldoUsuario()
        { 
            try
            {
                ValidarWebMonedero();
                EmpresaContext db = new EmpresaContext();

                CargosAbonos saldo = new CargosAbonos();
                
                return saldo.Saldo(int.Parse(getKey("id", "Token")),db);
            }
            catch (Exception ex)
            {

                Error(ex);
                return 0.0;
            }
        }
        public SaldosMonedero update(SaldosMonedero saldo)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {

                Error(ex, "El usuarios monedero");
                return null;
            }
        }
        public SaldosMonedero delete(SaldosMonedero saldo)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Error(ex, "El usuario monedero");
                return null;
            }
        }
    }
}
