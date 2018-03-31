using NS.MYS.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NS.MYS.Web.Data
{
    public class NSMYSWebContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public NSMYSWebContext() : base("name=NSMYSWebContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // configures one-to-many relationship
            modelBuilder.Entity<Photo>()
                .HasRequired<Catalogue>(s => s.Catalogue)
                .WithMany(g => g.Photos)
                .HasForeignKey<int>(s => s.CatalogueId);


            modelBuilder.Entity<Catalogue>()
                .HasMany<Photo>(g => g.Photos)
                .WithRequired(s => s.Catalogue)
                .WillCascadeOnDelete();
        }


        public DbSet<Catalogue> Catalogues { get; set; }
        public DbSet<Photo> Photos { get; set; }
        
    }

}
