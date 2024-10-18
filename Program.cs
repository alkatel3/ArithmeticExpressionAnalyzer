
namespace ArithmeticExpressionAnalyzer
{
    internal class Program
    {
        static ArithmeticExpressionTokenizer tokenizer = new ArithmeticExpressionTokenizer();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                //Get Expression
                Console.WriteLine("Введіть вираз:");
                var exp = Console.ReadLine();
                exp = exp.Replace(" ", "");
                //Tokenize Expression
                var tokens = tokenizer.Tokenize(exp);
                //Validate Expression
                var ValidationRes = ArithmeticExpressionValidator.Validate(tokens);
                if(ValidationRes != null && ValidationRes.Count() > 0)
                {
                    PrintErrors(ValidationRes, exp);
                    continue;
                }

                //Else
                Console.WriteLine($"Вираз правильний");

                var optimizedExpression = ArithmeticExpressionOptimizer.Optimize(tokens);
                diplayTokens(optimizedExpression);
                Console.ReadKey();

            }
        }

        private static void diplayTokens(List<Token> optimizedExpression)
        {
            foreach(var item in optimizedExpression)
            {
                Console.Write(item.Value);
            }
        }

        public static void PrintErrors(List<(int index, string token, string message)> errors, string expression)
        {
            var lastIndex = 0;
            Console.WriteLine("---------------------------------------");
            errors = errors.OrderBy(res => res.index).ToList();
            foreach (var error in errors)
            {
                Console.Write(expression.Substring(lastIndex, error.index - lastIndex));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(expression.Substring(error.index, error.token.Length));
                Console.ResetColor();
                lastIndex = error.index + error.token.Length;
            }

            Console.WriteLine(expression.Substring(lastIndex));
            foreach (var errorsMessage in errors)
            {
                Console.WriteLine($"Error: \"{errorsMessage.message}\"; Index: {errorsMessage.index}");
            }

            Console.WriteLine("---------------------------------------");
        }
    }
}
