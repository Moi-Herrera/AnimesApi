using AnimesApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Data
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //rewview debe tener un user
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.UserId);
        }
    } 
}
