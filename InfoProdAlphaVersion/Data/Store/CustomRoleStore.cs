using Domain.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Store
{
    //developped to Customize Identity Implementation
    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {

        public CustomRoleStore(ReportContext context)
            : base(context)

        {

        }

    }
}
