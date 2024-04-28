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

        public int spottingRange { get; set; } = 15;

        public List<batteryStats> gunStats { get; set; } = new List<batteryStats>();
        public List<rocketStats> rocketStats { get; set; } = new List<rocketStats>();
        public List<bombStats> bombStats { get; set; } = new List<bombStats>();
        public List<torpedoStats> torpedoStats { get; set; } = new List<torpedoStats>();
        public List<missileStats> missileStats { get; set; } = new List<missileStats>();

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

        public void generateStatCard()
        {
            int width = 2000;
            int height = 1600;
            var card = new Bitmap(width, height);

            Graphics cardGraphics = Graphics.FromImage(card);

            // Replace printstats with a series of custom, colored fonts
            // String text = printStats();

            String workingDirectory = Directory.GetCurrentDirectory();


            Color baseColor = Color.Black; //TODO: use styles based on faction
            Color textColor = Color.LightGray;


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

            int h1Margin = 70;
            int h2Margin = 58;
            int h3Margin = 46;
            int h4Margin = 40;
            int textFontMargin = 34;

            SolidBrush foregroundColor = new SolidBrush(white);

            //draw the card
            cardGraphics.FillRectangle(backgroundColor, 0, 0, width, height);

            // Try to get an image in
            workingDirectory = workingDirectory + "\\images\\src\\bknd3.jpg";
            //Console.WriteLine(workingDirectory);
            try
            {
                Image newImage = Image.FromFile(workingDirectory);
                cardGraphics.DrawImage(newImage, 0, 0, width, height);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to find image!");
            }

            //Name
            PointF startPoint = new PointF(5, 10);
            cardGraphics.DrawString(name, h2, foregroundColor, startPoint);

            //Point Value in top left corner
            PointF leftPoint = new PointF(width - 130, 55);
            cardGraphics.DrawString(pointValue.ToString(), h1, foregroundColor, leftPoint);

            //Integrity
            leftPoint = leftPoint + new Size(-1175, 30);
            cardGraphics.DrawString("AIRCRAFT", h4, foregroundColor, leftPoint);

            leftPoint = leftPoint + new Size(-20, h4Margin + 15);
            int pointX = (int)leftPoint.X;
            int pointY = (int)leftPoint.Y;
            Rectangle hpBox = new Rectangle(pointX, pointY, 300, 300);


            cardGraphics.DrawRectangle(graphicsPen, hpBox);
            cardGraphics.FillRectangle(foregroundColor, hpBox);


            leftPoint = leftPoint + new Size(100, 325);
            cardGraphics.DrawString("/ " + planecount, textFont, foregroundColor, leftPoint);

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
            cardGraphics.DrawString("\tATK\tRNG\tPOW\tACC\tAOE\tEVA", h4, foregroundColor, startPoint);

            foreach (var i in gunStats)
            {
                startPoint = startPoint + new Size(0, h4Margin);
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
                startPoint = startPoint + new Size(0, h4Margin);
                if (i.diveBomb) cardGraphics.DrawString(i.name + " (BOMB) (DIVE)", textFont, foregroundColor, startPoint);
                else cardGraphics.DrawString(i.name + " (BOMB)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.atk + "x" + i.volleys + "\t " + i.range + "\t " + i.power + "\t " + i.accuracy + "\t - \t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in rocketStats)
            {
                startPoint = startPoint + new Size(0, h4Margin);
                cardGraphics.DrawString(i.name + " (RKT)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.rocketAtk + "x" + i.rocketVolleys + "\t " + i.rocketRange + "\t " + i.rocketPower + "\t " + i.rocketAcc + "\t - \t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in torpedoStats)
            {
                startPoint = startPoint + new Size(0, h4Margin);
                cardGraphics.DrawString(i.name + " (TORP)", textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.torpTurrets + "x" + i.torpsPerTurret + "\t " + i.torpRange + "\t " + i.torpPower + "\t " + i.torpAcc + "\t " + i.torpAOE + "\t -";
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            foreach (var i in missileStats)
            {
                startPoint = startPoint + new Size(0, h4Margin);

                string weaponTitle;
                if (i.attackAir == true) { weaponTitle = i.name + " (MSL) (D/P)"; }
                else { weaponTitle = i.name + " (MSL)"; }
                cardGraphics.DrawString(weaponTitle, textFont, foregroundColor, startPoint);

                startPoint = startPoint + new Size(0, textFontMargin);
                String statBlock = "\t" + i.mslTurrets + "x" + i.mslsPerTurret + "\t " + i.mslRange + "\t " + i.mslPower + "\t " + i.mslAcc + "\t " + i.mslAOE + "\t " + i.mslEvasion;
                cardGraphics.DrawString(statBlock, h4, foregroundColor, startPoint);
            }

            // TODO: add any special abilities, if available
            //if (!String.IsNullOrEmpty(ability))
            //{
            //    startPoint = startPoint + new Size(0, h4Margin);
            //    cardGraphics.DrawString("ABILITIES:", h4, foregroundColor, startPoint);
            //
            //    startPoint = startPoint + new Size(0, h4Margin);
            //}


            //save the card
            //var curentDirectory = Directory.GetCurrentDirectory();
            String opPath = @"C:\Outputs\VorCardOutputs\planes\";
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