namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvaluationEnqueteMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationEnqueteAutoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        presentation = c.Single(nullable: false),
                        respectScript = c.Single(nullable: false),
                        traitementObjections = c.Single(nullable: false),
                        exploitationOutils = c.Single(nullable: false),
                        HistorisationOutils = c.Single(nullable: false),
                        discours = c.Single(nullable: false),
                        debit = c.Single(nullable: false),
                        intonation = c.Single(nullable: false),
                        ecouteReformulation = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationEnqueteAutoes", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationEnqueteAutoes", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationEnqueteAutoes");
        }
    }
}
