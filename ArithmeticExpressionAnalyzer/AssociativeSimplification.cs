using System;
using System.Collections.Generic;
using System.Linq;
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

        private static List<Token> subExpressionProcess(List<Token> expression)
        {
            var res = new List<Token>();
            var terms = SplitIntoTerms(expression);
            terms = ReplaceDivisionWithMultiplication(terms);
            //res = GroupAndFactorize(terms);
            return res;
        }

        private static List<List<Token>> SplitIntoTerms(List<Token> tokens)
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

                if (brakes== 0 && (token.Value == "+" || token.Value == "-"))
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
                    var temp = GetMultiple(term[i..]);
                    tempValue.Value = "1/" + temp.Multiple;
                    i += temp.Bias;
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
            // Знаходимо лексеми, які зустрічаються найчастіше
            var commonFactors = FindCommonFactors(terms);

            if (commonFactors.Count == 0)
            {
                // Якщо немає спільних множників, повертаємо оригінальний список
                return Flatten(terms);
            }

            // Виносимо спільний множник за дужки
            var result = new List<Token>();
            result.AddRange(commonFactors);
            result.Add(new Token("*"));
            result.Add(new Token("("));

            foreach (var term in terms)
            {
                var remaining = RemoveFactors(term, commonFactors);
                result.AddRange(remaining);
                result.Add(new Token("+"));
            }

            // Видалити останній зайвий "+"
            if (result[^1].Value == "+")
            {
                result.RemoveAt(result.Count - 1);
            }

            result.Add(new Token(")"));

            return result;
        }

        private static List<Token> FindCommonFactors(List<List<Token>> terms)
        {
            var factorCounts = new Dictionary<string, int>();
            var operations = new List<string>() { "*", "/", "-", "+", };

            foreach (var term in terms)
            {
                var uniqueFactors = new HashSet<string>(term.Where(t => !operations.Contains(t.Value)).Select(t => t.Value));

                foreach (var factor in uniqueFactors)
                {
                    if (factorCounts.ContainsKey(factor))
                    {
                        factorCounts[factor]++;
                    }
                    else
                    {
                        factorCounts[factor] = 1;
                    }
                }
            }

            return factorCounts
                .Where(kv => kv.Value == terms.Count)
                .Select(kv => new Token(kv.Key))
                .ToList();
        }

        private static List<Token> RemoveFactors(List<Token> term, List<Token> commonFactors)
        {
            var remaining = new List<Token>(term);

            foreach (var factor in commonFactors)
            {
                remaining.RemoveAll(t => t.Value == factor.Value);
            }

            return remaining;
        }

        private static List<Token> Flatten(List<List<Token>> terms)
        {
            var result = new List<Token>();

            foreach (var term in terms)
            {
                result.AddRange(term);
                result.Add(new Token("+"));
            }

            // Видалити останній зайвий "+"
            if (result[^1].Value == "+")
            {
                result.RemoveAt(result.Count - 1);
            }

            return result;
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
