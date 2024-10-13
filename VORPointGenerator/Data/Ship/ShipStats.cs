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
using VORPointGenerator.Data.Battery;
using VORPointGenerator.Data.Missile;
using VORPointGenerator.Data.Special;
using VORPointGenerator.Data.Torpedo;
using VORPointGenerator.Util;

namespace VORPointGenerator.Data.Ship
{
    internal class ShipStats
    {
        public string Name { get; set; } = string.Empty;
        public int MaxSpeed { get; set; }
        public int Maneuverability { get; set; }
        public int Health { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }
        public int SpottingRange { get; set; } = 12;
        public int SonarRange { get; set; } = 0;
        public int NumAircraft { get; set; }
        public bool Submarine { get; set; } = false;
        public bool Carrier { get; set; } = false;
        public bool SteelHull { get; set; } = false;

        public List<BatteryStats> GunBatteries { get; set; } = new List<BatteryStats>();
        public List<TorpedoStats> TorpedoBatteries { get; set; } = new List<TorpedoStats>();
        public List<MissileStats> MissileBatteries { get; set; } = new List<MissileStats>();
        public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();

        public int PointValue { get; set; }

        public string ShipFaction { get; set; } = string.Empty;
        public string HullCode { get; set; } = string.Empty;

        public double abilityWeight = 1.0;

        public string cameo = string.Empty;
        public string artist = string.Empty;
        public string artLink = string.Empty;

        public string PrintStats()
        {
            string output = string.Empty;

            output = "Name: " + Name;

            output = output + "\n" + ShipFaction + "\t" + HullCode + "\t COST:\t" + PointValue;

            output = output + "\nSPEED:\t\t" + MaxSpeed + "\tINTEGRITY:\t" + Health +
                "\nMANEUVER:\t" + Maneuverability + "\tSPOTTING:\t" + SpottingRange +
                "\nEVASION:\t" + Evasion + "\tSONAR:\t\t" + SonarRange +
                "\nARMOR:\t\t" + Armor + "\tAIRCRAFT:\t" + NumAircraft;

            output = output + "\nWeapons:";

            //add gun batteries
            foreach (var i in GunBatteries)
            {
                output = output + "\n" + i.Name + ": " + i.Turrets + " * " + i.GunsPerTurret +
                    "\nRANGE:\t" + i.Range + "\tPWR:\t" + i.Power + "\tACC:\t" + i.Accuracy;

            }
            //add torpedo batteries
            foreach (var i in TorpedoBatteries)
            {
                output = output + "\n" + i.Name + ": " + i.TorpTurrets + " * " + i.TorpsPerTurret +
                    "\nRANGE:\t" + i.TorpRange + "\tPWR:\t" + i.TorpPower + "\tAOE:\t" + i.TorpAOE + "\tACC:\t" + i.TorpAcc;

            }

            output = output + "\n";
            return output;
        }

