namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ipdtest : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Details_Activite_IPD", "INDICE", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Details_Activite_IPD", "INDICE", c => c.Int());
        }
    }
}
