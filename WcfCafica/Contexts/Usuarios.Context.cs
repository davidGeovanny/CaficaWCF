﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfCafica.Contexts
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UsuariosContext : DbContext
    {
        public UsuariosContext()
            : base(GetConnectionString(null))
        {
            this.Configuration.LazyLoadingEnabled = false;
    		this.Configuration.ProxyCreationEnabled = false;
        }
    	public UsuariosContext(string dbname) : base(GetConnectionString(dbname))
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        public static string GetConnectionString(string dbname)
        {
            PasswordEncrypt encriptacion = new PasswordEncrypt();
            ValidarToken Token = new ValidarToken();
        
            //Obtiene el nombre usuario desde el token de la conexion
            string user = Token.getKey("user","Token");
            if (user == null || user == "")
                user = "root";
        
            //Obtiene el password a la bd 
            string password = Token.getKey("password", "Token");
            if (password == null || password == "")
                password = "pwjr";
            else
                //Desencripta el password antes de conectarse a la bd
                password= encriptacion.DecryptText(password);
    
            //Verifica conexion a base de datos
            if (dbname==null)
                dbname = "Usuarios";
    
            // Server=localhost;Database={0};Uid=username;Pwd=password
            //var connString = ConfigurationManager.ConnectionStrings["UsuariosContext"].ConnectionString.ToString();
            //string connString = "metadata=res://*/Contexts.Usuarios.csdl|res://*/Contexts.Usuarios.ssdl|res://*/Contexts.Usuarios.msl;provider=MySql.Data.MySqlClient;provider connection string=\"server=10.10.1.100;user id={0};password={1};database=Usuarios\"";
            string connString = "metadata=res://*/Contexts.Usuarios.csdl|res://*/Contexts.Usuarios.ssdl|res://*/Contexts.Usuarios.msl;provider=MySql.Data.MySqlClient;provider connection string=\"server=10.10.1.100;user id={0};password={1};database={2}\"";
            return String.Format(connString, user, password,dbname);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BDEmpresas> BDEmpresas { get; set; }
        public virtual DbSet<BDEmpresasRoles> BDEmpresasRoles { get; set; }
        public virtual DbSet<GruposVistas> GruposVistas { get; set; }
        public virtual DbSet<Modulos> Modulos { get; set; }
        public virtual DbSet<RolesAcciones> RolesAcciones { get; set; }
        public virtual DbSet<SistemasRoles> SistemasRoles { get; set; }
        public virtual DbSet<t1> t1 { get; set; }
        public virtual DbSet<t2> t2 { get; set; }
        public virtual DbSet<t3> t3 { get; set; }
        public virtual DbSet<UsuariosRoles> UsuariosRoles { get; set; }
        public virtual DbSet<Sistemas> Sistemas { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<AccionesVistas> AccionesVistas { get; set; }
        public virtual DbSet<Vistas> Vistas { get; set; }
        public virtual DbSet<Tokens> Tokens { get; set; }
        public virtual DbSet<Tablas> Tablas { get; set; }
        public virtual DbSet<VistasTablas> VistasTablas { get; set; }
        public virtual DbSet<VistasGruposVistas> VistasGruposVistas { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
    }
}
