namespace VORPointGenerator.Data.Bomb
{
    public class BombReference
    {
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
        public int BombVolleySize { get; set; }
        /// <summary>
        /// in TNT equivilant
        /// </summary>
        public int BombWarheadSize { get; set; } 
        public bool LaserGuidance { get; set; } = false;
        public bool DiveBomb { get; set; } = false;
    }
}