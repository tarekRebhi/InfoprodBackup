namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ajout2itemgrillesFOetFOSAMRC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationFOes", "FCR", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationFOSAMRCs", "MajDonnees", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationFOSAMRCs", "MajDonnees");
            DropColumn("dbo.GrilleEvaluationFOes", "FCR");
        }
    }
}
