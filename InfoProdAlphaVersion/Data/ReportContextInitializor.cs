using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
   public class ReportContextInitializor: DropCreateDatabaseIfModelChanges<ReportContext>
    {

        protected override void Seed(ReportContext context)
        {
            base.Seed(context);
            //List<Contrat> contrats = new List<Contrat> { new Contrat { ID_CLIENT = "Tunis" }, new Contrat { ID_CLIENT = "zaghouan" }, new Contrat { ID_CLIENT = "Byzerte" } };
            //context.Contrats.AddRange(contrats);
            //context.SaveChanges();
        }
    }
}
