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
    
    public partial class  MovimientosMonedero
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MovimientosMonedero()
        {
            this.MovimientosMonederoDetalles = new HashSet<MovimientosMonederoDetalles>();
        }
    
        public long Id { get; set; }
        public long UsuarioMonederoId { get; set; }
        public System.DateTime FechaHora { get; set; }
        public Nullable<int> Canje { get; set; }
        public Nullable<int> Abono { get; set; }
        public string FolioTicket { get; set; }
        public Nullable<double> MontoTicket { get; set; }
        public Nullable<long> SocioMonederoId { get; set; }
        public Nullable<long> CentroCanjeMonederoId { get; set; }
        public string TipoProducto { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual CentrosCanjeMonedero CentrosCanjeMonedero { get; set; }
        public virtual SociosMonedero SociosMonedero { get; set; }
        public virtual UsuariosMonedero UsuariosMonedero { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MovimientosMonederoDetalles> MovimientosMonederoDetalles { get; set; }
    }
}
