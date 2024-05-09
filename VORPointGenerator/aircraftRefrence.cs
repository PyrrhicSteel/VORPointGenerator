using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator
{
    internal class aircraftRefrence
    {
        public string type { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string countryOfOrigin { get; set; } = string.Empty;
        //public int id { get; set; }
        public int numPlanes { get; set; }
        public int speed { get; set; } // max speed in meters per second
        public double thrust { get; set; } // in hp
        public double weight { get; set; } // in kg
        public int rateOfClimb { get; set; } // in m/s
        public int serviceCieling { get; set; } // in meters
        
        public List<batteryRefrence> gunRefrences { get; set; } = new List<batteryRefrence>();
        public List<bombRefrence> bombRefrences { get; set; } = new List<bombRefrence>();
        public List<torpedoBatRefrence>  torpedoBatRefrences { get; set; } = new List<torpedoBatRefrence>();
        public List<rocketRefrence> rocketRefrences { get; set; } = new List<rocketRefrence>();
        public List<missileBatRefrence> missiles { get; set; } = new List<missileBatRefrence>();

        public List<specialAbility> specialAbilities { get; set; } = new List<specialAbility>();
        public double abilityWeight { get; set; } = 1.0;

        // cameo Art
        public string cameo = string.Empty;
        public string artist = string.Empty;
        public string artLink = string.Empty;
    }
}
