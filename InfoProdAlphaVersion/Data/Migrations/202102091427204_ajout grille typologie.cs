namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajoutgrilletypologie : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrilleEvaluationBPPTypologies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nombreLogements = c.Single(nullable: false),
                        nombreLogementsRs = c.Single(nullable: false),
                        nombreSociaux = c.Single(nullable: false),
                        nombreSociauxRs = c.Single(nullable: false),
                        typeCommerce = c.Single(nullable: false),
                        typeCommerceRs = c.Single(nullable: false),
                        typeIndustrie = c.Single(nullable: false),
                        typeIndustrieRs = c.Single(nullable: false),
                        typeEtablissementSante = c.Single(nullable: false),
                        typeEtablissementSanteRs = c.Single(nullable: false),
                        typeAcceuil = c.Single(nullable: false),
                        typeAcceuilRs = c.Single(nullable: false),
                        capaciteAcceuilSante = c.Single(nullable: false),
                        capaciteAcceuilSanteRs = c.Single(nullable: false),
                        typeEtablissementHR = c.Single(nullable: false),
                        typeEtablissementHRRs = c.Single(nullable: false),
                        nombreEtoile = c.Single(nullable: false),
                        nombreEtoileRs = c.Single(nullable: false),
                        typeEnseignement = c.Single(nullable: false),
                        typeEnseignementRs = c.Single(nullable: false),
                        capaciteAcceuilEnseignement = c.Single(nullable: false),
                        capaciteAcceuilEnseignementRs = c.Single(nullable: false),
                        typeLoisirs = c.Single(nullable: false),
                        typeLoisirsRs = c.Single(nullable: false),
                        frigorifique = c.Single(nullable: false),
                        frigorifiqueRs = c.Single(nullable: false),
                        plateformeLogistique = c.Single(nullable: false),
                        plateformeLogistiqueRs = c.Single(nullable: false),
                        dateTempEvaluation = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        type = c.String(),
                        employeeId = c.Int(),
                        note = c.Single(nullable: false),
                        noteRs = c.Single(nullable: false),
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
            DropForeignKey("dbo.GrilleEvaluationBPPTypologies", "employeeId", "dbo.AspNetUsers");
            DropIndex("dbo.GrilleEvaluationBPPTypologies", new[] { "employeeId" });
            DropTable("dbo.GrilleEvaluationBPPTypologies");
        }
    }
}
