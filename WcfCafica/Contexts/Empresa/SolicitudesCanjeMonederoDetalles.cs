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
    
    public partial class  SolicitudesCanjeMonederoDetalles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SolicitudesCanjeMonederoDetalles()
        {
            this.MovimientosMonederoDetalles = new HashSet<MovimientosMonederoDetalles>();
        }
    
        public long Id { get; set; }
        public long SolicitudCanjeMonederoId { get; set; }
        public long UsuarioMonederoId { get; set; }
        public long PremioId { get; set; }
        public int PuntosCanje { get; set; }
        public Nullable<System.DateTime> VigenciaAl { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MovimientosMonederoDetalles> MovimientosMonederoDetalles { get; set; }
        public virtual PremiosMonedero PremiosMonedero { get; set; }
        public virtual SolicitudesCanjeMonedero SolicitudesCanjeMonedero { get; set; }
        public virtual UsuariosMonedero UsuariosMonedero { get; set; }
    }
}