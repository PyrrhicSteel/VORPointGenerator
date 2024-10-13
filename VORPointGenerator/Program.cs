using Newtonsoft.Json;

using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;

using VORPointGenerator.Data.Aircraft;
using VORPointGenerator.Data.Battery;
using VORPointGenerator.Data.Bomb;
using VORPointGenerator.Data.Gun;
using VORPointGenerator.Data.Missile;
using VORPointGenerator.Data.Rocket;
using VORPointGenerator.Data.Ship;
using VORPointGenerator.Data.Special;
using VORPointGenerator.Data.Torpedo;

namespace VORPointGenerator
{

    class Program
    {

        static void Main(string[] args)
        {
            List<ShipReference> shipRefrences = new List<ShipReference>();
            List<GunRefrence> gunRefrences = new List<GunRefrence>();
            List<TorpedoReference> torpedoRefrences = new List<TorpedoReference>();
            List<MissileReference> missileRefrences = new List<MissileReference>();
            List<AircraftReference> aircraftRefrences = new List<AircraftReference>();
            List<ShipStats> shipStats = new List<ShipStats>();
            List<AircraftStats> aircraftStats = new List<AircraftStats>();
            List<MissileStats> missileStats = new List<MissileStats>();

            GunReferenceList? gunRefrenceList;
            TorpedoReferenceList? torpedoRefrenceList;
            MissileReferenceList? missileRefrenceList;
            ShipReferenceList? shipRefrenceList;
            AircraftReferenceList? aircraftRefrenceList;

            ShipStatList? shipStatList;

            // String workingDirectory = Directory.GetCurrentDirectory();
            // Console.WriteLine(workingDirectory);



            // Read Weapons
            Console.WriteLine("READING WEAPON REFRENCE...");
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/gunRefrences.json");
                gunRefrenceList = JsonConvert.DeserializeObject<GunReferenceList>(text);
                gunRefrences = gunRefrenceList.GunRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find gun refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in gunRefrences)
            {
                Console.Write(i.Name + ", ");
            }

            // read torpedoes
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/torpedoRefrences.json");
                torpedoRefrenceList = JsonConvert.DeserializeObject<TorpedoReferenceList>(text);
                torpedoRefrences = torpedoRefrenceList.TorpedoRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find torpedo refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach(var i in torpedoRefrences)
            {
                Console.Write(i.Name + ", ");
            }

            // read missiles
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/missileRefrences.json");
                missileRefrenceList = JsonConvert.DeserializeObject<MissileReferenceList>(text);
                missileRefrences = missileRefrenceList.MissileRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find missile refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in missileRefrences)
            {
                Console.Write(i.Name + ", ");
            }

            Console.WriteLine("\nWEAPONS READ SUCESSFUL\n");

