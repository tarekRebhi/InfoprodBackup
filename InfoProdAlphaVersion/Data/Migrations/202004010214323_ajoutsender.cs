namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajoutsender : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventaires", "SenderInv", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inventaires", "SenderInv");
        }
    }
}
