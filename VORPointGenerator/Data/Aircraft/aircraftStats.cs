using System.Drawing.Imaging;
using System.Drawing;
using System;
using VORPointGenerator.Data.Battery;
using VORPointGenerator.Data.Bomb;
using VORPointGenerator.Data.Missile;
using VORPointGenerator.Data.Rocket;
using VORPointGenerator.Data.Special;
using VORPointGenerator.Data.Torpedo;

namespace VORPointGenerator.Data.Aircraft
{
    internal class AircraftStats
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CountryOfOrigin { get; set; } = string.Empty;
        public int PlaneCount { get; set; }
        public int Move { get; set; }
        public int EnergyGain { get; set; }
        public int MaxEnergy { get; set; }
        public int Cost { get; set; }

        public bool SteelHull { get; set; }

        public int SpottingRange { get; set; } = 15;

        public List<BatteryStats> GunStats { get; set; } = new List<BatteryStats>();
        public List<RocketStats> RocketStats { get; set; } = new List<RocketStats>();
        public List<BombStats> BombStats { get; set; } = new List<BombStats>();
        public List<TorpedoStats> TorpedoStats { get; set; } = new List<TorpedoStats>();
        public List<MissileStats> MissileStats { get; set; } = new List<MissileStats>();

        public List<SpecialAbility> SpecialAbilities { get; set; } = new List<SpecialAbility>();
        public double abilityWeight = 1.0;

        public int PointValue { get; set; }

