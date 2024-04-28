namespace VORPointGenerator
{
    public class bombRefrence
    {
        public string name { get; set; } = string.Empty;
        public int number { get; set; }
        public int bombVolleySize { get; set; }
        public int bombWarheadSize { get; set; } // in TNT equivilant
        public bool laserGuidance { get; set; } = false;
        public bool diveBomb { get; set; } = false;
    }
}