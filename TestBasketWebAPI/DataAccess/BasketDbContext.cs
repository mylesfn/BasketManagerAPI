using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestBasketWebAPI.DataAccess
{
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<TrustedDomain> TrustedDomains { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Basket>().Property(p => p.BasketId).ValueGeneratedOnAdd();
            builder.Entity<Customer>().Property(p => p.CustomerId).ValueGeneratedOnAdd();
            builder.Entity<Product>().Property(p => p.ProductId).ValueGeneratedOnAdd();

            builder.Entity<Customer>().HasData(new Customer() { CustomerId = -1, FirstName = "Site", LastName = "Owner", Email = "site.owner@basketweb.api", SessionExpiry = DateTime.Now.AddYears(10) });
            builder.Entity<Basket>().HasData(new Basket() { BasketId = -1, CustomerId = -1, TotalPrice = 0 });

            //TrustedDomains can be used to accept and reject requests from different domains. If this DbSet contains no records then all requests will
            //be accepted, however if it contains entries then only requests coming from the TrustedDomain.Domain will be allowed through
            //builder.Entity<TrustedDomain>().HasData(new TrustedDomain() { Domain = "localhost", AcceptRequests = true });
        }

        object _lock = new object();
        public void FinishInitialising()
        {
            lock (_lock)
            {
                if (this.Customers.FirstOrDefault(x => x.CustomerId == -1) == null)
                    this.Database.EnsureCreated();
            }
        }

        public string Save()
        {
            try
            {
                base.SaveChangesAsync();
                return "Successfully saved changes.";
            }
            catch(Exception ex)
            {
                return "Unable to save changes to basket. Error: " + ex.Message;
            }
        }

        public Basket GetBasket(int customerId)
        {
            return this.Baskets.Include(x => x.Products).Where(x => x.CustomerId == customerId).FirstOrDefault();
        }

        public Customer GetCustomerBySessionId(string sessionId)
        {
            return this.Customers.Include("Basket.Products").Where(x => x.SessionId == sessionId).FirstOrDefault();
        }

        public Customer GetCustomerByCustomerId(int customerId)
        {
            return this.Customers.Include("Basket.Products").Where(x => x.CustomerId == customerId).FirstOrDefault();
        }
    }
}
