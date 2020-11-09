using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationService : IDisposable
    {
        void Add(GrilleEvaluation eval);

        void Delete(GrilleEvaluation eval);

        void SaveChange();
        GrilleEvaluation findEvaluationBy(String champ);
        GrilleEvaluation getById(int? id);
        GrilleEvaluation getById(String champ);
        IEnumerable<GrilleEvaluation> GetAll();

        void Dispose();
        GrilleEvaluation getByLoginUser(string login);
        //GrilleEvaluation getByLogin(string login);

        GrilleEvaluation getByIdHermes(int idHermes);



    }
}
