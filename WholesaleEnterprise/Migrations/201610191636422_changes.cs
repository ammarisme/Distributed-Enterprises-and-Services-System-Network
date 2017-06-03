namespace WholesaleEnterprise.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductsInWholesaleSaleReturns",
                c => new
                    {
                        ProductInWholesaleSaleReturnId = c.Int(nullable: false, identity: true),
                        WholesaleSaleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductInWholesaleSaleReturnId)
                .ForeignKey("dbo.WholesaleSales", t => t.WholesaleSaleId)
                .Index(t => t.WholesaleSaleId);
            
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
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.Retailers", t => t.RetailerId)
                .Index(t => t.RetailerId)
                .Index(t => t.AccountId);
            
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
                .PrimaryKey(t => t.ProductInWholesaleSaleId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.WholesaleSales", t => t.WholesaleSaleId)
                .Index(t => t.WholesaleSaleId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductsInWholesaleSaleReturns", "WholesaleSaleId", "dbo.WholesaleSales");
            DropForeignKey("dbo.WholesaleSales", "RetailerId", "dbo.Retailers");
            DropForeignKey("dbo.ProductsInWholesaleSales", "WholesaleSaleId", "dbo.WholesaleSales");
            DropForeignKey("dbo.ProductsInWholesaleSales", "ProductId", "dbo.Product");
            DropForeignKey("dbo.WholesaleSales", "AccountId", "dbo.Accounts");
            DropIndex("dbo.ProductsInWholesaleSales", new[] { "ProductId" });
            DropIndex("dbo.ProductsInWholesaleSales", new[] { "WholesaleSaleId" });
            DropIndex("dbo.WholesaleSales", new[] { "AccountId" });
            DropIndex("dbo.WholesaleSales", new[] { "RetailerId" });
            DropIndex("dbo.ProductsInWholesaleSaleReturns", new[] { "WholesaleSaleId" });
            DropTable("dbo.ProductsInWholesaleSales");
            DropTable("dbo.WholesaleSales");
            DropTable("dbo.ProductsInWholesaleSaleReturns");
        }
    }
}
