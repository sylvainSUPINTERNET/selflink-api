using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Selflink_api.Db.Models;



namespace Selflink_api.Db

{

    [Obsolete("Use MongoDbClientSingleton instead ( driver native mongoDB, no EF too early for this for the moment !)")]
    public class SelflinkDbContext : DbContext
    {

        public DbSet<Link> Links { get; init; }

        public DbSet<Order> Orders { get; init; }

        public SelflinkDbContext(DbContextOptions<SelflinkDbContext> options) : base(options)
        {
        }


        /** RUN MIGRATION  FOR THIS TO WORK **/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Link>().ToCollection("links");
            modelBuilder.Entity<Order>().ToCollection("orders");
            

            // TODO definir des indexe
            //modelBuilder.Entity<Link>().HasIndex(x => x.ShortUrl).IsUnique();


            // fixtures
            // TODO migration seems not to work with mongodb EF
            // for ( int i = 0; i < 100; i++ ) {
            //     modelBuilder.Entity<Link>().HasData(
            //         new Link
            //         {
            //             Id = ObjectId.GenerateNewId(),
            //             Name = "test" + i,
            //             GoogleOAuth2Sub = "sub1234",
            //             Iban = "FR123456789",
            //             PaymentUrl = "https://www.google.com",
            //             StripeProductId = "prod1234",
            //             StripeLinkId = "link1234",
            //             StripePriceId = "price1234"
            //         }
            //     );
            // }

            

        }



    }
}