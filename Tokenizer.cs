using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArithmeticExpressionAnalyzer
{
    public class Tokenizer
    {
        public string[] Funstions { get; set; }
        public string FloatDigins { get; set; }
        public string Digins { get; set; }
        public string Variables { get; set; }
        public string[] Operations { get; set; }
        public string OpenBrake { get; set; }
        public string CloseBrake { get; set; }

        private readonly string pattern;

        public Tokenizer()
        {
            Funstions = [ "sin", "cos", "tg" ];
            Operations = [ "+", "-", "*", "/", "^" ];
            FloatDigins = @"\d+\.\d+";
            Digins = @"\d+";
            Variables = @"[a-zA-Z]";
            OpenBrake = @"\(";
            CloseBrake = @"\)";
            pattern = @$"({string.Join('|', Funstions)}|{FloatDigins}|{Digins}|{Variables}|{string.Join('|', Operations.Select(o => @"\" + o))}|{OpenBrake}|{CloseBrake})";
        }

        public List<(int index, string value)> Tokenize(string input)
        {
            List<(int, string)> tokens = new List<(int, string)>();
            MatchCollection matches = Regex.Matches(input, pattern);

            int lastIndex = 0;
            foreach (Match match in matches)
            {
                if (lastIndex < match.Index)
                {
                    string invalidToken = input.Substring(lastIndex, match.Index - lastIndex);
                    tokens.Add((lastIndex, invalidToken.Trim()));
                }
                lastIndex = match.Index + match.Length;
                tokens.Add((match.Index, match.Value));
            }

            if (lastIndex < input.Length)
            {
                string invalidToken = input.Substring(lastIndex).Trim();
                if (!string.IsNullOrEmpty(invalidToken))
                {
                    tokens.Add((lastIndex, invalidToken));
                }
            }

            return tokens;
        }

        public bool IsOperator(string token)
        {
            return Regex.IsMatch(token, $@"{string.Join('|', Operations.Select(o => @"\" + o))}");
        }

        public bool IsNumber(string token)
        {
            return Regex.IsMatch(token, $@"{FloatDigins}|{Digins}");
        }

        public bool IsVariable(string token)
        {
            return Regex.IsMatch(token, $@"{Variables}");
        }

        private bool IsFunction(string token)
        {
            return Regex.IsMatch(token, $@"\b(?:{string.Join('|', Funstions)})\b");
        }

        public TokenType CheckTokenType(string token)
        {
            return IsNumber(token) ? TokenType.digit :
                IsFunction(token) ? TokenType.function :
                IsOperator(token) ? TokenType.operation :
                IsVariable(token) ? TokenType.variable :
                token == "(" ? TokenType.openBrake :
                token == ")" ? TokenType.closeBrake : TokenType.unknown;
        }
    }
}
