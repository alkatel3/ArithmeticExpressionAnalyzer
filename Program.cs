
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
                if (ValidationRes != null && ValidationRes.Count() > 0)
                {
                    PrintErrors(ValidationRes, exp);
                    continue;
                }

                Console.WriteLine($"Вираз правильний");
                //Optimize Expression
                var optimizedExpression = ArithmeticExpressionOptimizer.Optimize(tokens);
                displayTokens(optimizedExpression);

                ValidationRes = ArithmeticExpressionValidator.Validate(optimizedExpression);
                if (ValidationRes != null && ValidationRes.Count() > 0)
                {
                    PrintErrors(ValidationRes, exp);
                    continue;
                }

                displayOptimising();
                Console.WriteLine();
                //Expression Build Tree
                var tree = new ArithmeticTree();
                tree.BuildTree(optimizedExpression);
                tree.DisplayTree();
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void displayOptimising()
        {
            if (ArithmeticExpressionOptimizer.Optimised.Count > 0)
            {
                Console.WriteLine("\nСпрощення виразу:");
                foreach (var item in ArithmeticExpressionOptimizer.Optimised)
                {
                    switch (item.Key)
                    {
                        case OpimizeType.UnarityMinus:
                            Console.WriteLine($"Унарний мінус: {item.Value}");
                            break;
                        case OpimizeType.AddingZero:
                            Console.WriteLine($"Додавання нуля: {item.Value}");
                            break;
                        case OpimizeType.MulZero:
                            Console.WriteLine($"Множення на нуль: {item.Value}");
                            break;
                        case OpimizeType.SubZero:
                            Console.WriteLine($"Віднімання нуля: {item.Value}");
                            break;
                        case OpimizeType.MulOne:
                            Console.WriteLine($"Множення на один: {item.Value}");
                            break;
                        case OpimizeType.DividedOne:
                            Console.WriteLine($"Ділення на один: {item.Value}");
                            break;
                        case OpimizeType.DivideZero:
                            Console.WriteLine($"Ділення нуля на число: {item.Value}");
                            break;
                        case OpimizeType.CalcConst:
                            Console.WriteLine($"Обчислення констант: {item.Value}");
                            foreach (var calc in ArithmeticExpressionOptimizer.Calculation)
                                Console.WriteLine(calc);
                            break;
                    }

                }
            }
        }

        private static void displayTokens(List<Token> optimizedExpression)
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
