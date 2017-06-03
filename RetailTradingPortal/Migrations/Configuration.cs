namespace RetailTradingPortal.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RetailTradingPortal.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RetailTradingPortal.DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(RetailTradingPortal.DAL.ApplicationDbContext db)
        {
            if (db.Products.Count() < 3)
            {
            Product product1 = new Product { ProductId=1, Unit="unit",StocksQuantity=100,ShortDescription="A short description", ProductName="Table", LongDescription="A longer description", Price=2000};
            Product product2 = new Product { ProductId = 2, Unit = "unit", StocksQuantity = 100, ShortDescription = "A short description", ProductName = "Laptop", LongDescription = "A longer description", Price = 2000 };
            Product product3 = new Product { ProductId = 3, Unit = "unit", StocksQuantity = 100, ShortDescription = "A short description", ProductName = "Anything", LongDescription = "A longer description", Price = 2000 };

            db.Products.Add(product1);
            db.Products.Add(product2);
            db.Products.Add(product3);
            }

            if (db.Enterprises.Count() < 3)
            {
                Enterprise vendor1 = new Enterprise { EnterpriseId = 1, EnterpriseName = "Default" };
                Enterprise vendor2 = new Enterprise { EnterpriseId = 2, EnterpriseName = "Supplier 3" };
                Enterprise vendor3 = new Enterprise { EnterpriseId = 3, EnterpriseName = "Supplier 3" };

                db.Enterprises.Add(vendor1);
                db.Enterprises.Add(vendor2);
                db.Enterprises.Add(vendor3);
            }

            //if (db.Customers.Count() < 3)
            //{
            //    Customer customer1 = new Customer {CustomerId=1, FirstName="A" ,LastName="D"};
            //    Customer customer2 = new Customer { CustomerId = 2, FirstName = "B", LastName = "E" };
            //    Customer customer3 = new Customer { CustomerId = 3, FirstName = "C", LastName = "F" };

            //    db.Customers.Add(customer1);
            //    db.Customers.Add(customer2);
            //    db.Customers.Add(customer3);
            //}

            //if (db.PurchaseOrders.Count() < 2)
            //{
            //    PurchaseOrder po1 = new PurchaseOrder { OrderId = 1, SupplierId = "1" };
            //    PurchaseOrder po2 = new PurchaseOrder { OrderId = 2, SupplierId = "2" };

            //    ProductInPurchaseOrder product1 = new ProductInPurchaseOrder { ProductInPurchaseOrderId = 1, Quantity = 10, Cost = 10, ProductId = 1, PurchaseOrderId = 1, };
            //    ProductInPurchaseOrder product2 = new ProductInPurchaseOrder { ProductInPurchaseOrderId = 2, Quantity = 11, Cost = 10, ProductId = 1, PurchaseOrderId = 1, };
            //    ProductInPurchaseOrder product3 = new ProductInPurchaseOrder { ProductInPurchaseOrderId = 3, Quantity = 13, Cost = 10, ProductId = 1, PurchaseOrderId = 2, };
            //    ProductInPurchaseOrder product4 = new ProductInPurchaseOrder { ProductInPurchaseOrderId = 4, Quantity = 1, Cost = 10, ProductId = 1, PurchaseOrderId = 2, };


            //    db.PurchaseOrders.Add(po1);
            //    db.PurchaseOrders.Add(po2);
            //    db.ProductsInPurchaseOrders.Add(product1);
            //    db.ProductsInPurchaseOrders.Add(product2);
            //    db.ProductsInPurchaseOrders.Add(product3);
            //    db.ProductsInPurchaseOrders.Add(product4);

            //}

            db.SaveChanges();
        }
    }
}
