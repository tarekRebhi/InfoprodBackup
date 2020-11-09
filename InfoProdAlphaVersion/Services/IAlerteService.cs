using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface IAlerteService:IDisposable
    {
        void Add(Alerte alerte);

        void Delete(Alerte alerte);

        void SaveChange();
        Alerte findAlerteBy(String champ);
        Alerte getById(int? id);
        Alerte getById(String champ);

        void Dispose();

        IEnumerable<Alerte> GetAll();
        List<Alerte> getByDate(DateTime date, int reciverId);
        void RemoveRecivedAlerteOfEmployee(int id);

        //void btnGetListItem_Click2(String id);  c'est il y'a traitement en deuxiéme rideau

        //List<String> btnGetListItem_Click();
    }
}
