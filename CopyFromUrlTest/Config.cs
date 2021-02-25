using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFromUrlTest
{
    public class Config
    {
        public string Destination { get; set; }
        public int Threads { get; set; }
        public double FileSizeMB { get; set; }
        public int NumFiles { get; set; }
        public string ContainerName { get; set; }
        public string Run { get; set; }
        public string Source { get; set; }
        public bool LoadMode { get; set; }

    }
}
