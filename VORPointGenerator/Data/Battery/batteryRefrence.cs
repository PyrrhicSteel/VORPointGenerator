 
namespace VORPointGenerator.Data.Battery
{
    /// <summary>
    /// used to pull gunrefrence data
    /// </summary>
    public class BatteryReference
    {
        /// <summary>
        /// id for the gun
        /// </summary>
        public int GunReferenceID { get; set; }
        public int TurretCount { get; set; }
        /// <summary>
        /// for subtracting from accuracy
        /// </summary>
        public int GunsPerTurret { get; set; } 

        // set of booleans to determine accuracy.
        // False by default so that more can be added 
        public bool localOpticalDirector = false; // local fire control
        public bool localRadarDirector = false;
        public bool opticalDirector = false; //centralized fire control
        public bool radarDirector = false;
        public bool rangefinder = false;
        public bool radarRangeFinder = false;
        public bool radarBlindFire = false; //only on us and british ships
        public bool mechanicalComputer = false; // i.e. iowa
        public bool digitalComputer = false; //modern weapons
        public bool airburstFuses = false; //most flak weapons
        public bool radarFuses = false; //vt fuses and some modern weapons. Used in conjunction with airburst fuses
        public bool poorShellQuaility = false;
        public bool CWISTracking = false;
    }
}