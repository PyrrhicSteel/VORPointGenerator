using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator.Data.Torpedo
{
    internal class TorpedoReference
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// in meters
        /// </summary>
        public int TorpRange { get; set; } 
        /// <summary>
        /// torpedo warhead size in kg TNT equivilant
        /// </summary>
        public int TorpWarheadSize { get; set; } 
        /// <summary>
        /// in knots
        /// </summary>
        public int TorpSpeed { get; set; } 
        /// <summary>
        /// number based on the number of advantages the torp has in guidence. Zero on non-modern ships
        /// </summary>
        public int TorpGuidance { get; set; } 
        /// <summary>
        /// for modern stuff
        /// </summary>
        public bool SelfGuided { get; set; } = false;
    }
}
