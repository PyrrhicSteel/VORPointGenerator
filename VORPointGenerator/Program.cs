using Newtonsoft.Json;

namespace VORPointGenerator
{

    class Program
    {

        static void Main(string[] args)
        {
            List<shipRefrence> shipRefrences = new List<shipRefrence>();
            List<gunRefrence> gunRefrences = new List<gunRefrence>();
            List<torpedoRefrence> torpedoRefrences = new List<torpedoRefrence>();
            List<aircraftRefrence> aircraftRefrences = new List<aircraftRefrence>();
            List<shipStats> shipStats = new List<shipStats>();
            List<aircraftStats> aircraftStats = new List<aircraftStats>();

            gunRefrenceList? gunRefrenceList;
            torpedoRefrenceList? torpedoRefrenceList;
            shipRefrenceList? shipRefrenceList;
            aircraftRefrenceList? aircraftRefrenceList;
            shipStatList? shipStatList;





            // Read Weapons
            Console.WriteLine("READING WEAPON REFRENCE...");
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/gunRefrences.json");
                gunRefrenceList = JsonConvert.DeserializeObject<gunRefrenceList>(text);
                gunRefrences = gunRefrenceList.gunRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find gun refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            // read torpedoes
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/torpedoRefrences.json");
                torpedoRefrenceList = JsonConvert.DeserializeObject<torpedoRefrenceList>(text);
                torpedoRefrences = torpedoRefrenceList.torpedoRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find torpedo refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            // read missiles


            // quick debug
            foreach (var i in gunRefrences)
            {
                Console.Write(i.name + ", ");
            }

            Console.WriteLine("\nWEAPONS READ SUCESSFUL\n");

            // Read Warships
            Console.WriteLine("READING WARSHIP REFRENCE...");

            try
            {
                string text = File.ReadAllText(@"./jsonFiles/shipRefrences.json");
                shipRefrenceList = JsonConvert.DeserializeObject<shipRefrenceList>(text);
                shipRefrences = shipRefrenceList.shipRefrences;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find ship refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in shipRefrences)
            {
                Console.Write(i.name + ", ");
            }
            Console.WriteLine("\nWARSHIPS READ SUCCESSFUL\n");

            // Read Aircraft
            Console.WriteLine("READING AIRCRAFT REFRENCE...");

            try
            {
                string text = File.ReadAllText(@"./jsonFiles/aircraftRefrences.json");
                aircraftRefrenceList = JsonConvert.DeserializeObject<aircraftRefrenceList>(text);
                aircraftRefrences = aircraftRefrenceList.aircraftRefrences;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find aircraft refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in aircraftRefrences)
            {
                Console.Write(i.name + ", ");
            }
            Console.WriteLine("\nAIRCRAFT READ SUCCESSFUL\n");



            Console.WriteLine("GENERATING SHIP STATCARDS...");
            foreach (var i in shipRefrences)
            {
                shipStats Warship = new shipStats();
                Warship.name = i.name;
                Warship.maxSpeed = (int)Math.Round((double)i.speed / 4);
                Warship.maneuverability = (int)Math.Round((double)i.horsepower / (i.displacement * 1.5));
                Warship.health = (int)Math.Round((double)i.displacement / 1500); //TODO: add a non-linear decay element to prevent hp values from getting too silly
                Warship.armor = (int)Math.Round((double)i.beltThickness / 50);
                Warship.evasion = (int)Math.Round((double)((i.length / 50) * -1) + Warship.maneuverability);
                if (i.hasSonar == true) { Warship.sonarRange = 10; }
                if (i.carrier == true) { Warship.numAircraft = (int)Math.Round(((double)i.aircraftCount / 25)); }
                else { Warship.numAircraft = (i.aircraftCount / 4); }
                Warship.submarine = i.submarine;
                Warship.carrier = i.carrier;
                Warship.steelHull = i.steelHull;


                Warship.shipFaction = i.shipFaction;
                Warship.hullCode = i.hullCode;

                // battery stats
                foreach (var j in i.batteries)
                {
                    batteryStats statBlock = new batteryStats();
                    gunRefrence batteryGun = new gunRefrence();

                    int fireControl = 0;

                    foreach (var z in gunRefrences)
                    {
                        if (z.id == j.gunRefrenceID)
                        {
                            batteryGun = z;
                        }
                    }

                    statBlock.name = batteryGun.name;
                    statBlock.turrets = j.turretCount;
                    statBlock.gunsPerTurret = j.gunsPerTurret;
                    statBlock.power = (int)Math.Round(((double)batteryGun.armorPenetration / 50));
                    statBlock.range = (int)Math.Round(((double)batteryGun.maxRange / 1750));

                    //TODO: have very high rofs increase accuracy

                    if (j.localOpticalDirector) { fireControl++; }
                    if (j.localRadarDirector) { fireControl++; }
                    if (j.opticalDirector) { fireControl++; }
                    if (j.radarDirector) { fireControl++; }
                    if (j.rangefinder) { fireControl++; }
                    if (j.radarRangeFinder) { fireControl++; }
                    if (j.radarBlindFire) { fireControl++; }
                    if (j.mechanicalComputer) { fireControl++; }
                    if (j.digitalComputer) { fireControl++; }
                    if (j.airburstFuses) { fireControl++; }
                    if (j.radarFuses) { fireControl++; }
                    if (j.poorShellQuaility) { fireControl--; }
                    statBlock.accuracy = fireControl + ((j.gunsPerTurret - 1) * -1);

                    statBlock.applyFixes();

                    Warship.gunBatteries.Add(statBlock);

                }
                //BUG: Torpedoes not getting added
                //Console.WriteLine("Number of loaded torpedoes: " + i.torpedoes.Count);
                foreach (var j in i.torpedoes)
                {
                    torpedoStats t = new torpedoStats();
                    torpedoRefrence tRef = new torpedoRefrence();

                    int torpFireControl = 0;

                    foreach (var z in torpedoRefrences)
                    {
                        if (z.id == j.torpRefrenceID)
                        {
                            tRef = z;
                            //Console.WriteLine(z.id);
                            //Console.WriteLine("found id!");
                        }
                    }

                    // increase torpfireControl if it is guided
                    if (j.torpedoDirector) { torpFireControl++; }
                    if (tRef.selfGuided) { torpFireControl = torpFireControl + 4; }

                    t.name = tRef.name;
                    t.torpPower = (int)Math.Round((double)tRef.torpWarheadSize / 50);
                    t.torpTurrets = j.turretCount;
                    t.torpsPerTurret = j.torpsPerTurret;
                    t.torpRange = (int)Math.Round(((double)tRef.torpRange / 1500));
                    t.torpAcc = torpFireControl;
                    t.torpAOE = (int)Math.Round(((double)tRef.torpSpeed / 15)) + torpFireControl;
                    t.torpCharges = j.torpReloads;

                    Warship.torpedoBatteries.Add(t);
                }
                //TODO: missile stats

                //TODO: depth charges


                Warship.ability = i.specialAbility;
                Warship.abilityWeight = i.abilityWeight;

                Warship.calculatePointValue();


                Console.WriteLine(Warship.printStats());

                shipStats.Add(Warship);
            }

            //generate aircraft
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "GENERATING AIRCRAFT STATCARDS...\n");

