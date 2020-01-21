using System.Text.RegularExpressions;

namespace SedDollarBot
{
    public class Substitution
    {
        public string Pattern { get; set; }
        
        public string Replacement { get; set; }
        
        public RegexOptions RegexOptions { get; set; }
    }
}