        public string Cameo { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string ArtLink { get; set; } = string.Empty;

        public string abyssalName = string.Empty;
        public string abyssalCameo = string.Empty;
        public string abyssalArtist = string.Empty;
        public string abyssalArtLink = string.Empty;

        public string PrintStats()
        {
            string output = string.Empty;
            output = "Name: " + Name;
            output = output + "\n" + Type + "\tCOST:\t" + PointValue;
            //ADD TYPE, ADD COST
            output = output + "\nAircraft: \t" + PlaneCount + "\n";
            output = output + "Move:\t" + Move + "\tEnergy Gain:\t" + EnergyGain + "\tMax Energy:\t" + MaxEnergy + "\tSpotting:\t" + SpottingRange + "\n";
            output = output + "Weapons:";
            foreach (var i in GunStats)
            {
                output = output + "\n" + i.Name + ": " + i.Turrets + " * " + i.GunsPerTurret +
                    "\nRANGE:\t" + i.Range + "\tPWR:\t" + i.Power + "\tACC:\t" + i.Accuracy;
            }
            foreach (var i in RocketStats)
            {
                output = output + "\nROCKET - " + i.Name + ": " + i.RocketVolleys + " * " + i.RocketAtk +
                    "\nRANGE:\t" + i.RocketRange + "\tPWR:\t" + i.RocketPower + "\tACC:\t" + i.RocketAcc;
            }
            foreach (var i in BombStats)
            {
                output = output + "\nBOMB - " + i.Name + ": " + i.Atk + " * " + i.Volleys +
                    "\nRANGE:\t1\tPWR:\t" + i.Power + "\tACC:\t" + i.Accuracy;
            }
            foreach (var i in TorpedoStats)
            {
                //output = output + "\nTORPEDO - " + i.name + ": " + i.torpTurrets;
                output = output + "\nTORPEDO - " + i.Name + ": " + i.TorpTurrets +
                    "\nRANGE:\t" + i.TorpRange + "\tPWR:\t" + i.TorpPower + "\tAOE:\t" + i.TorpAOE + "\tACC:\t" + i.TorpAcc;

            }
            output += "\n";

            return output;
        }
        internal void CalculatePointValue()
        {
            //Console.WriteLine(cost);

            // quick fix. might be moved to another method.

            if (EnergyGain == 0) EnergyGain = 1;


            Cost = (int)Math.Round(PlaneCount * (double)Move * EnergyGain * MaxEnergy * 0.02);

            // guns
            //Console.WriteLine(cost);
            foreach (var i in GunStats)
            {
                int gunsPerTurret = i.GunsPerTurret;
                if (gunsPerTurret == 0) { gunsPerTurret = 1; }
                Cost = Cost + (int)Math.Round(Math.Pow((double)(PlaneCount * i.Turrets * gunsPerTurret * i.Range * (i.Power + 1) * 0.5), 1 + i.Accuracy / 50));
            }

            // torpedoes
            //Console.WriteLine(cost);
            foreach (var i in TorpedoStats)
            {
                Cost = Cost + (int)Math.Round(Math.Pow((double)(PlaneCount * i.TorpTurrets * i.TorpsPerTurret * i.TorpRange * i.TorpPower * i.TorpAOE * 0.025), 1 + i.TorpAcc / 50));
            }

            // bombs
            //Console.WriteLine(cost);
            foreach (var i in BombStats)
            {
                Cost = Cost + (int)Math.Round(Math.Pow((double)(PlaneCount * i.Power * i.Range * i.Atk * i.Volleys * 0.025), 1 + i.Accuracy / 50));
            }

            // rockets
            //Console.WriteLine(cost);
            foreach (var i in RocketStats)
            {
                Cost = Cost + (int)Math.Round(Math.Pow((double)(PlaneCount * i.RocketVolleys * i.RocketPower * i.RocketAtk * 0.05), 1 + i.RocketAcc / 50));
            }

            // Anti-ship missiles
            foreach (var i in MissileStats)
            {
                double missileBias = 0.0001;

                int correctedRange = i.MslRange;
                if (correctedRange > 72) correctedRange = 72; //past six feet, a missile's range doesn't really matter for balance reasons
                int corrMslPwr = i.MslPower;
                if (i.MslPower == 0) corrMslPwr = 1;
                int missileStats = (int)Math.Round(PlaneCount * i.MslTurrets * i.MslsPerTurret * i.MslEvasion * Math.Pow((double)(correctedRange * corrMslPwr * i.MslAOE * missileBias), 1 + i.MslAcc / 75));
                //Console.WriteLine("Missile Cost: " + i.mslTurrets + " " + i.mslsPerTurret + " " + i.mslEvasion + " " + i.mslRange + " " + i.mslPower + " " + i.mslAOE + " " + i.mslAcc);

                Cost = Cost + missileStats;
            }

            if (SteelHull == true)
            {
                Cost = (int)(Cost * 0.1);
            }

            // round point value to 5
            //Console.WriteLine(cost);
            Cost = (int)(Cost * abilityWeight);
            PointValue = (int)Math.Round((double)Cost / 5) * 5;
            if (PointValue == 0) { PointValue = 5; }
        }

        public void GenerateStatCard(string name, string countryOfOrigin, string cameo, string artist, string artLink)
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
            Font descFont = new Font("Corbel", 18);
            Font descFontTitle = new Font("Corbel", 20, FontStyle.Bold);

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
            if (countryOfOrigin.Length > 0 && !countryOfOrigin.Equals("ABYSSAL"))
            {
                string flagDirectory = workingDirectory + "\\images\\src\\flags\\";

                if (countryOfOrigin.Equals("USA"))
                {
                    flagDirectory = flagDirectory + "usa.png";
                }
                if (countryOfOrigin.Equals("CAN"))
                {
                    flagDirectory = flagDirectory + "can.png";
                }
                if (countryOfOrigin.Equals("GER"))
                {
                    flagDirectory = flagDirectory + "ger.png";
                }
                if (countryOfOrigin.Equals("ITA"))
                {
                    flagDirectory = flagDirectory + "ita.png";
                }
                if (countryOfOrigin.Equals("JPN"))
                {
                    flagDirectory = flagDirectory + "jpn.png";
                }
                if (countryOfOrigin.Equals("RUS"))
                {
                    flagDirectory = flagDirectory + "rus.png";
                }
                if (countryOfOrigin.Equals("UK"))
                {
                    flagDirectory = flagDirectory + "uk.png";
                }
                if (countryOfOrigin.Equals("NL"))
                {
                    flagDirectory = flagDirectory + "neth.png";
                }
                if (countryOfOrigin.Equals("FRA"))
                {
                    flagDirectory = flagDirectory + "fra.png";
                }
                if (countryOfOrigin.Equals("SWE"))
                {
                    flagDirectory = flagDirectory + "swdn.png";
                }
                if (countryOfOrigin.Equals("AUS"))
                {
                    flagDirectory = flagDirectory + "aus.png";
                }
                if (countryOfOrigin.Equals("KOR"))
                {
                    flagDirectory = flagDirectory + "kor.png";
                }
                if (countryOfOrigin.Equals("PRC"))
                {
                    flagDirectory = flagDirectory + "prc.png";
                }

                PointF flagPoint = new PointF(width - 2000, -300);
                try
                {
                    Image newImage = Image.FromFile(flagDirectory);
                    cardGraphics.DrawImage(newImage, flagPoint.X, flagPoint.Y, 2000, 1000);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to find flag image!");
                }
            }

            //Name
            PointF startPoint = new PointF(5, 10);
            cardGraphics.DrawString(name, h2, foregroundColor, startPoint);

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
            cardGraphics.DrawString("/ " + PlaneCount, h2, foregroundColor, leftPoint);

            // Faction and class
            startPoint = startPoint + new Size(0, h1Margin);
            cardGraphics.DrawString(countryOfOrigin + "\t" + Type, textFont, foregroundColor, startPoint);

            // Base Stats
            string statblock = "SPEED\t\t " + Move + "\nENERGY\t" + EnergyGain +
                "\nMAX ENERGY\t" + MaxEnergy + "\nSPOTTING\t" + SpottingRange;
            startPoint = startPoint + new Size(0, h4Margin);
            cardGraphics.DrawString(statblock, h3, foregroundColor, startPoint);

            //Start Drawing Weapons
            startPoint = startPoint + new Size(0, h3Margin * 8);
            cardGraphics.DrawString("WEAPONS:", h3, foregroundColor, startPoint);

            startPoint = startPoint + new Size(0, h3Margin);

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

            foreach (var i in GunStats)
            {
                // move point
                startPoint = startPoint + new Size(0, h4Margin);

                // draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 2);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                // draw weapon title
                string weaponTitle;
                if (i.AttackAir == true) { weaponTitle = i.Name + " (GUN) (D/P)"; }
                else { weaponTitle = i.Name + " (GUN)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.Turrets + "x" + i.GunsPerTurret + "\t " + i.Range + "\t " + i.Power + "\t " + i.Accuracy + "\t -\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in BombStats)
            {
                // move point
                startPoint = startPoint + new Size(0, h4Margin);

                // draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 2);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                // draw weapon title
                if (i.DiveBomb) cardGraphics.DrawString(i.Name + " (BOMB) (DIVE)", textFont, foregroundColor, startPoint);
                else cardGraphics.DrawString(i.Name + " (BOMB)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.Atk + "x" + i.Volleys + "\t " + i.Range + "\t " + i.Power + "\t " + i.Accuracy + "\t - \t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in RocketStats)
            {
                // move point
                startPoint = startPoint + new Size(0, h4Margin);

                // draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 2);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                // draw weapon title
                cardGraphics.DrawString(i.Name + " (RKT)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.RocketAtk + "x" + i.RocketVolleys + "\t " + i.RocketRange + "\t " + i.RocketPower + "\t " + i.RocketAcc + "\t - \t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in TorpedoStats)
            {
                // move point
                startPoint = startPoint + new Size(0, h4Margin);

                // draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 2);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                // draw weapon title
                cardGraphics.DrawString(i.Name + " (TORP)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.TorpTurrets + "x" + i.TorpsPerTurret + "\t " + i.TorpRange + "\t " + i.TorpPower + "\t " + i.TorpAcc + "\t " + i.TorpAOE + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in MissileStats)
            {
                // move point
                startPoint = startPoint + new Size(0, h4Margin);

                // draw background box
                leftPoint = startPoint;
                leftPoint.X = leftPoint.X - 2;
                leftPoint.Y = leftPoint.Y - 8;

                pointX = (int)leftPoint.X;
                pointY = (int)leftPoint.Y;
                hpBox = new Rectangle(pointX, pointY, statblockWidth, textFontMargin + 2);

                cardGraphics.FillRectangle(backgroundColor, hpBox);

                // draw weapon title
                string weaponTitle;
                if (i.AttackAir == true) { weaponTitle = i.Name + " (MSL) (D/P)"; }
                else { weaponTitle = i.Name + " (MSL)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                string statBlock = "\t" + i.MslTurrets + "x" + i.MslsPerTurret + "\t " + i.MslRange + "\t " + i.MslPower + "\t " + i.MslAcc + "\t " + i.MslAOE + "\t " + i.MslEvasion;
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            //Add ability box
            leftPoint = startPoint + new Size(0, h4Margin);
            leftPoint.X = 2;

            pointX = (int)leftPoint.X;
            pointY = (int)leftPoint.Y;
            hpBox = new Rectangle(pointX, pointY, statblockWidth, 1000);


            cardGraphics.DrawRectangle(graphicsPen, hpBox);
            cardGraphics.FillRectangle(backgroundColor, hpBox);


            // add any special abilities, if available

            if (SpecialAbilities.Count > 0)
            {
                startPoint = startPoint + new Size(0, h4Margin + 4);
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


            // Draw background image
            try
            {
                Image newImage = Image.FromFile(cameoBkndDirectory);
                cardGraphics.DrawImage(newImage, cameoPoint.X, cameoPoint.Y, width, height);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to find image!");
            }

            // TODO: Draw the cameo image
            if (cameo.Length > 0)
            {
                string cameoDirectory = workingDirectory + "\\images\\src\\planes\\" + cameo;

                try
                {
                    Image newImage = Image.FromFile(cameoDirectory);
                    cardGraphics.DrawImage(newImage, cameoPoint.X, cameoPoint.Y, width, height);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(cameoDirectory);
                    Console.WriteLine("Failed to find cameo image!");
                }
            }

            // Draw the outline
            pointX = (int)cameoPoint.X;
            pointY = (int)cameoPoint.Y;
            Rectangle cameoBox = new Rectangle(pointX, pointY, width, height);

            cardGraphics.DrawRectangle(graphicsPen, cameoBox);

            // Credit the artist
            cameoPoint = cameoPoint + new Size(10, 1350);
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
            opPath = documents + @"\My Games\Valkyries of Ran\Aircraft";

            string safeName = clean(name);

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
                opPath = opPath + "\\" + countryOfOrigin + " " + type + " " + safeName + ".jpeg";
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
        private String clean(string s)
        {
            string output = s.Replace("\\", "");
            output = output.Replace("/", "");
            output = output.Replace("*", "");
            output = output.Replace("\"", "");
            output = output.Replace("<", "");
            output = output.Replace(">", "");
            output = output.Replace(".", "");
            output = output.Replace(":", "");
            output = output.Replace("|", "");
            output = output.Replace("?", "");

            return output;
        }
    }
}