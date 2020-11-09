namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vecteurplusMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationBPPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        presentationIdentification = c.Single(nullable: false),
                        objetAppel = c.Single(nullable: false),
                        validationNature = c.Single(nullable: false),
                        calendrier = c.Single(nullable: false),
                        envergure = c.Single(nullable: false),
                        complétudeQuestions = c.Single(nullable: false),
                        confirmationIntervenants = c.Single(nullable: false),
                        enchainementQuestions = c.Single(nullable: false),
                        traitementObjections = c.Single(nullable: false),
                        reformulation = c.Single(nullable: false),
                        transcriptionsInformations = c.Single(nullable: false),
                        codesAppropriés = c.Single(nullable: false),
                        noteClient = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        attitude = c.Single(nullable: false),
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
            
            CreateTable(
                "dbo.GrilleEvaluationKLMOes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        presentationIdentification = c.Single(nullable: false),
                        objetAppel = c.Single(nullable: false),
                        mutualisation = c.Single(nullable: false),
                        maitriseOeuvre = c.Single(nullable: false),
                        coordonneesAttr = c.Single(nullable: false),
                        avisAttr = c.Single(nullable: false),
                        traitementObjections = c.Single(nullable: false),
                        transcriptionsInformations = c.Single(nullable: false),
                        envoiMail = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        attitude = c.Single(nullable: false),
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
            
            CreateTable(
                "dbo.GrilleEvaluationQRs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        presentationIdentification = c.Single(nullable: false),
                        objetAppel = c.Single(nullable: false),
                        mutualisation = c.Single(nullable: false),
                        marcheRenouvlable = c.Single(nullable: false),
                        dureeMarche = c.Single(nullable: false),
                        coordonneesAttr = c.Single(nullable: false),
                        montantMarche = c.Single(nullable: false),
                        responsableMarche = c.Single(nullable: false),
                        traitementObjections = c.Single(nullable: false),
                        transcriptionsInformations = c.Single(nullable: false),
                        envoiMail = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        attitude = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationQRs", "employeeId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GrilleEvaluationKLMOes", "employeeId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GrilleEvaluationBPPs", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationQRs", new[] { "employeeId" });
            DropIndex("dbo.GrilleEvaluationKLMOes", new[] { "employeeId" });
            DropIndex("dbo.GrilleEvaluationBPPs", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationQRs");
            DropTable("dbo.GrilleEvaluationKLMOes");
            DropTable("dbo.GrilleEvaluationBPPs");
        }
    }
}
