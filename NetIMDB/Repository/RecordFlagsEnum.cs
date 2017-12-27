using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    [Flags]
    public enum RecordFlagsEnum:byte
    {
        Deleted = 0x01,
    }
}
