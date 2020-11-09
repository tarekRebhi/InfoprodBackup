namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempsSamRcMig : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Temps_SamRc", "mois", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Temps_SamRc", "mois", c => c.Int(nullable: false));
        }
    }
}
