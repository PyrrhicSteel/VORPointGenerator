namespace VORPointGenerator
{
    internal class aircraftStats
    {
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string countryOfOrigin { get; set; } = string.Empty;
        public int planecount { get; set; }
        public int move { get; set; }
        public int energyGain { get; set; }
        public int maxEnergy { get; set; }
        public int cost { get; set; }

        public int spottingRange { get; set; } = 15;

        public List<batteryStats> gunStats { get; set; } = new List<batteryStats>();
        public List<rocketStats> rocketStats { get; set; } = new List<rocketStats>();
        public List<bombStats> bombStats { get; set; } = new List<bombStats>();
        public List<torpedoStats> torpedoStats { get; set; } = new List<torpedoStats>();

        public int pointValue { get; set; }

        public string printStats()
        {
            string output = string.Empty;
            output = "Name: " + name;
            output = output + "\n" + type + "\tCOST:\t" + pointValue;
            //ADD TYPE, ADD COST
            output = output + "\nAircraft: \t" + planecount + "\n";
            output = output + "Move:\t" + move + "\tEnergy Gain:\t" + energyGain + "\tMax Energy:\t" + maxEnergy + "\tSpotting:\t" + spottingRange + "\n";
            output = output + "Weapons:";
            foreach (var i in gunStats)
            {
                output = output + "\n" + i.name + ": " + i.turrets + " * " + i.gunsPerTurret +
                    "\nRANGE:\t" + i.range + "\tPWR:\t" + i.power + "\tACC:\t" + i.accuracy;
            }
            foreach (var i in rocketStats)
            {
                output = output + "\nROCKET - " + i.name + ": " + i.rocketVolleys + " * " + i.rocketAtk +
                    "\nRANGE:\t" + i.rocketRange + "\tPWR:\t" + i.rocketPower + "\tACC:\t" + i.rocketAcc;
            }
            foreach (var i in bombStats)
            {
                output = output + "\nBOMB - " + i.name + ": " + i.atk + " * " + i.volleys +
                    "\nRANGE:\t1\tPWR:\t" + i.power + "\tACC:\t" + i.accuracy;
            }
            foreach (var i in torpedoStats)
            {
                //output = output + "\nTORPEDO - " + i.name + ": " + i.torpTurrets;
                output = output + "\nTORPEDO - " + i.name + ": " + i.torpTurrets +
                    "\nRANGE:\t" + i.torpRange + "\tPWR:\t" + i.torpPower + "\tAOE:\t" + i.torpAOE + "\tACC:\t" + i.torpAcc;

            }
            output = output + "\n";

            return output;
        }
        internal void calculatePointValue()
        {
            //Console.WriteLine(cost);
            
            // quick fix. might be moved to another method.
            
            if(energyGain == 0) energyGain = 1;


            cost = (int)Math.Round((double)move * energyGain * maxEnergy * 0.08);
            
            // guns
            //Console.WriteLine(cost);
            foreach (var i in gunStats) {
                int gunsPerTurret = i.gunsPerTurret;
                if (gunsPerTurret == 0) { gunsPerTurret = 1; }
                cost = cost + (int)Math.Round(Math.Pow((double)(planecount * i.turrets * gunsPerTurret * i.range * (i.power + 1) * 0.1), (1 + (i.accuracy / 50))));
            }
            
            // torpedoes
            //Console.WriteLine(cost);
            foreach (var i in torpedoStats) {
                cost = cost + (int)Math.Round(Math.Pow((double)(planecount * i.torpTurrets * i.torpsPerTurret * i.torpRange * i.torpPower * i.torpAOE * 0.05), (1 + (i.torpAcc / 50))));
            }

            // bombs
            //Console.WriteLine(cost);
            foreach (var i in bombStats)
            {
                cost = cost + (int)(Math.Round(Math.Pow((double)(planecount * i.power * i.range * i.atk * i.volleys * 0.05), (1 + (i.accuracy / 50)))));
            }

            // rockets
            //Console.WriteLine(cost);
            foreach (var i in rocketStats)
            {
                cost = cost + (int)((Math.Round(Math.Pow((double)(planecount * i.rocketVolleys * i.rocketPower * i.rocketAtk * 0.1), (1 + (i.rocketAcc / 50))))));
            }

            // TODO: Anti-ship missiles

            // round point value to 5
            //Console.WriteLine(cost);
            pointValue = (int)Math.Round(((double)(cost) / 5)) * 5;
            if (pointValue == 0) { pointValue = 5; }
        }
    }
}