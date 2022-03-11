using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Moralis.Platform.Services.ClientServices
{
    public static class CommandRunnerExtensions
    {
        public static string AdjustJsonForParseDate(this string json)
        {
            string adjusted = json;

            Regex r = new Regex("{\"__type\":\"Date\",\"iso\":(\"\\d{4}-[01]\\d-[0-3]\\dT[0-2]\\d:[0-5]\\d:[0-5]\\d\\.\\d+([+-][0-2]\\d:[0-5]\\d|Z)\")}");
            MatchCollection matches = r.Matches(json);

            // Use foreach-loop.
            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 2)
                {
                    adjusted = adjusted.Replace(match.Groups[0].Value, match.Groups[1].Value);
                }
            }

            return adjusted;
        }


        public static string JsonInsertParseDate(this string json)
        {
            string adjusted = json;

            Regex r = new Regex("(\"\\d{4}-[01]\\d-[0-3]\\dT[0-2]\\d:[0-5]\\d:[0-5]\\d\\.\\d+([+-][0-2]\\d:[0-5]\\d|Z)\")");
            MatchCollection matches = r.Matches(json);

            // Use foreach-loop.
            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 1)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{\"__type\":\"Date\",\"iso\":");
                    sb.Append(match.Groups[0].Value);
                    sb.Append("}");
                    
                    adjusted = adjusted.Replace(match.Groups[0].Value, sb.ToString());
                }
            }

            return adjusted;
        }
    }
}
