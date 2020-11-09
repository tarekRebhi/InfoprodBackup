using Data;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyReports.Data.Infrastracture
{
   public abstract class RepositoryBase<T> : IRepositoryBaseAsynch<T> where T : class
    {
        //where T : class	L’argument de type doit être un type référence. Cette contrainte s’applique également à tous les types de classe, d’interface, de délégué ou de tableau.
        private IDbSet<T> dbset;
        private IDbSet<T> dbsetIdentity;
        IDatabaseFactory databaseFactory;
        ReportContext dataContext;
        //ReportIdentityTestContext identityContext;

        protected RepositoryBase(IDatabaseFactory dbFactory)
        {
            this.databaseFactory = dbFactory;
            dbset = DataContext.Set<T>();
            //dbsetIdentity = IdentityContext.Set<T>();
        }
        protected ReportContext DataContext
        {
            get { return dataContext = databaseFactory.DataContext; }
        }
        //protected ReportIdentityTestContext IdentityContext
        //{
        //    get { return identityContext = databaseFactory.IdentityContext; }
        //}




        public object IQuerybale { get; private set; }

        public RepositoryBase(ReportContext myAlfrescoCtx)
        {
            this.dataContext = myAlfrescoCtx;
            dbset = myAlfrescoCtx.Set<T>();  //set permet de recupérer la dbset dont contient le type T. 
        }
        //public RepositoryBase(ReportIdentityTestContext myAlfrescoCtx)
        //{
        //    this.identityContext = myAlfrescoCtx;
        //    dbset = myAlfrescoCtx.Set<T>();  //set permet de recupérer la dbset dont contient le type T. 
        //}
        public void Add(T t)
        {
            dbset.Add(t);
        }

        public async Task<int> CountAsynch()       //task c'est un objet awaitable (promesse pour retourner réllement)
        {
          return  await dbset.CountAsync();          //verif c'est il y'a un retour sinon active le notifer pour qu'elle récupere les données lorsque il seront venus donc il ne se bloque continue ses instructions
        }

        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> delete)
        {
            var result = dbset.Where(delete);
            foreach(var item in result)
            {
                dbset.Remove(item);
            }        

        }

        public void Delete(T t)
        {
            dbset.Remove(t);

        }

        public Task<List<T>> FindAllAsynch()
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsynch(System.Linq.Expressions.Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> FindByConditionAsynch(System.Linq.Expressions.Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> condition)
        {
            return dbset.FirstOrDefault(condition);
        }

        public T GetById(int? id)
        {
           return dbset.Find(id);
        }

        public T GetByType(string id)
        {
            return dbset.Find(id);
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await dbset.ToListAsync();
        }

        public IEnumerable<T> GetMany(System.Linq.Expressions.Expression<Func<T, bool>> condition = null, System.Linq.Expressions.Expression<Func<T, bool>> sort = null)
        {
            IQueryable<T> query = dbset;
            if (condition != null) { query = query.Where(condition); }
            if(sort !=null) { query = query.OrderBy(sort); }
            return query;
                }
        
        public void Update(T entity)
        {
            dataContext.Entry(entity).State = EntityState.Modified;
        }
        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await dbset.Where(match).ToListAsync();
        }

        public async Task<List<T>> FindAllAsynch(Expression<Func<T, bool>> match)
        {
            return await dbset.Where(match).ToListAsync();
        }
        public virtual IEnumerable<T> GetAll()
        {
            return dbset.ToList();
        }
        public virtual List<T> GetAlltest()
        {
            return dbset.ToList();
        }

        public T GetById(string id)
        {
            return dbset.Find(id);
        }
    }
}
