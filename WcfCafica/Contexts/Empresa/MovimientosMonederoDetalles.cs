//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfCafica.Contexts.Empresa
{
    using System;
    using System.Collections.Generic;
    
    public partial class  MovimientosMonederoDetalles
    {
        public long Id { get; set; }
        public long MovimientoMonederoId { get; set; }
        public long PremioId { get; set; }
        public int Puntos { get; set; }
        public Nullable<long> SolicitudesCanjeMonederoDetallesId { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual PremiosMonedero PremiosMonedero { get; set; }
        public virtual SolicitudesCanjeMonederoDetalles SolicitudesCanjeMonederoDetalles { get; set; }
        public virtual MovimientosMonedero MovimientosMonedero { get; set; }
    }
}
