using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticExpressionAnalyzer
{
    public class Token
    {
        public int Index { get; set; }
        public string Value { get; set; }

        public Token(int index, string value)
        {
            Index = index;
            Value = value;
        }
    }
}
