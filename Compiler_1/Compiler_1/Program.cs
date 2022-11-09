using Lexer;

var sr = new StreamReader("C:/Users/Mixa/Desktop/Compiler_1/Test.txt");
var lexer = new Lexer.Lexer(sr);

for (;;)
{
    var token = lexer.NextToken();
    if (token.TokenType == TokenType.EOF)
    {
        Console.WriteLine(new Token(TokenType.EOF, ' ', ""));
        break;
    }
    Console.WriteLine(token);
}
