/**
 * <summary>
 * Real stats of a ship, pulled from a json model and used to calculate shipStat.
 * </summary>
 **/

namespace VORPointGenerator
{
    public class shipRefrence
    {
        //public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public bool hasSonar { get; set; } = false;
        public bool submarine { get; set; } = false;
        public bool carrier { get; set; } = false;
        public bool steelHull { get; set; } = false;

        public int speed { get; set; } // in Knots, for max speed stat
        public int displacement { get; set; } // in standard displacement Long Tons. ONLY USE STANDARD, full load will result in a bloated/unbalanced hp pool
        public int horsepower { get; set; } // in shp for developing maneuverability
        public int beltThickness { get; set; } // for developing armor. average of all known armor values, with thickest portion of belt counted Twice
        public int length { get; set; } // for calculating evasion
        public int aircraftCount { get; set; } // for calculating number of aircraft

        public List<batteryRefrence> batteries { get; set; } = new List<batteryRefrence>();
        public List<torpedoBatRefrence> torpedoes { get; set; } = new List<torpedoBatRefrence>();
        public List<missileBatRefrence> missiles { get; set; } = new List<missileBatRefrence>();

        public int depthChargeLauncherNumber { get; set; } = 0;
        public int depthChargeLauncherRange { get; set; } = 1;// projector range in yards. 1 for no projector.

        public List<specialAbility> specialAbilities { get; set; } = new List<specialAbility>();
        public double abilityWeight { get; set; } = 1.0;

        public string shipFaction { get; set; } = string.Empty;
        public string hullCode { get; set; } = string.Empty;

        // cameo Art
        public string cameo = string.Empty;
        public string artist = string.Empty;
        public string artLink = string.Empty;
    }
}
