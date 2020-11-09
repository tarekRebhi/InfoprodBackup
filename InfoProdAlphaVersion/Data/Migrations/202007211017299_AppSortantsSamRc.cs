namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppSortantsSamRc : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Appels_Sortants_SamRc", "ConvDuration", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "Closed", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "NoAgent", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "Overflow", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "Abandon", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "LastAgent", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "semaine", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "Annee", c => c.Int(nullable: false));
            AlterColumn("dbo.Appels_Sortants_SamRc", "Heure", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Appels_Sortants_SamRc", "Heure", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "Annee", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "semaine", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "LastAgent", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "Abandon", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "Overflow", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "NoAgent", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "Closed", c => c.Int());
            AlterColumn("dbo.Appels_Sortants_SamRc", "ConvDuration", c => c.Int());
        }
    }
}
