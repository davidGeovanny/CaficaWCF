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
    
    public partial class  ComprasDocsRelaciones
    {
        public long Id { get; set; }
        public long ComprasDocsOrigenId { get; set; }
        public long ComprasDocsDestinoId { get; set; }
        public string UsuarioCreo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModifico { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
    
        public virtual ComprasDocs ComprasDocs { get; set; }
        public virtual ComprasDocs ComprasDocs1 { get; set; }
    }
}
