namespace IntegrationSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intergration_init : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductsInPurchaseOrders", "ProductId", "dbo.Product");
            DropForeignKey("dbo.PurchaseOrders", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.ProductsInQuotations", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductsInQuotations", "QuotationId", "dbo.Quotations");
            DropForeignKey("dbo.Quotations", "SupplierId", "dbo.Retailers");
            DropForeignKey("dbo.PurchaseOrders", "SupplierId", "dbo.Retailers");
            DropForeignKey("dbo.ProductsInPurchaseOrders", "PurchaseOrderId", "dbo.PurchaseOrders");
            DropForeignKey("dbo.ProductsInRetailSales", "ProductId", "dbo.Product");
            DropForeignKey("dbo.RetailSales", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.RetailSales", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.ProductsInRetailSales", "RetailSaleId", "dbo.RetailSales");
            DropForeignKey("dbo.SpecificationInProduct", "Product_ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductsInRetailSaleReturns", "RetailSaleId", "dbo.RetailSales");
            DropForeignKey("dbo.WholesaleSales", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.ProductsInWholesaleSales", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductsInWholesaleSales", "WholesaleSaleId", "dbo.WholesaleSales");
            DropForeignKey("dbo.WholesaleSales", "RetailerId", "dbo.Retailers");
            DropForeignKey("dbo.ProductsInWholesaleSaleReturns", "WholesaleSaleId", "dbo.WholesaleSales");
            DropForeignKey("dbo.ProductInProductStockWasted", "ProductStockWasted_ProductStockWastedId", "dbo.ProductStockWasted");
            DropForeignKey("dbo.ProductInProductStocks", "ProductStocks_ProductStocksId", "dbo.ProductStocks");
            DropForeignKey("dbo.ProductStocks", "SupplierId", "dbo.Retailers");
            DropIndex("dbo.ProductInProductStockWasted", new[] { "ProductStockWasted_ProductStockWastedId" });
            DropIndex("dbo.ProductsInPurchaseOrders", new[] { "PurchaseOrderId" });
            DropIndex("dbo.ProductsInPurchaseOrders", new[] { "ProductId" });
            DropIndex("dbo.PurchaseOrders", new[] { "SupplierId" });
            DropIndex("dbo.PurchaseOrders", new[] { "AccountId" });
            DropIndex("dbo.Quotations", new[] { "SupplierId" });
            DropIndex("dbo.ProductsInQuotations", new[] { "ProductId" });
            DropIndex("dbo.ProductsInQuotations", new[] { "QuotationId" });
            DropIndex("dbo.ProductsInRetailSales", new[] { "RetailSaleId" });
            DropIndex("dbo.ProductsInRetailSales", new[] { "ProductId" });
            DropIndex("dbo.RetailSales", new[] { "CustomerId" });
            DropIndex("dbo.RetailSales", new[] { "AccountId" });
            DropIndex("dbo.SpecificationInProduct", new[] { "Product_ProductId" });
            DropIndex("dbo.ProductInProductStocks", new[] { "ProductStocks_ProductStocksId" });
            DropIndex("dbo.ProductsInRetailSaleReturns", new[] { "RetailSaleId" });
            DropIndex("dbo.ProductsInWholesaleSaleReturns", new[] { "WholesaleSaleId" });
            DropIndex("dbo.WholesaleSales", new[] { "RetailerId" });
            DropIndex("dbo.WholesaleSales", new[] { "AccountId" });
            DropIndex("dbo.ProductsInWholesaleSales", new[] { "WholesaleSaleId" });
            DropIndex("dbo.ProductsInWholesaleSales", new[] { "ProductId" });
            DropIndex("dbo.ProductStocks", new[] { "SupplierId" });
            DropTable("dbo.Customers");
            DropTable("dbo.ProductInProductStockWasted");
            DropTable("dbo.Product");
            DropTable("dbo.ProductsInPurchaseOrders");
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.Retailers");
            DropTable("dbo.Quotations");
            DropTable("dbo.ProductsInQuotations");
            DropTable("dbo.ProductsInRetailSales");
            DropTable("dbo.RetailSales");
            DropTable("dbo.SpecificationInProduct");
            DropTable("dbo.ProductInProductStocks");
            DropTable("dbo.ProductsInRetailSaleReturns");
            DropTable("dbo.ProductsInWholesaleSaleReturns");
            DropTable("dbo.WholesaleSales");
            DropTable("dbo.ProductsInWholesaleSales");
            DropTable("dbo.ProductStockWasted");
            DropTable("dbo.ProductStocks");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductStocks",
                c => new
                    {
                        ProductStocksId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(),
                        RecievedDate = c.DateTime(),
                        ApplicationUserId = c.String(),
                        SupplierId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProductStocksId);
            
            CreateTable(
                "dbo.ProductStockWasted",
                c => new
                    {
                        ProductStockWastedId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        ApplicationUserId = c.String(),
                    })
                .PrimaryKey(t => t.ProductStockWastedId);
            
            CreateTable(
                "dbo.ProductsInWholesaleSales",
                c => new
                    {
                        ProductInWholesaleSaleId = c.Int(nullable: false, identity: true),
                        UnitPrice = c.Single(nullable: false),
                        WholesaleSaleId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Single(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProductInWholesaleSaleId);
            
            CreateTable(
                "dbo.WholesaleSales",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        RetailerId = c.String(maxLength: 128),
                        OrderDate = c.DateTime(),
                        OrderDueDate = c.DateTime(),
                        OrderStatus = c.String(),
                        DeliveredDate = c.DateTime(),
                        DeliveryStatus = c.String(),
                        PaymentMethod = c.String(),
                        PaymentDuration = c.Int(nullable: false),
                        DeliveryMode = c.String(),
                        Remark = c.String(),
                        AccountId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.ProductsInWholesaleSaleReturns",
                c => new
                    {
                        ProductInWholesaleSaleReturnId = c.Int(nullable: false, identity: true),
                        WholesaleSaleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductInWholesaleSaleReturnId);
            
            CreateTable(
                "dbo.ProductsInRetailSaleReturns",
                c => new
                    {
                        ProductInRetailSaleReturnId = c.Int(nullable: false, identity: true),
                        RetailSaleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductInRetailSaleReturnId);
            
            CreateTable(
                "dbo.ProductInProductStocks",
                c => new
                    {
                        ProductInProductStocksId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ProductStocksId = c.Int(nullable: false),
                        QuantityRecieved = c.Int(),
                        QuantityDispatched = c.Int(),
                        Cost = c.Single(),
                        Remarks = c.String(),
                        ProductStocks_ProductStocksId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductInProductStocksId);
            
            CreateTable(
                "dbo.SpecificationInProduct",
                c => new
                    {
                        SpecificationInProductId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Specification = c.String(),
                        Value = c.String(),
                        Product_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.SpecificationInProductId);
            
            CreateTable(
                "dbo.RetailSales",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        OrderDate = c.DateTime(),
                        OrderDueDate = c.DateTime(),
                        OrderStatus = c.String(),
                        DeliveredDate = c.DateTime(),
                        DeliveryStatus = c.String(),
                        PaymentMethod = c.String(),
                        PaymentDuration = c.Int(nullable: false),
                        DeliveryMode = c.String(),
                        Remark = c.String(),
                        AccountId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.ProductsInRetailSales",
                c => new
                    {
                        ProductInRetailSaleId = c.Int(nullable: false, identity: true),
                        UnitPrice = c.Single(nullable: false),
                        RetailSaleId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Single(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProductInRetailSaleId);
            
            CreateTable(
                "dbo.ProductsInQuotations",
                c => new
                    {
                        ProductInQuotationId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        QuotationId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ProductInQuotationId);
            
            CreateTable(
                "dbo.Quotations",
                c => new
                    {
                        QuotationId = c.Int(nullable: false, identity: true),
                        SupplierId = c.String(maxLength: 128),
                        Status = c.String(),
                        PaymentMethod = c.String(),
                        PaymentDuration = c.Int(nullable: false),
                        DeliveryMethod = c.String(),
                    })
                .PrimaryKey(t => t.QuotationId);
            
            CreateTable(
                "dbo.Retailers",
                c => new
                    {
                        RetailerId = c.String(nullable: false, maxLength: 128),
                        RetailerName = c.String(),
                        RetailerAddress = c.String(),
                        Rating = c.Single(),
                        BusinessPhoneNumber = c.String(),
                        Status = c.String(),
                        BRCNumber = c.String(),
                        Category = c.String(),
                        Currency = c.String(),
                        Country = c.String(),
                        Region = c.String(),
                    })
                .PrimaryKey(t => t.RetailerId);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        SupplierId = c.String(maxLength: 128),
                        OrderDate = c.DateTime(),
                        OrderDueDate = c.DateTime(),
                        OrderStatus = c.String(),
                        DeliveredDate = c.DateTime(),
                        DeliveryStatus = c.String(),
                        PaymentMethod = c.String(),
                        PaymentDuration = c.Int(nullable: false),
                        DeliveryMode = c.String(),
                        Remark = c.String(),
                        AccountId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.ProductsInPurchaseOrders",
                c => new
                    {
                        ProductInPurchaseOrderId = c.Int(nullable: false, identity: true),
                        Cost = c.Single(nullable: false),
                        PurchaseOrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Single(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProductInPurchaseOrderId);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        Unit = c.String(),
                        RetailPrice = c.Single(),
                        WholesalePrice = c.Single(),
                        StocksQuantity = c.Single(),
                        ShortDescription = c.String(),
                        LongDescription = c.String(),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductInProductStockWasted",
                c => new
                    {
                        ProductInProductStockWastedId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ProductStockWastedId = c.Int(nullable: false),
                        Quantity = c.Int(),
                        Remarks = c.String(),
                        ProductStockWasted_ProductStockWastedId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductInProductStockWastedId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        City = c.String(),
                        Status = c.String(),
                        BillingAddress = c.String(),
                        Remark = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateIndex("dbo.ProductStocks", "SupplierId");
            CreateIndex("dbo.ProductsInWholesaleSales", "ProductId");
            CreateIndex("dbo.ProductsInWholesaleSales", "WholesaleSaleId");
            CreateIndex("dbo.WholesaleSales", "AccountId");
            CreateIndex("dbo.WholesaleSales", "RetailerId");
            CreateIndex("dbo.ProductsInWholesaleSaleReturns", "WholesaleSaleId");
            CreateIndex("dbo.ProductsInRetailSaleReturns", "RetailSaleId");
            CreateIndex("dbo.ProductInProductStocks", "ProductStocks_ProductStocksId");
            CreateIndex("dbo.SpecificationInProduct", "Product_ProductId");
            CreateIndex("dbo.RetailSales", "AccountId");
            CreateIndex("dbo.RetailSales", "CustomerId");
            CreateIndex("dbo.ProductsInRetailSales", "ProductId");
            CreateIndex("dbo.ProductsInRetailSales", "RetailSaleId");
            CreateIndex("dbo.ProductsInQuotations", "QuotationId");
            CreateIndex("dbo.ProductsInQuotations", "ProductId");
            CreateIndex("dbo.Quotations", "SupplierId");
            CreateIndex("dbo.PurchaseOrders", "AccountId");
            CreateIndex("dbo.PurchaseOrders", "SupplierId");
            CreateIndex("dbo.ProductsInPurchaseOrders", "ProductId");
            CreateIndex("dbo.ProductsInPurchaseOrders", "PurchaseOrderId");
            CreateIndex("dbo.ProductInProductStockWasted", "ProductStockWasted_ProductStockWastedId");
            AddForeignKey("dbo.ProductStocks", "SupplierId", "dbo.Retailers", "RetailerId");
            AddForeignKey("dbo.ProductInProductStocks", "ProductStocks_ProductStocksId", "dbo.ProductStocks", "ProductStocksId");
            AddForeignKey("dbo.ProductInProductStockWasted", "ProductStockWasted_ProductStockWastedId", "dbo.ProductStockWasted", "ProductStockWastedId");
            AddForeignKey("dbo.ProductsInWholesaleSaleReturns", "WholesaleSaleId", "dbo.WholesaleSales", "OrderId");
            AddForeignKey("dbo.WholesaleSales", "RetailerId", "dbo.Retailers", "RetailerId");
            AddForeignKey("dbo.ProductsInWholesaleSales", "WholesaleSaleId", "dbo.WholesaleSales", "OrderId");
            AddForeignKey("dbo.ProductsInWholesaleSales", "ProductId", "dbo.Product", "ProductId");
            AddForeignKey("dbo.WholesaleSales", "AccountId", "dbo.Accounts", "Id");
            AddForeignKey("dbo.ProductsInRetailSaleReturns", "RetailSaleId", "dbo.RetailSales", "OrderId");
            AddForeignKey("dbo.SpecificationInProduct", "Product_ProductId", "dbo.Product", "ProductId");
            AddForeignKey("dbo.ProductsInRetailSales", "RetailSaleId", "dbo.RetailSales", "OrderId");
            AddForeignKey("dbo.RetailSales", "CustomerId", "dbo.Customers", "CustomerId");
            AddForeignKey("dbo.RetailSales", "AccountId", "dbo.Accounts", "Id");
            AddForeignKey("dbo.ProductsInRetailSales", "ProductId", "dbo.Product", "ProductId");
            AddForeignKey("dbo.ProductsInPurchaseOrders", "PurchaseOrderId", "dbo.PurchaseOrders", "OrderId");
            AddForeignKey("dbo.PurchaseOrders", "SupplierId", "dbo.Retailers", "RetailerId");
            AddForeignKey("dbo.Quotations", "SupplierId", "dbo.Retailers", "RetailerId");
            AddForeignKey("dbo.ProductsInQuotations", "QuotationId", "dbo.Quotations", "QuotationId");
            AddForeignKey("dbo.ProductsInQuotations", "ProductId", "dbo.Product", "ProductId");
            AddForeignKey("dbo.PurchaseOrders", "AccountId", "dbo.Accounts", "Id");
            AddForeignKey("dbo.ProductsInPurchaseOrders", "ProductId", "dbo.Product", "ProductId");
        }
    }
}
