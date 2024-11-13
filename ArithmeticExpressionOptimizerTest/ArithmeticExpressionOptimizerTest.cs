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
        [DataRow("1+0*(1*0+a)", "1")]
        [DataRow("1+(1*0-a)", "1+(0-a)")]
        [DataRow("0*A+B+C", "B+C")]
        [DataRow("(A*0+B)+C", "(B)+C")]
        [DataRow("(A*0)+B+C", "B+C")]
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
        [DataRow("(A+0*B)*C", "(A)*C")]
        [DataRow("A*(B+0*C)", "A*(B)")]
        [DataRow("(A*0)*B+C", "C")]
        [DataRow("A+B*0+C", "A+C")]
        [DataRow("0*(A+B*C)", "0")]
        [DataRow("A+B*(0*C)", "A")]
        [DataRow("(0*A)+B*C", "B*C")]
        [DataRow("A*0*B+C", "C")]
        [DataRow("(A+B)*(0)", "0")]
        [DataRow("A/(B*0+1)", "A/(1)")] // Перевірка на уникає ділення на 0
        [DataRow("A*0-(B+C)", "0-(B+C)")]
        [DataRow("A-((B+0)*C)", "A-((B)*C)")]

        [DataRow("1+0/1", "1")]
        [DataRow("1-0/1", "1")]
        [DataRow("0/1+1", "1")]
        //[DataRow("1/0-1", "0-1")]
        [DataRow("1+0/(1+a)", "1")]
        //[DataRow("1+(1+a)*0", "1")]
        [DataRow("1+0/(1*0+a)", "1")]
        //[DataRow("1+(1*0-a)", "1+(0-a)")]
        [DataRow("0/A+B+C", "B+C")]
        //[DataRow("(A/0+B)+C", "(B)+C")]
        //[DataRow("(A/0)+B+C", "B+C")]
        //[DataRow("(A*0)-B+C", "0-B+C")]
        //[DataRow("((A*0))+B+C", "B+C")]
        //[DataRow("(A*0+B+C)", "(B+C)")]
        [DataRow("(a+b+5)*2+0/(0/5-(6+3+d))", "(a+b+5)*2")]
        //[DataRow("(a+b+5)*2+((((0/5)*(a+b))-(6+3+d)))*0", "(a+b+5)*2")]
        //[DataRow("((((0/5)*(a+b))-(6+3+d)))*0+(a+b+5)*2", "(a+b+5)*2")]
        //[DataRow("((((0/5)*(a+b))-(6+3+d)))*0-(a+b+5)*2", "0-(a+b+5)*2")]
        //[DataRow("A/2-((((0/5)*(a+b))-(6+3+d)))*0-(a+b+5)*2", "A/2-(a+b+5)*2")]
        //[DataRow("A/2+((((0/5)*(a+b))-(6+3+d)))*0-(a+b+5)*2", "A/2-(a+b+5)*2")]
        //[DataRow("A/2-((((0/5)*(a+b))-(6+3+d)))*0+(a+b+5)*2", "A/2+(a+b+5)*2")]
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
    }
}