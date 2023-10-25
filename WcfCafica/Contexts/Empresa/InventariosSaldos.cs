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
    
    public partial class  InventariosSaldos
    {
        public long Id { get; set; }
        public long ComponenteId { get; set; }
        public long AlmacenId { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int UltimoDia { get; set; }
        public double EntradasUnidades { get; set; }
        public double SalidasUnidades { get; set; }
        public double EntradasCosto { get; set; }
        public double SalidasCosto { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual Almacenes Almacenes { get; set; }
        public virtual Componentes Componentes { get; set; }
    }
}
