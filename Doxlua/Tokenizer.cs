
namespace Doxlua.Tokenizer 
{
    /// <summary>
    /// Tokenization of Lua source code.
    /// <list type ="bullet">
    /// <item><description>Token represents a single token in the Lua source code</description></item>
    /// <item><description>Token can be: Keyword, Identifier, Literal, Operator, Punctuation</description></item>
    /// <item><description>Literal can be: String, Number, Boolean, Nil</description></item>
    /// <item><description>Punctuation can be: ParOpen, ParClose, BraceOpen, BraceClose, BracketOpen, BracketClose, Comma, Comment</description></item>
    /// <item><description>Keyword can be: Function, Local, If, Else, ElseIf, For, While, Do, End, Return</description></item>
    /// <item><description>Operator can be: Plus, Minus, Multiply, Divide, Modulus, Power, Concatenate
    ///    Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual</description></item>
    /// <item><description>Identifier is a name that represents a variable, function, or table</description></item>
    /// </list>
    /// </summary>
    static class Tokenizer 
    {

    }


    // Internal Token representation
    interface IToken
    {
        /// <summary>
        /// Get the type of the token
        /// </summary>
        /// <returns>Type of the token</returns>
        string GetTType();

        /// <summary>
        /// Get the line number of the token
        /// </summary>
        /// <returns>Line number of the token</returns>
        int GetLineNumber();
    }

    interface IToken<T> : IToken
    {
        /// <summary>
        /// Get the value of the token
        /// </summary>
        /// <returns>Value of the token</returns>
        T GetValue();
    }

    static class TokenType
    {
        public const string Keyword = "Keyword";
        public const string Identifier = "Identifier";
        public const string Literal = "Literal";
        public const string Operator = "Operator";
        public const string Punctuation = "Punctuation";
    }

    class LineNumberTracker
    {
        private int LineNumber;

        public LineNumberTracker(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        public int GetLineNumber()
        {
            return LineNumber;
        }
    }

    enum KeywordType
    {
        Function,
        Local,
        If,
        Else,
        ElseIf,
        For,
        While,
        Do,
        End,
        Return
    }

    class Keyword : LineNumberTracker, IToken<KeywordType>
    {
        KeywordType Value;

        public Keyword(KeywordType value, int lineNumber) : base(lineNumber)
        {
            Value = value;
        }

        public string GetTType()
        {
            return TokenType.Keyword;
        }

        public KeywordType GetValue()
        {
            return Value;
        }
    }

    enum PunctuationType
    {
        ParOpen,
        ParClose,
        BraceOpen,
        BraceClose,
        BracketOpen,
        BracketClose,
        Comma,
        Comment
    }

    class Punctuation : LineNumberTracker, IToken<PunctuationType>
    {
        PunctuationType Value;

        public Punctuation(PunctuationType value, int lineNumber) : base(lineNumber)
        {
            Value = value;
        }

        public string GetTType()
        {
            return TokenType.Punctuation;
        }

        public PunctuationType GetValue()
        {
            return Value;
        }
    }

    namespace Literals
    {
        enum LiteralType
        {
            String, // contain a string
            Number, // contain an integer or float
            Boolean, // contain a boolean value
            Nil // contain a null value
        }

        // the literal type is more complex than the other types
        // because it can contain a string, number, boolean, or nil
        // on top of the literaltype
        class Literal : LineNumberTracker, IToken<LiteralType>
        {
            LiteralType Type;
            LiteralValue Value;
            
            class LiteralValue
            {
                public string? StringValue { get; set; }
                public double? NumberValue { get; set; }
                public bool? BooleanValue { get; set; }
                public object? NilValue { get; set; }
            }

            public Literal(string value, int lineNumber) : base(lineNumber)
            {
                Type = LiteralType.String;
                Value = new LiteralValue { StringValue = value };
            }

            public Literal(double value, int lineNumber) : base(lineNumber)
            {
                Type = LiteralType.Number;
                Value = new LiteralValue { NumberValue = value };
            }

            public Literal(bool value, int lineNumber) : base(lineNumber)
            {
                Type = LiteralType.Boolean;
                Value = new LiteralValue { BooleanValue = value };
            }

            public Literal(int lineNumber) : base(lineNumber)
            {
                Type = LiteralType.Nil;
                Value = new LiteralValue { NilValue = null };
            }

            public string GetTType()
            {
                return TokenType.Literal;
            }

            public LiteralType GetValue()
            {
                return Type;
            }

            public string GetInnerString()
            {
                if (Type == LiteralType.String)
                {
                    return Value.StringValue ?? string.Empty;
                }
                else
                {
                    throw new InvalidOperationException("Literal is not a string");
                }
            }

            public double GetInnerNumber()
            {
                if (Type == LiteralType.Number)
                {
                    return Value.NumberValue ?? 0;
                }
                else
                {
                    throw new InvalidOperationException("Literal is not a number");
                }
            }

            public bool GetInnerBoolean()
            {
                if (Type == LiteralType.Boolean)
                {
                    return Value.BooleanValue ?? false;
                }
                else
                {
                    throw new InvalidOperationException("Literal is not a boolean");
                }
            }

            public object GetInnerNil()
            {
                if (Type == LiteralType.Nil)
                {
                    return Value.NilValue;
                }
                else
                {
                    throw new InvalidOperationException("Literal is not nil");
                }
            }
        }
        

    }

}


