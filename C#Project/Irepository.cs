using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    public interface IRepository<T> where T : class
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Delete(int id);
        void Update(T entity);
    }

}