            // Read Warships
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "READING WARSHIP REFRENCE...");

            try
            {
                string text = File.ReadAllText(@"./jsonFiles/shipRefrences.json");
                shipRefrenceList = JsonConvert.DeserializeObject<ShipReferenceList>(text);
                shipRefrences = shipRefrenceList.ShipRefrences;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find ship refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in shipRefrences)
            {
                Console.Write(i.Name + ", ");
            }
            Console.WriteLine("\nWARSHIPS READ SUCCESSFUL\n");

            // Read Aircraft
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "READING AIRCRAFT REFRENCE...");

            try
            {
                string text = File.ReadAllText(@"./jsonFiles/aircraftRefrences.json");
                aircraftRefrenceList = JsonConvert.DeserializeObject<AircraftReferenceList>(text);
                aircraftRefrences = aircraftRefrenceList.AircraftRefrences;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find aircraft refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in aircraftRefrences)
            {
                Console.Write(i.Name + ", ");
            }
            Console.WriteLine("\nAIRCRAFT READ SUCCESSFUL\n");



            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "GENERATING SHIP STATCARDS...");
            foreach (var i in shipRefrences)
            {
                ShipStats Warship = new ShipStats();
                Warship.Name = i.Name;
                Warship.MaxSpeed = (int)Math.Round((double)i.Speed / 4);
                Warship.Maneuverability = (int)Math.Round((double)i.Horsepower / (i.Displacement * 1.5));
                // Warship.health = (int)Math.Round((double)i.displacement / 1500); //TODO: add a non-linear decay element to prevent hp values from getting too silly
                
                if(Warship.Maneuverability + 2 > Warship.MaxSpeed)
                {
                    double newManeuverability = (Warship.MaxSpeed / 3) * 2;
                    Warship.Maneuverability = (int)Math.Round(newManeuverability);
                }
                if(Warship.Maneuverability < 1)
                {
                    Warship.Maneuverability = 1;
                }

                // alternate health implementation
                double displacement = i.Displacement;
                Warship.Health = (int)Math.Round((double)i.Displacement / 2000);
                while (displacement > 500)
                {
                    displacement = displacement * 0.5;
                    Warship.Health++;
                }
                if (Warship.Health == 1) Warship.Health++;

                Warship.Armor = (int)Math.Round((double)i.BeltThickness / 35);
                Warship.Evasion = (int)Math.Round((double)((i.Length / 35) * -1) + 2  + Warship.Maneuverability);

                if (i.HasSonar == true) { Warship.SonarRange = 10; }
                if (i.Carrier == true) { Warship.NumAircraft = (int)Math.Round(((double)i.AircraftCount / 20)); }
                else { Warship.NumAircraft = (int)Math.Round((((double)i.AircraftCount + 1) / 4)); }
                if(i.Carrier == true && i.SteelHull == true) { Warship.NumAircraft = (int)Math.Round(((double)i.AircraftCount / 15)); }
                Warship.Submarine = i.Submarine;
                Warship.Carrier = i.Carrier;
                Warship.SteelHull = i.SteelHull;

                Warship.ShipFaction = i.ShipFaction;
                Warship.HullCode = i.HullCode;

                Warship.cameo = i.cameo;
                Warship.artist = i.artist;
                Warship.artLink = i.artLink;
                // battery stats
                foreach (var j in i.Batteries)
                {
                    BatteryStats statBlock = new BatteryStats();
                    GunRefrence batteryGun = new GunRefrence();

                    int fireControl = 0;

                    foreach (var z in gunRefrences)
                    {
                        if (z.Id == j.GunReferenceID)
                        {
                            batteryGun = z;
                        }
                    }

                    statBlock.Name = batteryGun.Name;
                    statBlock.Turrets = j.TurretCount;
                    statBlock.GunsPerTurret = j.GunsPerTurret;
                    statBlock.Power = (int)Math.Round(((double)batteryGun.ArmorPenetration / 50));
                    statBlock.Range = (int)Math.Round(((double)batteryGun.MaxRange / 1750));
                    statBlock.AttackAir = batteryGun.AttackAir;

                    // have very high rofs increase accuracy
                    int b = batteryGun.FireRate;
                    while(b > 10) //TINKER WITH THIS A BUNCH
                    {
                        b = (b / 10);
                        fireControl++;
                    }

                    // have large guns reduce accuracy
                    double o = batteryGun.Calibre;
                    while (o > 100)
                    {
                        o = (o * 0.75);
                        fireControl--;
                    }

                    if (j.localOpticalDirector) { fireControl++; }
                    if (j.localRadarDirector) { fireControl++; }
                    if (j.opticalDirector) { fireControl++; }
                    if (j.radarDirector) { fireControl++; }
                    if (j.rangefinder) { fireControl++; }
                    if (j.radarRangeFinder) { fireControl++; }
                    if (j.radarBlindFire) { fireControl++; }
                    if (j.mechanicalComputer) { fireControl++; }
                    if (j.digitalComputer) { fireControl+=2; }
                    if (j.airburstFuses) { fireControl++; }
                    if (j.radarFuses) { fireControl++; }
                    if (j.CWISTracking) { fireControl+=10; }
                    if (j.poorShellQuaility) { fireControl--; }
                    if (batteryGun.Laser) { fireControl += 10; }
                    statBlock.Accuracy = fireControl + ((j.GunsPerTurret - 1) * -1);

                    statBlock.ApplyFixes();

                    Warship.GunBatteries.Add(statBlock);

                }
                //Console.WriteLine("Number of loaded torpedoes: " + i.torpedoes.Count);
                foreach (var j in i.Torpedoes)
                {
                    TorpedoStats t = new TorpedoStats();
                    TorpedoReference tRef = new TorpedoReference();

                    int torpFireControl = 0;

                    foreach (var z in torpedoRefrences)
                    {
                        if (z.Id == j.TorpReferenceID)
                        {
                            tRef = z;
                            //Console.WriteLine(z.id);
                            //Console.WriteLine("found id!");
                        }
                    }

                    // increase torpfireControl if it is guided
                    if (j.TorpedoDirector) { torpFireControl++; }
                    if (tRef.SelfGuided) { torpFireControl = torpFireControl + 4; }
                    torpFireControl = torpFireControl + tRef.TorpGuidance;

                    t.Name = tRef.Name;
                    t.TorpPower = (int)Math.Round((double)tRef.TorpWarheadSize / 50);
                    t.TorpTurrets = j.TurretCount;
                    t.TorpsPerTurret = j.TorpsPerTurret;
                    t.TorpRange = (int)Math.Round(((double)tRef.TorpRange / 1500));
                    t.TorpAcc = torpFireControl;
                    t.TorpAOE = (int)Math.Round(((double)tRef.TorpSpeed / 15));
                    t.TorpCharges = j.TorpReloads;


                    Warship.TorpedoBatteries.Add(t);
                }
                // missile stats
                foreach (var j in i.Missiles)
                {
                    MissileStats m = new MissileStats();
                    MissileReference mRef = new MissileReference();

                    int mslFireCtrl = 0;

                    foreach (var z in missileRefrences)
                    {
                        if (z.Id == j.MissileReferenceID)
                        {
                            mRef = z;
                            // Console.WriteLine(z.id);
                            // Console.WriteLine("found id!");
                        }
                    }


                    if (j.DataLink) mslFireCtrl+=2;
                    if (mRef.SarhGuidance) mslFireCtrl += 3;
                    if (mRef.ArhGuidance) mslFireCtrl += 3;
                    if (mRef.GpsGuidance) mslFireCtrl += 2;
                    if (mRef.InertialGuidance) mslFireCtrl += 2;
                    if (mRef.InfraredGuidance) mslFireCtrl += 2;
                    if (mRef.OpticalGuidance) mslFireCtrl += 2;
                    if (mRef.AttackAir) mslFireCtrl += 2;
                    if (mRef.Cwis) mslFireCtrl += 6;
                    if (mRef.HomeOnJam) mslFireCtrl += 4;
                    if (mRef.AntiRadiation) mslFireCtrl += 4;
                    if (mRef.DataLinkSwarm) mslFireCtrl += 3;
                    if (mRef.AntiBallistic) mslFireCtrl += 3;
                    if (mRef.Sra2a) mslFireCtrl += 7;

                    //Console.WriteLine(mRef.name);

                    m.Name = mRef.Name;
                    m.MslPower = (int)Math.Round((double)mRef.MslWarheadSize / 50);
                    m.MslTurrets = j.TurretCount;
                    m.MslsPerTurret = j.MissilesPerTurret;
                    m.MslRange = (int)Math.Round(((double)mRef.MslRange / 1500));
                    m.MslAcc = mslFireCtrl - (int)Math.Round((double)mRef.MslSpeed / 500);
                    m.MslAOE = (int)Math.Round((double)mRef.MslSpeed / 100) + mslFireCtrl;
                    m.MslEvasion = (int)Math.Round(((double)mRef.MslSpeed / 75));

                    m.AttackAir = mRef.AttackAir;

                    if (mRef.Stealth) m.MslEvasion += 5;
                    if (mRef.SeaSkimming) m.MslEvasion += 3;

                    while (m.MslAOE > m.MslRange) m.MslAOE--; m.MslAcc++;

                    Warship.MissileBatteries.Add(m);
                }


                //TODO: depth charges

                // Console.WriteLine(i.name + ": " + i.specialAbilities.Count);
                foreach (var j in i.SpecialAbilities)
                {
                    // TODO: Investigate this
                    SpecialAbility s = new SpecialAbility();
                    s.Name = j.Name;
                    s.Description = j.Description;
                    Warship.SpecialAbilities.Add(s);
                }

                Warship.abilityWeight = i.AbilityWeight;

                Warship.CalculatePointValue();
                

                //Console.WriteLine(Warship.printStats());

                Warship.GenerateStatCard();
                
                shipStats.Add(Warship);
            }

            //generate aircraft
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "GENERATING AIRCRAFT STATCARDS...\n");

            foreach (var i in aircraftRefrences)
            {
                AircraftStats Aircraft = new AircraftStats();
                Aircraft.Name = i.Name;
                Aircraft.Type = i.Type;
                Aircraft.CountryOfOrigin = i.CountryOfOrigin;
                Aircraft.PlaneCount = i.NumPlanes;
                //.move = (int)Math.Round(((double)i.speed / 7));  // old

                double speed = i.Speed;
                Aircraft.Move = (int)Math.Round((speed / 18));

                if (Aircraft.Move > 10)
                {
                    Aircraft.Move = 0;
                    while (speed > 10)
                    {
                        speed = speed * 0.8;
                        Aircraft.Move++;
                    }
                }

                double TWRatio = ((double)i.Thrust / (double)i.Weight);

                //TODO: final values feel wonky, adjust bias number as needed once more aircraft have been added
                
                Aircraft.EnergyGain = (int)Math.Round(Math.Pow(((double)TWRatio * i.RateOfClimb * i.RateOfClimb * 0.0196210657), 0.5));
                if (Aircraft.EnergyGain > 5)
                {
                    double gainEnergy = TWRatio * i.RateOfClimb * i.RateOfClimb;
                    Aircraft.EnergyGain = (int)(TWRatio * i.RateOfClimb * i.RateOfClimb * 0.000002);
                    while (gainEnergy > 10)
                    {
                        gainEnergy = gainEnergy * 0.2;
                        Aircraft.EnergyGain++;
                    }
                }
                 
                Aircraft.MaxEnergy = (int)Math.Round((((double)i.Speed * i.ServiceCeiling) / 250000));
                if (Aircraft.MaxEnergy > 20)
                {
                    // Console.WriteLine(Aircraft.name + ": " + Aircraft.maxEnergy + " Recalcuating Energy");


                    Aircraft.MaxEnergy = (int)Math.Round((((double)i.Speed * i.ServiceCeiling) / 1500000));
                    double energyMax = i.Speed * i.ServiceCeiling;
                    while (energyMax > 20)
                    {
                        energyMax = energyMax * 0.5;
                        Aircraft.MaxEnergy++;
                    }
                    // Console.WriteLine("New energy: " + Aircraft.maxEnergy);
                }

                Aircraft.Cameo = i.cameo;
                Aircraft.Artist = i.artist;
                Aircraft.ArtLink = i.artLink;

                Aircraft.abyssalName = i.abyssalName;
                Aircraft.abyssalCameo = i.abyssalCameo;
                Aircraft.abyssalArtist = i.abyssalArtist;
                Aircraft.abyssalArtLink = i.abyssalArtLink;

                //batteries
                foreach (var j in i.GunReferences)
                {
                    BatteryStats statBlock = new BatteryStats();
                    GunRefrence batteryGun = new GunRefrence();

                    int fireControl = 0;

                    foreach (var z in gunRefrences)
                    {
                        if (z.Id == j.GunReferenceID)
                        {
                            batteryGun = z;
                        }
                    }

                    statBlock.Name = batteryGun.Name;
                    statBlock.Turrets = j.TurretCount;
                    statBlock.GunsPerTurret = j.GunsPerTurret;
                    statBlock.Power = (int)Math.Round(((double)batteryGun.ArmorPenetration / 50));
                    statBlock.Range = (int)Math.Round(((double)batteryGun.MaxRange / 1750));

                    // have very high rofs increase accuracy
                    int b = batteryGun.FireRate;
                    while (b > 10) //TINKER WITH THIS A BUNCH
                    {
                        b = (b / 10);
                        fireControl++;
                    }

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

                    // reduce number of guns, but increase accuracy
                    while (statBlock.GunsPerTurret > 1)
                    {
                        statBlock.GunsPerTurret --;
                        fireControl++;
                    }


                    statBlock.Accuracy = fireControl + ((statBlock.GunsPerTurret - 1) / 2 * -1);

                    statBlock.ApplyFixes();

                    Aircraft.GunStats.Add(statBlock);

                }

                // Add rockets
                foreach (var j in i.RocketReferences)
                { 
                    RocketStats rocket = new RocketStats();
                    rocket.Name = j.RocketName;
                    rocket.RocketAtk = j.RocketVolleySize;
                    rocket.RocketVolleys = (int)Math.Round((double)j.RocketNumber / j.RocketVolleySize);
                    rocket.RocketRange = (int)Math.Round((double)j.RocketMaxRange / 500);
                    rocket.RocketPower = (int)Math.Round((double)j.RocketWarheadSize / 50);

                    int fireControl = 1;

                    if (j.RailDropped) { fireControl-=2; }

                    int totalRockets = j.RocketNumber * i.NumPlanes;

                    while (totalRockets > 40)
                    {
                        rocket.RocketVolleys = rocket.RocketVolleys / 2;
                        fireControl += 2;

                        // Console.WriteLine("Looptest: " + Aircraft.name + " " + rocket.rocketVolleys);

                        totalRockets = rocket.RocketAtk * rocket.RocketVolleys * i.NumPlanes;
                    }

                    rocket.RocketAcc = fireControl - j.RocketVolleySize;


                    Aircraft.RocketStats.Add(rocket);
                }
                
                foreach (var j in i.BombReferences)
                {
                    BombStats bomb = new BombStats();
                    bomb.Name = j.Name;
                    bomb.Atk = j.BombVolleySize;
                    bomb.Volleys = (int)Math.Round((double)j.Number / j.BombVolleySize);
                    bomb.Power = (int)Math.Round((double)j.BombWarheadSize / 50);

                    int bombGuidance = 0;
                    if(j.LaserGuidance == true) { bombGuidance ++; }

                    bomb.DiveBomb = j.DiveBomb;

                    bomb.Accuracy = bombGuidance;

                    Aircraft.BombStats.Add(bomb);
                }
                // add torpedoes
                foreach (var j in i.TorpedoBatReferences)
                {
                    TorpedoStats t = new TorpedoStats();
                    TorpedoReference tRef = new TorpedoReference();

                    //Console.WriteLine(i.name); 

                    int torpFireControl = 0;

                    foreach (var z in torpedoRefrences)
                    {
                        
                        if (z.Id == j.TorpReferenceID)
                        {
                            tRef = z;
                        }
                    }

                    // increase torpfireControl if it is guided
                    if (j.TorpedoDirector) { torpFireControl++; }
                    if (tRef.SelfGuided) { torpFireControl = torpFireControl + 4; }
                    torpFireControl = torpFireControl + tRef.TorpGuidance;

                    t.Name = tRef.Name;
                    t.TorpPower = (int)Math.Round((double)tRef.TorpWarheadSize / 50);
                    t.TorpTurrets = j.TurretCount;
                    t.TorpsPerTurret = j.TorpsPerTurret;
                    t.TorpRange = (int)Math.Round(((double)tRef.TorpRange / 1500));
                    t.TorpAcc = torpFireControl;
                    t.TorpAOE = (int)Math.Round(((double)tRef.TorpSpeed / 15));
                    t.TorpCharges = j.TorpReloads;

                    Aircraft.TorpedoStats.Add(t);
                }

                // Add missiles
                foreach (var j in i.Missiles)
                {
                    MissileStats m = new MissileStats();
                    MissileReference mRef = new MissileReference();

                    int mslFireCtrl = 0;

                    foreach (var z in missileRefrences)
                    {
                        if (z.Id == j.MissileReferenceID)
                        {
                            mRef = z;
                        }
                    }

                    if (j.DataLink) mslFireCtrl += 2;
                    if (mRef.SarhGuidance) mslFireCtrl += 3;
                    if (mRef.ArhGuidance) mslFireCtrl += 3;
                    if (mRef.GpsGuidance) mslFireCtrl += 3;
                    if (mRef.InertialGuidance) mslFireCtrl += 3;
                    if (mRef.InfraredGuidance) mslFireCtrl += 3;
                    if (mRef.OpticalGuidance) mslFireCtrl += 3;
                    if (mRef.AttackAir) mslFireCtrl += 3;
                    if (mRef.Cwis) mslFireCtrl += 6;
                    if (mRef.HomeOnJam) mslFireCtrl += 4;
                    if (mRef.AntiRadiation) mslFireCtrl += 4;
                    if (mRef.DataLinkSwarm) mslFireCtrl += 4;
                    if (mRef.AntiBallistic) mslFireCtrl += 3;
                    if (mRef.Sra2a) mslFireCtrl += 7;

                    //Console.WriteLine(mRef.name);

                    m.Name = mRef.Name;
                    m.MslPower = (int)Math.Round((double)mRef.MslWarheadSize / 50);
                    m.MslTurrets = j.TurretCount;
                    m.MslsPerTurret = j.MissilesPerTurret;
                    m.MslRange = (int)Math.Round(((double)mRef.MslRange / 1500));
                    m.MslAcc = mslFireCtrl - (int)Math.Round((double)mRef.MslSpeed / 500);
                    m.MslAOE = (int)Math.Round((double)mRef.MslSpeed / 100) + mslFireCtrl;
                    m.MslEvasion = (int)Math.Round(((double)mRef.MslSpeed / 75));

                    m.AttackAir = mRef.AttackAir;

                    if (mRef.Stealth) m.MslEvasion += 8;
                    if (mRef.SeaSkimming) m.MslEvasion += 3;

                    Aircraft.MissileStats.Add(m);
                }

                // Console.WriteLine(i.name + ": " + i.specialAbilities.Count);
                foreach (var j in i.SpecialAbilities)
                {
                    // TODO: Investigate this
                    SpecialAbility s = new SpecialAbility();
                    s.Name = j.Name;
                    s.Description = j.Description;
                    Aircraft.SpecialAbilities.Add(s);
                }

                Aircraft.abilityWeight = i.AbilityWeight;
                // Calculate cost
                Aircraft.CalculatePointValue();

                // Generate a stat card for the normal aircraft
                Aircraft.GenerateStatCard(Aircraft.Name, Aircraft.CountryOfOrigin, Aircraft.Cameo, Aircraft.Artist, Aircraft.ArtLink);

                // Generate abyssal version of this statcard
                if (Aircraft.abyssalName  != string.Empty)
                {
                    Aircraft.GenerateStatCard(Aircraft.abyssalName, "ABYSSAL", Aircraft.abyssalCameo, Aircraft.abyssalArtist, Aircraft.abyssalArtLink);
                }

                //Console.WriteLine(Aircraft.printStats());

                aircraftStats.Add(Aircraft);
            }
        }

        //TODO: Generate a lua script that spawns a notecard statblock for each ship and aircraft
    }
}