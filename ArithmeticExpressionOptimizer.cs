namespace ArithmeticExpressionAnalyzer
{
    public enum OpimizeType
    {
        AddingZero,
        DivideZero,
        SubZero, 
        MulZero,
        MulOne,
        DividedOne,
        UnarityMinus,
        CalcConst,
    }

    public class ArithmeticExpressionOptimizer
    {
        private static List<Token> _tokens;
        public static Dictionary<OpimizeType, int> Optimised { get; private set; }

        public static List<Token> Optimize(List<Token> tokens)
        {
            Optimised = new();
            _tokens = tokens;

            Token? previous = null;
            Token? next = null;
            for (int i = 0; i < _tokens.Count; i++)
            {
                var token = tokens[i];
                next = (i+1<tokens.Count) ? tokens[i+1] : null;
                var tokenType = ArithmeticExpressionTokenizer.CheckTokenType(token.Value);
                var op = false;
                if(tokenType == TokenType.operation) 
                    op = ZeroCalc(token, previous, next);

                if (op)
                {
                    previous = null;
                    next = null;
                    i = -1;
                    continue;
                }

                previous = token;
            }

            previous = null;
            next = null;
            for (int i = 0; i < _tokens.Count; i++)
            {
                var token = tokens[i];
                next = (i + 1 < tokens.Count) ? tokens[i + 1] : null;
                var tokenType = ArithmeticExpressionTokenizer.CheckTokenType(token.Value);
                var op = false;
                if (tokenType == TokenType.operation)
                    op = ZeroOneCalc(token, previous, next);

                if (op)
                {
                    previous = null;
                    next = null;
                    i = -1;
                    continue;
                }
                previous = token;
            }

            previous = null;
            next = null;
            for (int i = 0; i < _tokens.Count; i++)
            {
                var token = tokens[i];
                next = (i + 1 < tokens.Count) ? tokens[i + 1] : null;
                var op = false;
                if (token.Value == "*" ||
                    token.Value == "^" ||
                    token.Value == "/")
                    op = Calc(token, previous, next, 0);

                if (op)
                {
                    previous = null;
                    next = null;
                    i = -1;
                    continue;
                }
                previous = token;
            }

            previous = null;
            next = null;
            for (int i = 0; i < _tokens.Count; i++)
            {
                var token = tokens[i];
                next = (i + 1 < tokens.Count) ? tokens[i + 1] : null;
                var op = false;
                if (token.Value == "-" ||
                    token.Value == "+")
                    op = Calc(token, previous, next, 1);

                if (op)
                {
                    previous = null;
                    next = null;
                    i = -1;
                    continue;
                }
                previous = token;
            }

            previous = null;
            next = null;
            for (int i = 0; i < _tokens.Count; i++)
            {
                var token = tokens[i];
                next = (i + 1 < tokens.Count) ? tokens[i + 1] : null;
                if (token.Value == "-" && (previous == null || previous.Value == "("))
                {
                    _tokens.Insert(i, new Token(i, "0"));
                    WriteOptimisation(OpimizeType.UnarityMinus);
                }

                previous = token;
            }

            return _tokens;
        }

        private static bool Calc(Token? token, Token? previousToken, Token? nextToken, int iteration)
        {
            if (previousToken!=null && ArithmeticExpressionTokenizer.CheckTokenType(previousToken?.Value) == TokenType.digit &&
                nextToken != null && ArithmeticExpressionTokenizer.CheckTokenType(nextToken?.Value) == TokenType.digit)
            {
                var removeStart = _tokens.IndexOf(previousToken);
                var removeEnd = _tokens.IndexOf(nextToken);
                var left = Double.Parse(previousToken.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                var right = Double.Parse(nextToken.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

                if(removeStart-1>=0 && _tokens[removeStart - 1].Value == "-")
                {
                    left *= -1;
                    _tokens[removeStart - 1].Value = "+";
                }

                double res = 0;

                if (iteration == 0)
                {
                    res = token.Value switch
                    {
                        "*" => left * right,
                        "/" => left / right,
                        "^" => Math.Pow(left, right),
                    };
                }
                else
                {
                    res = token.Value switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                    };
                }

                _tokens.RemoveRange(removeStart, removeEnd - removeStart + 1);

                if(res < 0 && removeStart - 1 >= 0 && _tokens[removeStart - 1].Value == "+")
                {
                    _tokens[removeStart - 1].Value = "-";
                    res *= -1;
                }

                _tokens.Insert(removeStart, new Token(removeStart, res.ToString(System.Globalization.CultureInfo.InvariantCulture)));
                WriteOptimisation(OpimizeType.CalcConst);

                return true;
            }

            return false;
        }

        private static bool ZeroOneCalc(Token? token, Token? previousToken, Token? nextToken)
        {
            if (token?.Value == "*" && (previousToken?.Value == "1" || nextToken?.Value == "1") ||
                (token?.Value == "/" && nextToken?.Value == "1") ||
                ((token?.Value == "-" || token?.Value == "+") && (previousToken?.Value == "0" || nextToken?.Value == "0") && previousToken?.Value != null)
                )
            {
                var removeStart = _tokens.IndexOf(token);
                var removeEnd = _tokens.IndexOf(token);

                if (previousToken.Value == "1" || previousToken?.Value == "0")
                {
                    removeStart = _tokens.IndexOf(previousToken);
                }
                else if (nextToken?.Value == "1" || nextToken?.Value == "0")
                {
                    removeEnd = _tokens.IndexOf(nextToken);
                }

                _tokens.RemoveRange(removeStart, removeEnd - removeStart + 1);

                switch(token?.Value)
                {
                    case "*":
                        WriteOptimisation(OpimizeType.MulOne);
                        break;
                    case "/":
                        WriteOptimisation(OpimizeType.DividedOne);
                        break;
                    case "-":
                        WriteOptimisation(OpimizeType.AddingZero);
                        break;
                    case "+":
                        WriteOptimisation(OpimizeType.AddingZero);
                        break;
                    default:
                        break;
                };

                return true;
            }

            return false;
        }

        private static bool ZeroCalc(Token? token, Token? previousToken, Token? nextToken)
        {
            if ((token?.Value == "*" || token?.Value == "/") && (previousToken?.Value == "0" || nextToken?.Value == "0"))
            {
                if (token.Value == "/" && nextToken?.Value == "0")
                    throw new DivideByZeroException($"Ділення на 0, індекс {nextToken.Index}");

                var removeStart = _tokens.IndexOf(token);
                var removeEnd = _tokens.IndexOf(token);
                if (previousToken?.Value == ")")
                {
                    var previousTokenIndex = _tokens.IndexOf(previousToken);
                    var openBrake = _tokens[..previousTokenIndex].Last(token => token.Value == "(");
                    var openBrakeIndex = _tokens.IndexOf(openBrake);
                    removeStart = openBrakeIndex;
                }
                else
                    removeStart -= 1;

                if (ArithmeticExpressionTokenizer.CheckTokenType(nextToken.Value) == TokenType.function)
                    nextToken = _tokens[_tokens.IndexOf(nextToken) + 1];

                if (nextToken?.Value == "(")
                {
                    var nextTokenIndex = _tokens.IndexOf(nextToken);
                    var closeBrake = _tokens[nextTokenIndex..].First(token => token.Value == ")");
                    var closeBrakeIndex = _tokens.IndexOf(closeBrake);
                    removeEnd = closeBrakeIndex;
                }
                else
                    removeEnd++;

                var newTokenIndex = _tokens[removeStart].Index;
                _tokens.RemoveRange(removeStart, removeEnd - removeStart + 1);
                _tokens.Insert(removeStart, new Token(newTokenIndex, "0"));
                if (token?.Value == "*")
                    WriteOptimisation(OpimizeType.MulZero);
                else
                    WriteOptimisation(OpimizeType.DivideZero);

                return true;
            }

            return false;
        }

        private static void WriteOptimisation(OpimizeType opimizeType)
        {
            if (Optimised.ContainsKey(opimizeType))
                Optimised[opimizeType]++;
            else
                Optimised[opimizeType] = 1;
        }
    }
}
