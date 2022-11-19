using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexer;
using static Lexer.Token;

namespace Compiler_1.Parser
{
    public class Parser
    {
       
        private Token _readToken;
        private string _buffer;

        public Parser(Token readToken, string buffer)
        {
            _readToken = readToken;
            _buffer = readToken.NextToken();
        }



    }
}
