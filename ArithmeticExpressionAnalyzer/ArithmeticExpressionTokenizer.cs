using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArithmeticExpressionAnalyzer
{
    public class ArithmeticExpressionTokenizer
    {
        public static string[]? Funstions { get; private set; }
        public static string? FloatDigins { get; private set; }
        public static string? Digins { get; private set; }
        public static string? Variables { get; private set; }
        public static string[]? Operations { get; private set; }
        public static string? OpenBrake { get; private set; }
        public static string? CloseBrake { get; private set; }

        private readonly string pattern;

        public ArithmeticExpressionTokenizer()
        {
            Funstions = [ "sin", "cos", "tg", "func" ];
            Operations = [ "+", "-", "*", "/"];
            FloatDigins = @"\d+\.\d+";
            Digins = @"\d+";
            Variables = @"\b[_@]?[a-zA-Z][a-zA-Z0-9_]*\b";
            OpenBrake = @"\(";
            CloseBrake = @"\)";
            pattern = @$"({string.Join('|', Funstions)}|{FloatDigins}|{Digins}|{Variables}|{string.Join('|', Operations.Select(o => @"\" + o))}|{OpenBrake}|{CloseBrake})";
        }

        public List<Token> Tokenize(string input)
        {
            List<Token> tokens = new List<Token>();
            MatchCollection matches = Regex.Matches(input, pattern);

            int lastIndex = 0;
            foreach (Match match in matches)
            {
                if (lastIndex < match.Index)
                {
                    string invalidToken = input.Substring(lastIndex, match.Index - lastIndex);
                    tokens.Add(new Token(lastIndex, invalidToken.Trim()));
                }
                lastIndex = match.Index + match.Length;
                tokens.Add(new Token(match.Index, match.Value));
            }

            if (lastIndex < input.Length)
            {
                string invalidToken = input.Substring(lastIndex).Trim();
                if (!string.IsNullOrEmpty(invalidToken))
                {
                    tokens.Add(new Token(lastIndex, invalidToken));
                }
            }

            return tokens;
        }

        public static bool IsOperator(string token)
        {
            return Regex.IsMatch(token, $@"{string.Join('|', Operations.Select(o => @"\" + o))}");
        }

        public static bool IsNumber(string token)
        {
            return Regex.IsMatch(token, $@"^({FloatDigins}|{Digins})$");
        }

        public static bool IsVariable(string token)
        {
            return Regex.IsMatch(token, $@"{Variables}");
        }

        public static bool IsFunction(string token)
        {
            return Regex.IsMatch(token, $@"\b(?:{string.Join('|', Funstions)})\b");
        }

        public static TokenType CheckTokenType(string token)
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
