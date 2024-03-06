using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;

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
                    attackStats = attackStats + (int)Math.Round(Math.Pow((double)(i.range * 1 * i.turrets * i.gunsPerTurret), (1 + (i.accuracy / 75))));
                }
                else
                {
                    attackStats = attackStats + (int)Math.Round(Math.Pow((double)(i.range * i.power * i.turrets * i.gunsPerTurret), (1 + (i.accuracy / 75))));
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
        //TODO: Generate PNG files for this ship
        public Bitmap generateStatCard()
        {
            int width = 800;
            int height = 640;
            var card = new Bitmap(width, height);

            Graphics cardGraphics = Graphics.FromImage(card);

            //TODO: Replace printstats with a series of custom, colored fonts
            String text = printStats();

            Color baseColor = Color.Black; //TODO: use styles based on faction
            Color textColor = Color.White;


            Color white = Color.White;


            //set background color
            SolidBrush backgroundColor = new SolidBrush(baseColor);

            //set up font
            Font h1 = new Font("Trajan Pro", 36);
            Font h2 = new Font("Trajan Pro", 28);
            Font h3 = new Font("Trajan Pro", 24);
            Font h4 = new Font("Trajan Pro", 18);
            Font textFont = new Font("Trajan Pro", 12);

            SolidBrush foregroundColor = new SolidBrush(textColor);



            //draw the card
            cardGraphics.FillRectangle(backgroundColor, 0, 0, width, height);

            //Name
            PointF startPoint = new PointF(5, 10);
            cardGraphics.DrawString(name, h2, foregroundColor, startPoint);

            //Point Value in top left corner
            PointF leftPoint = new PointF(width - 130, 55);
            cardGraphics.DrawString(pointValue.ToString(), h1, foregroundColor, leftPoint);

            // Faction and class
            startPoint = startPoint + new Size(0, 40);
            cardGraphics.DrawString(shipFaction + "\t" + hullCode, textFont, foregroundColor, startPoint);

            // Base Stats
            String statblock = "SPEED\t\t " + maxSpeed + "\nMANEUVER\t" + maneuverability + 
                "\nEVASION\t" + evasion + "\nARMOR\t" + armor + "\nSPOTTING\t" + spottingRange
                + "\nSONAR\t\t" + sonarRange + "\nAircraft\t" + numAircraft;
            startPoint = startPoint + new Size(0, 35);
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);


            //Start Drawing Weapons
            startPoint = startPoint + new Size(0, 210);
            cardGraphics.DrawString("WEAPONS:", h3, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, 30);
            cardGraphics.DrawString("\tATK\tRNG\tPOW\tACC\tAOE", h4, foregroundColor, startPoint);

            foreach (var i in gunBatteries)
            {
                startPoint = startPoint + new Size(0, 32);
                cardGraphics.DrawString(i.name + " (GUN)", textFont, foregroundColor, startPoint);
                
                startPoint = startPoint + new Size(0, 26);
                String statBlock = "\t" + i.turrets + "x" + i.gunsPerTurret + "\t " + i.range + "\t " + i.power + "\t " + i.accuracy + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in torpedoBatteries)
            {
                startPoint = startPoint + new Size(0, 30);
                cardGraphics.DrawString(i.name + " (TORP)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, 30);
                String statBlock = "\t" + i.torpTurrets + "x" + i.torpsPerTurret + "\t " + i.torpRange + "\t " + i.torpPower + "\t " + i.torpAcc + "\t " + i.torpAOE;
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            // TODO: Missile batteries

            if (!String.IsNullOrEmpty(ability))
            {
                startPoint = startPoint + new Size(0, 35);
                cardGraphics.DrawString("ABILITIES:", h4, foregroundColor, startPoint);
            }
            

            //save the card
            //var curentDirectory = Directory.GetCurrentDirectory();
            String opPath = @"C:\Outputs\VorCardOutputs\ships\";
            opPath = opPath + name + ".jpeg";

            //Console.WriteLine(opPath);
            try
            {
                //File.Create(opPath);
                card.Save(opPath, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write ship!");
                Console.WriteLine(e);
            }


            return card;
        }
    }

}
