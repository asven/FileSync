using SubSonic.SqlGeneration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSync.Common
{
    public class Sync
    {
        public int ID { get; set; }
        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
