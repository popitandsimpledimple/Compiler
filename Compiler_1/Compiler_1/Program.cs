var sr = new StreamReader("C:/Users/Mixa/Desktop/Compiler_1/Test.txt");
var lexer = new Lexer.Lexer(sr);

for (; ; )
{
    var token = lexer.NextToken();
    Console.WriteLine(token);
    break;

}