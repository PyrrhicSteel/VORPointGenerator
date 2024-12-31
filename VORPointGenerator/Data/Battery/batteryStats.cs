
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

        internal void CombineBattery(BatteryStats statBlock)
        {
            Name = Name + " & "+ statBlock.Name;
            Range = Math.Max(Range, statBlock.Range);
            Power = Math.Max(Power, statBlock.Power);
            Accuracy = (int)Math.Round((Accuracy + statBlock.Accuracy) / 1.25);

            // This will never be relevant lol
            Laser = Laser | statBlock.Laser;
        }
    }
}