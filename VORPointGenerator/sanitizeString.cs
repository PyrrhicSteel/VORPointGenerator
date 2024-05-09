using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator
{
    public class sanitizeString
    {
        List<char> bannedChars = new List<char>() { '/', '>', '<', '\\', '|', '?', '*' };
        public string sanitize(string tgt)
        {
            string fixedString = string.Empty;
            if (tgt == null)
            {
                return tgt;
            }
            fixedString = tgt;
            foreach (char c in bannedChars)
            {
                fixedString = fixedString.Replace(c, ' ');
            }
            return fixedString;
        }
    }
}
