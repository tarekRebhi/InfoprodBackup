namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSamRcFO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationFOSAMRCs", "Identification", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationFOSAMRCs", "FCRSAMRC", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationFOSAMRCs", "FCRSAMRC");
            DropColumn("dbo.GrilleEvaluationFOSAMRCs", "Identification");
        }
    }
}
