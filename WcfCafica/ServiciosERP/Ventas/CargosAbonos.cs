using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Ventas
{
    public class CargosAbonos : WsBase
    {
        //Metodo para calcular el abono  del movimiento
        public int? Abono(double? FactorMonto, int? FactorPuntos, double? MontoCompra,long UsuarioMonederoId, EmpresaContext db)
        {
            try
            {
                double var =((MontoCompra * FactorPuntos) / FactorMonto) ?? 0.0;

                if (var< 1)
                    throw new Exception("El monto del ticket es insuficiente para generar puntos.");

                int abono = Convert.ToInt32(Math.Round(var, 0));

               

                SaldosMonedero SaldoActual = db.SaldosMonedero.Where(c => c.UsuarioMonederoId==UsuarioMonederoId).SingleOrDefault();
                if(SaldoActual==null)
                { 
                    SaldosMonedero Saldo = new SaldosMonedero();
                    Saldo.UsuarioMonederoId = UsuarioMonederoId;
                    Saldo.Saldo = abono;
                    db.Entry(Saldo).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    SaldoActual.Saldo = SaldoActual.Saldo + abono;
                    db.Entry(SaldoActual).State = System.Data.Entity.EntityState.Modified;
                }


                db.SaveChanges();


                return abono;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //Actuliza el saldo del usuario en la tabla saldosMonedero
        public double Cargo(long UsuarioMonederoId,int TotalCanje,EmpresaContext db)
        {
            try
            {
                //Obtengo el saldo real
                //double saldo = Saldo(UsuarioMonederoId, db);

                //Consulto si tengo saldo disponible para hacer el movimiento
                /*if (TotalCanje > Saldo(UsuarioMonederoId, db))
                    throw new Exception("El cliente no cuenta con suficiente saldo para realizar el canje");*/

                //Actualizao el saldo de cliente
                SaldosMonedero SaldoActual = db.SaldosMonedero.Where(c => c.UsuarioMonederoId == UsuarioMonederoId).SingleOrDefault();
                SaldoActual.Saldo =SaldoActual.Saldo-TotalCanje;
                db.Entry(SaldoActual).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return SaldoActual.Saldo;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public double Saldo(long UsuarioId, EmpresaContext db)
        {
            try
            {
                var saldo = db.SaldosMonedero.Where(c => c.UsuarioMonederoId == UsuarioId).SingleOrDefault();

                double SaldoReal = saldo == null ? 0.0 : saldo.Saldo;

                //Analisis por detalle
                //double SaldoPendiente = db.SolicitudesCanjeMonederoDetalles.Where(c => c.VigenciaAl > DateTime.Now && c.UsuarioMonederoId == UsuarioId && c.Estado == "PENDIENTE").Sum(s => (double?)s.PuntosCanje) ?? 0.0;

                //Analisis por documento
                double SaldoPendiente = db.SolicitudesCanjeMonedero.Where(c => c.VigenciaAl > DateTime.Now && c.UsuarioMonederoId == UsuarioId && c.Estado == "PENDIENTE").Sum(s => (double?)s.TotalPuntosCanje) ?? 0.0;

                return SaldoReal - SaldoPendiente;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}