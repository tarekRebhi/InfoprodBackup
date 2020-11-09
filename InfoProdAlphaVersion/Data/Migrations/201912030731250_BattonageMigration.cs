namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BattonageMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationBattonages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nature = c.Single(nullable: false),
                        typologie = c.Single(nullable: false),
                        avancement = c.Single(nullable: false),
                        destination = c.Single(nullable: false),
                        codeRP = c.Single(nullable: false),
                        moa = c.Single(nullable: false),
                        moaArchitectes = c.Single(nullable: false),
                        bureauxEtudes = c.Single(nullable: false),
                        bailleurSociaux = c.Single(nullable: false),
                        dateAccords = c.Single(nullable: false),
                        datePC = c.Single(nullable: false),
                        dateConsultation = c.Single(nullable: false),
                        dateTravaux = c.Single(nullable: false),
                        dateLivraison = c.Single(nullable: false),
                        surfacePlancher = c.Single(nullable: false),
                        surfaceTerrain = c.Single(nullable: false),
                        surfaceTypologie = c.Single(nullable: false),
                        nombreBatiment = c.Single(nullable: false),
                        nombreEtage = c.Single(nullable: false),
                        ascenseur = c.Single(nullable: false),
                        nombreAscenseur = c.Single(nullable: false),
                        lieuExecution = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationBattonages", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationBattonages", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationBattonages");
        }
    }
}
