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
        public static List<string> Calculation { get; private set; }

        public static List<Token> Optimize(List<Token> tokens)
        {
            Optimised = new();
            Calculation = new();
            _tokens = tokens;

            OpimizeWrapper(isOperation, ZeroCalc);
            OpimizeWrapper(isOperation, ZeroOneCalc);
            OpimizeWrapper(isMul, Calc);
            OpimizeWrapper(isAdd, Calc);

            Token? previous = null;
            Token? next = null;
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

        private static void OpimizeWrapper (Predicate<Token> predicate, Func<Token, Token, Token,int> opimise)
        {
            Token? previous = null;
            Token? next = null;
            for (int i = 0; i < _tokens.Count; i++)
            {
                var token = _tokens[i];
                next = (i + 1 < _tokens.Count) ? _tokens[i + 1] : null;
                if (predicate.Invoke(token))
                    i -= opimise.Invoke(token, previous, next);

                i = i >= _tokens.Count || i < 0 ? _tokens.Count - 1 : i;
                previous = i < 0 ? null : _tokens[i];
            }
        }

        private static bool isOperation(Token token)
        {
            var tokenType = ArithmeticExpressionTokenizer.CheckTokenType(token.Value);
            return tokenType == TokenType.operation;
        }

        private static bool isMul(Token token)
        {
            return token.Value == "*" ||
                    token.Value == "/";
        }
        private static bool isAdd(Token token)
        {
            return token.Value == "-" ||
                    token.Value == "+";
        }

        private static int Calc(Token? token, Token? previousToken, Token? nextToken)
        {
            if (previousToken!=null && ArithmeticExpressionTokenizer.CheckTokenType(previousToken?.Value) == TokenType.digit &&
                nextToken != null && ArithmeticExpressionTokenizer.CheckTokenType(nextToken?.Value) == TokenType.digit)
            {
                var removeStart = _tokens.IndexOf(previousToken);
                var removeEnd = _tokens.IndexOf(nextToken);
                var left = Double.Parse(previousToken.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                var right = Double.Parse(nextToken.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

  

                if (_tokens.Count > (removeEnd + 1) &&
                    (token.Value == "+" || token.Value == "-") &&
                    (_tokens[removeEnd + 1].Value == "*" || _tokens[removeEnd + 1].Value == "/"))
                    return 0;
                else if (_tokens.Count > (removeEnd + 1) &&
                    (token.Value == "*" || token.Value == "/") 
                     )
                    return 0;

                if(removeStart-1>=0 && _tokens[removeStart - 1].Value == "-")
                {
                    left *= -1;
                    _tokens[removeStart - 1].Value = "+";
                }
                double res = 0;
                res = token.Value switch
                {
                    "*" => left * right,
                    "/" => left / right,
                    "+" => left + right,
                    "-" => left - right,
                };

                _tokens.RemoveRange(removeStart, removeEnd - removeStart + 1);
                Calculation.Add($"{left} {token.Value} {right} = {res}");
                if (res < 0 && removeStart - 1 >= 0 && _tokens[removeStart - 1].Value == "+")
                {
                    _tokens[removeStart - 1].Value = "-";
                    res *= -1;
                }

                _tokens.Insert(removeStart, new Token(removeStart, res.ToString(System.Globalization.CultureInfo.InvariantCulture)));
                WriteOptimisation(OpimizeType.CalcConst);
                return 2;
            }

            return 0;
        }

        private static int ZeroOneCalc(Token? token, Token? previousToken, Token? nextToken)
        {
            if (token?.Value == "*" && (previousToken?.Value == "1" || nextToken?.Value == "1") ||
                (token?.Value == "/" && nextToken?.Value == "1") ||
                ((token?.Value == "-" || token?.Value == "+") && (previousToken?.Value == "0" || nextToken?.Value == "0") && previousToken?.Value != null)
                )
            {
                var i = _tokens.IndexOf(token);
                var removeStart = _tokens.IndexOf(token);
                var removeEnd = _tokens.IndexOf(token);

                if (previousToken.Value == "1" || previousToken?.Value == "0")
                {
                    removeStart = _tokens.IndexOf(previousToken);

                    if (_tokens[removeEnd].Value == "-")
                    {
                        removeEnd--;
                    }
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

                return i - removeStart+1;
            }

            return 0;
        }

        private static int ZeroCalc(Token? token, Token? previousToken, Token? nextToken)
        {
            if ((token?.Value == "*" || token?.Value == "/") && (previousToken?.Value == "0" || nextToken?.Value == "0"))
            {
                if (token.Value == "/" && nextToken?.Value == "0")
                    throw new DivideByZeroException($"Ділення на 0, індекс {nextToken.Index}");

                var i = _tokens.IndexOf(token);
                var removeStart = i;
                var removeEnd = i;
                if (previousToken?.Value == ")")
                {
                     var previousTokenIndex = _tokens.IndexOf(previousToken);
                    var openBrake = _tokens[..previousTokenIndex].Last(token => token.Value == "(");
                    var openBrakeIndex = _tokens.IndexOf(openBrake);
                    var brakeCount = 0;

                    while (_tokens[openBrakeIndex..previousTokenIndex].Any(t => t.Value == ")"))
                    {
                        var brakeCountTemp = _tokens[openBrakeIndex..(previousTokenIndex)].Count(t => t.Value == ")") - brakeCount;
                        brakeCount += brakeCountTemp;

                        if (brakeCountTemp == 0)
                            break;

                        while (brakeCountTemp > 0)
                        {
                            openBrake = _tokens[..openBrakeIndex].Last(token => token.Value == "(");
                            openBrakeIndex = _tokens.IndexOf(openBrake);
                            brakeCountTemp--;
                        }

                    }

                    removeStart = openBrakeIndex == 0 ? 0 : openBrakeIndex - 1;
                }
                else
                    removeStart -= (removeStart==1 || _tokens[removeStart-2].Value=="(") ? 1 : 2 ;

                if (ArithmeticExpressionTokenizer.CheckTokenType(nextToken.Value) == TokenType.function)
                    nextToken = _tokens[_tokens.IndexOf(nextToken) + 1];

                if (nextToken?.Value == "(")
                {
                    var nextTokenIndex = _tokens.IndexOf(nextToken);
                    var closeBrake = _tokens[nextTokenIndex..].First(token => token.Value == ")");
                    var closeBrakeIndex = _tokens.IndexOf(closeBrake);
                    var brakeCount = 0;
                    while (_tokens[nextTokenIndex..closeBrakeIndex].Any(t => t.Value == "(")){
                        var brakeCountTemp = _tokens[(nextTokenIndex+1)..closeBrakeIndex].Count(t => t.Value == "(") - brakeCount;
                        brakeCount += brakeCountTemp;

                        if (brakeCountTemp == 0)
                            break;

                        while (brakeCountTemp > 0)
                        {
                            closeBrake = _tokens[(closeBrakeIndex+1)..].First(token => token.Value == ")");
                            closeBrakeIndex = _tokens.IndexOf(closeBrake);
                            brakeCountTemp--;
                        }

                    }

                    removeEnd = closeBrakeIndex;
                }
                else
                    removeEnd++;

                while(removeStart-1>=0 &&(_tokens[removeStart-1].Value == "/" || _tokens[removeStart-1].Value == "*"))
                {
                    removeStart -= 2;
                    if (previousToken?.Value == ")")
                    {
                        var previousTokenIndex = _tokens.IndexOf(previousToken);
                        var openBrake = _tokens[..previousTokenIndex].Last(token => token.Value == "(");
                        var openBrakeIndex = _tokens.IndexOf(openBrake);
                        removeStart = openBrakeIndex;
                    }
                }

                if (_tokens.Count > (removeEnd + 1) && _tokens[removeEnd + 1].Value == "-" && (removeStart == 0 || _tokens[removeStart].Value == "("))
                {
                    removeEnd--;
                }

                while (removeStart>0 && _tokens[removeStart-1].Value == "(" &&
                    removeEnd+1 < _tokens.Count && _tokens[removeEnd + 1].Value == ")"){
                    removeEnd++;
                    removeStart--;
                }
                
                if (_tokens.Count>(removeEnd + 1) && (_tokens[removeEnd+1].Value == "/" || _tokens[removeEnd+1].Value == "*"))
                {
                    removeEnd += 2;
                    i++;
                }

                if (_tokens.Count > (removeEnd + 1) && _tokens[removeEnd + 1].Value == "+" && (removeStart==0 || _tokens[removeStart - 1].Value == "("))
                {
                    removeEnd++;
                }

                _tokens.RemoveRange(removeStart,
                    (removeEnd - removeStart + 1)>_tokens.Count ? _tokens.Count : (removeEnd - removeStart + 1));
                if (token?.Value == "*")
                    WriteOptimisation(OpimizeType.MulZero);
                else
                    WriteOptimisation(OpimizeType.DivideZero);

                return i - removeStart +1;
            }

            return 0;
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
