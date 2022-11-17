namespace Lexer
{
    public enum TokenType
    {
        CHAR,
        NONE,
        DOUBLE,
        INT,  
        STRING,
        IDENTIFIER,
        KEYWORD,
        OPERATOR,
        SEPARATORS,
        EOF
    }

    public class Token
    {
        public Token(TokenType tokenType, object value, string source )
        {
            TokenType = tokenType;
            Source = source;
            Value = value;
        }

        public TokenType TokenType { get; }
        public string Source { get; }
        public object Value { get; }



        public override string ToString()
        {
            return $"{TokenType}\t{Value}\t{Source}";
        }
    }


}