        internal void CalculatePointValue()
        {
            int baseStats = 0;
            int attackStats = 0;

            // Tune these values for balancing
            double gunBias = 0.3;
            double torpBias = 0.025;
            double mslBias = 0.00065;

            int aircraftCVbias = 75;
            int aircraftFloatBias = 50;

            int speedBias = 10;
            int maneuverabilityBias = 10;
            int healthBias = 60;
            int spottingRangeBias = 1;
            int sonarRangeBias = 1;
            int evasionBias = 10;
            int armorBias = 150;

            baseStats = baseStats + MaxSpeed * speedBias;
            baseStats = baseStats + Maneuverability * maneuverabilityBias;
            baseStats = baseStats + Health * healthBias;
            baseStats = baseStats + SpottingRange * spottingRangeBias;
            baseStats = baseStats + SonarRange * sonarRangeBias;
            baseStats = baseStats + Evasion * evasionBias;
            baseStats = baseStats + Armor * armorBias;
            if (Carrier == true)
            {
                baseStats = baseStats + NumAircraft * aircraftCVbias;
            }
            else { baseStats = baseStats + NumAircraft * aircraftFloatBias; }


            //add points for guns
            foreach (var i in GunBatteries)
            {
                if (i.Power == 0)
                {
                    double batteryCost = Math.Pow((double)(i.Range * 1 * i.Turrets * i.GunsPerTurret * gunBias), 1 + i.Accuracy / 75);
                    if (i.AttackAir == false) { batteryCost = batteryCost * 0.8; }
                    attackStats = attackStats + (int)Math.Round(batteryCost);
                }
                else
                {
                    attackStats = attackStats + (int)Math.Round(i.Turrets * i.GunsPerTurret * Math.Pow((double)(i.Range * i.Power * gunBias), 1 + i.Accuracy / 75));
                }
            }
            //add points for torpedoes
            foreach (var i in TorpedoBatteries)
            {
                //int numAttacks = i.torpCharges + 1;
                attackStats = attackStats + (int)Math.Round(i.TorpTurrets * i.TorpsPerTurret * (i.TorpCharges + 1) * Math.Pow((double)(i.TorpRange * i.TorpPower * i.TorpAOE * torpBias), 1 + i.TorpAcc / 75));
            }
            //add points for missiles
            foreach (var i in MissileBatteries)
            {
                int correctedRange = i.MslRange;
                if (correctedRange > 72) correctedRange = 72; //past six feet, a missile's range doesn't really matter for balance reasons
                int missileStats = (int)Math.Round(i.MslTurrets * i.MslsPerTurret * i.MslEvasion * Math.Pow((double)(correctedRange * i.MslPower * i.MslAOE * mslBias), 1 + i.MslAcc / 75));

                //Console.WriteLine("Missile Cost: " + i.mslTurrets + " " + i.mslsPerTurret + " " + i.mslEvasion + " " + i.mslRange + " " + i.mslPower + " " + i.mslAOE + " " + i.mslAcc);

                attackStats = attackStats + missileStats;
            }

            //TODO: add points for depth charges

            if (SteelHull == true)
            {
                baseStats = (int)(baseStats * 0.5);
                attackStats = (int)(attackStats * 0.25);
            }

            PointValue = baseStats + attackStats;
            PointValue = (int)Math.Round((double)(PointValue * abilityWeight) / 2 / 5) * 5;

            Console.WriteLine(Name + ":\n\t\t\t\t\tBASE - " + baseStats + "\tWEAPONS - " + attackStats + ",\tTOTAL - " + PointValue);
        }
        // Generate PNG files for this ship
        public void GenerateStatCard()
        {
            int width = 2000;
            int height = 1600;
            var card = new Bitmap(width, height);

            int statblockWidth = 1100;

            Graphics cardGraphics = Graphics.FromImage(card);

            // Replace printstats with a series of custom, colored fonts
            // String text = printStats();

            string workingDirectory = Directory.GetCurrentDirectory();


            Color baseColor = Color.Black; //TODO: use styles based on faction
            Color textColor = Color.LightGray;

            Color atkColor = ColorTranslator.FromHtml("#571C1D"); // Red
            Color rngColor = ColorTranslator.FromHtml("#1D571C"); // Green
            Color powColor = ColorTranslator.FromHtml("#57561C"); // Gold
            Color accColor = ColorTranslator.FromHtml("#1c1d57"); // Blue
            Color aoeColor = ColorTranslator.FromHtml("#561C57"); // Purple
            Color evaColor = ColorTranslator.FromHtml("#1C5756"); // Teal

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
            string bkndDirectory = workingDirectory + "\\images\\src\\bknd2.jpg";
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
            if (ShipFaction.Length > 0 && !ShipFaction.Equals("ABYSSAL"))
            {
                string flagDirectory = workingDirectory + "\\images\\src\\flags\\";
                if (ShipFaction.Equals("UNITED STATES"))
                {
                    flagDirectory = flagDirectory + "usa.png";
                }
                if (ShipFaction.Equals("CANADA"))
                {
                    flagDirectory = flagDirectory + "can.png";
                }
                if (ShipFaction.Equals("GERMANY"))
                {
                    flagDirectory = flagDirectory + "ger.png";
                }
                if (ShipFaction.Equals("ITALY"))
                {
                    flagDirectory = flagDirectory + "ita.png";
                }
                if (ShipFaction.Equals("JAPAN"))
                {
                    flagDirectory = flagDirectory + "jpn.png";
                }
                if (ShipFaction.Equals("RUSSIA"))
                {
                    flagDirectory = flagDirectory + "rus.png";
                }
                if (ShipFaction.Equals("UNITED KINGDOM"))
                {
                    flagDirectory = flagDirectory + "uk.png";
                }
                if (ShipFaction.Equals("NETHERLANDS"))
                {
                    flagDirectory = flagDirectory + "neth.png";
                }
                if (ShipFaction.Equals("FRANCE"))
                {
                    flagDirectory = flagDirectory + "fra.png";
                }
                if (ShipFaction.Equals("AUSTRALIA"))
                {
                    flagDirectory = flagDirectory + "aus.png";
                }
                if (ShipFaction.Equals("INDONESIA"))
                {
                    flagDirectory = flagDirectory + "idsa.png";
                }
                if (ShipFaction.Equals("SWEDEN"))
                {
                    flagDirectory = flagDirectory + "swdn.png";
                }
                if (ShipFaction.Equals("UNITED NATIONS"))
                {
                    flagDirectory = flagDirectory + "un.png";
                }
                if (ShipFaction.Equals("PEOPLE'S REPUBLIC OF CHINA"))
                {
                    flagDirectory = flagDirectory + "prc.png";
                }
                if (ShipFaction.Equals("SOUTH KOREA"))
                {
                    flagDirectory = flagDirectory + "kor.png";
                }

                PointF flagPoint = new PointF(width - 2000, -300);
                try
                {
                    Image newImage = Image.FromFile(flagDirectory);
                    cardGraphics.DrawImage(newImage, flagPoint.X, flagPoint.Y, 2000, 1000);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(Name + ": Failed to find flag image!");
                }
            }

            //Name
            PointF startPoint = new PointF(5, 10);
            cardGraphics.DrawString(Name, h2, foregroundColor, startPoint);

            //Point Value in top left corner
            PointF leftPoint = new PointF(width - 200, 55);
            cardGraphics.DrawString(PointValue.ToString(), h1, foregroundColor, leftPoint);

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
            cardGraphics.DrawString("/ " + Health, h2, foregroundColor, leftPoint);

            // Faction and class
            startPoint = startPoint + new Size(0, h1Margin);
            cardGraphics.DrawString(ShipFaction + " \t" + HullCode, textFont, foregroundColor, startPoint);

            // Base Stats
            string statblock = "SPEED\t\t" + MaxSpeed;
            startPoint = startPoint + new Size(0, h4Margin * 2);
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 5);
            statblock = "MANEUVER\t" + Maneuverability;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 5);
            statblock = "EVASION\t" + Evasion;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 5);
            statblock = "ARMOR\t\t" + Armor;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 5);
            statblock = "SPOTTING\t" + SpottingRange;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 5);
            statblock = "SONAR\t\t" + SonarRange;
            cardGraphics.DrawString(statblock, h4, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h4Margin + 5);
            statblock = "AIRCRAFT\t" + NumAircraft;
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

            foreach (var i in GunBatteries)
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
                if (i.AttackAir == true) { weaponTitle = i.Name + " (GUN) (D/P)"; }
                else { weaponTitle = i.Name + " (GUN)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.Turrets + "x" + i.GunsPerTurret + "\t " + i.Range + "\t " + i.Power + "\t " + i.Accuracy + "\t -\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in TorpedoBatteries)
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
                cardGraphics.DrawString(i.Name + " (TORP) (x " + i.TorpCharges + " reloads)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.TorpTurrets + "x" + i.TorpsPerTurret + "\t " + i.TorpRange + "\t " + i.TorpPower + "\t " + i.TorpAcc + "\t " + i.TorpAOE + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in MissileBatteries)
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
                if (i.AttackAir == true) { weaponTitle = i.Name + " (MSL) (D/P)"; }
                else { weaponTitle = i.Name + " (MSL)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.MslTurrets + "x" + i.MslsPerTurret + "\t " + i.MslRange + "\t " + i.MslPower + "\t " + i.MslAcc + "\t " + i.MslAOE + "\t " + i.MslEvasion;
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

            if (SpecialAbilities.Count > 0)
            {
                startPoint = startPoint + new Size(0, h4Margin + 6);
                // cardGraphics.DrawString("ABILITIES:", h4, foregroundColor, startPoint);

                // startPoint = startPoint + new Size(0, h4Margin);
                foreach (var i in SpecialAbilities)
                {
                    cardGraphics.DrawString(i.Name, descFontTitle, foregroundColor, startPoint);
                    startPoint = startPoint + new Size(0, descFontMargin);
                    if (i.Description.Length > 0)
                    {
                        cardGraphics.DrawString(i.Description, descFont, foregroundColor, startPoint);
                        startPoint = startPoint + new Size(0, descFontMargin * i.NumLines);
                    }
                    startPoint = startPoint + new Size(0, 5);
                }
            }

            // Draw the ship's cameo (1430 x 870 images)
            width = 870;
            height = 1430;

            cameoPoint = cameoPoint + new Size(420, 0);
            string cameoBkndDirectory = workingDirectory + "\\images\\src\\GenericShipImage.jpg";

            if (SteelHull) cameoBkndDirectory = workingDirectory + "\\images\\src\\GenericSteelHullImage.jpg";

            // Draw background image
            try
            {
                Image newImage = Image.FromFile(cameoBkndDirectory);
                cardGraphics.DrawImage(newImage, cameoPoint.X, cameoPoint.Y, width, height);

            }
            catch (Exception ex)
            {
                Console.WriteLine(Name + ": Failed to find image!");
            }

            // TODO: Draw the cameo image
            if (cameo.Length > 0)
            {
                string cameoDirectory = workingDirectory + "\\images\\src\\ships\\" + cameo;

                try
                {
                    Image newImage = Image.FromFile(cameoDirectory);
                    cardGraphics.DrawImage(newImage, cameoPoint.X, cameoPoint.Y, width, height);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(cameoDirectory);
                    Console.WriteLine(Name + ": Failed to find cameo image!");
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
            string opPath;

            // New way, saving to a custom directory in documents
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            opPath = documents + @"\My Games\Valkyries of Ran\Ships";

            var san = new SanitizeString();
            string safeName = san.Sanitize(Name);

            try
            {
                if (!Directory.Exists(opPath))
                {
                    Directory.CreateDirectory(opPath);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Failed to create Directory: " + opPath);
            }
                opPath = opPath + "\\" + ShipFaction + " " + HullCode + " " + safeName + ".jpeg";
            try
            {
                card.Save(opPath, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write ship!");
                Console.WriteLine(e);
            }
        }
    }
}
