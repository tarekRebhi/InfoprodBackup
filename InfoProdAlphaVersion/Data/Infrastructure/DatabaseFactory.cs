using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReports.Data.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private ReportContext dataContext;
        //private ReportIdentityTestContext identiyContext;
        public ReportContext DataContext { get { return dataContext; } }
        //public ReportIdentityTestContext IdentityContext { get { return identiyContext; } }

        

        public DatabaseFactory()
        {
            dataContext =new ReportContext();
            //identiyContext = new ReportIdentityTestContext();
        }
        protected override void DisposeCore()
        {
            if (DataContext != null)
                DataContext.Dispose();
            //if (IdentityContext != null)
            //    IdentityContext.Dispose();
        }
    }

}
