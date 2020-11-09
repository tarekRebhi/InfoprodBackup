namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAMRCMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationBOSAMRCs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        identificationClient = c.Single(nullable: false),
                        qualificationDemandes = c.Single(nullable: false),
                        pertinenceReponse = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        dateTempEvaluation = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        numIntervention = c.String(),
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
                "dbo.GrilleEvaluationFOSAMRCs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        accueil = c.Single(nullable: false),
                        decouverteAttentes = c.Single(nullable: false),
                        utilisationOutils = c.Single(nullable: false),
                        miseAttente = c.Single(nullable: false),
                        tempsAttente = c.Single(nullable: false),
                        pertinenceReponse = c.Single(nullable: false),
                        conclusionContact = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        attitude = c.Single(nullable: false),
                        historisation = c.Single(nullable: false),
                        priseConge = c.Single(nullable: false),
                        dateTempEvaluation = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        numIntervention = c.String(),
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
            DropForeignKey("dbo.GrilleEvaluationFOSAMRCs", "employeeId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GrilleEvaluationBOSAMRCs", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationFOSAMRCs", new[] { "employeeId" });
            DropIndex("dbo.GrilleEvaluationBOSAMRCs", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationFOSAMRCs");
            DropTable("dbo.GrilleEvaluationBOSAMRCs");
        }
    }
}
