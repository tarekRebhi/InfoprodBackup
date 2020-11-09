namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QREvalChangesMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationQRs", "mailContact", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationQRs", "mailContact");
        }
    }
}
