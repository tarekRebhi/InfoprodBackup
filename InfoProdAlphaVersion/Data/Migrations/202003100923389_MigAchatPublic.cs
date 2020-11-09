namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigAchatPublic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationAchatPublics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        accueil = c.Single(nullable: false),
                        identificatioClient = c.Single(nullable: false),
                        decouverteDemande = c.Single(nullable: false),
                        maitrisePlateforme = c.Single(nullable: false),
                        miseAttente = c.Single(nullable: false),
                        tempsAttente = c.Single(nullable: false),
                        qualiteService = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        conclusionContact = c.Single(nullable: false),
                        attitude = c.Single(nullable: false),
                        qualification = c.Single(nullable: false),
                        priseConge = c.Single(nullable: false),
                        dateTempEvaluation = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        type = c.String(),
                        employeeId = c.Int(),
                        note = c.Single(nullable: false),
                        commentaireQualite = c.String(),
                        commentaireAgent = c.String(),
                        enregistrementFullName = c.String(),
                        enregistrementUrl = c.String(),
                        enregistrementDirectory = c.String(),
                        senderId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.employeeId)
                .Index(t => t.employeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrilleEvaluationAchatPublics", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationAchatPublics", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationAchatPublics");
        }
    }
}
