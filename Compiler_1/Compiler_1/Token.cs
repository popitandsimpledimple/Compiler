namespace Lexer
{
    public enum TokenType
    {
        NONE,
        REAL,
        INT,
        CHAR,
        STRING,
        IDENTIFIER,
        KEYWORD,
        OPERATOR,
        SEPARATORS
    }

    public class Token
    {
        public Token(TokenType tokenType, string source, object value)
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
            return $"{TokenType}\t{Source}\t{Value}";
        }
    }


}