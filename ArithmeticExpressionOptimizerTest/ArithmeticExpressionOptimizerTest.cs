using ArithmeticExpressionAnalyzer;

namespace ArithmeticExpressionOptimizerTest
{
    [TestClass]
    public class ArithmeticExpressionOptimizerTest
    {
        static ArithmeticExpressionTokenizer tokenizer = null!;
        [TestInitialize]
        public void Setup()
        {
            tokenizer = new ArithmeticExpressionTokenizer();
        }

        [TestMethod()]
        [DataRow("1+0*1", "1")]
        [DataRow("1-0*1", "1")]
        [DataRow("0*1+1", "1")]
        [DataRow("1*0-1", "0-1")]
        [DataRow("1+0*(1+a)", "1")]
        [DataRow("1+(1+a)*0", "1")]
        [DataRow("1+tg(1+a)*0", "1")]
        [DataRow("1+0*(1*0+a)", "1")]
        [DataRow("1+0*tg(1*0+a)", "1")]
        [DataRow("1+(1*0-a)", "1+(0-a)")]
        [DataRow("0*A+B+C", "B+C")]
        [DataRow("(A*0+B)+C", "B+C")]
        [DataRow("sin(A*0+B)+C", "sin(B)+C")]
        [DataRow("(A*0)+B+C", "B+C")]
        [DataRow("sin(A*0)+B+C", "sin(0)+B+C")]
        [DataRow("(A*0)-B+C", "0-B+C")]
        [DataRow("((A*0))+B+C", "B+C")]
        [DataRow("(A*0+B+C)", "(B+C)")]
        [DataRow("(a+b+5)*2+0*(0/5-(6+3+d))", "(a+b+5)*2")]
        [DataRow("(a+b+5)*2+((((0/5)*(a+b))-(6+3+d)))*0", "(a+b+5)*2")]
        [DataRow("((((0/5)*(a+b))-(6+3+d)))*0+(a+b+5)*2", "(a+b+5)*2")]
        [DataRow("((((0/5)*(a+b))-(6+3+d)))*0-(a+b+5)*2", "0-(a+b+5)*2")]
        [DataRow("A/2-((((0/5)*(a+b))-(6+3+d)))*0-(a+b+5)*2", "A/2-(a+b+5)*2")]
        [DataRow("A/2+((((0/5)*(a+b))-(6+3+d)))*0-(a+b+5)*2", "A/2-(a+b+5)*2")]
        [DataRow("A/2-((((0/5)*(a+b))-(6+3+d)))*0+(a+b+5)*2", "A/2+(a+b+5)*2")]

        [DataRow("0*(A+B)+C", "C")]
        [DataRow("(A+B)*0+C", "C")]
        [DataRow("(A+0*B)*C", "A*C")]
        [DataRow("A*(B+0*C)", "A*B")]
        [DataRow("(A*0)*B+C", "C")]
        [DataRow("A+B*0+C", "A+C")]
        [DataRow("0*(A+B*C)", "0")]
        [DataRow("A+B*(0*C)", "A")]
        [DataRow("(0*A)+B*C", "B*C")]
        [DataRow("A*0*B+C", "C")]
        [DataRow("(A+B)*(0)", "0")]
        [DataRow("A/(B*0+1)", "A/1")]
        [DataRow("A*0-(B+C)", "0-(B+C)")]
        [DataRow("A-((B+0)*C)", "A-(B*C)")]

        [DataRow("1+0/1", "1")]
        [DataRow("1-0/1", "1")]
        [DataRow("0/1+1", "1")]
        [DataRow("1+0/(1+a)", "1")]
        [DataRow("1+0/(1*0+a)", "1")]
        [DataRow("0/A+B+C", "B+C")]
        [DataRow("(a+b+5)*2+0/(0/5-(6+3+d))", "(a+b+5)*2")]

        [DataRow("A*(0+B*C)", "A*(B*C)")]
        [DataRow("(A+B)*0-C", "0-C")]
        [DataRow("sin(A*0+B)*C", "sin(B)*C")]
        [DataRow("A/(1+0*C)", "A/1")]
        [DataRow("(0*A)+(B+C)", "(B+C)")]
        [DataRow("0*(A+B*C+D)", "0")]
        [DataRow("A+B*0*C+D", "A+D")]
        [DataRow("0/(A+B)+C", "C")]
        [DataRow("A/(0+B*C)", "A/(B*C)")]
        [DataRow("sin(0+A*B)+C", "sin(A*B)+C")]
        [DataRow("cos(A*0+B)*C", "cos(B)*C")]
        [DataRow("tg(0*A+B)+C", "tg(B)+C")]
        [DataRow("A*0/(B+C)", "0")]
        [DataRow("(A+0)+B*C", "A+B*C")]
        [DataRow("(A+0*B*C+D)", "(A+D)")]
        [DataRow("A/(B+0*C)-D", "A/B-D")]
        [DataRow("sin(A+0*B)+cos(C)", "sin(A)+cos(C)")]
        [DataRow("(A*0+B+C)*(D+0)", "(B+C)*D")]
        [DataRow("(0/A)+B*C", "B*C")]
        [DataRow("A-(0/B)+C", "A+C")]
        [DataRow("A+B*(C+0)", "A+B*C")]
        [DataRow("0*(A+B+C)+D", "D")]
        [DataRow("1+A*(0+B)", "1+A*B")]
        [DataRow("A*(B+0*C)-D", "A*B-D")]
        [DataRow("(A+B)+0*C", "(A+B)")]
        [DataRow("cos(A*0)+B+C", "cos(0)+B+C")]
        [DataRow("0*A/(B+C)", "0")]
        [DataRow("A*(0)+B*C", "B*C")]
        [DataRow("0/(A+B+C)+D", "D")]
        [DataRow("0+A*(B+0)", "A*B")]
        [DataRow("A*0+sin(B)+cos(C)", "sin(B)+cos(C)")]
        [DataRow("0-(A+B*C)", "0-(A+B*C)")]
        [DataRow("0*(A+B/C)", "0")]
        //[DataRow("(A+B*C)/0", "(A+B*C)/0")]
        [DataRow("0+(A+0/B)*C", "A*C")]
        [DataRow("0-(A*B+C/D)", "0-(A*B+C/D)")]
        [DataRow("A*(B+0)-C", "A*B-C")]
        [DataRow("(a+b+5)*2+0*(0/5-(6+3+d))", "(a+b+5)*2")]
        public void ZeroMul(string exp, string expectedResult)
        {
            //Arrange
            var tokens = tokenizer.Tokenize(exp);
            //Act
            var optimizedExpression = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = String.Join("", optimizedExpression.Select(t => t.Value));
            //Assert
            Assert.AreEqual(expectedResult, result);
        }


