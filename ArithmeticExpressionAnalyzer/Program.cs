
using System.Linq;

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
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                Console.WriteLine($"Вираз правильний");
                //Optimize Expression
                var optimizedExpression = ArithmeticExpressionOptimizer.Optimize(tokens);
                displayTokens(optimizedExpression);
                displayOptimising();
                Console.WriteLine();


                //Expression Build Tree
                var tree1 = new ArithmeticTree();
                tree1.BuildTree(optimizedExpression);
                //tree.DisplayTree();

                // Обчислення виразу
                var result1 = StaticPipeline.EvaluateTree(tree1.Root);
                double total_cycles1 = result1.Last().OutPutRow;
                double serial_cycles1 = result1.Sum(n => n.Duration) * StaticPipeline.LayersCount;
                double speedup1 = serial_cycles1 / total_cycles1;
                double efficiency1 = speedup1 / StaticPipeline.LayersCount;

                Console.WriteLine($"Початковий вираз зі спрощеннями:\n" +
                    $"Час виконання: {total_cycles1}\n" +
                    $"Коефіцієнт прискорення: {speedup1:F2}\n" +
                    $"Коефіцієнт ефективності: {efficiency1:F2}\n\n");
                StaticPipeline.Display(result1, 0, true);

                var associativeSimplificationExpression = AssociativeSimplification.Execute(tokens);
                var tree2 = new ArithmeticTree();
                tree2.BuildTree(tokenizer.Tokenize(String.Join("", associativeSimplificationExpression.Select(t => t.Value)))); 
                var result2 = StaticPipeline.EvaluateTree(tree2.Root);
                double total_cycles2 = result2.Last().OutPutRow;
                double serial_cycles2 = result2.Sum(n => n.Duration) * StaticPipeline.LayersCount;
                double speedup2 = serial_cycles2/ total_cycles2;
                double efficiency2 = speedup2 / StaticPipeline.LayersCount;

                Console.WriteLine($"Вираз після асоціативного спрощення:\n" +
                    $"Час виконання: {total_cycles2}\n" +
                    $"Коефіцієнт прискорення: {speedup2:F2}\n" +
                    $"Коефіцієнт ефективності: {efficiency2:F2}\n\n");
                StaticPipeline.Display(result2, 10, true);

                var distributivitySimplificationExpression = DistributivitySimplification.Execute(tokens);
                var tree3 = new ArithmeticTree();
                tree3.BuildTree(tokenizer.Tokenize(String.Join("", distributivitySimplificationExpression.Select(t => t.Value))));
                var result3 = StaticPipeline.EvaluateTree(tree3.Root);
                double total_cycles3 = result3.Last().OutPutRow;
                double serial_cycles3 = result3.Sum(n => n.Duration) * StaticPipeline.LayersCount;
                double speedup3 = serial_cycles3 / total_cycles3;
                double efficiency3 = speedup3 / StaticPipeline.LayersCount;

                Console.WriteLine($"Вираз після дистрибутивного спрощення спрощення:\n" +
                    $"Час виконання: {total_cycles3}\n" +
                    $"Коефіцієнт прискорення: {speedup3:F2}\n" +
                    $"Коефіцієнт ефективності: {efficiency3:F2}\n\n");
                StaticPipeline.Display(result3, 20, true);

                //Console.WriteLine($"Застосування асоціативного закону");
                //var associativeSimplificationExpression = DistributivitySimplification.Execute(tokens);
                //var associativeSimplificationExpression = AssociativeSimplification.Execute(tokens);
                //Console.WriteLine(String.Join("", associativeSimplificationExpression.Select(t => t.Value)));

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
