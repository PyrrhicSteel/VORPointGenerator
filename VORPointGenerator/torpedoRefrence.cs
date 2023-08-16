using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator
{
    internal class torpedoRefrence
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public int torpRange { get; set; } // in meters
        public int torpWarheadSize { get; set; } // torpedo warhead size in kg TNT equivilant
        public int torpSpeed { get; set; } // in knots
        public int torpGuidance { get; set; } // number based on the number of advantages the torp has in guidence. Zero on non-modern ships
        //for modern stuff
        public bool selfGuided { get; set; } = false;
    }
}
