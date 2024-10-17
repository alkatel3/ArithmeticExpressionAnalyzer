namespace ArithmeticExpressionAnalyzer
{
    internal class ArithmeticExpressionValidator
    {
        private static Dictionary<TokenType, Func<string, string, (bool, string)>> TokensValidation = new Dictionary<TokenType, Func<string, string, (bool, string)>>
        {
            [TokenType.digit] = IsOperandPositionValid,
            [TokenType.operation] = IsOperationPositionValid,
            [TokenType.function] = IsOperandPositionValid,
            [TokenType.variable] = IsOperandPositionValid,
            [TokenType.openBrake] = IsOpenBrakePositionValid,
            [TokenType.closeBrake] = IsCloseBrakePositionValid,
            [TokenType.unknown] = (item1, item2) => (false, $"Не типовий символ '{item1}'")
        };

        static List<(int index, string token, string message)> errors = new List<(int index, string token, string message)>();
        //static List<(int, string)> errorsMessages = new List<(int, string)>();
        static Stack<int> brakes = new Stack<int>();

        public static List<(int index, string token, string message)> Validate(List<Token> tokens)
        {
            errors.Clear();
            string? previous = null;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                var tokenType = ArithmeticExpressionTokenizer.CheckTokenType(token.Value);
                (bool res, string error) isValid;

                if (tokenType == TokenType.openBrake)
                    brakes.Push(token.Index);

                isValid = TokensValidation[tokenType].Invoke(token.Value, previous);

                if (!isValid.res)
                {
                    errors.Add((token.Index, token.Value, isValid.error));
                    //errorsMessages.Add((token.index, isValid.error));
                }

                previous = token.Value;
            }

            if (brakes.Any())
                foreach (var item in brakes)
                {
                    errors.Add((item, "(", "Лишня відкриваюча дужка"));
                    //errorsMessages.Add((item, "Лишня відкриваюча дужка"));
                }

            errors = errors.Distinct().OrderBy(e => e.Item1).ToList();

            return errors;
        }

        public static (bool res, string error) IsOperandPositionValid(string token, string? previousToken)
        {
            if (String.IsNullOrEmpty(previousToken))
                return (true, "");

            var previousTokenType = ArithmeticExpressionTokenizer.CheckTokenType(previousToken);

            return (previousTokenType == TokenType.operation ||
                previousTokenType == TokenType.openBrake, $"'{token}' не може стояти після '{previousToken}'");
        }

        public static (bool res, string error) IsOperationPositionValid(string token, string? previousToken)
        {
            if (String.IsNullOrEmpty(previousToken) && (token == "+" || token == "-"))
                return (true, "");
            else if (String.IsNullOrEmpty(previousToken))
                return (false, $"Вираз не може починатись з '{token}'");
            else if (previousToken == "(" && token == "-")
                return (true, "");

            var previousTokenType = ArithmeticExpressionTokenizer.CheckTokenType(previousToken);

            return (previousTokenType == TokenType.digit ||
                previousTokenType == TokenType.variable ||
                previousTokenType == TokenType.closeBrake, $"'{token}' не може стояти після '{previousToken}'");
        }

        public static (bool res, string error) IsOpenBrakePositionValid(string token, string? previousToken)
        {
            if (token != "(")
                throw new ArgumentException();

            if (String.IsNullOrEmpty(previousToken))
                return (true, "");

            var previousTokenType = ArithmeticExpressionTokenizer.CheckTokenType(previousToken);

            return (previousTokenType == TokenType.operation ||
                previousTokenType == TokenType.function ||
                previousTokenType == TokenType.openBrake, $"'{token}' не може стояти після '{previousToken}'");
        }

        public static (bool res, string error) IsCloseBrakePositionValid(string token, string? previousToken)
        {
            if (token != ")")
                throw new ArgumentException();

            if (String.IsNullOrEmpty(previousToken))
                return (false, $"Вираз не може починатись з '{token}'");

            if (!brakes.Any())
            {
                return (false, "Лишня закриваюча дужка");
            }
            else
            {
                brakes.Pop();
            }

            var previousTokenType = ArithmeticExpressionTokenizer.CheckTokenType(previousToken);


            return (previousTokenType == TokenType.digit ||
                previousTokenType == TokenType.variable ||
                previousTokenType == TokenType.closeBrake, $"'{token}' не може стояти після '{previousToken}'");

        }
    }
}
