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
    
    public partial class  ComprasDocsDetalles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ComprasDocsDetalles()
        {
            this.ComprasDocsRelacionesDetalles = new HashSet<ComprasDocsRelacionesDetalles>();
            this.ComprasDocsLotesSeries = new HashSet<ComprasDocsLotesSeries>();
            this.ComprasDocsRelacionesDetalles1 = new HashSet<ComprasDocsRelacionesDetalles>();
        }
    
        public long Id { get; set; }
        public long ComprasDocsId { get; set; }
        public long ComponenteId { get; set; }
        public double CantidadCompra { get; set; }
        public Nullable<double> CantidadPendiente { get; set; }
        public long UnidadCompraId { get; set; }
        public double Cantidad { get; set; }
        public long UnidadId { get; set; }
        public long GrupoUnidadesId { get; set; }
        public Nullable<double> PrecioUnitario { get; set; }
        public Nullable<double> DescuentoPorcentaje { get; set; }
        public Nullable<double> DescuentoImporte { get; set; }
        public Nullable<double> Importe { get; set; }
        public string Notas { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual Componentes Componentes { get; set; }
        public virtual ComprasDocs ComprasDocs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComprasDocsRelacionesDetalles> ComprasDocsRelacionesDetalles { get; set; }
        public virtual GruposUnidades GruposUnidades { get; set; }
        public virtual Unidades Unidades { get; set; }
        public virtual Unidades Unidades1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComprasDocsLotesSeries> ComprasDocsLotesSeries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComprasDocsRelacionesDetalles> ComprasDocsRelacionesDetalles1 { get; set; }
    }
}