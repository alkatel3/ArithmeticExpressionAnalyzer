﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public static class DistributivitySimplification
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
                        openBrakeIndex -= 1;
                        res.RemoveAt(res.Count - 1);

                        subExpression = new List<Token>();
                        subExpression.AddRange(tokens[openBrakeIndex..(openBrakeIndex + 2)]);
                        subExpression.AddRange(subExpressionProcess(tokens[(openBrakeIndex +2)..(closeBrakeIndex)]));
                        subExpression.AddRange(tokens[(closeBrakeIndex)..(closeBrakeIndex+1)]);
                    }
                    else
                    {
                        subExpression = subExpressionProcess(tokens[(openBrakeIndex + 1)..closeBrakeIndex]);
                    }

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

        private static void SimpleAssociate(List<Token> tokens, List<Token> res, int openBrakeIndex, List<Token> subExpression)
        {
            List<Token> left, mul;

            switch (tokens[openBrakeIndex - 1].Value)
            {
                case "-":
                    var brakes = 0;
                    subExpression.ForEach(t =>
                    {
                        if (t.Value == "(")
                            brakes++;
                        else if (t.Value == ")")
                            brakes--;

                        if (brakes == 0)
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
                    left = GetLeftOperand(res[..(res.Count - 1)]);
                    mul = GetMul(left, subExpression);
                    res.RemoveRange(res.Count - left.Count - 1, left.Count + 1);
                    res.AddRange(mul);
                    break;
                case "/":
                    left = GetLeftOperand(res[..(res.Count - 1)]);
                    res.Add(new Token(-1, "("));
                    res.AddRange(subExpression);
                    res.Add(new Token(-1, ")"));
                    break;
            }
        }

        private static int HardAssociate(List<Token> tokens, List<Token> res, int openBrakeIndex, int closeBrakeIndex, List<Token> subExpression)
        {
            List<Token> mul, left, right;

            if(openBrakeIndex == 0)
            {
                if (tokens[closeBrakeIndex + 1].Value == "*")
                {
                    right = GetRightOperand(tokens[(closeBrakeIndex + 2)..]).Item1;
                    mul = GetMul(subExpression, right);
                }
                else
                {
                    right = GetRightOperand(tokens[(closeBrakeIndex + 2)..], true).Item1;
                    mul = new List<Token>();
                    if (subExpression.Count > 1)
                    {
                        mul.Add(new Token(-1, "("));
                        mul.AddRange(subExpression);
                        mul.Add(new Token(-1, ")"));
                    }
                    else
                        mul.AddRange(subExpression);

                    mul.Add(new Token(-1, "/"));
                    if (right.Count > 1 && (!right.Any(t=>t.Value=="(") || GetFirstOperationIndex(right, ["*"]) == -1))
                    {
                        mul.Add(new Token(-1, "("));
                        mul.AddRange(right);
                        mul.Add(new Token(-1, ")"));
                    }
                    else
                        mul.AddRange(right);

                }

                res.AddRange(mul);
                return right.Count + tokens[(closeBrakeIndex + 2)..].Count(t=>t.Value == "(") + 2;
            }

            switch (tokens[openBrakeIndex - 1].Value)
            {
                case "-":
                    var brakes = 0;
                    subExpression.ForEach(t =>
                    {
                        if (t.Value == "(")
                            brakes++;
                        else if (t.Value == ")")
                            brakes--;

                        if (brakes == 0)
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
                        }
                    });

                    if (subExpression[0].Value == "+")
                    {
                        res.RemoveAt(res.Count - 1);
                        if (!res.Any() || res[res.Count() - 1].Value == "(")
                            subExpression.RemoveAt(0);
                    }
                    else
                    {
                        subExpression.Insert(0, new Token(-1, "-"));
                    }

                    res.RemoveAt(res.Count - 1);
                    right = GetRightOperand(tokens[(closeBrakeIndex + 2)..]).Item1;
                    if (tokens[closeBrakeIndex + 1].Value == "*")
                    {
                        mul = GetMul(subExpression, right);

                        if (mul[0].Value != "-")
                            res.Add(new Token(-1, "+"));

                        res.AddRange(mul);
                    }
                    else if (tokens[closeBrakeIndex + 1].Value == "/")
                    {
                        res.Add(new Token(-1, "+"));
                        res.Add(new Token(-1, "("));
                        res.AddRange(subExpression);
                        res.Add(new Token(-1, ")"));
                        res.Add(tokens[closeBrakeIndex + 1]);
                        res.Add(new Token(-1, "("));
                        res.AddRange(right);
                        res.Add(new Token(-1, ")"));
                    }

                    return right.Count+3;
                case "+":
                    right = GetRightOperand(tokens[(closeBrakeIndex+2)..]).Item1;
                    mul = GetMul(subExpression, right);
                    res.AddRange(mul);
                    return right.Count + 1;
                case "*":
                    left = GetLeftOperand(res[..(res.Count - 1)]);
                    mul = GetMul(left, subExpression);
                    right = GetRightOperand(tokens[(closeBrakeIndex + 2)..]).Item1;
                    if (tokens[closeBrakeIndex + 1].Value == "*")
                    {
                        mul = GetMul(mul, right);
                        res.RemoveRange(res.Count - left.Count - 1, left.Count + 1);
                        res.AddRange(mul);
                    }
                    else if(tokens[closeBrakeIndex + 1].Value == "/")
                    {
                        res.RemoveRange(res.Count - left.Count - 1, left.Count + 1);
                        res.Add(new Token(-1, "("));
                        res.AddRange(mul);
                        res.Add(new Token(-1, ")"));
                        res.Add(tokens[closeBrakeIndex + 1]);
                        res.Add(new Token(-1, "("));
                        res.AddRange(right);
                        res.Add(new Token(-1, ")"));
                    }
                    return right.Count + 3;
                case "/":
                    var temp = GetRightOperand(tokens[(openBrakeIndex)..], true);
                    right = temp.Item1;

                    bool isEqual = true;

                    for (int i = 0; i < right.Count; i++)
                    {
                        if (right[i].Value != tokens[openBrakeIndex + i].Value)
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual && temp.replace ==0)
                    {
                        res.AddRange(right);
                    }
                    else
                    {
                        res.Add(new Token(-1, "("));
                        res.AddRange(right);
                        res.Add(new Token(-1, ")"));
                    }

            return temp.Item2;
            }
            return 0;
        }

        private static (List<Token> res, int bias, int replace) GetRightOperand(List<Token> tokens, bool isDiv=false)
        {
            var next = tokens.FirstOrDefault(t => t.Value == "+" || t.Value == "-");
            var bias = 0;
            int replace = 0;

            if (next is null)
            {
                if (isDiv)
                    (bias, replace) = ReplaseDivToMul(tokens);

                return (subExpressionProcess(tokens), bias, replace);
            }

            var ind = tokens.IndexOf(next);

            while (tokens[..ind].Count(t => t.Value == "(") != tokens[..ind].Count(t => t.Value == ")"))
            {
                next = tokens[(ind+1)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                if (next is null)
                {
                    if (isDiv)
                    {
                        (bias, replace) = ReplaseDivToMul(tokens);
                        if(bias !=0 && bias != tokens.Count)
                        {
                            var DivRes = new List<Token>();
                            DivRes.Add(new Token(-1,"("));
                            DivRes.AddRange(subExpressionProcess(tokens[..bias]));
                            DivRes.Add(new Token(-1, ")"));
                            DivRes.Add(tokens[bias]);
                            if (tokens[(bias + 1)..].Count > 1)
                            {
                                DivRes.Add(new Token(-1, "("));
                                DivRes.AddRange(subExpressionProcess(tokens[(bias + 1)..]));
                                DivRes.Add(new Token(-1, ")"));
                            }
                            else
                            {
                                DivRes.AddRange(subExpressionProcess(tokens[(bias + 1)..]));
                            }
                            return (DivRes, tokens.Count, replace);
                        }
                    }

                    return (subExpressionProcess(tokens), bias, replace);
                }

                ind = tokens.IndexOf(next);
            }

            if (isDiv)
                (bias, replace) = ReplaseDivToMul(tokens[..ind]);

            return (subExpressionProcess(tokens[..ind]), bias, replace);
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

        private static List<Token> GetMul(List<Token> left, List<Token> right)
        {
            Token mark = null;
            List<Token> l = null;

            var indDivLeft = GetFirstOperationIndex(left, ["/"]);
            List<Token> tempLeft = new List<Token>();
            if (indDivLeft != -1 && GetFirstOperationIndex(left, ["-", "+"]) == -1)
            {
                if (left[0].Value != "(")
                {
                    left.Insert(0, new Token(-1, "("));
                    indDivLeft++;
                    left.Insert(indDivLeft, new Token(-1, ")"));
                    indDivLeft++;
                }

                tempLeft = left;
                left = left[1..(indDivLeft - 1)];
            }

            var indDivRight = GetFirstOperationIndex(right, ["/"]);
            List<Token> tempRight = new List<Token>();
            if (indDivRight != -1 && GetFirstOperationIndex(right, ["-", "+"]) == -1)
            {
                if (right[0].Value != "(")
                {
                    right.Insert(0, new Token(-1, "("));
                    indDivRight++;
                    right.Insert(indDivRight, new Token(-1, ")"));
                    indDivRight++;
                }

                tempRight = right;
                right = right[1..(indDivRight-1)];
            }

            var res = new List<Token>();
            Token leftMark = null;
            Token rightMark = null;

            for (int i1 = 0; i1 < left.Count; i1++)
            {
                List<Token> r = null;
                if (ArithmeticExpressionTokenizer.IsOperator(left[i1].Value))
                {
                    leftMark = left[i1];
                    i1++;
                }

                if (ArithmeticExpressionTokenizer.IsFunction(left[i1].Value))
                {
                    var next = GetFirstOperationIndex(left[i1..], ["+", "-"]);
                    if (next != -1)
                    {
                        l = left[i1..(next)];
                        i1 = next - 1;
                    }
                    else
                    {
                        l = left[i1..];
                        i1 = left.Count - 1;
                    }
                }
                else
                {
                    l = left[i1..(i1 + 1)];
                }

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

                        var ind = left.IndexOf(next);

                        while (left[i1..ind].Count(t => t.Value == "(") != left[i1..ind].Count(t => t.Value == ")"))
                        {
                            next = left[(ind + 2)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                            if (next is null)
                            {
                                l = left[i1..];
                                break;
                            }

                            ind += left[(ind + 1)..].IndexOf(next) + 1;
                        }

                        l = left[i1..ind];
                        i1 = ind - 1;
                    }
                }

                for (int i2 = 0; i2 < right.Count; i2++)
                {
                    if (ArithmeticExpressionTokenizer.IsOperator(right[i2].Value))
                    {
                        rightMark = right[i2];
                        i2++;
                    }

                    if (ArithmeticExpressionTokenizer.IsFunction(right[i2].Value))
                    {
                        var next = GetFirstOperationIndex(right[i2..], ["+", "-"]);
                        if (next != -1)
                        {
                            r = right[i2..(next)];
                            i2 = next-1;
                        }
                        else
                        {
                            r = right[i2..];
                            i2 = right.Count-1;
                        }
                    }
                    else
                    {
                        r = right[i2..(i2 + 1)];
                    }

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

                            var ind = right.IndexOf(next);

                            while (right[i2..ind].Count(t => t.Value == "(") != right[i2..ind].Count(t => t.Value == ")"))
                            {
                                next = right[(ind + 2)..].FirstOrDefault(t => t.Value == "+" || t.Value == "-");

                                if (next is null)
                                {
                                    ind = right.Count();
                                    break;
                                }

                                ind += right[(ind + 1)..].IndexOf(next) + 1;
                            }

                            r = right[i2..ind];
                            mark = GetMulMark(leftMark, rightMark);
                            if (mark is not null)
                                res.Add(mark);

                            res.AddRange(GetMul(l, r));
                            i2 = ind - 1;
                            continue;
                        }
                    }

                    mark = GetMulMark(leftMark, rightMark);

                    if (mark is not null)
                        res.Add(mark);

                    res.AddRange(l);
                    res.Add(new Token(-1, "*"));
                    res.AddRange(r);

                    rightMark = null;
                }

                leftMark = null;

            }
            if (tempLeft.Any() && !tempRight.Any())
            {
                if (GetFirstOperationIndex(res, ["-", "+"]) != -1)
                {
                    res.Insert(0, new Token(-1, "("));
                    res.Add(new Token(-1, ")"));
                }
                res.Add(new Token(-1, "/"));
                res.AddRange(tempLeft[(indDivLeft + 1)..]);
            }
            else if (!tempLeft.Any() && tempRight.Any())
            {
                if (GetFirstOperationIndex(res, ["-", "+"]) != -1)
                {
                    res.Insert(0, new Token(-1, "("));
                    res.Add(new Token(-1, ")"));
                }
                res.Add(new Token(-1, "/"));
                res.AddRange(tempRight[(indDivRight + 1)..]);
            }
            else if (tempLeft.Any() && tempRight.Any())
            {
                if (GetFirstOperationIndex(res, ["-", "+"]) != -1)
                {
                    res.Insert(0, new Token(-1, "("));
                    res.Add(new Token(-1, ")"));
                }
                res.Add(new Token(-1, "/"));
                var denominator = GetMul(tempLeft[(indDivLeft+2)..^1], tempRight[(indDivRight+2)..^1]);
                if (GetFirstOperationIndex(denominator, ["-", "+"]) != -1)
                {
                    res.Add(new Token(-1, "("));
                    res.AddRange(denominator);
                    res.Add(new Token(-1, ")"));
                }
                else
                {
                    res.AddRange(denominator);
                }
            }
            
            return res;
        }

        private static Token GetMulMark(Token? leftMark, Token rightMark)
        {
            return leftMark == null && rightMark == null ? null :
                leftMark == null ? rightMark :
                rightMark == null ? leftMark :
                leftMark.Value == rightMark.Value ? new Token(-1, "+") : new Token(-1, "-");
        }

        private static (int bias, int replace) ReplaseDivToMul(List<Token> tokens)
        {
            var indDiv = GetFirstOperationIndex(tokens, ["*","/"]);
            var replase = 0;

            if (indDiv == -1)
                return (0, 0);

            while (indDiv != -1)
            {
                if (tokens[indDiv].Value == "*")
                    break;

                tokens[indDiv].Value = "*";
                replase++;
                var temp = GetFirstOperationIndex(tokens[(indDiv + 1)..], ["*", "/"]);
                if (temp == -1)
                {
                    indDiv = tokens.Count;
                    break;
                }
                else
                    indDiv += temp + 1;
            }

            return (indDiv, replase);
        }

        private static int GetFirstOperationIndex(List<Token> tokens, string[] operation)
        {
            var indDiv = -1;
            var nextDiv = tokens.FirstOrDefault(t => operation.Contains(t.Value));

            if (nextDiv != null)
            {
                indDiv = tokens.IndexOf(nextDiv);

                while (tokens[..indDiv].Count(t => t.Value == "(") != tokens[..indDiv].Count(t => t.Value == ")"))
                {
                    nextDiv = tokens[(indDiv + 1)..].FirstOrDefault(t => operation.Contains(t.Value));

                    if (nextDiv is null)
                        return -1;

                    indDiv = tokens.IndexOf(nextDiv);
                }
            }

            return indDiv;
        }
    }
}