namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PRVMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationPRVs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        presentation = c.Single(nullable: false),
                        objetAppel = c.Single(nullable: false),
                        validationDisponibilite = c.Single(nullable: false),
                        questionnement = c.Single(nullable: false),
                        reformulation = c.Single(nullable: false),
                        propositionPRV = c.Single(nullable: false),
                        objections = c.Single(nullable: false),
                        calendrier = c.Single(nullable: false),
                        validation = c.Single(nullable: false),
                        recapitulatif = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationPRVs", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationPRVs", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationPRVs");
        }
    }
}
