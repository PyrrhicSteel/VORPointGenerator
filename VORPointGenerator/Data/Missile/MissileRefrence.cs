using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator.Data.Missile
{
    internal class MissileReference
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// in meters
        /// </summary>
        public int MslRange { get; set; } 
        /// <summary>
        /// missile warhead size in kg TNT equivilant
        /// </summary>
        public int MslWarheadSize { get; set; } 
        /// <summary>
        /// in knots
        /// </summary>
        public int MslSpeed { get; set; } 
        /// <summary>
        /// number based on the number of advantages the missile has in guidence. Zero on non-modern ships
        /// </summary>
        public int MslGuidance { get; set; } = 0; 
        //public int mslEvasion { get; set; }
        public bool AttackGround { get; set; } = false;
        public bool AttackAir { get; set; } = false;
        public bool SeaSkimming { get; set; } = false;
        public bool ArhGuidance { get; set; } = false;
        public bool GpsGuidance { get; set; } = false;
        public bool InertialGuidance { get; set; } = false;
        public bool SarhGuidance { get; set; } = false;
        public bool InfraredGuidance { get; set; } = false;
        public bool Cwis { get; set; } = false;
        public bool HomeOnJam { get; set; } = false;
        public bool OpticalGuidance { get; set; } = false;
        /// <summary>
        /// Basically just the LRASM
        /// </summary>
        public bool DataLinkSwarm { get; set; } = false; 
        public bool AntiRadiation { get; set; } = false;
        /// <summary>
        /// short range air-to-air missiles
        /// </summary>
        public bool Sra2a { get; set; } = false; 
        public bool AntiBallistic { get; set; } = false;
        public bool Stealth { get; set; } = false;
    }

}
