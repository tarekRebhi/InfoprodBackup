namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrilleEvaluationHLAuto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationHLAutoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        accueil = c.Single(nullable: false),
                        Identification = c.Single(nullable: false),
                        MajDonnees = c.Single(nullable: false),
                        decouverteAttentes = c.Single(nullable: false),
                        utilisationOutils = c.Single(nullable: false),
                        miseAttente = c.Single(nullable: false),
                        tempsAttente = c.Single(nullable: false),
                        pertinenceReponse = c.Single(nullable: false),
                        conclusionContact = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        attitude = c.Single(nullable: false),
                        historisation = c.Single(nullable: false),
                        FCRSAMRC = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationHLAutoes", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationHLAutoes", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationHLAutoes");
        }
    }
}
