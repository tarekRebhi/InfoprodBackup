using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.CustumConvention
{
    public class DateTime2Convention:Convention
    {
        public DateTime2Convention()
        {
            Properties<DateTime>().Configure(x => x.HasColumnType("datetime2"));
        }
    }
}
