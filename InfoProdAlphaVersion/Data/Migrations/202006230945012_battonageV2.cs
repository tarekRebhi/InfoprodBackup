namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class battonageV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationBattonages", "nomProgramme", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "postIT", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "objects", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "notreClients", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "mairie", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationBattonages", "mairie");
            DropColumn("dbo.GrilleEvaluationBattonages", "notreClients");
            DropColumn("dbo.GrilleEvaluationBattonages", "objects");
            DropColumn("dbo.GrilleEvaluationBattonages", "postIT");
            DropColumn("dbo.GrilleEvaluationBattonages", "nomProgramme");
        }
    }
}
