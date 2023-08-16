namespace VORPointGenerator
{
    public class torpedoBatRefrence
    {
        public int torpRefrenceID { get; set; }
        public int turretCount { get; set; }
        public int torpsPerTurret { get; set; }
        public int torpReloads { get; set; } // number of reloads
        public bool torpedoDirector { get; set; } = false;
    }
}