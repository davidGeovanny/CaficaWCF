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
    
    public partial class  Resguardos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Resguardos()
        {
            this.ResguardosDetalles = new HashSet<ResguardosDetalles>();
            this.ResguardosLotesSeries = new HashSet<ResguardosLotesSeries>();
        }
    
        public long Id { get; set; }
        public long AlmacenId { get; set; }
        public System.DateTime Fecha { get; set; }
        public string TipoDocumento { get; set; }
        public string Descripcion { get; set; }
        public Nullable<long> ResponsableId { get; set; }
        public Nullable<long> InventariosESId { get; set; }
        public string Cancelado { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual Almacenes Almacenes { get; set; }
        public virtual Responsables Responsables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResguardosDetalles> ResguardosDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResguardosLotesSeries> ResguardosLotesSeries { get; set; }
    }
}
