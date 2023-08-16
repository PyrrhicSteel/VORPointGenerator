namespace VORPointGenerator
{
    public class rocketRefrence
    {
        public string rocketName { get; set; } = string.Empty;
        public int rocketNumber { get; set; }
        public int rocketVolleySize { get; set; }
        public int rocketMaxRange { get; set; } // generally a really arbitrary measurement
        public int rocketWarheadSize { get; set; } //in TNT equivilant
        public bool railDropped { get; set; } = false; //reduce accuracy if true
    }
}