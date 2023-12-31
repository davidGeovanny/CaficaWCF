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
    
    public partial class  ResguardosLotesSeries
    {
        public long Id { get; set; }
        public long ResguardosDetalleId { get; set; }
        public Nullable<long> ComponenteId { get; set; }
        public Nullable<long> ResponsableId { get; set; }
        public Nullable<long> ResguardoLotesSeriesOrigenId { get; set; }
        public string TipoDocumento { get; set; }
        public Nullable<long> LotesSeriesId { get; set; }
        public string EstadoDevolucion { get; set; }
        public string Cancelado { get; set; }
        public Nullable<long> ResguardoId { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual Componentes Componentes { get; set; }
        public virtual LotesSeries LotesSeries { get; set; }
        public virtual Resguardos Resguardos { get; set; }
        public virtual ResguardosDetalles ResguardosDetalles { get; set; }
        public virtual Responsables Responsables { get; set; }
    }
}
