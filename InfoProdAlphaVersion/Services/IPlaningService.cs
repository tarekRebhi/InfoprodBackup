using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPlaningService :IDisposable
    {
        void Add(Planing planing);

        void Delete(Planing planing);

        void SaveChange();
        Planing findPlaningBy(String champ);
        Planing getById(int? id);
        Planing getById(String champ);
        IEnumerable<Planing> GetAll();

        void Dispose();
    }
}
