using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public class AssociativeSimplification
    {
        public static List<Token> Execute(List<Token> expression)
        {
            return subExpressionProcess(expression);
        }

        private static List<Token> subExpressionProcess(List<Token> tokens)
        {
            if (!tokens.Exists(t => t.Value == "(" || t.Value == ")"))
                return tokens;

            var res = new List<Token>();
            var brakes = 0;
            var brakesPairs = 0;
            var openBrakeIndex = 0;
            var closeBrakeIndex = 0;
            List<Token> subExpression;

            for (var i = 0; i < tokens.Count; i++)
            {
                var tokenValue = tokens[i].Value;
                if (tokenValue == "(")
                {
                    if (brakes == 0)
                        openBrakeIndex = i;

                    brakes++;
                    brakesPairs++;
                }
                else if (tokenValue == ")")
                {
                    brakes--;

                    if (brakes == 0)
                        closeBrakeIndex = i;
                }
                else if (brakes == 0)
                {
                    res.Add(tokens[i]);
                }


                if (brakes == 0 && brakesPairs > 0)
                {
                    if (openBrakeIndex - 1 < tokens.Count && (openBrakeIndex - 1) >= 0 && ArithmeticExpressionTokenizer.IsFunction(tokens[openBrakeIndex - 1].Value))
                    {
                        res.AddRange(tokens[(openBrakeIndex)..(closeBrakeIndex+1)]);
                        continue;
                    }

                    subExpression = subExpressionProcess(tokens[(openBrakeIndex + 1)..closeBrakeIndex]);

                    if (openBrakeIndex != 0)
                    {
                        if (closeBrakeIndex + 1 == tokens.Count)
                        {
                            SimpleAssociate(tokens, res, openBrakeIndex, subExpression);
                        }
                        else
                        {
                            if (tokens[closeBrakeIndex + 1].Value == "+" || tokens[closeBrakeIndex + 1].Value == "-")
                            {
                                SimpleAssociate(tokens, res, openBrakeIndex, subExpression);
                            }
                            else
                            {
                                i += HardAssociate(tokens, res, openBrakeIndex, closeBrakeIndex, subExpression);
                            }
                        }
                    }
                    else if(closeBrakeIndex+1 == tokens.Count || tokens[closeBrakeIndex+1].Value == "+" || tokens[closeBrakeIndex + 1].Value == "-")
                    {
                        res.AddRange(subExpression);
                    }
                    else
                    {
                        i += HardAssociate(tokens, res, openBrakeIndex, closeBrakeIndex, subExpression);
                    }
                    brakesPairs = 0;
                }
            }

            return res;
        }

        private static int HardAssociate(List<Token> tokens, List<Token> res, int openBrakeIndex, int closeBrakeIndex, List<Token> subExpression)
        {
            List<Token> mul, left, right;

            if(openBrakeIndex == 0)
            {
                right = GetRightOperand(tokens[(closeBrakeIndex + 2)..]);
                mul = GetMul(subExpression, right);
                res.AddRange(mul);
                return right.Count + 1;
            }

            switch (tokens[openBrakeIndex - 1].Value)
            {
                case "-":
                    subExpression.ForEach(t =>
                    {
                        switch (t.Value)
                        {
                            case "-":
                                t.Value = "+";
                                break;
                            case "+":
                                t.Value = "-";
                                break;
                        }
                    });
                    if (subExpression[0].Value == "+")
                    {
                        res.RemoveAt(res.Count - 1);
                        if (!res.Any() || res[res.Count() - 1].Value == "(")
                            subExpression.RemoveAt(0);
                    }

                    right = GetRightOperand(tokens[(closeBrakeIndex + 2)..]);
                    mul = GetMul(subExpression, right);
                    res.AddRange(mul);
                    return right.Count+1;
                case "+":
                    right = GetRightOperand(tokens[(closeBrakeIndex+2)..]);
                    mul = GetMul(subExpression, right);
                    res.AddRange(mul);
                    return right.Count + 1;
                case "*":
                    left = GetLeftOperand(res[..(res.Count - 1)]);
                    mul = GetMul(left, subExpression);
                    right = GetRightOperand(tokens[(closeBrakeIndex + 2)..]);
                    mul = GetMul(mul, right);
                    res.RemoveRange(res.Count - left.Count - 1, left.Count + 1);
                    res.AddRange(mul);
                    return right.Count + 1;
            }
            return 0;
        }

        private static List<Token> GetRightOperand(List<Token> tokens)
        {
            var next = tokens.FirstOrDefault(t => t.Value == "+" || t.Value == "-");

            if (next is null)
            {
                return tokens;
            }

            var ind = tokens.IndexOf(next);

            while (tokens[ind..].Count(t => t.Value == "(") != tokens[ind..].Count(t => t.Value == "("))
            {
                next = tokens.FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                if (next is null)
                {
                    return tokens;
                }

                ind = tokens.IndexOf(next);
            }

            return tokens[..ind];
        }

        private static void SimpleAssociate(List<Token> tokens, List<Token> res, int openBrakeIndex, List<Token> subExpression)
        {
            switch (tokens[openBrakeIndex - 1].Value)
            {
                case "-":
                    subExpression.ForEach(t =>
                    {
                        switch (t.Value)
                        {
                            case "-":
                                t.Value = "+";
                                break;
                            case "+":
                                t.Value = "-";
                                break;
                        }
                    });
                    if (subExpression[0].Value == "+")
                    {
                        res.RemoveAt(res.Count - 1);
                        if (!res.Any() || res[res.Count() - 1].Value == "(")
                            subExpression.RemoveAt(0);
                    }

                    res.AddRange(subExpression);
                    break;
                case "+":
                    res.AddRange(subExpression);
                    break;
                case "*":
                    List<Token> left = GetLeftOperand(res[..(res.Count - 1)]);
                    List<Token> mul = GetMul(left, subExpression);
                    res.RemoveRange(res.Count - left.Count - 1, left.Count + 1);
                    res.AddRange(mul);
                    break;
            }
        }

        private static List<Token> GetMul(List<Token> left, List<Token> right)
        {
            var res = new List<Token>();
            Token leftMark = null;
            Token rightMark = null;

            for(int i1 = 0; i1 < left.Count; i1++)
            {
                List<Token> r = null;
                if (ArithmeticExpressionTokenizer.IsOperator(left[i1].Value))
                {
                    leftMark = left[i1];
                    i1++;
                }

                List<Token> l = left[i1..(i1+1)];
                if (i1 + 1 < left.Count && (left[i1 + 1].Value == "*" || left[i1 + 1].Value == "/"))
                {
                    var next = left[(i1 + 2)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                    if (next is null)
                    {
                        l = left[i1..];
                        i1 = left.Count - 1;
                    }
                    else
                    {

                        var ind = left[(i1 + 2)..].IndexOf(next);

                        while (left[i1..ind].Count(t => t.Value == "(") != left[i1..ind].Count(t => t.Value == "("))
                        {
                            next = left[(i1 + 2)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                            if (next is null)
                            {
                                l = left[i1..];
                            }

                            ind = left[(i1 + 2)..].IndexOf(next);
                        }

                        l = left[i1..(ind - 1)];
                        i1 = ind - 1;
                    }
                }

                for(int i2 = 0;i2 < right.Count; i2++)
                {
                    if (ArithmeticExpressionTokenizer.IsOperator(right[i2].Value))
                    {
                        rightMark = right[i2];
                        i2++;
                    }

                    r = right[i2..(i2+1)];

                    if (i2 + 1 < right.Count && (right[i2 + 1].Value == "*" || right[i2 + 1].Value == "/"))
                    {
                        var next = right[(i2 + 2)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                        if (next is null)
                        {
                            r = right[i2..];
                            i2 = right.Count - 1;
                        }
                        else
                        {

                            var ind = right[(i2 + 2)..].IndexOf(next);

                            while (right[i2..ind].Count(t => t.Value == "(") != right[i2..ind].Count(t => t.Value == "("))
                            {
                                next = right[(i2 + 2)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                                if (next is null)
                                {
                                    l = right[i2..];
                                }

                                ind = right[(i2 + 2)..].IndexOf(next);
                            }

                            l = right[i2..(ind - 1)];
                            i2 = ind - 1;
                        }
                    }

                    Token mark = GetMulMark(leftMark, rightMark);

                    if (mark is not null)
                        res.Add(mark);

                    res.AddRange(l);
                    res.Add(new Token(-1, "*"));
                    res.AddRange(r);
                }
            }

            return res;
        }

        private static Token GetMulMark(Token? leftMark, Token rightMark)
        {
            return leftMark == null && rightMark == null ? null :
                leftMark == null ? rightMark :
                rightMark == null ? leftMark :
                leftMark.Value == rightMark.Value ? leftMark : new Token(-1, "-");
        }

        private static List<Token> GetLeftOperand(List<Token> left)
        {
            var next = left.LastOrDefault(t => t.Value == "+" || t.Value == "-");

            if (next is null)
            {
                return left;
            }

            var ind = left.IndexOf(next);

            while (left[ind..].Count(t => t.Value == "(") != left[ind..].Count(t => t.Value == "("))
            {
                next = left.LastOrDefault(t => t.Value == "+" || t.Value == "-");

                if (next is null)
                {
                    return left;
                }

                ind = left.IndexOf(next);
            }

            return left[(ind + 1)..];
        }
    }
}

//20/5*(3+1)=1
//20/5*
