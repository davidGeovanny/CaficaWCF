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
    
    public partial class  CodigosISOMonedas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CodigosISOMonedas()
        {
            this.Monedas = new HashSet<Monedas>();
        }
    
        public long Id { get; set; }
        public string Codigo { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Monedas> Monedas { get; set; }
    }
}