            foreach (var i in aircraftRefrences)
            {
                aircraftStats Aircraft = new aircraftStats();
                Aircraft.name = i.name;
                Aircraft.type = i.type;
                Aircraft.countryOfOrigin = i.countryOfOrigin;
                Aircraft.planecount = i.numPlanes;
                Aircraft.move = (int)Math.Round(((double)i.speed / 7));
                double TWRatio = ((double)i.thrust / (double)i.weight);
                //Console.WriteLine(TWRatio);
                //TODO: final values feel wonky, adjust bias number as needed once more aircraft have been added
                Aircraft.energyGain = (int)Math.Round(Math.Pow(((double)TWRatio * i.rateOfClimb * i.rateOfClimb * 0.0196210657), 0.5)); 
                Aircraft.maxEnergy = (int)Math.Round((((double)i.speed * i.serviceCieling) / 250000));

                //batteries
                foreach (var j in i.gunRefrences)
                {
                    batteryStats statBlock = new batteryStats();
                    gunRefrence batteryGun = new gunRefrence();

                    int fireControl = 0;

                    foreach (var z in gunRefrences)
                    {
                        if (z.id == j.gunRefrenceID)
                        {
                            batteryGun = z;
                        }
                    }

                    statBlock.name = batteryGun.name;
                    statBlock.turrets = j.turretCount;
                    statBlock.gunsPerTurret = j.gunsPerTurret;
                    statBlock.power = (int)Math.Round(((double)batteryGun.armorPenetration / 50));
                    statBlock.range = (int)Math.Round(((double)batteryGun.maxRange / 1750));

                    //TODO: have very high rofs increase accuracy
                    if (j.localOpticalDirector) { fireControl++; }
                    if (j.localRadarDirector) { fireControl++; }
                    if (j.opticalDirector) { fireControl++; }
                    if (j.radarDirector) { fireControl++; }
                    if (j.rangefinder) { fireControl++; }
                    if (j.radarRangeFinder) { fireControl++; }
                    if (j.radarBlindFire) { fireControl++; }
                    if (j.mechanicalComputer) { fireControl++; }
                    if (j.digitalComputer) { fireControl++; }
                    if (j.airburstFuses) { fireControl++; }
                    if (j.radarFuses) { fireControl++; }
                    if (j.poorShellQuaility) { fireControl--; }
                    statBlock.accuracy = fireControl + ((j.gunsPerTurret - 1) / 2 * -1);

                    statBlock.applyFixes();

                    Aircraft.gunStats.Add(statBlock);

                }

                // Add rockets
                foreach (var j in i.rocketRefrences)
                { 
                    rocketStats rocket = new rocketStats();
                    rocket.name = j.rocketName;
                    rocket.rocketAtk = j.rocketVolleySize;
                    rocket.rocketVolleys = (int)Math.Round((double)j.rocketNumber / j.rocketVolleySize);
                    rocket.rocketRange = (int)Math.Round((double)j.rocketMaxRange / 500);
                    rocket.rocketPower = (int)Math.Round((double)j.rocketWarheadSize / 50);

                    int fireControl = 1;

                    if (j.railDropped) { fireControl--; }

                    rocket.rocketAcc = fireControl - j.rocketVolleySize;

                    Aircraft.rocketStats.Add(rocket);
                }
                
                foreach (var j in i.bombRefrences)
                {
                    bombStats bomb = new bombStats();
                    bomb.name = j.name;
                    bomb.atk = j.bombVolleySize;
                    bomb.volleys = (int)Math.Round((double)j.number / j.bombVolleySize);
                    bomb.power = (int)Math.Round((double)j.bombWarheadSize / 50);

                    int bombGuidance = 0;
                    if(j.laserGuidance == true) { bombGuidance ++; }

                    bomb.accuracy = bombGuidance;

                    Aircraft.bombStats.Add(bomb);
                }
                // add torpedoes
                foreach (var j in i.torpedoBatRefrences)
                {
                    torpedoStats t = new torpedoStats();
                    torpedoRefrence tRef = new torpedoRefrence();

                    Console.WriteLine(i.name); 

                    int torpFireControl = 0;

                    foreach (var z in torpedoRefrences)
                    {
                        if (z.id == j.torpRefrenceID)
                        {
                            tRef = z;
                        }
                    }

                    // increase torpfireControl if it is guided
                    if (j.torpedoDirector) { torpFireControl++; }
                    if (tRef.selfGuided) { torpFireControl = torpFireControl + 4; }

                    t.name = tRef.name;
                    t.torpPower = (int)Math.Round((double)tRef.torpWarheadSize / 50);
                    t.torpTurrets = j.turretCount;
                    t.torpsPerTurret = j.torpsPerTurret;
                    t.torpRange = (int)Math.Round(((double)tRef.torpRange / 1500));
                    t.torpAcc = torpFireControl;
                    t.torpAOE = (int)Math.Round(((double)tRef.torpSpeed / 15)) + torpFireControl;
                    t.torpCharges = j.torpReloads;

                    Aircraft.torpedoStats.Add(t);
                }
                //TODO: Calculate cost
                Aircraft.calculatePointValue();

                //add aircraft to stuff
                Console.WriteLine(Aircraft.printStats());

                aircraftStats.Add(Aircraft);
            }
        }

        //TODO: Generate a lua script that spawns a notecard statblock for each ship and aircraft
    }
}