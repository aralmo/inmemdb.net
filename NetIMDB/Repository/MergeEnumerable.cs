using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    public class MergeEnumerable<TModel> : IEnumerable<TModel>
    {
        MergedEnumerator<TModel> enumerator;
        public MergeEnumerable(IEnumerator<TModel>[] enumerators)
        {
            enumerator = new MergedEnumerator<TModel>(enumerators);
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return enumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return enumerator;
        }
    }
}
