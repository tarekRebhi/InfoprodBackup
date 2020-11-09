using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface IIndicateurService:IDisposable
    {
        void Add(Indicateur indicateur);

        void Delete(Indicateur indicateur);

        void SaveChange();
        Indicateur findIndicateurBy(String champ);
        Indicateur getById(int? id);
        Indicateur getById(String champ);


        IEnumerable<Indicateur> GetAll();

        void Dispose();
    }
}
