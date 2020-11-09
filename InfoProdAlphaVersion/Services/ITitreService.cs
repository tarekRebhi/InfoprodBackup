using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface ITitreService : IDisposable
    {
        void Add(Titre titre);

        void Delete(Titre titre);

        void SaveChange();
        Titre findEmployeeBy(String champ);
        Titre getById(int? id);
        Titre getById(String champ);
        IEnumerable<Titre> GetAll();

        void Dispose();
    }
}
