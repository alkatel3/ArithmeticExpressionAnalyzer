using System.Xml.Linq;

namespace ArithmeticExpressionAnalyzer
{
    public class Node
    {
        public Node Left { get; private set; }
        public Node Right { get; private set; }

        public string Value { get;  set; }

        public Node()
        {
        }

        public Node(string value)
        {
            Value = value;
        }

        public void SetLeftNode(Node node)
        {
            Left = node;
        }

        public void SetRightNode(Node node)
        {
            Right = node;
        }
    }
}