using System.Drawing.Imaging;
using System.Drawing;
using System;

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

        public bool steelHull { get; set; }

        public int spottingRange { get; set; } = 15;

        public List<batteryStats> gunStats { get; set; } = new List<batteryStats>();
        public List<rocketStats> rocketStats { get; set; } = new List<rocketStats>();
        public List<bombStats> bombStats { get; set; } = new List<bombStats>();
        public List<torpedoStats> torpedoStats { get; set; } = new List<torpedoStats>();
        public List<missileStats> missileStats { get; set; } = new List<missileStats>();

        public List<specialAbility> specialAbilities { get; set; } = new List<specialAbility>();
        public double abilityWeight = 1.0;

        public int pointValue { get; set; }

        public string cameo { get; set; } = string.Empty;
        public string artist { get; set; } = string.Empty;
        public string artLink { get; set; } = string.Empty;

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

            // Anti-ship missiles
            foreach (var i in missileStats)
            {
                double missileBias = 0.0001;
                
                int correctedRange = i.mslRange;
                if (correctedRange > 72) correctedRange = 72; //past six feet, a missile's range doesn't really matter for balance reasons
                int corrMslPwr = i.mslPower;
                if (i.mslPower == 0) corrMslPwr = 1;
                int missileStats = (int)Math.Round((planecount * i.mslTurrets * i.mslsPerTurret * i.mslEvasion) * Math.Pow((double)(correctedRange * corrMslPwr * i.mslAOE * missileBias), (1 + (i.mslAcc / 75))));
                //Console.WriteLine("Missile Cost: " + i.mslTurrets + " " + i.mslsPerTurret + " " + i.mslEvasion + " " + i.mslRange + " " + i.mslPower + " " + i.mslAOE + " " + i.mslAcc);

                cost = cost + missileStats;
            }

            if (steelHull == true)
            {
                cost = (int)((double)cost * 0.3);
            }

            // round point value to 5
            //Console.WriteLine(cost);
            cost = (int)(cost * abilityWeight);
            pointValue = (int)Math.Round(((double)(cost) / 5)) * 5;
            if (pointValue == 0) { pointValue = 5; }
        }

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
            if (countryOfOrigin.Length > 0 && !countryOfOrigin.Equals("ABYSSAL"))
            {
                String flagDirectory = workingDirectory + "\\images\\src\\flags\\";
                
                if (countryOfOrigin.Equals("USA"))
                {
                    flagDirectory = flagDirectory + "usa.png";
                    //Console.WriteLine(flagDirectory);
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
            cardGraphics.DrawString("/ " + planecount, h2, foregroundColor, leftPoint);

            // Faction and class
            startPoint = startPoint + new Size(0, h1Margin);
            cardGraphics.DrawString(countryOfOrigin + "\t" + type, textFont, foregroundColor, startPoint);

            // Base Stats
            String statblock = "SPEED\t\t " + move + "\nENERGY\t" + energyGain +
                "\nMAX ENERGY\t" + maxEnergy + "\nSPOTTING\t" + spottingRange;
            startPoint = startPoint + new Size(0, (h4Margin));
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

            foreach (var i in gunStats)
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
                if (i.attackAir == true) { weaponTitle = i.name + " (GUN) (D/P)"; }
                else { weaponTitle = i.name + " (GUN)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.turrets + "x" + i.gunsPerTurret + "\t " + i.range + "\t " + i.power + "\t " + i.accuracy + "\t -\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in bombStats)
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
                if (i.diveBomb) cardGraphics.DrawString(i.name + " (BOMB) (DIVE)", textFont, foregroundColor, startPoint);
                else cardGraphics.DrawString(i.name + " (BOMB)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.atk + "x" + i.volleys + "\t " + i.range + "\t " + i.power + "\t " + i.accuracy + "\t - \t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in rocketStats)
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
                cardGraphics.DrawString(i.name + " (RKT)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.rocketAtk + "x" + i.rocketVolleys + "\t " + i.rocketRange + "\t " + i.rocketPower + "\t " + i.rocketAcc + "\t - \t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in torpedoStats)
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
                cardGraphics.DrawString(i.name + " (TORP)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.torpTurrets + "x" + i.torpsPerTurret + "\t " + i.torpRange + "\t " + i.torpPower + "\t " + i.torpAcc + "\t " + i.torpAOE + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in missileStats)
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
                if (i.attackAir == true) { weaponTitle = i.name + " (MSL) (D/P)"; }
                else { weaponTitle = i.name + " (MSL)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.mslTurrets + "x" + i.mslsPerTurret + "\t " + i.mslRange + "\t " + i.mslPower + "\t " + i.mslAcc + "\t " + i.mslAOE + "\t " + i.mslEvasion;
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

            if (specialAbilities.Count > 0)
            {
                startPoint = startPoint + new Size(0, h4Margin + 4);
                // cardGraphics.DrawString("ABILITIES:", h4, foregroundColor, startPoint);

                // startPoint = startPoint + new Size(0, h4Margin);
                foreach (var i in specialAbilities)
                {
                    cardGraphics.DrawString(i.name, descFontTitle, foregroundColor, startPoint);
                    startPoint = startPoint + new Size(0, descFontMargin);
                    if (i.description.Length > 0)
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
                Console.WriteLine("Failed to find image!");
            }

            // TODO: Draw the cameo image
            if (cameo.Length > 0)
            {
                String cameoDirectory = workingDirectory + "\\images\\src\\planes\\" + cameo;

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
            //var curentDirectory = Directory.GetCurrentDirectory();
            String opPath = @"C:\Outputs\VorCardOutputs\planes\";
            string safeName = name;
            
            // remove file-dangerous characters from the name of the aircraft before publishing file
            sanitizeString s = new sanitizeString();
            safeName = s.sanitize(safeName);
            opPath = opPath + safeName + ".jpeg";

            try
            {
                //File.Create(opPath);
                card.Save(opPath, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write aircraft!");
                Console.WriteLine(e);

            }


            return;
        }
    }
}