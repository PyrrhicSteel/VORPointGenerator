namespace VORPointGenerator.Data.Torpedo
{
    public class TorpedoStats
    {
        public string Name { get; set; } = string.Empty;
        public int TorpTurrets { get; set; }
        public int TorpsPerTurret { get; set; }
        public int TorpRange { get; set; }
        public int TorpPower { get; set; }
        public int TorpAcc { get; set; }
        public int TorpAOE { get; set; }
        public int TorpCharges { get; set; }
    }
}