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
using Image = System.Drawing.Image;

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

        public List<specialAbility> specialAbilities { get; set; } = new List<specialAbility>();

        public double abilityWeight = 1.0;

        public int pointValue { get; set; }

        public string shipFaction { get; set; } = string.Empty;
        public string hullCode { get; set; } = string.Empty;

        public string cameo = string.Empty;
        public string artist = string.Empty;
        public string artLink = string.Empty;

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
            double mslBias = 0.00075;

            int aircraftCVbias = 100;
            int aircraftFloatBias = 50;

            int speedBias = 10;
            int maneuverabilityBias = 10;
            int healthBias = 50;
            int spottingRangeBias = 2;
            int sonarRangeBias = 2;
            int evasionBias = 10;
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
                attackStats = (int)((double)attackStats * 0.25);
            }
            pointValue = baseStats + attackStats ;
            pointValue = (int)Math.Round(((double)(pointValue * abilityWeight) / 2 / 5)) * 5;
        }
        // Generate PNG files for this ship
        public void generateStatCard()
        {
            int width = 2000;
            int height = 1600;
            var card = new Bitmap(width, height);

            int statblockWidth = 1100;

            Graphics cardGraphics = Graphics.FromImage(card);

            // Replace printstats with a series of custom, colored fonts
            // String text = printStats();

            String workingDirectory = Directory.GetCurrentDirectory();

            
            Color baseColor = Color.Black; //TODO: use styles based on faction
            Color textColor = Color.LightGray;

            Color atkColor = System.Drawing.ColorTranslator.FromHtml("#571C1D"); // Red
            Color rngColor = System.Drawing.ColorTranslator.FromHtml("#1D571C"); // Green
            Color powColor = System.Drawing.ColorTranslator.FromHtml("#57561C"); // Gold
            Color accColor = System.Drawing.ColorTranslator.FromHtml("#1c1d57"); // Blue
            Color aoeColor = System.Drawing.ColorTranslator.FromHtml("#561C57"); // Purple
            Color evaColor = System.Drawing.ColorTranslator.FromHtml("#1C5756"); // Teal

            Brush atkBrush = new SolidBrush(atkColor);
            Brush rngBrush = new SolidBrush(rngColor);
            Brush powBrush = new SolidBrush(powColor);
            Brush accBrush = new SolidBrush(accColor);
            Brush aoeBrush = new SolidBrush(aoeColor);
            Brush evaBrush = new SolidBrush(evaColor);

            Color white = Color.White;

            Pen graphicsPen = new Pen(textColor, 10);

            //set background color
            SolidBrush backgroundColor = new SolidBrush(baseColor);

            //set up font
            Font h1 = new Font("Trajan Pro", 60, FontStyle.Bold);
            Font h2 = new Font("Trajan Pro", 48, FontStyle.Bold);
            Font h3 = new Font("Trajan Pro", 36, FontStyle.Bold);
            Font h4 = new Font("Trajan Pro", 30, FontStyle.Bold);
            Font textFont = new Font("Trajan Pro", 24, FontStyle.Bold);
            Font descFont = new Font("Corbel", 20);
            Font descFontTitle = new Font("Corbel", 22, FontStyle.Bold);

            int h1Margin = 74;
            int h2Margin = 62;
            int h3Margin = 50;
            int h4Margin = 46;
            int textFontMargin = 44;
            int descFontMargin = 30;

            SolidBrush foregroundColor = new SolidBrush(white);

            //draw the card
            cardGraphics.FillRectangle(backgroundColor, 0, 0, width, height);

            // Try to get an image in
            String bkndDirectory = workingDirectory + "\\images\\src\\bknd2.jpg";
            //Console.WriteLine(workingDirectory);
            try
            {
                Image newImage = Image.FromFile(bkndDirectory);
                cardGraphics.DrawImage(newImage, 0, 0, width, height);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to find image!");
            }

            // Get faction flag in
            if(shipFaction.Length > 0 && !shipFaction.Equals("ABYSSAL")) {
                String flagDirectory = workingDirectory + "\\images\\src\\flags\\";
                if(shipFaction.Equals("UNITED STATES")){
                    flagDirectory = flagDirectory + "usa.png";
                }
                if (shipFaction.Equals("CANADA"))
                {
                    flagDirectory = flagDirectory + "can.png";
                }
                if (shipFaction.Equals("GERMANY"))
                {
                    flagDirectory = flagDirectory + "ger.png";
                }
                if (shipFaction.Equals("ITALY"))
                {
                    flagDirectory = flagDirectory + "ita.png";
                }
                if (shipFaction.Equals("JAPAN"))
                {
                    flagDirectory = flagDirectory + "jpn.png";
                }
                if (shipFaction.Equals("RUSSIA"))
                {
                    flagDirectory = flagDirectory + "rus.png";
                }
                if (shipFaction.Equals("UNITED KINGDOM"))
                {
                    flagDirectory = flagDirectory + "uk.png";
                }
                if (shipFaction.Equals("NETHERLANDS"))
                {
                    flagDirectory = flagDirectory + "neth.png";
                }
                if (shipFaction.Equals("FRANCE"))
                {
                    flagDirectory = flagDirectory + "fra.png";
                }
                if (shipFaction.Equals("AUSTRALIA"))
                {
                    flagDirectory = flagDirectory + "aus.png";
                }
                if (shipFaction.Equals("INDONESIA"))
                {
                    flagDirectory = flagDirectory + "idsa.png";
                }
                if (shipFaction.Equals("SWEDEN"))
                {
                    flagDirectory = flagDirectory + "swdn.png";
                }
                if (shipFaction.Equals("UNITED NATIONS"))
                {
                    flagDirectory = flagDirectory + "un.png";
                }

                PointF flagPoint = new PointF(width - 2000, -300);
                try
                {
                    Image newImage = Image.FromFile(flagDirectory);
                    cardGraphics.DrawImage(newImage, flagPoint.X, flagPoint.Y, 2000, 1000);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(name + ": Failed to find flag image!");
                }
            }

            //Name
            PointF startPoint = new PointF(5, 10);
            cardGraphics.DrawString(name, h2, foregroundColor, startPoint);

            //Point Value in top left corner
            PointF leftPoint = new PointF(width - 200, 55);
            cardGraphics.DrawString(pointValue.ToString(), h1, foregroundColor, leftPoint);

            // Integrity
            leftPoint = leftPoint + new Size(-1075, 50);
            cardGraphics.DrawString("INTEGRITY", h4, foregroundColor, leftPoint);

            leftPoint = leftPoint + new Size(-20, h4Margin + 15);
            int pointX = (int)leftPoint.X;
            int pointY = (int)leftPoint.Y;
            Rectangle hpBox = new Rectangle(pointX, pointY, 300, 300);


            cardGraphics.DrawRectangle(graphicsPen, hpBox);
            cardGraphics.FillRectangle(foregroundColor, hpBox);

            PointF cameoPoint = leftPoint; // save leftPoint for drawing later


            leftPoint = leftPoint + new Size(100, 325);
            cardGraphics.DrawString("/ " + health, h2, foregroundColor, leftPoint);

            // Faction and class
            startPoint = startPoint + new Size(0, h1Margin);
            cardGraphics.DrawString(shipFaction + " \t" + hullCode, textFont, foregroundColor, startPoint);

            // Base Stats
            String statblock = "SPEED\t\t" + maxSpeed; 
            startPoint = startPoint + new Size(0, (h4Margin * 2));
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, (h4Margin + 5));
            statblock = "MANEUVER\t" + maneuverability;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, (h4Margin + 5));
            statblock = "EVASION\t" + evasion;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, (h4Margin + 5));
            statblock = "ARMOR\t\t" + armor;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, (h4Margin + 5));
            statblock = "SPOTTING\t" + spottingRange;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, (h4Margin + 5));
            statblock = "SONAR\t\t" + sonarRange;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, (h4Margin + 5));
            statblock = "AIRCRAFT\t" + numAircraft;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            //Start Drawing Weapons
            startPoint = startPoint + new Size(0, h4Margin * 2);
            cardGraphics.DrawString("WEAPONS:", h3, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 10);

            // Draw Stat Columns
            leftPoint = startPoint;
            leftPoint.X = leftPoint.X + 165;
            leftPoint.Y = leftPoint.Y - 5;
            pointX = (int)leftPoint.X;
            pointY = (int)leftPoint.Y;

            hpBox = new Rectangle(pointX, pointY, 110, 1200);
            cardGraphics.FillRectangle(atkBrush, hpBox);

            leftPoint.X = leftPoint.X + 160;
            pointX = (int)leftPoint.X;
            hpBox = new Rectangle(pointX, pointY, 110, 1200);
            cardGraphics.FillRectangle(rngBrush, hpBox);

            leftPoint.X = leftPoint.X + 160;
            pointX = (int)leftPoint.X;
            hpBox = new Rectangle(pointX, pointY, 110, 1200);
            cardGraphics.FillRectangle(powBrush, hpBox);

            leftPoint.X = leftPoint.X + 160;
            pointX = (int)leftPoint.X;
            hpBox = new Rectangle(pointX, pointY, 110, 1200);
            cardGraphics.FillRectangle(accBrush, hpBox);

            leftPoint.X = leftPoint.X + 155;
            pointX = (int)leftPoint.X;
            hpBox = new Rectangle(pointX, pointY, 110, 1200);
            cardGraphics.FillRectangle(aoeBrush, hpBox);

            leftPoint.X = leftPoint.X + 155;
            pointX = (int)leftPoint.X;
            hpBox = new Rectangle(pointX, pointY, 110, 1200);
            cardGraphics.FillRectangle(evaBrush, hpBox);


            cardGraphics.DrawString("\tATK\tRNG\tPOW\tACC\tAOE\tEVA", h4, foregroundColor, startPoint);

            foreach (var i in gunBatteries)
            {
                //move point
                startPoint = startPoint + new Size(0, h4Margin + 6);

                //draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 6);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                //draw weapon title
                string weaponTitle;
                if (i.attackAir == true) { weaponTitle = i.name + " (GUN) (D/P)"; }
                else { weaponTitle = i.name + " (GUN)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);
                
                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.turrets + "x" + i.gunsPerTurret + "\t " + i.range + "\t " + i.power + "\t " + i.accuracy + "\t -\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in torpedoBatteries)
            {
                startPoint = startPoint + new Size(0, h4Margin + 6);

                //draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 6);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                //draw weapon
                cardGraphics.DrawString(i.name + " (TORP) (x " + i.torpCharges + " reloads)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.torpTurrets + "x" + i.torpsPerTurret + "\t " + i.torpRange + "\t " + i.torpPower + "\t " + i.torpAcc + "\t " + i.torpAOE + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in missileBatteries)
            {
                startPoint = startPoint + new Size(0, h4Margin + 6);

                //draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 6);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                //draw weapon

                string weaponTitle;
                if (i.attackAir == true) { weaponTitle = i.name + " (MSL) (D/P)"; }
                else { weaponTitle = i.name + " (MSL)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.mslTurrets + "x" + i.mslsPerTurret + "\t " + i.mslRange + "\t " + i.mslPower + "\t " + i.mslAcc + "\t " + i.mslAOE + "\t " + i.mslEvasion;
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            //Add ability box
            leftPoint = startPoint + new Size(0, h4Margin + 6);
            leftPoint.X = 2;

            pointX = (int)leftPoint.X;
            pointY = (int)leftPoint.Y;
            hpBox = new Rectangle(pointX, pointY, statblockWidth, 1000);


            cardGraphics.DrawRectangle(graphicsPen, hpBox);
            cardGraphics.FillRectangle(backgroundColor, hpBox);


            // add any special abilities, if available

            if (specialAbilities.Count > 0)
            {
                startPoint = startPoint + new Size(0, h4Margin + 6);
                // cardGraphics.DrawString("ABILITIES:", h4, foregroundColor, startPoint);

                // startPoint = startPoint + new Size(0, h4Margin);
                foreach(var i in specialAbilities)
                {
                    cardGraphics.DrawString(i.name, descFontTitle, foregroundColor, startPoint);
                    startPoint = startPoint + new Size(0, descFontMargin);
                    if(i.description.Length > 0)
                    {
                        cardGraphics.DrawString(i.description, descFont, foregroundColor, startPoint);
                        startPoint = startPoint + new Size(0, descFontMargin * i.numLines);
                    }
                    startPoint = startPoint + new Size(0, 5);
                }
            }

            // Draw the ship's cameo (1430 x 870 images)
            width = 870;
            height = 1430;

            cameoPoint = cameoPoint + new Size(420, 0);
            String cameoBkndDirectory = workingDirectory + "\\images\\src\\GenericShipImage.jpg";
            
            
            // Draw background image
            try
            {
                Image newImage = Image.FromFile(cameoBkndDirectory);
                cardGraphics.DrawImage(newImage, cameoPoint.X, cameoPoint.Y, width, height);

            }
            catch (Exception ex)
            {
                Console.WriteLine( name + ": Failed to find image!");
            }

            // TODO: Draw the cameo image
            if (cameo.Length > 0) {
                String cameoDirectory = workingDirectory + "\\images\\src\\ships\\" + cameo;

                try
                {
                    Image newImage = Image.FromFile(cameoDirectory);
                    cardGraphics.DrawImage(newImage, cameoPoint.X, cameoPoint.Y, width, height);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(cameoDirectory);
                    Console.WriteLine(name + ": Failed to find cameo image!");
                }
            }

            // Draw the outline
            pointX = (int)cameoPoint.X;
            pointY = (int)cameoPoint.Y;
            Rectangle cameoBox = new Rectangle(pointX, pointY, width, height);

            cardGraphics.DrawRectangle(graphicsPen, cameoBox);

            // Credit the artist
            cameoPoint = cameoPoint + new Size(10, 1340);
            if (cameo.Length > 0)
            {
                cardGraphics.DrawString("Artist: " + artist, textFont, foregroundColor, cameoPoint);
                cameoPoint = cameoPoint + new Size(0, textFontMargin);

                cardGraphics.DrawString(artLink, textFont, foregroundColor, cameoPoint);
                cameoPoint = cameoPoint + new Size(0, textFontMargin);
            }


            //save the card
            //var curentDirectory = Directory.GetCurrentDirectory();
            String opPath = @"C:\Outputs\VorCardOutputs\ships\";
            opPath = opPath + shipFaction + " " + hullCode + " " + name + ".jpeg";

            

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
