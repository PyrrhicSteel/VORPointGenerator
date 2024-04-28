using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator
{
    internal class bombStats
    {
        public string name { get; set; } = string.Empty;
        public int atk { get; set; }
        public int volleys { get; set; }
        public int power { get; set; }
        public int accuracy { get; set; }
        //TODO: add range increase in case of modern guided bombs
        public int range { get; set; } = 1;
        public bool diveBomb { get; set; }

    }
}
