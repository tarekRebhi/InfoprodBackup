namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SCGISI : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationBOes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        identificationClient = c.Single(nullable: false),
                        qualificationDemande = c.Single(nullable: false),
                        respectProcess = c.Single(nullable: false),
                        pertinenceReponse = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        approcheCommerciale = c.Single(nullable: false),
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
            
            CreateTable(
                "dbo.GrilleEvaluationFOes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        accueil = c.Single(nullable: false),
                        analyseDemande = c.Single(nullable: false),
                        maitriseOutils = c.Single(nullable: false),
                        connaissanceProduit = c.Single(nullable: false),
                        respectProcess = c.Single(nullable: false),
                        pertinenceReponse = c.Single(nullable: false),
                        autonomie = c.Single(nullable: false),
                        approcheCommerciale = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        priseConge = c.Single(nullable: false),
                        identificationClient = c.Single(nullable: false),
                        qualificationDemande = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationFOes", "employeeId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GrilleEvaluationBOes", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationFOes", new[] { "employeeId" });
            DropIndex("dbo.GrilleEvaluationBOes", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationFOes");
            DropTable("dbo.GrilleEvaluationBOes");
        }
    }
}
