namespace VORPointGenerator.Data.Torpedo
{
    public class TorpedoBatReference
    {
        public int TorpReferenceID { get; set; }
        public int TurretCount { get; set; }
        public int TorpsPerTurret { get; set; }
        /// <summary>
        /// number of reloads
        /// </summary>
        public int TorpReloads { get; set; } 
        public bool TorpedoDirector { get; set; } = false;
    }
}