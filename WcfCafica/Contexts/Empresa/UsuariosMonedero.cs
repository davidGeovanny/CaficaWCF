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
    
    public partial class  UsuariosMonedero
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsuariosMonedero()
        {
            this.SaldosMonedero = new HashSet<SaldosMonedero>();
            this.SolicitudesCanjeMonedero = new HashSet<SolicitudesCanjeMonedero>();
            this.SolicitudesCanjeMonederoDetalles = new HashSet<SolicitudesCanjeMonederoDetalles>();
            this.MovimientosMonedero = new HashSet<MovimientosMonedero>();
        }
    
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Calle { get; set; }
        public string NoExterior { get; set; }
        public string NoInterior { get; set; }
        public Nullable<long> ColoniaId { get; set; }
        public Nullable<long> CodigoPostalId { get; set; }
        public Nullable<long> CiudadId { get; set; }
        public Nullable<long> MunicipioId { get; set; }
        public Nullable<long> EstadoId { get; set; }
        public Nullable<long> PaisId { get; set; }
        public string Sexo { get; set; }
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        public string Activo { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public string RegistroCompletado { get; set; }
    
        public virtual Ciudades Ciudades { get; set; }
        public virtual CodigosPostales CodigosPostales { get; set; }
        public virtual Colonias Colonias { get; set; }
        public virtual Estados Estados { get; set; }
        public virtual Municipios Municipios { get; set; }
        public virtual Paises Paises { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaldosMonedero> SaldosMonedero { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudesCanjeMonedero> SolicitudesCanjeMonedero { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudesCanjeMonederoDetalles> SolicitudesCanjeMonederoDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MovimientosMonedero> MovimientosMonedero { get; set; }
    }
}
