namespace VORPointGenerator.Data.Gun
{
    public class GunRefrence
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// in mm. 
        /// </summary>
        public int Calibre { get; set; } 
        /// <summary>
        /// in m, as per navweps
        /// </summary>
        public int MaxRange { get; set; } 
        /// <summary>
        /// armor penetratin of side armor at 3660m, in mm
        /// </summary>
        public int ArmorPenetration { get; set; }  
        /// <summary>
        /// for adding to accuracy. in rounds per minute
        /// </summary>
        public int FireRate { get; set; } 
        public bool AttackAir { get; set; } = false;
        public bool Laser { get; set; } = false;
        public bool AttackSea { get; set; } = true;
        public string OrginCountry { get; set; } = string.Empty;
    }
}
