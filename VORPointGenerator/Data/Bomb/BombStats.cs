using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator.Data.Bomb
{
    internal class BombStats
    {
        public string Name { get; set; } = string.Empty;
        public int Atk { get; set; }
        public int Volleys { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        //TODO: add range increase in case of modern guided bombs
        public int Range { get; set; } = 1;
        public bool DiveBomb { get; set; }

    }
}
