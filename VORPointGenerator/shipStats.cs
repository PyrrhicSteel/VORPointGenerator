﻿using System;
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
        public List<missileStats> missileBatteries { get; set; } = new List<missileStats>();

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

            // Tune these values for balancing
            double gunBias = 0.4;
            double torpBias = 0.05;
            double mslBias = 0.0005;

            int aircraftCVbias = 100;
            int aircraftFloatBias = 50;

            int speedBias = 10;
            int maneuverabilityBias = 10;
            int healthBias = 50;
            int spottingRangeBias = 2;
            int sonarRangeBias = 2;
            int evasionBias = 20;
            int armorBias = 30;

            baseStats = baseStats + (maxSpeed * speedBias);
            baseStats = baseStats + (maneuverability * maneuverabilityBias);
            baseStats = baseStats + (health * healthBias);
            baseStats = baseStats + (spottingRange * spottingRangeBias);
            baseStats = baseStats + (sonarRange * sonarRangeBias);
            baseStats = baseStats + (evasion * evasionBias);
            baseStats = baseStats + (armor * armorBias);
            if (carrier == true)
            {
                baseStats = baseStats + (numAircraft * aircraftCVbias);
            } else { baseStats = baseStats + (numAircraft * aircraftFloatBias); }
                

            //add points for guns
            foreach (var i in gunBatteries)
            {
                if (i.power == 0)
                {
                    double batteryCost = Math.Pow((double)(i.range * 1 * i.turrets * i.gunsPerTurret * gunBias), (1 + (i.accuracy / 75)));
                    if(i.attackAir == false) { batteryCost = batteryCost * 0.8; }
                    attackStats = attackStats + (int)Math.Round(batteryCost);
                }
                else
                {
                    attackStats = attackStats + (int)Math.Round((i.turrets * i.gunsPerTurret) * Math.Pow((double)(i.range * i.power * gunBias), (1 + (i.accuracy / 75))));
                }
            }
            //add points for torpedoes
            foreach (var i in torpedoBatteries)
            {
                //int numAttacks = i.torpCharges + 1;
                attackStats = attackStats + (int)Math.Round((i.torpTurrets * i.torpsPerTurret * (i.torpCharges + 1)) * Math.Pow((double)(i.torpRange * i.torpPower * i.torpAOE * torpBias), (1 + (i.torpAcc / 75))));
            }
            //add points for missiles
            foreach (var i in missileBatteries)
            {
                int correctedRange = i.mslRange;
                if (correctedRange > 72) correctedRange = 72; //past six feet, a missile's range doesn't really matter for balance reasons
                int missileStats = (int)Math.Round((i.mslTurrets * i.mslsPerTurret * i.mslEvasion) * Math.Pow((double)(correctedRange * i.mslPower * i.mslAOE * mslBias), (1 + (i.mslAcc / 75))));
                
                //Console.WriteLine("Missile Cost: " + i.mslTurrets + " " + i.mslsPerTurret + " " + i.mslEvasion + " " + i.mslRange + " " + i.mslPower + " " + i.mslAOE + " " + i.mslAcc);

                attackStats = attackStats + missileStats;
            }

            //TODO: add points for depth charges

            if (steelHull == true)
            {
                baseStats = (int)((double)baseStats * 0.5);
                attackStats = (int)((double)attackStats * 0.5);
            }
            pointValue = (int)Math.Round(((double)(baseStats + attackStats) / 2 / 5)) * 5;
            
        }
        // Generate PNG files for this ship
        public void generateStatCard()
        {
            int width = 1000;
            int height = 800;
            var card = new Bitmap(width, height);

            Graphics cardGraphics = Graphics.FromImage(card);

            //TODO: Replace printstats with a series of custom, colored fonts
            String text = printStats();

            Color baseColor = Color.Black; //TODO: use styles based on faction
            Color textColor = Color.LightGray;


            Color white = Color.White;

            Pen graphicsPen = new Pen(textColor, 10);


            //set background color
            SolidBrush backgroundColor = new SolidBrush(baseColor);

            //set up font
            Font h1 = new Font("Trajan Pro", 36);
            Font h2 = new Font("Trajan Pro", 28);
            Font h3 = new Font("Trajan Pro", 24);
            Font h4 = new Font("Trajan Pro", 18);
            Font textFont = new Font("Trajan Pro", 12);

            SolidBrush foregroundColor = new SolidBrush(white);

            //draw the card
            cardGraphics.FillRectangle(backgroundColor, 0, 0, width, height);

            //Name
            PointF startPoint = new PointF(5, 10);
            cardGraphics.DrawString(name, h2, foregroundColor, startPoint);

            //Point Value in top left corner
            PointF leftPoint = new PointF(width - 130, 55);
            cardGraphics.DrawString(pointValue.ToString(), h1, foregroundColor, leftPoint);

            //Integrity
            leftPoint = leftPoint + new Size(-575, 25);
            cardGraphics.DrawString("INTEGRITY", h4, foregroundColor, leftPoint);

            leftPoint = leftPoint + new Size(5, 35);
            int pointX = (int)leftPoint.X;
            int pointY = (int)leftPoint.Y;
            Rectangle hpBox = new Rectangle(pointX, pointY, 150, 150);


            cardGraphics.DrawRectangle(graphicsPen, hpBox);
            cardGraphics.FillRectangle(foregroundColor, hpBox);


            leftPoint = leftPoint + new Size(45, 165);
            cardGraphics.DrawString("/ " + health, textFont, foregroundColor, leftPoint);

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
            cardGraphics.DrawString("\tATK\tRNG\tPOW\tACC\tAOE\tEVA", h4, foregroundColor, startPoint);

            foreach (var i in gunBatteries)
            {
                startPoint = startPoint + new Size(0, 28);
                string weaponTitle;
                if (i.attackAir == true) { weaponTitle = i.name + " (GUN) (D/P)"; }
                else { weaponTitle = i.name + " (GUN)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);
                
                startPoint = startPoint + new Size(0, 22);
                String statBlock = "\t" + i.turrets + "x" + i.gunsPerTurret + "\t " + i.range + "\t " + i.power + "\t " + i.accuracy + "\t -\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in torpedoBatteries)
            {
                startPoint = startPoint + new Size(0, 28);
                cardGraphics.DrawString(i.name + " (TORP) (x " + i.torpCharges + " reloads)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, 22);
                String statBlock = "\t" + i.torpTurrets + "x" + i.torpsPerTurret + "\t " + i.torpRange + "\t " + i.torpPower + "\t " + i.torpAcc + "\t " + i.torpAOE + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in missileBatteries)
            {
                startPoint = startPoint + new Size(0, 28);

                string weaponTitle;
                if (i.attackAir == true) { weaponTitle = i.name + " (MSL) (D/P)"; }
                else { weaponTitle = i.name + " (MSL)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, 22);
                String statBlock = "\t" + i.mslTurrets + "x" + i.mslsPerTurret + "\t " + i.mslRange + "\t " + i.mslPower + "\t " + i.mslAcc + "\t " + i.mslAOE + "\t " + i.mslEvasion;
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            // TODO: add any special abilities, if available
            if (!String.IsNullOrEmpty(ability))
            {
                startPoint = startPoint + new Size(0, 35);
                cardGraphics.DrawString("ABILITIES:", h4, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, 35);
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


            return;
        }
    }

}
