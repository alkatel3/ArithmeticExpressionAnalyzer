using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public enum TokenType
    {
        function,
        operation,
        digit,
        variable,
        openBrake,
        closeBrake,
        unknown
    }

}
