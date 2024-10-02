using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORPointGenerator.Util
{
    public class SanitizeString
    {
        List<char> bannedChars = new List<char>() { '/', '>', '<', '\\', '|', '?', '*' };
        public string Sanitize(string tgt)
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
