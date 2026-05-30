namespace VORPointGenerator.Data.Missile
{
    public class MissileBatReference
    {
        public int MissileReferenceID { get; set; }
        // TODO: Dummy TurretCount and MissileCount out, replace with 'MissileCount' and 'VolleySize.'
        public int TurretCount { get; set; } // TODO: Replace with LauncherCount, to assist in determining volleysize
        public int MissileCount { get; set; } // TODO: The total stock of missiles on the platform
        public bool DataLink { get; set; } = false;


        public int VolleySize { get; set; } = 0; // The product of LauncherCount and LauncherMethod
        public string LaunchMethod {  get; set; }
        /** Couldn't get an enum to work, so here are the options:
            * HARDPOINT - Launcher dropped from aircraft, 4X attacks per turn
            * BOMBBAY - Missiles stored in an internal bomb bay 4X attacks per bomb bay
            * VLS - Missiles stored in VLS tubes 10X attacks per VLS bank
            * BOX - Missiles stored on an internal box launcher 2X attacks per launcher
            * SINGLEARM - Launcher fires missiles in single attacks AKA MANPADS, 1X attack per turn
            * CWIS - Launcher designed to rapidly ripple-fire missiles. 15X attacks per turn
        **/
    }
}