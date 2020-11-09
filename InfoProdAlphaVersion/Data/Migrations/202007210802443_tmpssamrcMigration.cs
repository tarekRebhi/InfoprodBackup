namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tmpssamrcMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Temps_SamRc", "AgentId", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_prevew", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "semaine", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Annee", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_log", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_pret_camp", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_com", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_att", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_acw", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_autres", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Dej", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Brief", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Formation", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Qualif", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Cafe", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_Back_OFFICE", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_phonning", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_admin", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "tps_pause_9", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Mission_OP", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_QAS", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_Inter_technique", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_Appel_Ip", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "Temps_P_debrif", c => c.Int(nullable: false));
            AlterColumn("dbo.Temps_SamRc", "att_ape", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Temps_SamRc", "att_ape", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_debrif", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_Appel_Ip", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_Inter_technique", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_QAS", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Mission_OP", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_pause_9", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_admin", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_phonning", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_Back_OFFICE", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Cafe", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Qualif", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Formation", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Brief", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Temps_P_Dej", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_autres", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_acw", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_att", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_com", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_pret_camp", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_log", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "Annee", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "semaine", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "tps_prevew", c => c.Int());
            AlterColumn("dbo.Temps_SamRc", "AgentId", c => c.Int());
        }
    }
}
