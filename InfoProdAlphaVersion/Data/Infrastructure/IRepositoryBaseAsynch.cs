using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyReports.Data.Infrastracture
{
   public interface IRepositoryBaseAsynch<T> :IRepositoryBase<T> where T:class
    {
        Task<int> CountAsynch();
        Task<List<T>> FindByConditionAsynch(Expression<Func<T, bool>> condition);
        Task<T> FindAsynch(Expression<Func<T, bool>> condition);
        Task<List<T>> FindAllAsynch(Expression<Func<T, bool>> match); //pas d'expression vue qu'il n'ya pas de condition 
       
        Task<List<T>> GetAllAsync();
    }
}