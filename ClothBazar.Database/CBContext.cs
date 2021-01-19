using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClothBazar.Entities;

namespace ClothBazar.Database
{
    public class CBContext:DbContext
    {
        public CBContext() : base("ClothBazarConnection")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Write Fluent API configurations here

            //Property Configurations
            modelBuilder.Entity<Category>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
        }

        public DbSet<Product>Products { get; set; }
        public DbSet<Category>Categories { get; set; }
        public DbSet<Config> Configurations { get; set;}
    }
}
