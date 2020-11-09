using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReports.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
          
        ReportContext DataContext { get;}
        //ReportIdentityTestContext IdentityContext { get; }
    }

}
