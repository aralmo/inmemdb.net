using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB
{
    public static class Constants
    {
        public const string WRAPPER_ASSEMBLY_NAME = "NetIMDB.WrapperSpace.DynamicModules";
        public const string DYNAMIC_MODULE_NAME = "NetIMDBDynamicModule";
        public const string WRAPPER_TYPE_NAME_SUFIX = "_wrapper";
        public const int RECORD_HEADER_SIZE = 5;
        public const int FILE_WRITE_BUFFER_SIZE = 1024 * 1024; //1Mb
        public static byte[] EMPTY_STRING = new byte[] { 0, 0, 0, 0, 228, 4, 0, 0 }; //2 enteros, longitud del texto en bytes y codepage
    }
}
