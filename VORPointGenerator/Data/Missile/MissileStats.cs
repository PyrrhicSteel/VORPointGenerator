namespace VORPointGenerator.Data.Missile
{
    public class MissileStats
    {
        public string Name { get; set; } = string.Empty;
        public int MslTurrets { get; set; }
        public int MslsPerTurret { get; set; }
        public int MslRange { get; set; }
        public int MslPower { get; set; }
        public int MslAcc { get; set; }
        public int MslAOE { get; set; }
        public int MslEvasion { get; set; }

        public bool AttackAir { get; set; } = false;
    }
}