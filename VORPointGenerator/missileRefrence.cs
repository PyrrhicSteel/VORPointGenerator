using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator
{
    internal class missileRefrence
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public int mslRange { get; set; } // in meters
        public int mslWarheadSize { get; set; } // missile warhead size in kg TNT equivilant
        public int mslSpeed { get; set; } // in knots
        public int mslGuidance { get; set; } = 0; // number based on the number of advantages the missile has in guidence. Zero on non-modern ships
        //public int mslEvasion { get; set; }
        public bool attackGround { get; set; } = false;
        public bool attackAir { get; set; } = false;
        public bool seaSkimming { get; set; } = false;
        public bool arhGuidance { get; set; } = false;
        public bool gpsGuidance { get; set; } = false;
        public bool inertialGuidance { get; set; } = false;
        public bool sarhGuidance { get; set; } = false;
        public bool infraredGuidance { get; set; } = false;
        public bool cwis {  get; set; } = false;
        public bool homeOnJam { get; set; } = false;
        public bool antiRadiation { get; set; } = false;
        public bool sra2a { get; set; } = false; // short range air-to-air missiles

    }

}
