using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xch_analyze
{
    class logdata
    {
        public string filename { get; set; }
        public int plotsize { get; set; }
        public int buffersize { get; set; }
        public int buckets { get; set; }
        public int threads { get; set; }
        public double phase1 { get; set; }
        public double phase2 { get; set; }
        public double phase3 { get; set; }
        public double phase4 { get; set; }
        public double avg { get; set; }
        public double total { get; set; }
        public double copytime { get; set; }
        public DateTime datestart { get; set; }
        public DateTime datelastchange { get; set; }
        public bool complete { get; set; }

    }
}
