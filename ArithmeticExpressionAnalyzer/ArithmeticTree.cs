
using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;
using System.Xml.Linq;

namespace ArithmeticExpressionAnalyzer
{
    public class ArithmeticTree
    {
        public Node Root { get; set; }
        private string[] add = { "-", "+" };
        private string[] mul = { "*", "/" };

        public void BuildTree(List<Token> tokens)
        {
            Root = getNode(tokens);
        }

        private Node getNode(List<Token> tokens, bool isMin = false, bool isDiv = false)
        {
            if (tokens.Count == 0)
                return new Node();
            else if (tokens.Count == 1)
            {
                if (isMin)
                {
                    var res = new Node("*");
                    res.SetLeftNode(new("-1"));
                    res.SetRightNode(new(tokens[0].Value));
                    return res;
                }

                if (isDiv)
                {
                    var res = new Node("/");
                    res.SetLeftNode(new("1"));
                    res.SetRightNode(new(tokens[0].Value));
                    return res;
                }

                return new(tokens[0].Value);
            }

            bool _isMin = false;
            bool _isDiv = false;
            Node node, left, right;
            Token val = null;
            List<int> propNode = new List<int>();
            List<string[]> operations = new List<string[]> { add, mul };
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
                    _isMin = val.Value == "-" && propNode.Count > 1 && propNode.Count != 2;
                    _isDiv = val.Value == "/" && propNode.Count > 1 && propNode.Count != 2;
                    break;
                }

                if(i == operations.Count-1 && val is null)
                {
                    if (ArithmeticExpressionTokenizer.CheckTokenType(tokens[0].Value) == TokenType.function)
                    {
                        node = new Node(tokens[0].Value);
                        left = getNode(tokens[2..^1]);
                        node.SetRightNode(left);
                        if (isMin)
                        {
                            var node2 = new Node("*");
                            node2.SetLeftNode(new("-1"));
                            node2.SetRightNode(node);
                            return node2;
                        }
                        if (isDiv)
                        {
                            var node2 = new Node("/");
                            node2.SetLeftNode(new("1"));
                            node2.SetRightNode(node);
                            return node2;
                        }

                        return node;
                    }

                    else if (tokens.Count == 1)
                        return new(tokens[0].Value);

                    if (isMin)
                    {
                        var node2 = new Node("*");
                        node2.SetLeftNode(new("-1"));
                        node2.SetRightNode(getNode(tokens[1..^1]));
                        return node2;
                    }
                    if (isDiv)
                    {
                        var node2 = new Node("/");
                        node2.SetLeftNode(new("1"));
                        node2.SetRightNode(getNode(tokens[1..^1]));
                        return node2;
                    }

                    tokens = tokens[1..^1];
                    i = -1;
                }
            }

            node = new Node(_isMin ? "+" : _isDiv ? "*" : val.Value);
            left = getNode(tokens[0..tokens.IndexOf(val)], isMin, isDiv);
            right = getNode(tokens[(tokens.IndexOf(val)+1)..tokens.Count], _isMin, _isDiv);
            node.SetLeftNode(left);
            node.SetRightNode(right);
            return node;
        }

        public void DisplayTree()
        {
            if (Root.Deep > 10)
                DisplayNode(Root, 0);
            else
            {
                int initialX = Console.WindowWidth / 2;
                DisplayNode(Root, initialX, Console.GetCursorPosition().Top + 1, Console.WindowWidth / 4);
            }
        }

        private void DisplayNode(Node node, int tabs)
        {
            if (node == null)
                return;
 
            if(node.Right != null)
            {
                DisplayNode(node.Right, tabs+1);
            }
            Console.WriteLine((tabs-1 < 0 ? "" : new string('\t', tabs - 1) + "------ ") + node.Value);
            if (node.Left != null)
            {
                DisplayNode(node.Left, tabs + 1);
            }
        }

        private void DisplayNode(Node node, int x, int y, int offset)
        {
            if (node == null)
                return;
            //if ((x + offset / 2) >= Console.WindowWidth)
            //{
            //    // Розширюємо ширину буфера до x + 1, щоб забезпечити місце для нової позиції
            //    Console.WindowWidth = x + offset / 2;
            //}

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
