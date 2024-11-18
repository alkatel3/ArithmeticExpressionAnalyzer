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
        [DataRow("0+(A+0/B)*C", "A*C")]
        [DataRow("0-(A*B+C/D)", "0-(A*B+C/D)")]
        [DataRow("A*(B+0)-C", "A*B-C")]
        [DataRow("1+0", "1")]
        [DataRow("0+A", "A")]
        [DataRow("(A+0)+B", "A+B")]
        [DataRow("A*0", "0")]
        [DataRow("0*B", "0")]
        [DataRow("A*(B+0*C)", "A*B")]
        [DataRow("A/1", "A")]
        [DataRow("(A+B)/1", "(A+B)")]
        [DataRow("A*1", "A")]
        [DataRow("(A+B)*1", "(A+B)")]
        [DataRow("-A+B", "0-A+B")]
        [DataRow("-(A+B)", "0-(A+B)")]
        [DataRow("2*3+1", "7")]
        [DataRow("4+5*2", "14")]
        [DataRow("0/b/c/v/d/e/g*t-v-b-d-s-e-g", "0-v-b-d-s-e-g")]
        [DataRow("a*(b+(c+d)/e)+b*0+5+4-1*n", "a*(b+(c+d)/e)+9-n")]
        [DataRow("0+b*0+0*a+a*b+1", "a*b+1")]
        [DataRow("2+3+4+5+6+7+8*s-p", "27+8*s-p")]
        [DataRow("(a+b+5)*2+0*(0/5-(6+3+d))", "(a+b+5)*2")]
        [DataRow("a*(b+0*c)/(d+e*0)+f*(g+h*0)", "a*b/d+f*g")]
        [DataRow("(x*y+0*z)/(w+1*(v*0+t))-u", "(x*y)/(w+t)-u")]
        [DataRow("0*a+b*(c+0*d)/e+f*(0+g*h)-i", "b*c/e+f*(g*h)-i")]
        [DataRow("p*(q+0+r)/(s*t+0*u)+v*(w*0)", "p*(q+r)/(s*t)")]
        [DataRow("((a+b*0)/c+d*0)+e*(f+0*g)/(h+i*0)", "(a/c)+e*f/h")]
        [DataRow("j*(k+0*l*m+n*(o*0+p))/q", "j*(k+n*p)/q")]
        [DataRow("(0+z)/x+(y*(0+z*w))/(u+0*v)+t", "z/x+(y*(z*w))/u+t")]
        [DataRow("((a+0*b)*c+d*0+e)/(f+g*0)", "(a*c+e)/f")]
        [DataRow("(x*0+y)/(z+w)+u*(v+0*w)*s", "y/(z+w)+u*v*s")]
        [DataRow("(a+b*0)/c+d*e*(f+0*g+h*0)", "a/c+d*e*f")]

        public void OptimizeTest(string exp, string expectedResult)
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

        [TestMethod()]
        [DataRow("a+(a+b)", "a+a+b")]
        [DataRow("a-(a+b)", "a-a-b")]
        [DataRow("a-(-a-b)", "a+a+b")]
        //[DataRow("a*(a+b)", "a*a+a*b")]
        //[DataRow("a*(-a-b)", "-a*a-a*b")]
        //[DataRow("a*(-a*b)", "-a*a*b")]
        //[DataRow("(a+b)*a", "a*a+a*b")]
        //[DataRow("(-a-b)*a", "-a*a-a*b")]
        //[DataRow("(-a*b)*a", "-a*a*b")]
        //[DataRow("a*(a+b)*c", "a*a*c+a*b*c")]
        //[DataRow("a*(-a-b)*c", "-a*a*c-a*b*c")]
        //[DataRow("a*(-a*b)*c", "-a*a*b*c")]
        //[DataRow("(a+c)*(a+b)", "a*a+a*b")]
        //[DataRow("(a-c)*(-a-b)", "-a*a-a*b+c*a+c*b")]
        //[DataRow("d*(a-c)*(-a-b)", "-d*a*a-d*a*b+d*c*a+d*c*b")]
        //[DataRow("d*(a-(c+b)*f)", "d*a-d*c*f-d*b*f")]
        //[DataRow("d+(a-f)", "d+a-f")]
        //[DataRow("d-(a-f)", "d-a+f")]
        public void AssociativeSimplificationTest(string exp, string expectedResult)
        {
            //Arrange
            var tokens = tokenizer.Tokenize(exp);
            //Act
            var optimizedExpression = AssociativeSimplification.Execute(tokens);
            var result = String.Join("", optimizedExpression.Select(t => t.Value));
            //Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}