using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VORPointGenerator.Data.Battery;
using VORPointGenerator.Data.Bomb;
using VORPointGenerator.Data.Missile;
using VORPointGenerator.Data.Rocket;
using VORPointGenerator.Data.Special;
using VORPointGenerator.Data.Torpedo;

namespace VORPointGenerator.Data.Aircraft
{
    internal class AircraftReference
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CountryOfOrigin { get; set; } = string.Empty;
        //public int id { get; set; }
        public int NumPlanes { get; set; }
        public int Speed { get; set; } // max speed in meters per second
        public double Thrust { get; set; } // in hp
        public double Weight { get; set; } // in kg
        public int RateOfClimb { get; set; } // in m/s
        public int ServiceCeiling { get; set; } // in meters

        public List<BatteryReference> GunReferences { get; set; } = new List<BatteryReference>();
        public List<BombReference> BombReferences { get; set; } = new List<BombReference>();
        public List<TorpedoBatReference> TorpedoBatReferences { get; set; } = new List<TorpedoBatReference>();
        public List<RocketReference> RocketReferences { get; set; } = new List<RocketReference>();
        public List<MissileBatReference> Missiles { get; set; } = new List<MissileBatReference>();

        public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();
        public double AbilityWeight { get; set; } = 1.0;

        // cameo Art
        public string cameo = string.Empty;
        public string artist = string.Empty;
        public string artLink = string.Empty;

        public string abyssalName = string.Empty;
        public string abyssalCameo = string.Empty;
        public string abyssalArtist = string.Empty;
        public string abyssalArtLink = string.Empty;
    }
}
