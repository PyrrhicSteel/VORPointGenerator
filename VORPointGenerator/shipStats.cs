using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator
{
    internal class shipStats
    {
        public string name { get; set; } = string.Empty;
        public int maxSpeed { get; set; }
        public int maneuverability { get; set; }
        public int health { get; set; } 
        public int armor { get; set; }
        public int evasion { get; set; }
        public int spottingRange { get; set; } = 12;
        public int sonarRange { get; set; } = 0;
        public int numAircraft { get; set; }
        public bool submarine { get; set; } = false;
        public bool carrier { get; set; } = false;
        public bool steelHull { get; set; } = false;

        public List<batteryStats> gunBatteries { get; set; } = new List<batteryStats>();
        public List<torpedoStats> torpedoBatteries { get; set; } = new List<torpedoStats>();
        public List<missleStats> missileBatteries { get; set; } = new List<missleStats>();

        public string ability = string.Empty;
        public double abilityWeight = 1.0;

        public int pointValue { get; set; }

        public string shipFaction { get; set; } = string.Empty;
        public string hullCode { get; set; } = string.Empty;

        public string printStats()
        {
            string output = string.Empty;

            output = "Name: " + name;

            output = output + "\n" + shipFaction +  "\t" + hullCode + "\t COST:\t" + pointValue;

            output = output + "\nSPEED:\t\t" + maxSpeed + "\tINTEGRITY:\t" + health +
                "\nMANEUVER:\t" + maneuverability + "\tSPOTTING:\t" + spottingRange +
                "\nEVASION:\t" + evasion + "\tSONAR:\t\t" + sonarRange +
                "\nARMOR:\t\t" + armor + "\tAIRCRAFT:\t" + numAircraft;

            output = output + "\nWeapons:";

            //add gun batteries
            foreach (var i in gunBatteries)
            {
                output = output + "\n" + i.name + ": " + i.turrets + " * " + i.gunsPerTurret + 
                    "\nRANGE:\t" + i.range + "\tPWR:\t" + i.power + "\tACC:\t" + i.accuracy;

            }
            //add torpedo batteries
            foreach (var i in torpedoBatteries)
            {
                output = output + "\n" + i.name + ": " + i.torpTurrets + " * " + i.torpsPerTurret +
                    "\nRANGE:\t" + i.torpRange + "\tPWR:\t" + i.torpPower + "\tAOE:\t" + i.torpAOE + "\tACC:\t" + i.torpAcc;

            }

            output = output + "\n";
            return output;
        }

        internal void calculatePointValue()
        {
            int baseStats = 0;
            int attackStats = 0;

            baseStats = baseStats + (maxSpeed * 10);
            baseStats = baseStats + (maneuverability * 10);
            baseStats = baseStats + (health * 10);
            baseStats = baseStats + (spottingRange * 2);
            baseStats = baseStats + (sonarRange * 2);
            baseStats = baseStats + (evasion * 15);
            if (carrier == true)
            {
                baseStats = baseStats + (numAircraft * 30);
            } else { baseStats = baseStats + (numAircraft * 5); }
                

            //add points for guns
            foreach (var i in gunBatteries)
            {
                if (i.power == 0)
                {
                    attackStats = attackStats + (int)Math.Round(Math.Pow((double)(i.range * 1 * i.turrets * i.gunsPerTurret), (1 + (i.accuracy / 50))));
                }
                else
                {
                    attackStats = attackStats + (int)Math.Round(Math.Pow((double)(i.range * i.power * i.turrets * i.gunsPerTurret), (1 + (i.accuracy / 50))));
                }
            }
            //add points for torpedoes
            foreach (var i in torpedoBatteries)
            {
                attackStats = attackStats + (int)Math.Round(Math.Pow((double)(i.torpTurrets * i.torpsPerTurret * i.torpRange * i.torpPower * i.torpAOE), (1 + (i.torpAcc / 50))));
            }
            Console.WriteLine(attackStats);

            
            //add points for depth charges

            pointValue = (int)Math.Round(((double)(baseStats + attackStats) / 2 / 5)) * 5;
            
        }
    }

}
