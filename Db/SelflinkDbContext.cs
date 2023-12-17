using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Selflink_api.Db.Models;

namespace Selflink_api.Db

{
    public class SelflinkDbContext : DbContext
    {

        public DbSet<Link> Links { get; init; }

        public SelflinkDbContext(DbContextOptions<SelflinkDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Link>().ToCollection("links");
        }

    }
}