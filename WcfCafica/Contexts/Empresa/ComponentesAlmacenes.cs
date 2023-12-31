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
    
    public partial class  ComponentesAlmacenes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ComponentesAlmacenes()
        {
            this.InventariosESDetalles = new HashSet<InventariosESDetalles>();
            this.InventariosFisicosDetalles = new HashSet<InventariosFisicosDetalles>();
            this.ResguardosDetalles = new HashSet<ResguardosDetalles>();
        }
    
        public long Id { get; set; }
        public long ComponenteId { get; set; }
        public long AlmacenId { get; set; }
        public Nullable<long> AlmacenesGruposComponentesId { get; set; }
        public Nullable<long> Maximo { get; set; }
        public Nullable<long> Reorden { get; set; }
        public Nullable<long> Minimo { get; set; }
        public string Localizacion { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual Almacenes Almacenes { get; set; }
        public virtual AlmacenesGruposComponentes AlmacenesGruposComponentes { get; set; }
        public virtual Componentes Componentes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosESDetalles> InventariosESDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosFisicosDetalles> InventariosFisicosDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResguardosDetalles> ResguardosDetalles { get; set; }
    }
}
