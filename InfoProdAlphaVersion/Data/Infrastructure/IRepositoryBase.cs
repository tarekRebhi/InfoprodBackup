using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyReports.Data.Infrastracture
{
   public interface IRepositoryBase<T> where T :class           //contrainte pour que T soit de type reference que des classes pas des types values int string
    {
        void Add(T t);
        void Delete(T t);
        void Delete(Expression<Func<T, bool>> delete); // delete par une expression et codition expression lamda plus dynamique,
        T GetById(String id);
        T GetById(int? id);
        T Get(Expression<Func<T, bool>> condition);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> condition = null,
            Expression<Func<T, bool>> sort = null);
        IEnumerable<T> GetAll();
        List<T> GetAlltest();
        void Update(T entity);

    }
}
