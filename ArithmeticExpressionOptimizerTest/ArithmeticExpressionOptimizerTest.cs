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
        [DataRow("(a+b)+a", "a+b+a")]
        [DataRow("-(a+b)+a", "-a-b+a")]
        [DataRow("-(-a-b)+a", "a+b+a")]
        [DataRow("((a+b))+a", "a+b+a")]
        [DataRow("-(-(a+b))+a", "a+b+a")]
        [DataRow("-(-(a+b)-3)+a", "a+b+3+a")]
        [DataRow("a*(a+b)", "a*a+a*b")]
        [DataRow("a*(-a-b)", "-a*a-a*b")]
        [DataRow("a*(-a*b)", "-a*a*b")]
        [DataRow("c*a*(-a*b)", "-c*a*a*b")]
        [DataRow("c*a*(-a*sin(b))", "-c*a*a*sin(b)")]
        [DataRow("(a+b)*a", "a*a+b*a")]
        [DataRow("c+(a+b)*a", "c+a*a+b*a")]
        [DataRow("c-(a+b)*a", "c-a*a-b*a")]
        [DataRow("(-a-b)*a", "-a*a-b*a")]
        [DataRow("(-a*b)*a", "-a*b*a")]
        [DataRow("a*(a+b)*c", "a*a*c+a*b*c")]
        [DataRow("a*(-a-b)*c", "-a*a*c-a*b*c")]
        [DataRow("a*(-a*b)*c", "-a*a*b*c")]
        [DataRow("(a+c)*(a+b)", "a*a+a*b+c*a+c*b")]
        [DataRow("(a-c)*(-a-b)", "-a*a-a*b+c*a+c*b")]
        [DataRow("d*(a-c)*(-a-b)", "-d*a*a-d*a*b+d*c*a+d*c*b")]
        [DataRow("d*(a-(c+b))", "d*a-d*c-d*b")]
        [DataRow("(a-(c+b))*f", "a*f-c*f-b*f")]
        [DataRow("d*(a-(c+b))*f", "d*a*f-d*c*f-d*b*f")]
        [DataRow("d*(a-(c+b)*r)*f", "d*a*f-d*c*r*f-d*b*r*f")]
        [DataRow("d+(a-f)", "d+a-f")]
        [DataRow("d-(a-f)", "d-a+f")]
        [DataRow("(a+b)-(c+d)", "a+b-c-d")]
        [DataRow("a+(b-(c+d))", "a+b-c-d")]
        [DataRow("a-((b+c)-(d+e))", "a-b-c+d+e")]
        [DataRow("(a+b)*(c+d)", "a*c+a*d+b*c+b*d")]
        [DataRow("a*((b+c)*(d+e))", "a*b*d+a*b*e+a*c*d+a*c*e")]
        [DataRow("(a+b)*(c-(d+e))", "a*c-a*d-a*e+b*c-b*d-b*e")]
        [DataRow("a*(b+(c-(d+e)))", "a*b+a*c-a*d-a*e")]
        [DataRow("((a+b)*(c+d))*e", "a*c*e+a*d*e+b*c*e+b*d*e")]
        [DataRow("(a-b)*((c+d)-e)", "a*c+a*d-a*e-b*c-b*d+b*e")]
        [DataRow("(a+b)*(c-(d-e))", "a*c-a*d+a*e+b*c-b*d+b*e")]
        [DataRow("(a+b)*(c+d)*(e+f)", "a*c*e+a*c*f+a*d*e+a*d*f+b*c*e+b*c*f+b*d*e+b*d*f")]
        [DataRow("(a+b)*((c+d)*(e+f))", "a*c*e+a*c*f+a*d*e+a*d*f+b*c*e+b*c*f+b*d*e+b*d*f")]
        [DataRow("a+(b*c)-(d/(e+f))", "a+b*c-d/(e+f)")]
        [DataRow("a*((b+c)-(d*e))", "a*b+a*c-a*d*e")]
        [DataRow("a*((b-c)*(d+e))", "a*b*d+a*b*e-a*c*d-a*c*e")]
        [DataRow("(a+b)*((c+d)-(e-f))", "a*c+a*d-a*e+a*f+b*c+b*d-b*e+b*f")]
        [DataRow("((a+b)-c)*(d-(e+f))", "a*d-a*e-a*f+b*d-b*e-b*f-c*d+c*e+c*f")]
        [DataRow("a*(b-(c-(d+e)))", "a*b-a*c+a*d+a*e")]
        [DataRow("(a-(b+c))*((d+e)-f)", "a*d+a*e-a*f-b*d-b*e+b*f-c*d-c*e+c*f")]
        [DataRow("((a-b)+c)*(d+e)*(f+g)", "a*d*f+a*d*g+a*e*f+a*e*g-b*d*f-b*d*g-b*e*f-b*e*g+c*d*f+c*d*g+c*e*f+c*e*g")]
        [DataRow("a*((b+c)-(d-e))*(f+g)", "a*b*f+a*b*g+a*c*f+a*c*g-a*d*f-a*d*g+a*e*f+a*e*g")]
        [DataRow("a*(b+c)*((d-e)*(f+g))", "a*b*d*f+a*b*d*g-a*b*e*f-a*b*e*g+a*c*d*f+a*c*d*g-a*c*e*f-a*c*e*g")]
        [DataRow("(a-(b+c))*((d+e)*(f-g))", "a*d*f-a*d*g+a*e*f-a*e*g-b*d*f+b*d*g-b*e*f+b*e*g-c*d*f+c*d*g-c*e*f+c*e*g")]
        [DataRow("(a+(b*c))/(d-(e+f))", "(a+b*c)/(d-e-f)")]

        //[DataRow("(a+b)/c", "a/c+b/c")]
        //[DataRow("(a-b)/c", "a/c-b/c")]
        [DataRow("a/(b+c)", "a/(b+c)")]
        [DataRow("(a+b)/(c-d)", "(a+b)/(c-d)")]
        [DataRow("(a-b)/(c+d)", "(a-b)/(c+d)")]
        [DataRow("(a+b)/(c-d)/(e+f)", "(a+b)/(c*e+c*f-d*e-d*f)")]
        [DataRow("a/((b+c)*(d+e))", "a/(b*d+b*e+c*d+c*e)")]
        [DataRow("(a*b)/c", "(a*b)/c")]
        [DataRow("(a+b)/(c*(d+e))", "(a+b)/(c*d+c*e)")]
        [DataRow("a/(b-c)", "a/(b-c)")]
        [DataRow("(a+b)/(c+d)/e", "(a+b)/(c*e+d*e)")]
        [DataRow("(a*b)/(c-d)", "(a*b)/(c-d)")]
        [DataRow("(a/b)/(c/d)", "(a/b)/(c/d)")]
        [DataRow("a/(b/(c+d))", "a/(b/(c+d))")]
        [DataRow("(a+b)/(c-d)/(e-f)", "(a+b)/(c*e-c*f-d*e+d*f)")]
        [DataRow("a/(b+c)/d", "a/(b*d+c*d)")]
        [DataRow("a/(b*c*d)", "a/(b*c*d)")]
        [DataRow("(a+b)/(c-d)/(e+f)*g", "(a+b)/(c*e+c*f-d*e-d*f)*g")]
        [DataRow("a/(b+c)*(d/e)", "a/(b+c)*(d/e)")]
        [DataRow("a/(b+c)*(d+e)", "a/(b+c)*(d+e)")]
        [DataRow("(a-b)/((c+d)*(e/f))", "(a-b)/((c*e+d*e)/f)")]
        [DataRow("(a+b*c)/(d-e*f)", "(a+b*c)/(d-e*f)")]
        [DataRow("(a+b)/(c-d)/(e-f)*g/h", "(a+b)/(c*e-c*f-d*e+d*f)*(g/h)")]
        [DataRow("(a+b)/(c+d)/(e*f)", "(a+b)/(c*e*f+d*e*f)")]
        [DataRow("(a/b)/(c/(d+e))", "(a/b)/(c/(d+e))")]
        [DataRow("(a+b)/(c+d)/((e-f)*g)", "(a+b)/(c*e*g-c*f*g+d*e*g-d*f*g)")]  
        [DataRow("(a-b*c)/(d+e*f)/(g-h)", "(a-b*c)/(d*g-d*h+e*f*g-e*f*h)")]
        [DataRow("a/(b+c)/(d-e)/(f+g)", "a/(b*d*f+b*d*g-b*e*f-b*e*g+c*d*f+c*d*g-c*e*f-c*e*g)")]
        [DataRow("(a*b*c)/(d+e)/(f*g*h)", "(a*b*c)/(d*f*g*h+e*f*g*h)")]
        [DataRow("(a+b)/(c*d)/(e/f)", "(a+b)/(c*d*e/f)")]  
        [DataRow("(a/b)/(c+d)/(e*f*g)", "(a/b)/(c*e*f*g+d*e*f*g)")]
        [DataRow("(a+b)/(c-d)/(e-f)/(g+h)", "(a+b)/(c*e*g+c*e*h-c*f*g-c*f*h-d*e*g-d*e*h+d*f*g+d*f*h)")]
        [DataRow("a/(b*c*d)/(e-f+g)", "a/(b*c*d*e-b*c*d*f+b*c*d*g)")]


        [DataRow("a+(b-(c+d-e))", "a+b-c-d+e")]
        [DataRow("(a-b)*(c+(d-e))", "a*c+a*d-a*e-b*c-b*d+b*e")]
        [DataRow("((a+b)*c)-(d+e)", "a*c+b*c-d-e")]
        [DataRow("a-(b*(c+d-e))", "a-b*c-b*d+b*e")]
        [DataRow("(a+b)*(c/(d+e))", "(a*c+b*c)/(d+e)")]
        [DataRow("(a+b)*(c-d)*(e+f)", "a*c*e+a*c*f-a*d*e-a*d*f+b*c*e+b*c*f-b*d*e-b*d*f")]
        [DataRow("(a*b)*(c/(d-e))", "a*b*c/(d-e)")]
        [DataRow("(a*b)*(c+b)/(d-e)", "(a*b*c+a*b*b)/(d-e)")]
        [DataRow("(a+(b*c))-(d/(e-f))", "a+b*c-d/(e-f)")]
        [DataRow("(a+b)*(c-d)/(e+f)", "(a*c-a*d+b*c-b*d)/(e+f)")]
        [DataRow("a*((b+c)-(d/(e+f)))", "a*b+a*c-a*d/(e+f)")]
        [DataRow("a/(b+(c-d))", "a/(b+c-d)")]
        [DataRow("((a+b)*c)-(d/(e+f))", "a*c+b*c-d/(e+f)")]
        [DataRow("(a/(b+c))-(d*e)", "a/(b+c)-d*e")]
        [DataRow("a/(b*(c+d-e))", "a/(b*c+b*d-b*e)")]
        [DataRow("(a+b)/(c*(d+e)-f)", "(a+b)/(c*d+c*e-f)")]
        [DataRow("(a+b)*(c/(d-e)+f)", "a*c/(d-e)+a*f+b*c/(d-e)+b*f")] // 
        [DataRow("(a+b-c)*(d+e*f)", "a*d+a*e*f+b*d+b*e*f-c*d-c*e*f")]
        [DataRow("((a+b)/(c-d))*(e+f)", "(a*e+a*f+b*e+b*f)/(c-d)")]
        [DataRow("(a+b*c)*(d-e)/(f+g)", "(a*d-a*e+b*c*d-b*c*e)/(f+g)")]
        [DataRow("a*(b+(c/(d-e)))", "a*b+a*c/(d-e)")]
        [DataRow("(a-b)*(c/(d+e*f))", "(a*c-b*c)/(d+e*f)")]
        [DataRow("(a+b)/(c-d)/(e+f*g)", "(a+b)/(c*e+c*f*g-d*e-d*f*g)")]
        [DataRow("a*(b+c)/(d-(e-f))", "(a*b+a*c)/(d-e+f)")]
        [DataRow("(a+(b-c))*((d+e)/(f-g))", "(a*d+a*e+b*d+b*e-c*d-c*e)/(f-g)")]
        [DataRow("(a*b)/(c+(d-e))", "(a*b)/(c+d-e)")]
        [DataRow("(a+b)*(c-d-e)/(f+g)", "(a*c-a*d-a*e+b*c-b*d-b*e)/(f+g)")]
        [DataRow("((a+b)*c)/(d-e-f)", "(a*c+b*c)/(d-e-f)")]
        [DataRow("(a+b)*(c+d)/(e-(f+g))", "(a*c+a*d+b*c+b*d)/(e-f-g)")]


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

