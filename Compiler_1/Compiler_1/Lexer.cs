using System.Text.Json;

namespace Lexer;

public class Lexer
{
    private StreamReader _reader;
    private string _buffer;

    char Read()
    {
        var c = (char)_reader.Read();
        _buffer += c;
        return c;
    }

    public Lexer(StreamReader reader)
    {
        _reader = reader;
    }

    public Token NextToken()
    {

        var c = (char)_reader.Peek();

        string[] separators = { "[", "]", "(", ")", ".", ":", ";" };
         
         for (int i = 0; i < separators.Length; i++)
         {
             if (_buffer  == separators[i])
             {
                 return new Token(TokenType.SEPARATORS, "", _buffer);
             }
         }
         

        if (c is >= '0' and <= '9')
        {
            return Number();
        }

        {
           

            if (c is >= 'a' and <= 'z' || c is >= 'A' and <= 'Z')
            {
                return Id();
            }

        }

        return new Token(TokenType.NONE, " ", _buffer);



    }


    Token Number()
    {
        while ('0' <= _reader.Peek() && _reader.Peek() <= '9')
        {
            Read();
        }

        if (('0' <= _reader.Peek() && _reader.Peek() <= '9')|| '_' <= _reader.Peek()) 
        {
            return new Token(TokenType.IDENTIFIER, _buffer, _buffer);
        }
        return new Token(TokenType.INT, _buffer, _buffer);
    }

    public enum Keywords
    {
        CASE,
        REAL,
        INTEGER,
        BEGIN,
        DOWNTO,
        END,
        WRITELN,
        WRITE,
        VAR,
        REGISTER,
        TYPEDEF,
        CHAR,
        EXTERN,
        RESTRICT,
        RETURN,
        UNION,
        CONST,
        FLOAT,
        AUTO,
        DOUBLE,
        INLINE,
        INT,
        STRUCT,
        BREAK,
        ELSE,
        LONG,
        SWITCH,
        VOLATILE,
        DO,
        IF,
        STATIC,
        WHILE,
        SHORT,
        UNSIGNED,
        CONTINUE,
        FOR,
        SIGNED,
        VOID,
        DEFAULT,
        GOTO,
        SIZEOF,
        PROGRAM
    }

    Token Id()
    {
        while (('a' <= _reader.Peek() && _reader.Peek() <= 'z') || ('A' <= _reader.Peek() && _reader.Peek() <= 'Z'))
        {
            Read();
        }

        if (Enum.IsDefined(typeof(Keywords), _buffer))
        {
            return new Token(TokenType.KEYWORD, _buffer, _buffer);
        }

        if (('a' <= _reader.Peek() && _reader.Peek() <= 'z') || ('A' <= _reader.Peek() && _reader.Peek() <= 'Z') 
            || '_' <= _reader.Peek())
        {
            return new Token(TokenType.IDENTIFIER, _buffer, _buffer);
        }
        return new Token(TokenType.IDENTIFIER, _buffer, _buffer);
    }







    /*

    Token SEParators()
    {
        switch (_buffer)
        {
            case "[": return new Token(TokenType.SEPARATORS, "LSBRACKET", _buffer);
            case "]": return new Token(TokenType.SEPARATORS, "RSBRACKET", _buffer);
            case "(": return new Token(TokenType.SEPARATORS, "LRBRACKET", _buffer);
            case ")": return new Token(TokenType.SEPARATORS, "RRBRACKET", _buffer);
            case ".": return new Token(TokenType.SEPARATORS, "DOT", _buffer);
            case ",": return new Token(TokenType.SEPARATORS, "COMMA", _buffer);
            case "?": return new Token(TokenType.SEPARATORS, "QUESTION", _buffer);
            case ":": return new Token(TokenType.SEPARATORS, "SEM", _buffer);
            case ";": return new Token(TokenType.SEPARATORS, "COL", _buffer);
            case "..": return new Token(TokenType.SEPARATORS, "RANGE", _buffer);
        }

        if (c is ' ' or '\n' or '\t' or '\r') 
        {
            Read();
        }

        return new Token(TokenType.SEPARATORS, " ", _buffer);

    }
    */



}

