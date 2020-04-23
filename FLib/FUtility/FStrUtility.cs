using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FLib.FUtility
{
    public class FStrUtility
    {
        /// <summary>
        /// return a string list from pattern matching
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> getRegex(string pattern, string input)
        {
            //** VARS
            List<string> result = null;
            //** INIT
            if ((pattern.Length == 0) || (input.Length == 0)) return null;
            //** CODE
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(input);


            if ((match.Groups == null) || (match.Groups.Count == 0)) return null;
            result = new List<string>();
            for (int i = 1; i <= match.Groups.Count; i++)
            {
                result.Add(match.Groups[i].Value.ToString());
            }
            return result;
        }


      



        public static string getFirstRegex(string pattern, string input)
        {
            if ((input == null) || (input.Length) == 0) return "";
            List<string> res = getRegex(pattern, input);
            if (res.Count == 0) return "";
            return res[0];
        }

        public static string getAllRegex(string pattern, string input, string interRegex = " ")
        {
            List<string> res = getRegex(pattern, input);
            if (res.Count == 0) return "";
            string result = "";
            for (int i = 0; i < res.Count; i++)
            {
                result += (result.Length > 0 ? interRegex : "");
                result += res[i];
            }
            return result;
        }

        public static string getNoCmtRegex(string input)
        {
            var CMT_REGEX = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
            string result = Regex.Replace(input, CMT_REGEX, "$1");
            return result;
        }

        public static string getFirstLine(string input)
        {
            if ((input == null) || (input.Length == 0)) return "";
            if (input.IndexOf(Environment.NewLine) < 0) return input;
            string result = input.Substring(0, input.IndexOf(Environment.NewLine));
            return result;
        }

        public static string getNotFirstLine(string input, int linesCount = 1)
        {
            if ((input == null) || (input.Length == 0)) return "";
            if (input.IndexOf(Environment.NewLine) < 0) return "";

            var lines = Regex.Split(input, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());

        }

        public static string getRemoveNewLine(string input)
        {
            if ((input == null) || (input.Length == 0)) return "";
            if (input.IndexOf(Environment.NewLine) < 0) return input;


            return input.Replace(Environment.NewLine, "");

        }

        public static int getNumLines(string input)
        {
            if ((input == null) || (input.Length == 0)) return 0;
            if (input.IndexOf(Environment.NewLine) < 0) return 1;
            int result = input.Split('\n').Length;
            return result;
        }

        public static int getNumSubString(string substring, string input)
        {
            int result = new Regex(Regex.Escape(substring)).Matches(input).Count;

            return result;
        }

        public static string getLineNum(string input, int linesCount)
        {
            if ((input == null) || (input.Length == 0)) return "";
            //if (input.IndexOf(Environment.NewLine) < 0) return ;

            string result = getNotFirstLine(input, linesCount);
            result = getFirstLine(result);
            return result;
        }

        public static string removeStartStopApex(string input)
        {
            if ((input.StartsWith("\"")) || (input.StartsWith("\'"))) input = input.Substring(1);
            if (((input.EndsWith("\"")) || (input.EndsWith("\'"))) && (input.Length > 1)) input = input.Substring(0,input.Length - 1);
            return input;
        }

        public static string getPlainFromHtml(string input)
        {
            string html = System.Web.HttpUtility.HtmlDecode(input);
            Regex objRegExp = new Regex("<(.|\n)+?>");
            string plainText = objRegExp.Replace(html, "");
            return plainText;
        }

        public static string getHtmlFromPlain(string input)
        {
            string html = System.Web.HttpUtility.HtmlEncode(input);
            html = html.Replace("\r\n", "<br>");
            html = html.Replace("\n", "<br>");
            
            return html;
        }


    }
}
