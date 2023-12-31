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
    
    public partial class  Componentes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Componentes()
        {
            this.ComponentesAlmacenes = new HashSet<ComponentesAlmacenes>();
            this.ComponentesFormulaDetalles = new HashSet<ComponentesFormulaDetalles>();
            this.ComponentesFormula = new HashSet<ComponentesFormula>();
            this.ComponentesCodigosBarras = new HashSet<ComponentesCodigosBarras>();
            this.ComponentesImagenes = new HashSet<ComponentesImagenes>();
            this.ComponentesEquivalenciasPartes = new HashSet<ComponentesEquivalenciasPartes>();
            this.ComponentesFormulaDetalles1 = new HashSet<ComponentesFormulaDetalles>();
            this.ComponentesImpuestos = new HashSet<ComponentesImpuestos>();
            this.InventariosESDetalles = new HashSet<InventariosESDetalles>();
            this.InventariosFisicosDetalles = new HashSet<InventariosFisicosDetalles>();
            this.LotesSeries = new HashSet<LotesSeries>();
            this.ExistenciasLotesSeries = new HashSet<ExistenciasLotesSeries>();
            this.InventariosSaldos = new HashSet<InventariosSaldos>();
            this.InventariosCostos = new HashSet<InventariosCostos>();
            this.PremiosMonederoDetalles = new HashSet<PremiosMonederoDetalles>();
            this.ResguardosDetalles = new HashSet<ResguardosDetalles>();
            this.ResguardosLotesSeries = new HashSet<ResguardosLotesSeries>();
            this.ComprasDocsDetalles = new HashSet<ComprasDocsDetalles>();
        }
    
        public long Id { get; set; }
        public long TipoComponenteId { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
        public long GrupoComponentesId { get; set; }
        public long SubgrupoComponentesId { get; set; }
        public string Activo { get; set; }
        public string AplicaIVA { get; set; }
        public string AplicaISR { get; set; }
        public string Inventariable { get; set; }
        public string TipoSeguimiento { get; set; }
        public Nullable<long> GrupoUnidadesId { get; set; }
        public Nullable<long> UnidadInventarioId { get; set; }
        public Nullable<long> UnidadVentaId { get; set; }
        public Nullable<long> UnidadCompraId { get; set; }
        public Nullable<double> Costo { get; set; }
        public Nullable<double> Rendimiento { get; set; }
        public Nullable<long> MarcaId { get; set; }
        public string NoParte { get; set; }
        public string Modelo { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual GruposComponentes GruposComponentes { get; set; }
        public virtual GruposUnidades GruposUnidades { get; set; }
        public virtual MarcasComponentes MarcasComponentes { get; set; }
        public virtual SubgruposComponentes SubgruposComponentes { get; set; }
        public virtual TiposComponentes TiposComponentes { get; set; }
        public virtual Unidades Unidades { get; set; }
        public virtual Unidades Unidades1 { get; set; }
        public virtual Unidades Unidades2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesAlmacenes> ComponentesAlmacenes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesFormulaDetalles> ComponentesFormulaDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesFormula> ComponentesFormula { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesCodigosBarras> ComponentesCodigosBarras { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesImagenes> ComponentesImagenes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesEquivalenciasPartes> ComponentesEquivalenciasPartes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesFormulaDetalles> ComponentesFormulaDetalles1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentesImpuestos> ComponentesImpuestos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosESDetalles> InventariosESDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosFisicosDetalles> InventariosFisicosDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LotesSeries> LotesSeries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExistenciasLotesSeries> ExistenciasLotesSeries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosSaldos> InventariosSaldos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventariosCostos> InventariosCostos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PremiosMonederoDetalles> PremiosMonederoDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResguardosDetalles> ResguardosDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResguardosLotesSeries> ResguardosLotesSeries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComprasDocsDetalles> ComprasDocsDetalles { get; set; }
    }
}
