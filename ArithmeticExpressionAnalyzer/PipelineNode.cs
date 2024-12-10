

namespace ArithmeticExpressionAnalyzer
{
    public class PipelineNode
    {
        public string Value { get; set; }
        public int Duration { get; set; }
        public bool IsLoaded { get; set; }
        public int OutPutRow { get; set; } = -1;
        public int InputPutRow { get; set; } = -1;
        public PipelineNode Left { get; set; }
        public PipelineNode Right { get; set; }

        public PipelineNode(string _value)
        {
            Value = _value;
            Duration = GetOparationDuration(_value);

            if(Duration == -1)
            {
                throw new Exception();
            }
        }

        public PipelineNode(Node node)
        {
            Value = node.Value;
            Duration = GetOparationDuration(node.Value);

            if (Duration == -1)
            {
                throw new Exception();
            }

            if (node.Left is not null && GetOparationDuration(node.Left.Value) != -1)
                Left = new PipelineNode(node.Left);

            if (node.Right is not null && GetOparationDuration(node.Right.Value) != -1)
                Right = new PipelineNode(node.Right);
        }

        public int GetOparationDuration(string duration)
        {
            return duration switch
            {
                "-" => 1,
                "+" => 1,
                "*" => 3,
                "/" => 4,
                _ => -1
            };
        }

        // Метод для отримання вузлів за рівнями
        public static List<List<PipelineNode>> GetNodesByLevels(PipelineNode root)
        {
            var levels = new List<List<PipelineNode>>();
            var queue = new Queue<(PipelineNode, int)>();
            queue.Enqueue((root, 0));

            while (queue.Count > 0)
            {
                var (node, level) = queue.Dequeue();

                if (levels.Count <= level)
                    levels.Add(new List<PipelineNode>());
                if (!ArithmeticExpressionTokenizer.IsOperand(node.Value))
                    levels[level].Add(node);

                if (node.Left != null)
                    queue.Enqueue((node.Left, level + 1));

                if (node.Right != null)
                    queue.Enqueue((node.Right, level + 1));
            }

            levels.Reverse(); // Повертаємо рівні знизу вгору
            return levels;
        }
    }
}