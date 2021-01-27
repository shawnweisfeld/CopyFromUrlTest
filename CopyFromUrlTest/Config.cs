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

        public int MinFileSizeMB { get; set; }
        public int MaxFileSizeMB { get; set; }
        public int NumFiles { get; set; }
        public string ContainerName { get; set; }
        public string Run { get; set; }
        public string Sources { get; set; }

    }
}
