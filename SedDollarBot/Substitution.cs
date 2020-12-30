using System.Text.RegularExpressions;

namespace SedDollarBot
{
    public class Substitution
    {
        public long ChatId { get; set; }
        
        public string Pattern { get; set; }

        public string Replacement { get; set; }

        public RegexOptions Options { get; set; }
        
        public bool DeleteAfter { get; set; }

        public override string ToString()
        {
            return $"s/{Pattern}/{Replacement}/{OptionsToFlags()}";
        }

        private string OptionsToFlags() 
        {
            var res = "a";

            if ((Options & RegexOptions.Multiline) != 0)
            {
                res += "m";
            }

            if ((Options & RegexOptions.IgnoreCase) != 0)
            {
                res += "i";
            }

            return res;
        }
    }
}