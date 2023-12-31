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
    
    public partial class  InventariosFisicos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InventariosFisicos()
        {
            this.InventariosFisicosDetalles = new HashSet<InventariosFisicosDetalles>();
        }
    
        public long Id { get; set; }
        public long AlmacenId { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
        public Nullable<long> InventariosESEntradaId { get; set; }
        public Nullable<long> InventariosESSalidaId { get; set; }
        public Nullable<System.DateTime> FechaHoraCancelacion { get; set; }
        public Nullable<System.DateTime> FechaHoraAplicacion { get; set; }
        public string UsuarioCancelo { get; set; }
        public string UsuarioAplico { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual Almacenes Almacenes { get; set; }
        public virtual InventariosES InventariosES { get; set; }
        public virtual InventariosES InventariosES1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosFisicosDetalles> InventariosFisicosDetalles { get; set; }
    }
}
