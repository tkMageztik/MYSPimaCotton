namespace NS.MYS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Catalogues",
                c => new
                    {
                        CatalogueId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CatalogueId);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        PhotoId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        Order = c.Int(nullable: false),
                        CatalogueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PhotoId)
                .ForeignKey("dbo.Catalogues", t => t.CatalogueId, cascadeDelete: true)
                .Index(t => t.CatalogueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "CatalogueId", "dbo.Catalogues");
            DropIndex("dbo.Photos", new[] { "CatalogueId" });
            DropTable("dbo.Photos");
            DropTable("dbo.Catalogues");
        }
    }
}
