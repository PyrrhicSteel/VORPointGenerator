namespace VORPointGenerator
{
    public class missileStats
    {
        public string name { get; set; } = String.Empty;
        public int mslTurrets { get; set; }
        public int mslsPerTurret { get; set; }
        public int mslRange { get; set; }
        public int mslPower { get; set; }
        public int mslAcc { get; set; }
        public int mslAOE { get; set; }
        public int mslEvasion { get; set; }

        public bool attackAir = false;
    }
}