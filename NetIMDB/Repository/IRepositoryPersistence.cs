using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    public interface IRepositoryPersistence<IModelInterface>
    {
        /// <summary>
        /// Guarda la información en el stream pasado como parámetro.
        /// </summary>
        void SaveToStream(Stream output_stream);

        /// <summary>
        /// Guarda la información del repositorio en el archivo pasado como parámetro.
        /// </summary>
        /// <param name="filename"></param>
        void SaveToFile(string filename);
    }
}
