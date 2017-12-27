using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    public interface IRepository<IModelInterface>
    {
        void Add(IModelInterface entity);
        void Remove(IModelInterface entity);

        IEnumerable<IModelInterface> AsEnumerable();
    }
}
