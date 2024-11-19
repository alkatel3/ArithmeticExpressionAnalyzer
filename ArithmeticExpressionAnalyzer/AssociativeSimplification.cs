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
                    subExpression = subExpressionProcess(tokens[(openBrakeIndex + 1)..closeBrakeIndex]);

                    if (openBrakeIndex != 0)
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
                                    res.RemoveAt(res.Count - 1);

                                res.AddRange(subExpression);
                                break;
                            case "+":
                                res.AddRange(subExpression);
                                break;
                            case "*":
                                var 
                                break;
                        }
                    }
                }
            }

            return res;
        }
    }
}

20/5*(3+1)=1
20/5*
