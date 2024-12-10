using System.Xml.Linq;

namespace ArithmeticExpressionAnalyzer
{
    public class Node
    {
        public Node Left { get; private set; }
        public Node Right { get; private set; }
        public int Deep { get; private set; }
        public string Value { get; set; }

        public Node()
        {
        }

        public Node(string value)
        {
            Value = value;
            Deep = 1;
        }

        public void SetLeftNode(Node node)
        {
            Left = node;
            RecalcDeep();
        }

        public void SetRightNode(Node node)
        {
            Right = node;
            RecalcDeep();
        }

        private void RecalcDeep()
        {
            if (Left != null && Right != null)
                Deep = Left.Deep > Right.Deep ? Left.Deep + 1 : Right.Deep + 1;
            else if (Left == null && Right == null)
                Deep = 1;
            else if (Left == null)
                Deep = Right.Deep + 1;
            else if (Right == null)
                Deep = Left.Deep + 1;
        }

        public double Evaluate()
        {
            if (ArithmeticExpressionTokenizer.IsOperand(Value))
            {
                return double.Parse(Value); // Якщо це не оператор, повертаємо числове значення
            }

            double leftValue = Left.Evaluate(); // Рекурсивно обчислюємо ліве піддерево
            double rightValue = Right.Evaluate(); // Рекурсивно обчислюємо праве піддерево

            return Value switch
            {
                "+" => leftValue + rightValue,
                "-" => leftValue - rightValue,
                "*" => leftValue * rightValue,
                "/" => leftValue / rightValue,
                _ => throw new InvalidOperationException("Невідомий оператор")
            };
        }
    }
}