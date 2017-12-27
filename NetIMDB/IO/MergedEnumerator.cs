using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
  
    public class MergedEnumerator<TModel>:IEnumerator<TModel>
    {
        IEnumerator<TModel>[] enumerators;
        int current_enumerator = 0;

        public MergedEnumerator(IEnumerator<TModel>[] enumerators)
        {
            this.enumerators = enumerators;
        }

        public TModel Current
        {
            get
            {
                return enumerators[current_enumerator].Current;
            }
        }
               
        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return enumerators[current_enumerator].Current;
            }
        }

        public bool MoveNext()
        {
            bool r = false;
            while (!r)
            {
                r = enumerators[current_enumerator].MoveNext();
                if (!r)
                {
                    if (current_enumerator == enumerators.Length-1)
                    {
                        return false; //hemos llegado al final.
                    }
                    else
                    {
                        current_enumerator++;
                    }
                }
            }

            return true;
        }

        public void Reset()
        {
            current_enumerator = 0;
            foreach (var enumerator in enumerators)
                enumerator.Reset();
        }

        //truco para permitir recorrer el enumerador con un foreach.
        public IEnumerator<TModel> GetEnumerator()
        {
            return this;
        }
    }
}
