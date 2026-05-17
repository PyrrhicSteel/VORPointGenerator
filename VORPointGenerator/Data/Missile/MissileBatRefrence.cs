namespace VORPointGenerator.Data.Missile
{
    public class MissileBatReference
    {
        public int MissileReferenceID { get; set; }
        // TODO: Dummy TurretCount and MissilesPerTurret out, replace with 'MissileCount' and 'VolleySize.'
        public int TurretCount { get; set; }
        public int MissilesPerTurret { get; set; }
        public bool DataLink { get; set; } = false;

        public int MissileCount { get; set; }
        public int VolleySize { get; set; } = 0;

        
    }
}