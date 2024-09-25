namespace ArithmeticExpressionAnalyzer
{
    public enum TokenType
    {
        function,
        operation, 
        digit,
        variable,
        openBrake,
        closeBrake,
        unknown
    }

    internal class Program
    {
        static Tokenizer tokenizer = new Tokenizer();
        static Dictionary<TokenType, Func<string, string, (bool, string)>> TokensValidation = new Dictionary<TokenType, Func<string, string, (bool, string)>>
        {
            [TokenType.digit] = IsOperandPositionValid,
            [TokenType.operation] = IsOperationPositionValid,
            [TokenType.function] = IsOperandPositionValid,
            [TokenType.variable] = IsOperandPositionValid,
            [TokenType.openBrake] = IsOpenBrakePositionValid,
            [TokenType.closeBrake] = IsCloseBrakePositionValid,
            [TokenType.unknown] = (item1, item2) => (false, $"Не типовий символ '{item1}'")
        };
        static List<(int, string)> errors = new List<(int, string)>();
        static List<(int, string)> errorsMessages= new List<(int, string)>();
        static Stack<int> brakes = new Stack<int>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("Введіть вираз:");
                var exp = Console.ReadLine();
                exp = exp.Replace(" ", "");
                var res = tokenizer.Tokenize(exp);
                string? previous = null;
                for (int i = 0; i < res.Count; i++)
                {
                    var token = res[i];
                    var tokenType = tokenizer.CheckTokenType(token.value);
                    (bool res, string error) isValid;

                    if (tokenType == TokenType.openBrake)
                        brakes.Push(token.index);

                    isValid = TokensValidation[tokenType].Invoke(token.value, previous);

                    if (!isValid.res)
                    {
                        errors.Add(token);
                        errorsMessages.Add((token.index, isValid.error));
                    }

                    previous = token.value;
                }

                if (brakes.Any())
                    foreach (var item in brakes)
                    {
                        errors.Add((item, "("));
                        errorsMessages.Add((item, "Лишня відкриваюча дужка"));
                    }

                errors = errors.Distinct().OrderBy(e => e.Item1).ToList();
                var lastIndex = 0;
                Console.WriteLine("---------------------------------------");
                foreach (var error in errors)
                {
                    Console.Write(exp.Substring(lastIndex, error.Item1 - lastIndex));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(exp.Substring(error.Item1, error.Item2.Length));
                    Console.ResetColor();
                    lastIndex = error.Item1 + error.Item2.Length;
                }

                Console.WriteLine(exp.Substring(lastIndex));

                errorsMessages = errorsMessages.OrderBy(e => e.Item1).ToList();
                foreach (var errorsMessage in errorsMessages)
                {
                    Console.WriteLine($"Error: \"{errorsMessage.Item2}\"; Index: {errorsMessage.Item1}");
                }

                if (!errorsMessages.Any())
                    Console.WriteLine($"Вираз правильний");
                Console.WriteLine("---------------------------------------");
                errors.Clear();
                errorsMessages.Clear();
                brakes.Clear();
                Console.ReadKey();

            }
        }

        public static (bool res, string error) IsOperandPositionValid(string token, string? previousToken)
        {
            if (String.IsNullOrEmpty(previousToken))
                return (true, "");

            var previousTokenType = tokenizer.CheckTokenType(previousToken);

            return (previousTokenType == TokenType.operation ||
                previousTokenType == TokenType.openBrake, $"'{token}' не може стояти після '{previousToken}'");
        }

        public static (bool res, string error) IsOperationPositionValid(string token, string? previousToken)
        {
            if (String.IsNullOrEmpty(previousToken) && (token == "+" || token == "-"))
                return (true, "");
            else if(String.IsNullOrEmpty(previousToken))
                return (false, $"Вираз не може починатись з '{token}'");

            var previousTokenType = tokenizer.CheckTokenType(previousToken);

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

            var previousTokenType = tokenizer.CheckTokenType(previousToken);

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
                return(false, "Лишня закриваюча дужка");
            }
            else
            {
                brakes.Pop();
            }

            var previousTokenType = tokenizer.CheckTokenType(previousToken);

            
            return (previousTokenType == TokenType.digit ||
                previousTokenType == TokenType.variable ||
                previousTokenType == TokenType.closeBrake, $"'{token}' не може стояти після '{previousToken}'");

        }
    }
}
