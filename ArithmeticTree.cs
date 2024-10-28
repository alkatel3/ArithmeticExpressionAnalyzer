
using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;

namespace ArithmeticExpressionAnalyzer
{
    public class ArithmeticTree
    {
        public Node Root { get; set; }
        private string[] add = { "-", "+" };
        private string[] mul = { "*", "/" };
        private string[] pow = { "^" };

        public void BuildTree(List<Token> tokens)
        {
            Root = getNode(tokens);
        }

        private Node getNode(List<Token> tokens)
        {
            if (tokens.Count == 0)
                return new Node();
            else if(tokens.Count == 1)
                return new(tokens[0].Value);

            Node node, left, right;
            Token val = null;
            List<int> propNode = new List<int>();
            List<string[]> operations = new List<string[]> { add, mul, pow };
            for(int i = 0; i < operations.Count; i++)
            {
                var brakes = 0;

                for (int j = 0; j < tokens.Count; j++)
                {
                    if (tokens[j].Value == "(")
                        brakes++;
                    else if (tokens[j].Value == ")")
                        brakes--;
                    else if (brakes == 0 && operations[i].Contains(tokens[j].Value)){
                        propNode.Add(j);
                    }
                    else
                        continue;
                }

                if (propNode.Count > 0)
                {
                    val = tokens[propNode[propNode.Count / 2]];
                    break;
                }

                if(i == operations.Count-1 && val is null)
                {
                    if (ArithmeticExpressionTokenizer.CheckTokenType(tokens[0].Value) == TokenType.function)
                    {
                        node = new Node(tokens[0].Value);
                        left = getNode(tokens[2..^1]);
                        node.SetLeftNode(left);
                        return node;
                    }

                    else if (tokens.Count == 1)
                        return new(tokens[0].Value);

                    tokens = tokens[1..^1];
                    i = -1;
                }
            }

            node = new Node(val.Value);
            left = getNode(tokens[0..tokens.IndexOf(val)]);
            right = getNode(tokens[(tokens.IndexOf(val)+1)..tokens.Count]);
            node.SetLeftNode(left);
            node.SetRightNode(right);
            return node;
        }

        public void DisplayTree()
        {
            int initialX = Console.WindowWidth / 2;
            DisplayNode(Root, initialX, Console.GetCursorPosition().Top + 1, Console.WindowWidth / 10);
        }

        private void DisplayNode(Node node, int x, int y, int offset)
        {
            if (node == null)
                return;

            Console.SetCursorPosition(x, y);
            Console.Write(node.Value);
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
