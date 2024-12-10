using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public static class StaticPipeline
    {
        public static double EvaluateTree(Node root)
        {
            // Отримуємо вузли за рівнями
            var levels = GetNodesByLevels(root);
            var res =new List<List<Node>>();

            foreach (var level in levels)
            {
                res.AddRange(GetLevels(level));
            }
   
            // Виконуємо обчислення знизу вгору (починаючи з найглибшого рівня)
            //foreach (var level in levels)
            //{
            //    foreach (var node in level)
            //    {
            //        if (ArithmeticExpressionTokenizer.IsOperator(node.Value))
            //        {
            //            double leftValue = double.Parse(node.Left.Value);
            //            double rightValue = double.Parse(node.Right.Value);

            //            // Обчислюємо поточний вузол
            //            node.Value = node.Value switch
            //            {
            //                "+" => (leftValue + rightValue).ToString(),
            //                "-" => (leftValue - rightValue).ToString(),
            //                "*" => (leftValue * rightValue).ToString(),
            //                "/" => (leftValue / rightValue).ToString(),
            //                _ => throw new InvalidOperationException("Невідомий оператор")
            //            };

            //            // Звільняємо дочірні вузли (оптимізація пам’яті)
            //            node.SetLeftNode(null);
            //            node.SetRightNode(null);
            //        }
            //    }
            //}

            //// Повертаємо результат, що міститься в корені дерева
            //return double.Parse(root.Value);
            return 0;
        }

        private static List<List<Node>> GetLevels(List<Node> level)
        {
            var operations = new List<string> { "*", "/", "-", "+" };
            var res = new List<List<Node>>();

            foreach (var operation in operations)
            {
                if (level.Any(l => l.Value == operation))
                {
                    var temp = level.Where(l => l.Value == operation).ToList();

                    temp.ForEach(n =>
                    {
                        if (n.Left is not null && ArithmeticExpressionTokenizer.IsOperand(n.Left.Value))
                            n.SetLeftNode(null);

                        if (n.Right is not null && ArithmeticExpressionTokenizer.IsOperand(n.Right.Value))
                            n.SetRightNode(null);
                    });

                    res.Add(temp);

                    
                }
            }

            return res;
        }

        // Метод для отримання вузлів за рівнями
        private static List<List<Node>> GetNodesByLevels(Node root)
        {
            var levels = new List<List<Node>>();
            var queue = new Queue<(Node, int)>();
            queue.Enqueue((root, 0));

            while (queue.Count > 0)
            {
                var (node, level) = queue.Dequeue();

                if (levels.Count <= level)
                    levels.Add(new List<Node>());
                if (!ArithmeticExpressionTokenizer.IsOperand(node.Value))
                levels[level].Add(node);

                if (node.Left != null)
                    queue.Enqueue((node.Left, level + 1));

                if (node.Right != null)
                    queue.Enqueue((node.Right, level + 1));
            }

            levels.RemoveAt(levels.Count - 1);

            levels.Reverse(); // Повертаємо рівні знизу вгору
            return levels;
        }
    }

}
