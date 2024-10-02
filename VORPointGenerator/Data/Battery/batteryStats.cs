namespace VORPointGenerator.Data.Battery
{
    public class BatteryStats
    {
        public string Name { get; set; } = string.Empty;
        public int Turrets { get; set; }
        public int GunsPerTurret { get; set; }
        public int Range { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public bool AttackAir { get; set; }
        public bool Laser { get; set; }

        /// <summary>
        /// apply misc balance fixes
        /// </summary>
        internal void ApplyFixes()
        {

            while (Turrets * GunsPerTurret > 20)
            {
                Turrets = Turrets / 2;
                Accuracy++;
            }

            if (Range == 0)
            {
                Range = 1;
            }

        }
    }
}