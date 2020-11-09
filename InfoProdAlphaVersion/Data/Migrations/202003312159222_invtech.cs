namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invtech : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventaires",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Manager = c.String(),
                        Societe = c.String(),
                        Matricule = c.String(),
                        Nom = c.String(),
                        Prenom = c.String(),
                        Mail = c.String(),
                        Qualification = c.String(),
                        Service = c.String(),
                        Adresse = c.String(),
                        Ville = c.String(),
                        CIN = c.String(),
                        Date_Delivrance_CIN = c.String(),
                        Tel1 = c.String(),
                        Tel2 = c.String(),
                        Teletravail = c.String(),
                        PC = c.String(),
                        Connexion = c.String(),
                        Type = c.String(),
                        Debit = c.String(),
                        Hermes = c.String(),
                        Acces_VPN = c.String(),
                        Acces_TSE = c.String(),
                        Opérationnel = c.String(),
                        Commentaire1 = c.String(),
                        OrdinateurPro = c.String(),
                        Modele = c.String(),
                        SacaDos = c.String(),
                        Ecran = c.String(),
                        Adaptateur_HDMI = c.String(),
                        Peripheriques = c.String(),
                        Commentaire2 = c.String(),
                        Contrat_Teletravail = c.String(),
                        Date_Teletravail = c.String(),
                        Commentaire3 = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Inventaires");
        }
    }
}
