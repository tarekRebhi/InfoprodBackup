using Domain.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Store
{
    public class CustomUserStore : UserStore<Employee, CustomRole, int,
     CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ReportContext context)
            : base(context)
        {

        }
    }

}
