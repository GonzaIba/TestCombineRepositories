using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ApiFP.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Rubro> Rubros { get; set; }
        public DbSet<Archivo> Archivos { get; set; }
        public DbSet<EstadoFactura> EstadoFactura { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Property(p => p.FirstName).IsRequired().HasColumnType("varchar").HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(p => p.LastName).IsRequired().HasColumnType("varchar").HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Cuit).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(p => p.BusinessName).IsRequired().HasColumnType("varchar").HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Profile).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Category).IsRequired().HasColumnType("varchar").HasMaxLength(50);            

            modelBuilder.Entity<Rubro>().ToTable("Rubros");
            modelBuilder.Entity<Archivo>().ToTable("Archivos");            
        }

    }
}