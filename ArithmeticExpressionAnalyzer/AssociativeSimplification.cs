using ArithmeticExpressionAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public class AssociativeSimplification
    {
        public static List<Token> Execute(List<Token> expression)
        {
            return subExpressionProcess(expression);
        }

        private static List<Token> subExpressionProcess(List<Token> expression)
        {
            var res = new List<Token>();
            expression = RemoveUselessBrakes(expression);
            var terms = SplitIntoTerms(expression);
            terms = ReplaceDivisionWithMultiplication(terms);
            terms = ProcessDifficultMultiple(terms);
            terms = terms.OrderByDescending(t => t.FirstOrDefault().Value).ToList();
            res = GroupAndFactorize(terms);
            res = ReplaceMultiplicationWithDivision(res);
            return res;
        }

        private static List<Token> RemoveUselessBrakes(List<Token> tokens)
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
                    subExpression = RemoveUselessBrakes(tokens[(openBrakeIndex + 1)..closeBrakeIndex]);

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
                                res.Add(new Token("("));
                                res.AddRange(subExpression);
                                res.Add(new Token(")"));
                            }
                        }
                    }
                    else if (closeBrakeIndex + 1 == tokens.Count || tokens[closeBrakeIndex + 1].Value == "+" || tokens[closeBrakeIndex + 1].Value == "-")
                    {
                        res.AddRange(subExpression);
                    }
                    else
                    {
                        res.Add(new Token("("));
                        res.AddRange(subExpression);
                        res.Add(new Token(")"));
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
                default:
                    res.Add(new Token("("));
                    res.AddRange(subExpression);
                    res.Add(new Token(")"));
                    break;
            }
        }

        private static List<List<Token>> ProcessDifficultMultiple(List<List<Token>> terms)
        {
            for (int i = 0; i < terms.Count; i++)
            {
                var multiples = SplitIntoMultiples(terms[i]);
                terms[i] = new();
                for (var j = 0; j < multiples.Count; j++)
                {
                    if (multiples[j].Count > 1 && multiples[j][0].Value !="-1")
                    {
                        if (ArithmeticExpressionTokenizer.IsFunction(multiples[j][0].Value))
                        {
                            var func = multiples[j][0];
                            multiples[j] = subExpressionProcess(multiples[j][2..^1]);

                            multiples[j].Insert(0, new Token("("));
                            multiples[j].Insert(0, func);
                            multiples[j].Add(new Token(")"));
                        }
                        else
                        {
                            multiples[j] = subExpressionProcess(multiples[j][1..^1]);
                            multiples[j].Insert(0, new Token("("));
                            multiples[j].Add(new Token(")"));
                        }

                        terms[i].Add(new Token(String.Join("", multiples[j].Select(m => m.Value))));
                    }
                    else
                    {
                        terms[i].Add(multiples[j][0]);
                    }

                    if(j!= multiples.Count - 1)
                    {
                        terms[i].Add(new Token("*"));
                    }
                }

            }

            return terms;
        }

        private static List<List<Token>> SplitIntoMultiples(List<Token> tokens)
        {
            var terms = new List<List<Token>>();
            var currentTerm = new List<Token>();
            var brakes = 0;

            foreach (var token in tokens)
            {
                if (token.Value == "(")
                    brakes++;

                if (token.Value == ")")
                    brakes--;

                if (brakes == 0 && token.Value == "*")
                {
                    if (currentTerm.Count > 0)
                    {
                        terms.Add(currentTerm);
                        currentTerm = new List<Token>();
                    }

                    continue;
                }
                currentTerm.Add(token);
            }

            if (currentTerm.Count > 0)
            {
                terms.Add(currentTerm);
            }

            return terms;
        }

        private static List<Token> ReplaceMultiplicationWithDivision(List<Token> tokens)
        {
            var reg = new Regex(@"^1/.*$");

            for (int i = 0; i < tokens.Count; i++)
            {
                if (reg.IsMatch(tokens[i].Value) && i > 0 && tokens[i-1].Value == "*")
                {
                    tokens[i].Value = tokens[i].Value[2..];
                    tokens[i-1].Value = "/";
                }
                else if(tokens[i].Value == "-1")
                {
                    if (i == 0 || tokens[i-1].Value=="(")
                    {
                        tokens[i].Value = "-";
                        tokens.RemoveAt(i + 1);
                        i--;
                    }
                    else if(tokens[i - 1].Value == "+")
                    {
                        tokens[i - 1].Value = "-";
                        tokens.RemoveRange(i, 2);
                    }
                }
            }

            return tokens;
        }

        private static List<List<Token>> SplitIntoTerms(List<Token> tokens)
        {
            var terms = new List<List<Token>>();
            var currentTerm = new List<Token>();
            var brakes = 0;

            for (var i = 0; i<tokens.Count; i++)
            {
                if (tokens[i].Value == "(")
                    brakes++;

                if (tokens[i].Value == ")")
                    brakes--;

                if (brakes== 0 && (tokens[i].Value == "+" || tokens[i].Value == "-"))
                {
                    if (currentTerm.Count > 0)
                    {
                        terms.Add(currentTerm);
                        currentTerm = new List<Token>();
                    }

                    if (tokens[i].Value == "-")
                    {
                        currentTerm.Insert(0, new Token("*"));
                        currentTerm.Insert(0, new Token("-1"));
                    }

                    continue;
                }
                currentTerm.Add(tokens[i]);
            }

            if (currentTerm.Count > 0)
            {
                terms.Add(currentTerm);
            }

            return terms;
        }

        private static List<List<Token>> ReplaceDivisionWithMultiplication(List<List<Token>> terms)
        {
            var res = new List<List<Token>>();

            foreach (var term in terms)
                res.Add(ReplaceDivisionWithMultiplication(term));

            return res;
        }

        private static List<Token> ReplaceDivisionWithMultiplication(List<Token> term)
        {
            var res = new List<Token>();
            var isDiv = false;

            for (int i = 0; i<term.Count; i++)
            {
                var tempValue = term[i];
                if (isDiv)
                {
                    if (term[i].Value != "(")
                    {
                        tempValue.Value = "1/" + term[i].Value;
                    }
                    else
                    {
                        var temp = GetMultiple(term[i..]);
                        tempValue.Value = "1/" + temp.Multiple;
                        i += temp.Bias;
                    }

                    isDiv = false;
                }

                if (term[i].Value == "/")
                {
                    term[i].Value = "*";
                    isDiv = true;
                }

                res.Add(tempValue);
            }

            return res;
        }

        private static (string Multiple, int Bias) GetMultiple(List<Token> tokens)
        {
            var term = new List<Token>();
            var brakes = 0;

            foreach (var token in tokens)
            {
                if (token.Value == "(")
                    brakes++;

                if (token.Value == ")")
                    brakes--;

                if (brakes == 0 && (token.Value == "*" || token.Value == "/"))
                {
                    break;
                }

                term.Add(token);
            }

            return (String.Join("", term.Select(t => t.Value)), term.Count()-1);
        }

        private static List<Token> GroupAndFactorize(List<List<Token>> terms)
        {
            var res = new List<Token>();

            var factorCounts = new Dictionary<string, List<List<Token>>>();
            var operations = new List<string>() { "*", "/", "-", "+", };

            foreach (var term in terms)
            {
                var uniqueFactors = new HashSet<string>(term.Where(t => !operations.Contains(t.Value)).Select(t => t.Value));

                foreach (var factor in uniqueFactors)
                {
                    if (factorCounts.ContainsKey(factor))
                    {
                        factorCounts[factor].Add(term);
                    }
                    else
                    {
                        factorCounts[factor] = new List<List<Token>>();
                        factorCounts[factor].Add(term);
                    }
                }
            }

            var maxMultiple = factorCounts.Max(f => f.Value.Count);
            var groupToSimplify = factorCounts
                .Where(kv => kv.Value.Count == maxMultiple && kv.Value.Count>1).FirstOrDefault().Key;

            if (groupToSimplify == "-1")
            {
                 maxMultiple = factorCounts.Where(f=>f.Key!="-1").Max(f => f.Value.Count);
                var groupToSimplifyTemp = factorCounts
                    .Where(kv => kv.Value.Count == maxMultiple && kv.Value.Count > 1 && kv.Key != "-1").FirstOrDefault().Key;

                groupToSimplify = groupToSimplifyTemp == null ? groupToSimplify : groupToSimplifyTemp;
            }

            if(groupToSimplify is null)
            {
                for(int i = 0; i<terms.Count; i++)
                {
                    res.AddRange(terms[i]);

                    if (i != terms.Count - 1)
                    {
                        res.Add(new Token("+"));
                    }
                }

                return res;
            }

            var commonFactors = new List<string>();
            foreach(var item in factorCounts[groupToSimplify])
            {
                var multiples = item.Where(t => !operations.Contains(t.Value)).Select(t => t.Value).ToList();
                if (commonFactors.Count == 0)
                {
                    commonFactors = multiples;
                }
                else
                {
                    commonFactors = commonFactors.Intersect(multiples).ToList();
                }
            }

            var group = terms.Where(t => factorCounts[groupToSimplify].Contains(t)).ToList();
            var resGroup = RemoveFaktors(group, commonFactors);

            if(commonFactors.Count > 0)
            {
                for(var i = 0;i<commonFactors.Count; i+=2)
                {
                    if (i == commonFactors.Count - 1)
                        break;

                    commonFactors.Insert(i + 1, "*");
                }
            }
            
            if (resGroup.Count > 1)
            {
                res.AddRange(commonFactors.Select(f => new Token(f)));
                res.Add(new Token("*"));
                res.Add(new Token("("));
                res.AddRange(GroupAndFactorize(resGroup));
                res.Add(new Token(")"));
            }
            else if (resGroup[0][0].Value=="0") {}
            else
            {
                res.AddRange(commonFactors.Select(f => new Token(f)));
                res.Insert(0, new Token("*"));
                res.Insert(0, resGroup[0][0]);
            }

            terms.RemoveAll(t => factorCounts[groupToSimplify].Contains(t));
            if (terms.Count > 0)
            {
                res.Add(new Token("+"));
                res.AddRange(GroupAndFactorize(terms));
            }
            return res;
        }

        private static List<List<Token>> RemoveFaktors(List<List<Token>> group, List<string> commonFactors)
        {
            var numberIndex = -1;
            var number = 0.0;
            foreach(var commonFactor in commonFactors)
            {
                for(var i = 0; i < group.Count(); i++)
                {
                    if (group[i].Count == 1 || group[i].Count == 3 && (group[i][0].Value == "-1" || group[i][2].Value == "-1") && !commonFactor.Contains("-1"))
                    {
                        if (number != 0)
                        {
                            if (group[i].Count == 3)
                                number--;
                            else
                                number++;

                            group.RemoveAt(i);
                            group[numberIndex][0].Value = number.ToString();
                            i--;
                        }
                        else
                        {
                            numberIndex = i;
                            number = 1;
                            group[i][0].Value = number.ToString();
                        }

                        continue;
                    }

                    var temp = group[i].FirstOrDefault(f => f.Value == commonFactor);
                    var index = group[i].IndexOf(temp);

                    if(index == 0)
                        group[i].RemoveRange(0, 2);
                    else
                        group[i].RemoveRange(index-1, 2);

                    if (group[i].Count == 1 && ArithmeticExpressionTokenizer.IsNumber(group[i][0].Value) && number != 0)
                    {
                        number += Double.Parse(group[i][0].Value);
                        group.RemoveAt(i);
                        group[numberIndex][0].Value = number.ToString();
                        i--;
                    }
                }
            }

            return group;
        }
    }
}
