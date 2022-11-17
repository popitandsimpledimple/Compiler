using System;
using System.Globalization;

namespace Lexer;
public class Lexer
{
    private StreamReader _text;
    private string _clipboard;
    
    char Read()
    {
        var c = (char)_text.Read();
        _clipboard += c;
        return c;
        
    }
   
    public Lexer(StreamReader reader) 
    {
        _text = reader;
    }
    public Token NextToken()
    {
        _clipboard = "";
        var c = (char)_text.Peek();
        


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
                for (; ; )  
                {
                    Read();
                    if (_text.Peek() == '\'')
                    {
                        Read();
                        break;
                    }
                }
                return new Token(TokenType.STRING, _clipboard, _clipboard);
            }

            if (c == '/')
            {
                Read();
                if (_text.Peek() == '/')
                {
                    while (_text.Peek() != '\n')
                    {
                        Read();
                        if (_text.EndOfStream)
                            break;
                    }
                }
            }

            if (c == '{')
            {
                Read();
                while (_text.Peek() != '}')
                {
                    Read();
                    if (_text.EndOfStream)
                        break;
                }
            }


            if ((_text.Peek() is >= 'a' and <= 'z') || (_text.Peek() is >= 'A' and <= 'Z') || (_text.Peek() is '_'))
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

