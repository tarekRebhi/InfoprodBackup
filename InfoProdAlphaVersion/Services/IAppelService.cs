using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAppelService:IDisposable
    {
        void Add(Appel appel);
        void Delete(Appel appel);

        void SaveChange();
        Appel findAppelBy(String champ);
        Appel getById(int? id);
        Appel getById(String champ);

        void Dispose();

        IEnumerable<Appel> GetAll();
    }
}
