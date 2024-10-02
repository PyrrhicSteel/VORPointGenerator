namespace VORPointGenerator.Data.Rocket
{
    public class RocketReference
    {
        public string RocketName { get; set; } = string.Empty;
        public int RocketNumber { get; set; }
        public int RocketVolleySize { get; set; }
        /// <summary>
        /// generally a really arbitrary measurement
        /// </summary>
        public int RocketMaxRange { get; set; } 
        /// <summary>
        /// in TNT equivilant
        /// </summary>
        public int RocketWarheadSize { get; set; }
        /// <summary>
        /// reduce accuracy if true
        /// </summary>
        public bool RailDropped { get; set; } = false; 
    }
}