        [TestMethod()]
        [DataRow("(A+B*C)/0")]
        public void DivideByZero(string exp)
        {
            //Arrange
            var tokens = tokenizer.Tokenize(exp);
            //Act
            Assert.ThrowsException<DivideByZeroException>(() =>
            {
                var optimizedExpression = ArithmeticExpressionOptimizer.Optimize(tokens);
            });
        }

        // Тест для додавання нуля
        [TestMethod]
        [DataRow("1+0", "1")]
        [DataRow("0+A", "A")]
        [DataRow("(A+0)+B", "A+B")]
        public void AddingZeroTest(string expression, string expected)
        {
            var tokens = tokenizer.Tokenize(expression);
            var optimizedTokens = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = string.Join("", optimizedTokens.Select(t => t.Value));
            Assert.AreEqual(expected, result);
            Assert.IsTrue(ArithmeticExpressionOptimizer.Optimised.ContainsKey(OpimizeType.AddingZero));
        }

        // Тест для множення на нуль
        [TestMethod]
        [DataRow("A*0", "0")]
        [DataRow("0*B", "0")]
        [DataRow("A*(B+0*C)", "A*B")]
        public void MultiplyZeroTest(string expression, string expected)
        {
            var tokens = tokenizer.Tokenize(expression);
            var optimizedTokens = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = string.Join("", optimizedTokens.Select(t => t.Value));
            Assert.AreEqual(expected, result);
            Assert.IsTrue(ArithmeticExpressionOptimizer.Optimised.ContainsKey(OpimizeType.MulZero));
        }

        // Тест для ділення на 1
        [TestMethod]
        [DataRow("A/1", "A")]
        [DataRow("(A+B)/1", "(A+B)")]
        public void DivideByOneTest(string expression, string expected)
        {
            var tokens = tokenizer.Tokenize(expression);
            var optimizedTokens = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = string.Join("", optimizedTokens.Select(t => t.Value));
            Assert.AreEqual(expected, result);
            Assert.IsTrue(ArithmeticExpressionOptimizer.Optimised.ContainsKey(OpimizeType.DividedOne));
        }

        // Тест для множення на 1
        [TestMethod]
        [DataRow("A*1", "A")]
        [DataRow("(A+B)*1", "(A+B)")]
        public void MultiplyByOneTest(string expression, string expected)
        {
            var tokens = tokenizer.Tokenize(expression);
            var optimizedTokens = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = string.Join("", optimizedTokens.Select(t => t.Value));
            Assert.AreEqual(expected, result);
            Assert.IsTrue(ArithmeticExpressionOptimizer.Optimised.ContainsKey(OpimizeType.MulOne));
        }

        // Тест для унарного мінуса
        [TestMethod]
        [DataRow("-A+B", "0-A+B")]
        [DataRow("-(A+B)", "0-(A+B)")]
        public void UnaryMinusTest(string expression, string expected)
        {
            var tokens = tokenizer.Tokenize(expression);
            var optimizedTokens = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = string.Join("", optimizedTokens.Select(t => t.Value));
            Assert.AreEqual(expected, result);
            Assert.IsTrue(ArithmeticExpressionOptimizer.Optimised.ContainsKey(OpimizeType.UnarityMinus));
        }

        // Тест для обчислення констант
        [TestMethod]
        [DataRow("2*3+1", "7")]
        [DataRow("4+5*2", "14")]
        [DataRow("4-5*2", "-6")]
        [DataRow("4/5*2", "1.6")]
        public void CalculateConstantTest(string expression, string expected)
        {
            var tokens = tokenizer.Tokenize(expression);
            var optimizedTokens = ArithmeticExpressionOptimizer.Optimize(tokens);
            var result = string.Join("", optimizedTokens.Select(t => t.Value));
            Assert.AreEqual(expected, result);
            Assert.IsTrue(ArithmeticExpressionOptimizer.Optimised.ContainsKey(OpimizeType.CalcConst));
        }
    }
}