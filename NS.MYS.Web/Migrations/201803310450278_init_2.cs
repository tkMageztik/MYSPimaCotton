namespace NS.MYS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Catalogues", "Observation", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Catalogues", "Description", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Catalogues", "Description", c => c.String());
            DropColumn("dbo.Catalogues", "Observation");
        }
    }
}
