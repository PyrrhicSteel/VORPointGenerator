namespace VORPointGenerator
{
    public class batteryStats
    {
        public string name { get; set; } = string.Empty;
        public int turrets { get; set; }
        public int gunsPerTurret { get; set; }
        public int range { get; set; }
        public int power { get; set; }
        public int accuracy { get; set; }
        public bool attackAir { get; set; }
        public bool laser { get; set; }

        //apply misc balance fixes
        internal void applyFixes()
        {
            
            while((turrets * gunsPerTurret) > 20)
            {
                turrets = turrets / 2;
                accuracy++;
            }

            if(range == 0)
            {
                range = 1;
            }

        }
    }
}