using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using RetailEnterprise.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RetailEnterprise.DAL
    /*This is the Data Access Layer
     * All the classes that does the communication between the Database and 
     * Programs will be coming within this namespace.
     */
{
    /*This is the class that is used to access the database. 
     * Instantiate this object and access variables as accessing database tables.
     * 
     * Database Name - RetailEnterprise
     * Database Service - SQLExpress
     * Intergrated Security  - True
     */
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Data Source=.\\SQLEXPRESS;Initial Catalog=RetailEnterprise1;Integrated Security=True", throwIfV1Schema: false)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //  Removing Entity Framework default conventions
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PrimaryKeyNameForeignKeyDiscoveryConvention>();

            // Configuring Identity framework tables
            modelBuilder.Entity<ApplicationUser>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(e => e.UserId);
            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(e => e.UserId);
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /* Following are Database tables. Database will be generated based on this.  
         * Instantiating this class will allow accessing each of below properties. */


        //These tables store System access account information.
        public DbSet<Account> Accounts { get; set; }
        
        // These tables stores Customer and Supplier resources information
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        
        //These tables store Product and related information
        public DbSet<Product> Products { get; set; }
        public DbSet<SpecificationInProduct> SpecificationInProduct { get; set; }
        
        // These tables store Product Stocks and Products in each Stocks information.
        public DbSet<ProductStocks> Stocks { get; set; }
        public DbSet<ProductInProductStocks> ProductsInProductStocks { get; set; }
        // stock wasted tables
        public DbSet<ProductStockWasted> ProductStockWasteds { get; set; }
        public DbSet<ProductInProductStockWasted> ProductInProductStockWasteds { get; set; }



        // These tables store "Sales" Order information
        public DbSet<Sale> Sales { get; set; }
        public DbSet<ProductInSale> ProductsInSales { get; set; }
        public DbSet<ProductInSaleReturn> ProductsInSaleReturns { get; set; }

        // These tables store "Purchase" Order information.
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<ProductInPurchaseOrder> ProductsInPurchaseOrders { get; set; }

        // These tables store Quotation Request and Recieved Quotation information
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<ProductInQuotation> ProductsInQuotations { get; set; }

        public DbSet<Setting> Settings { get; set; }
    }
}