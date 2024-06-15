namespace VORPointGenerator
{
    public class gunRefrence
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public int calibre { get; set; } // in mm. 
        public int maxRange { get; set; } // in m, as per navweps
        public int armorPenetration { get; set; }  // armor penetratin of side armor at 3660m, in mm
        public int fireRate { get; set; } // for adding to accuracy. in rounds per minute
        public bool attackAir { get; set; } = false;
        public bool laser { get; set; } = false;
        public bool attackSea { get; set; } = true;
        public string orginCountry = string.Empty;
    }
}
