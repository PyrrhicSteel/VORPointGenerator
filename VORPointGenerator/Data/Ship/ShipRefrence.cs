using VORPointGenerator.Data.Battery;
using VORPointGenerator.Data.Missile;
using VORPointGenerator.Data.Special;
using VORPointGenerator.Data.Torpedo;

/**
 * <summary>
 * Real stats of a ship, pulled from a json model and used to calculate shipStat.
 * </summary>
 **/
namespace VORPointGenerator.Data.Ship
{
    public class ShipReference
    {
        //public int id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool HasSonar { get; set; } = false;
        public bool Submarine { get; set; } = false;
        public bool Carrier { get; set; } = false;
        public bool SteelHull { get; set; } = false;

        /// <summary>
        /// in Knots, for max speed stat
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// in standard displacement Long Tons. ONLY USE STANDARD, full load will result in a bloated/unbalanced hp pool
        /// </summary>
        public int Displacement { get; set; }
        /// <summary>
        /// in shp for developing maneuverability
        /// </summary>
        public int Horsepower { get; set; }
        /// <summary>
        /// for developing armor. average of all known armor values, with thickest portion of belt counted Twice
        /// </summary>
        public int BeltThickness { get; set; }
        /// <summary>
        /// for calculating evasion
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// for calculating number of aircraft
        /// </summary>
        public int AircraftCount { get; set; }

        public List<BatteryReference> Batteries { get; set; } = new List<BatteryReference>();
        public List<TorpedoBatReference> Torpedoes { get; set; } = new List<TorpedoBatReference>();
        public List<MissileBatReference> Missiles { get; set; } = new List<MissileBatReference>();

        public int DepthChargeLauncherNumber { get; set; } = 0;
        /// <summary>
        /// projector range in yards. 1 for no projector.
        /// </summary>
        public int DepthChargeLauncherRange { get; set; } = 1;

        public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();
        public double AbilityWeight { get; set; } = 1.0;

        public string ShipFaction { get; set; } = string.Empty;
        public string HullCode { get; set; } = string.Empty;

        // cameo Art
        public string cameo = string.Empty;
        public string artist = string.Empty;
        public string artLink = string.Empty;
    }
}
