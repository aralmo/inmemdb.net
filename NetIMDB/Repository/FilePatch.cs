using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetIMDB.Repository
{
    public class FilePatch
    {
        public PatchSection[] sections;
    }
    public class PatchSection
    {
        public long offset;
        public byte[] bytes;
    }
}
