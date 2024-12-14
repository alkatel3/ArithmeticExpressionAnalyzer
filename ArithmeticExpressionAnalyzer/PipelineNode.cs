

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
        public int Level { get; private set; }

        public PipelineNode(string _value)
        {
            Value = _value;
            Duration = GetOparationDuration(_value);

            if(Duration == -1)
            {
                throw new Exception();
            }
        }

        public PipelineNode(Node node, int level = 0)
        {
            Value = node.Value;
            Level = level;

            Duration = GetOparationDuration(node.Value);

            if (Duration == -1)
            {
                throw new Exception();
            }

            if (node.Left is not null && GetOparationDuration(node.Left.Value) != -1)
                Left = new PipelineNode(node.Left, level+1);

            if (node.Right is not null && GetOparationDuration(node.Right.Value) != -1)
                Right = new PipelineNode(node.Right, level + 1);
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

                levels[level].Add(node);

                if (node.Left != null)
                    queue.Enqueue((node.Left, level + 1));

                if (node.Right != null)
                    queue.Enqueue((node.Right, level + 1));
            }

            levels.Reverse(); // Повертаємо рівні знизу вгору
            return levels;
        }

        public void DisplayNode()
        {
            int initialX = Console.WindowWidth / 2;
            DisplayNode(this, initialX, Console.GetCursorPosition().Top + 1, Console.WindowWidth / 4);
        }

        private void DisplayNode(PipelineNode node, int x, int y, int offset)
        {
            if (node == null)
                return;

            Console.SetCursorPosition(x, y);
            Console.Write(node.Value);
            if (offset == 0 || offset == 1)
            {
                offset = 2;
            }
            if (node.Left != null)
            {
                Console.SetCursorPosition(x - offset / 2, y + 1);
                Console.Write("/");
                DisplayNode(node.Left, x - offset, y + 2, offset / 2);
            }

            if (node.Right != null)
            {
                Console.SetCursorPosition(x + offset / 2, y + 1);
                Console.Write("\\");
                DisplayNode(node.Right, x + offset, y + 2, offset / 2);
            }
        }
    }
}