namespace RetailEnterprise.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class retail_enterprise : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        EnterpriseId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ReportHead = c.String(),
                        Logo = c.String(),
                    })
                .PrimaryKey(t => t.EnterpriseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Settings");
        }
    }
}
