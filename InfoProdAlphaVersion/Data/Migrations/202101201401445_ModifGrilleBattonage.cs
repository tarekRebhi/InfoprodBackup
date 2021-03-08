namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifGrilleBattonage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrilleEvaluationBattonages", "natureRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "typologieRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "avancementRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "destinationRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "codeRPRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "nomProgrammeRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "postITRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "objectsRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "notreClientsRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "moaRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "moaArchitectesRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "bureauxEtudesRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "bailleurSociauxRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "mairieRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "dateAccordsRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "datePCRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "dateConsultationRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "dateTravauxRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "dateLivraisonRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "surfacePlancherRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "surfaceTerrainRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "surfaceTypologieRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "nombreBatimentRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "nombreEtageRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "ascenseurRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "nombreAscenseurRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "lieuExecutionRs", c => c.Single(nullable: false));
            AddColumn("dbo.GrilleEvaluationBattonages", "noteRs", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrilleEvaluationBattonages", "noteRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "lieuExecutionRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "nombreAscenseurRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "ascenseurRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "nombreEtageRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "nombreBatimentRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "surfaceTypologieRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "surfaceTerrainRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "surfacePlancherRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "dateLivraisonRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "dateTravauxRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "dateConsultationRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "datePCRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "dateAccordsRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "mairieRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "bailleurSociauxRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "bureauxEtudesRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "moaArchitectesRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "moaRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "notreClientsRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "objectsRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "postITRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "nomProgrammeRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "codeRPRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "destinationRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "avancementRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "typologieRs");
            DropColumn("dbo.GrilleEvaluationBattonages", "natureRs");
        }
    }
}
