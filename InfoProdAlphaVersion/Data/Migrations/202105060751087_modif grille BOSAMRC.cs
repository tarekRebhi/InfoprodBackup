namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifgrilleBOSAMRC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationBOSAMRCs", "Historisation", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBOSAMRCs", "FCR", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationBOSAMRCs", "FCR");
            DropColumn("dbo.GrilleEvaluationBOSAMRCs", "Historisation");
        }
    }
}
