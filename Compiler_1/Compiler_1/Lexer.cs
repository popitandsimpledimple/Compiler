using System;
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
    bool EOF()
    {
      return _reader.EndOfStream;
    }
    public Lexer(StreamReader reader) //StreamReader предназначен для ввода символов в определенной кодировке
    {
        _reader = reader;
    }
    public Token NextToken()
    {
        _buffer = "";
        var c = (char)_reader.Peek();


        if (c == '#')
        {
            Read();
            return NumChar();
        }


        if (c is >= '0' and <= '9')
        {
            return Number();
        }

        {
            if (c == '\'')
            {
                for (;;)  // диапозон работ
                {
                    Read();
                    if (_reader.Peek() == '\'')
                    {
                        Read();
                        break;
                    }
                }
                return new Token(TokenType.STRING, _buffer, _buffer);
            }

            if (c == '/') 
            {
                Read();
                if (_reader.Peek() == '/')
                {
                    while (_reader.Peek() != '\n')
                    {
                        Read();
                        if (EOF())
                            break;
                    }
                }
            }

            if (c == '{')
            {
                Read();
                while (_reader.Peek() != '}')
                {
                    Read();
                    if (EOF())
                        break;
                }
            }


            if ((_reader.Peek() is >= 'a' and <= 'z') || (_reader.Peek() is >= 'A' and <= 'Z') || (_reader.Peek() is '_')) 
            {
                Read();
                return Id();
            }

            if ((c is '%') || (c is '&') || (c is '$'))
            {
                Read();
                return NumSystem();
            }


            Read();

            switch (_buffer)
            {
                case "[": return new Token(TokenType.SEPARATORS, "LSBRACKET", _buffer);
                case "]": return new Token(TokenType.SEPARATORS, "RSBRACKET", _buffer);
                case "(": return new Token(TokenType.SEPARATORS, "LRBRACKET", _buffer);
                case ")": return new Token(TokenType.SEPARATORS, "RRBRACKET", _buffer);
                case ".": return new Token(TokenType.SEPARATORS, "DOT", _buffer);
                case ",": return new Token(TokenType.SEPARATORS, "COMMA", _buffer);
                case ";": return new Token(TokenType.SEPARATORS, "SEM", _buffer);
                case "..": return new Token(TokenType.SEPARATORS, "RANGE", _buffer);
                case "=": return new Token(TokenType.OPERATOR, "Equal", _buffer);
                case ":":
                    {
                        if (_reader.Peek() == '=')
                        {
                            Read();
                            if (_buffer == ":=")
                            {
                                return new Token(TokenType.OPERATOR, "Assign", _buffer);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, "COL", _buffer);
                    }
                case "<":
                    {
                        if ((_reader.Peek() == '=')||(_reader.Peek() == '>')|| (_reader.Peek() == '<')) 
                        {
                            Read();
                            if (_buffer == "<=")
                            {
                                return new Token(TokenType.OPERATOR, "LessThanOrEqual", _buffer);
                            }
                            if (_buffer == "<>")
                            {
                                return new Token(TokenType.OPERATOR, "NotEqual", _buffer);
                            }
                            if (_buffer == "<<")
                            {
                                return new Token(TokenType.OPERATOR, "BitwiseShiftToTheLeft", _buffer);
                            }

                        }
                        return new Token(TokenType.SEPARATORS, "LessThan", _buffer);
                    }
                case ">":
                    {
                        if ((_reader.Peek() == '=')||(_reader.Peek() == '>'))
                        {
                            Read();
                            if (_buffer == ">=")
                            {
                                return new Token(TokenType.OPERATOR, "GreaterOrEqual", _buffer);
                            }
                            if (_buffer == ">>")
                            {
                                return new Token(TokenType.OPERATOR, "BitwiseShiftToTheRight", _buffer);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, "GreaterThan", _buffer);
                    }                   
                case "+":
                    {
                        if (_reader.Peek() == '=')
                        {
                            Read();
                            if (_buffer == "+=")
                            {
                                return new Token(TokenType.OPERATOR, "AdditionAssign", _buffer);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, "Addition", _buffer);
                    }
                case "-":
                    {
                        if (_reader.Peek() == '=')
                        {
                            Read();
                            if (_buffer == "-=")
                            {
                                return new Token(TokenType.OPERATOR, "SubtractionAssign", _buffer);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, "Subtraction", _buffer);
                    }
                case "*":
                    {
                        if (_reader.Peek() == '=')
                        {
                            Read();
                            Read();
                            if (_buffer == "*=")
                            {
                                return new Token(TokenType.OPERATOR, "MultiplicationAssign", _buffer);
                            }
                        }
                        return new Token(TokenType.OPERATOR, "Multiplication", _buffer);
                    }
                case "/":
                    {
                        if (_reader.Peek() == '=')
                        {
                            Read();
                            if (_buffer == "/=")
                            {
                                return new Token(TokenType.OPERATOR, "DivisionAssign", _buffer);
                            }
                        }
                        return new Token(TokenType.OPERATOR, "Division", _buffer);
                    }
            }

            {
                if (EOF())
                return new Token(TokenType.EOF,' ', _buffer);
            }
        }

        return NextToken();
    }


    Token NumChar()
    {
        if (_buffer == "#")
        {
            if ('0' <= _reader.Peek() && _reader.Peek() <= '9')
            {
                while ('0' <= _reader.Peek() && _reader.Peek() <= '9')
                {
                    Read();
                }
                return new Token(TokenType.CHAR, _buffer, _buffer);
            }
        }
        return new Token(TokenType.NONE, _buffer, _buffer);
    }

   Token NumSystem()
    {
        if (_buffer == "%")                                  
        {
            if ('0' <= _reader.Peek() && _reader.Peek() <= '1')
            {
                while ('0' <= _reader.Peek() && _reader.Peek() <= '1')
                {
                    Read();
                }
                return new Token(TokenType.INT, _buffer, _buffer);
            }
            return new Token(TokenType.NONE, _buffer, _buffer);
        }
        {
            if (_buffer == "&")
            {
                if ('0' <= _reader.Peek() && _reader.Peek() <= '7')
                {
                    while ('0' <= _reader.Peek() && _reader.Peek() <= '7')
                    {
                        Read();
                    }
                    return new Token(TokenType.INT, _buffer, _buffer);
                }
                return new Token(TokenType.NONE, _buffer, _buffer);
            }

            if (_buffer == "$")
            {
                if (('0' <= _reader.Peek() && _reader.Peek() <= '9') || ('A' <= _reader.Peek() && _reader.Peek() <= 'F') || ('a' <= _reader.Peek() && _reader.Peek() <= 'f'))
                {
                    while (('0' <= _reader.Peek() && _reader.Peek() <= '9') || ('A' <= _reader.Peek() && _reader.Peek() <= 'F') || ('a' <= _reader.Peek() && _reader.Peek() <= 'f'))
                    {
                        Read();
                    }
                    return new Token(TokenType.INT, _buffer, _buffer);
                }
                return new Token(TokenType.NONE, _buffer, _buffer);
            }
                return new Token(TokenType.NONE, _buffer, _buffer);
        }

   }


    Token Number()
    {
        while ('0' <= _reader.Peek() && _reader.Peek() <= '9')
        {
            Read();
        }

        if (_reader.Peek() == '.')
        {
            Read();
            if (_reader.Peek() != '.') 
                while ('0' <= _reader.Peek() && _reader.Peek() <= '9')
                    Read();
            return new Token(TokenType.REAL, _buffer, _buffer);
        }
        return new Token(TokenType.INT, _buffer, _buffer);
    }
    Token Id()
    {
        while (('a' <= _reader.Peek() && _reader.Peek() <= 'z') || ('1' <= _reader.Peek() && _reader.Peek() <= '9') ||
               '_' <= _reader.Peek() || ('A' <= _reader.Peek() && _reader.Peek() <= 'Z'))
        {
            Read();
        }


        if (Enum.IsDefined(typeof(Keywords), _buffer))
        {
            return new Token(TokenType.KEYWORD, _buffer, _buffer);
        }
        return new Token(TokenType.IDENTIFIER, _buffer, _buffer);
        
    }

   

    public enum Keywords
    {
        And,
        Array,
        Asm,
        Begin,
        Break,
        Case,
        Const,
        Constructor,
        Continue,
        Destructor,
        Div,
        Do,
        Downto,
        Else,
        End,
        False,
        File,
        For,
        Function,
        Goto,
        If,
        Implementation,
        In,
        Inline,
        Interface,
        Label,
        Mod,
        Nil,
        Not,
        Object,
        Of,
        On,
        Operator,
        Or,
        Packed,
        Procedure,
        Program,
        Record,
        Repeat,
        Set,
        Shl,
        Shr,
        String,
        Then,
        To,
        True,
        Type,
        Unit,
        Until,
        Uses,
        Var,
        While,
        With,
        Xor,
        As,
        Class,
        Constref,
        Dispose,
        Except,
        Exit,
        Exports,
        Finalization,
        Finally,
        Inherited,
        Initialization,
        Is,
        Library,
        New,
        Out,
        Property,
        Raise,
        Self,
        Threadvar,
        Try,
        Absolute,
        Abstract,
        Alias,
        Assembler,
        Cdecl,
        Cppdecl,
        Default,
        Export,
        External,
        Forward,
        Generic,
        Index,
        Local,
        Name,
        Nostackframe,
        Oldfpccall,
        Override,
        Pascal,
        Private,
        Protected,
        Public,
        Published,
        Read,
        Register,
        Reintroduce,
        Safecall,
        Softfloat,
        Specialize,
        Stdcall,
        Virtual,
        Write,
        Writeln,
        Readln
    }

}  