            switch (_clipboard)
            {
                case "[": return new Token(TokenType.SEPARATORS, Separators.LSBRACKET, _clipboard);
                case "]": return new Token(TokenType.SEPARATORS, Separators.RSBRACKET, _clipboard);
                case ")": return new Token(TokenType.SEPARATORS, Separators.RRBRACKET, _clipboard);
                case ".": return new Token(TokenType.SEPARATORS, Separators.DOT, _clipboard);
                case ",": return new Token(TokenType.SEPARATORS, Separators.COMMA, _clipboard);
                case ";": return new Token(TokenType.SEPARATORS, Separators.SEM, _clipboard);
                case "..": return new Token(TokenType.SEPARATORS, Separators.RANGE, _clipboard);
                case "=": return new Token(TokenType.OPERATOR, Operator.Equal, _clipboard);
                case "(":
                    {
                        if (_text.Peek() == '*')
                        {
                            Read();
                            for (; ; )
                            {
                                Read();
                                if (_clipboard.EndsWith('*') && (_text.Peek() == ')'))
                                {
                                    Read();
                                    return NextToken();
                                }
                                if (_text.EndOfStream)
                                {
                                    return new Token(TokenType.NONE, ' ', _clipboard);
                                }
                                
                            }
                        }
                        return new Token(TokenType.SEPARATORS, Separators.LRBRACKET, _clipboard);
                    }
                case ":":
                    {
                        if (_text.Peek() == '=')
                        {
                            Read();
                            if (_clipboard == ":=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.Assign, _clipboard);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, Separators.COL, _clipboard);
                    }
                case "<":
                    {
                        if ((_text.Peek() == '=') || (_text.Peek() == '>') || (_text.Peek() == '<'))
                        {
                            Read();
                            if (_clipboard == "<=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.LessThanOrEqual, _clipboard);
                            }
                            if (_clipboard == "<>")
                            {
                                return new Token(TokenType.OPERATOR, Operator.NotEqual, _clipboard);
                            }

                        }
                        return new Token(TokenType.SEPARATORS, Separators.LessThan, _clipboard);
                    }
                case ">":
                    {
                        if ((_text.Peek() == '=') || (_text.Peek() == '>'))
                        {
                            Read();
                            if (_clipboard == ">=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.GreaterOrEqual, _clipboard);
                            }

                        }
                        return new Token(TokenType.SEPARATORS, Separators.GreaterThan, _clipboard);
                    }
                case "+":
                    {
                        if (_text.Peek() == '=')
                        {
                            Read();
                            if (_clipboard == "+=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.AdditionAssign, _clipboard);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, Separators.Addition, _clipboard);
                    }
                case "-":
                    {
                        if (_text.Peek() == '=')
                        {
                            Read();
                            if (_clipboard == "-=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.SubtractionAssign, _clipboard);
                            }
                        }
                        return new Token(TokenType.SEPARATORS, Separators.Subtraction, _clipboard);
                    }
                case "*":
                    {
                        if (_text.Peek() == '=')
                        {
                            Read();
                            Read();
                            if (_clipboard == "*=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.MultiplicationAssign, _clipboard);
                            }
                        }
                        return new Token(TokenType.OPERATOR, Operator.Multiplication, _clipboard);
                    }
                case "/":
                    {
                        if (_text.Peek() == '=')
                        {
                            Read();
                            if (_clipboard == "/=")
                            {
                                return new Token(TokenType.OPERATOR, Operator.DivisionAssign, _clipboard);
                            }
                        }
                        return new Token(TokenType.OPERATOR, Operator.Division, _clipboard);
                    }
            }

            {
                if (_text.EndOfStream)
                    return new Token(TokenType.EOF, ' ', _clipboard);
            }
        }

        return NextToken();
    }

    bool IsDigit(char c, int numeralSystem)
    {
        return numeralSystem switch
        {
            10 => '0' <= c && c <= '9',
            16 => ('a' <= c && c <= 'f' || 'A' <= c && c <= 'F' || '0' <= c && c <= '9'),
            2 => '0' <= c && c <= '1',
            8 => '0' <= c && c <= '7',
        };
    }


    Token NumChar()
    {
        if (_clipboard == "#")
        {
            if (IsDigit((char) _text.Peek(), 10))
            {
                while ('0' <= _text.Peek() && _text.Peek() <= '9')
                {
                    Read();
                }

                if (_text.Peek() == '.')
                {
                    Read();
                    if (_text.Peek() != '.')
                        while ('0' <= _text.Peek() && _text.Peek() <= '9')
                            Read();
                    return new Token(TokenType.DOUBLE, _clipboard, _clipboard);
                }

                return new Token(TokenType.CHAR, _clipboard, _clipboard);
            }
        }
        return new Token(TokenType.NONE, _clipboard, _clipboard);
    }



    Token NumSystem()
    {
        int numeralSystem = _clipboard switch
        {
            "%" => 2,
            "&" => 8,
            "$" => 16,
            _ => 10
        };

        while (IsDigit((char) _text.Peek(), numeralSystem))
        {
            Read();
            
            if (_text.Peek() == '.')
            {
                Read();
                if (_text.Peek() != '.')
                    while ('0' <= _text.Peek() && _text.Peek() <= '9')
                        Read();
            }

        }

        return new Token(TokenType.INT, _clipboard, _clipboard);

        if (_clipboard.Length == 1)
        {
            return new Token(TokenType.NONE, _clipboard, _clipboard);
        }

        return new Token(TokenType.INT, _clipboard, _clipboard);

    }


    Token Number()
    {
        while ('0' <= _text.Peek() && _text.Peek() <= '9')
        {
            Read();
        }

        if (_text.Peek() == '.')
        {
            Read();
            if (_text.Peek() != '.')
                while ('0' <= _text.Peek() && _text.Peek() <= '9')
                    Read();
            return new Token(TokenType.DOUBLE, _clipboard, _clipboard);
        }
        return new Token(TokenType.INT, _clipboard, _clipboard);
    }
    Token Id()
    {
        while (('a' <= _text.Peek() && _text.Peek() <= 'z') || ('1' <= _text.Peek() && _text.Peek() <= '9') ||
               '_' <= _text.Peek() || ('A' <= _text.Peek() && _text.Peek() <= 'Z'))
        {
            Read();
        }

        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        string keyword = ti.ToLower(_clipboard);
        if (Enum.IsDefined(typeof(Keywords), ti.ToTitleCase(keyword)))
            {
                return new Token(TokenType.KEYWORD, _clipboard, _clipboard); 
            }
        return new Token(TokenType.IDENTIFIER, _clipboard, _clipboard);

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


    public enum Operator
    {
        Equal,
        Assign,
        LessThanOrEqual,
        NotEqual,
        GreaterOrEqual,
        AdditionAssign,
        SubtractionAssign,
        MultiplicationAssign,
        Multiplication,
        DivisionAssign,
        Division
    }

    public enum Separators
    {
        LSBRACKET,
        RSBRACKET,
        LRBRACKET,
        RRBRACKET,
        DOT,
        COMMA,
        SEM,
        RANGE,
        COL,
        LessThan,
        GreaterThan,
        Addition,
        Subtraction

    }

}

