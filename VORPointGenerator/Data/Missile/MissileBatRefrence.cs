namespace VORPointGenerator.Data.Missile
{
    public class MissileBatReference
    {
        public int MissileReferenceID { get; set; }
        public int TurretCount { get; set; }
        public int MissilesPerTurret { get; set; }
        public bool DataLink { get; set; } = false;
    }
}