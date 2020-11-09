namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesBOSCGISIMig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationBOes", "numIntervention", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationBOes", "numIntervention");
        }
    }
}
