
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


    /// <summary>
    /// Token represents a single token in the Lua source code
    /// <list type ="bullet">
    /// <item><description>Token can be: Keyword, Identifier, Literal, Operator, Punctuation</description></item>
    /// <item><description>Token can be: String, Number, Boolean, Nil</description></item>
    /// <item><description>Token can be: ParOpen, ParClose, BraceOpen, BraceClose, BracketOpen, BracketClose, Comma, Comment</description></item>
    /// <item><description>Token can be: Function, Local, If, Else, ElseIf, For, While, Do, End, Return</description></item>
    /// <item><description>Token can be: Plus, Minus, Multiply, Divide, Modulus, Power, Concatenate
    ///    Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual</description></item>
    /// <item><description>Token can be: Identifier is a name that represents a variable, function, or table</description></item>
    /// </list>
    /// </summary>
    public interface IToken
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

    public static class Utils
    {
        public static string Info(IToken token)
        {
            return $"{token.GetTType()} L{token.GetLineNumber()}";
        }
    }

    static class TokenType
    {
        public const string Identifier = "Identifier";
        public const string Keyword = "Keyword";
        public const string Literal = "Literal";
        public const string Operator = "Operator";
        public const string Punctuation = "Punctuation";
    }

    public class LineNumberTracker(int lineNumber)
    {
        private readonly int LineNumber = lineNumber;

        public int GetLineNumber()
        {
            return LineNumber;
        }
    }
}

// Identifier
namespace Doxlua.Tokenizer
{
    /// <summary>
    /// Identifier is a name that represents a variable, function, or table
    /// </summary>
    public class Identifier(string value, int lineNumber) : LineNumberTracker(lineNumber), IToken<string>
    {
        readonly string Value = value;

        public string GetTType()
        {
            return TokenType.Identifier;
        }

        public string GetValue()
        {
            return Value;
        }
        
        override public string ToString()
        {
            return $"{Utils.Info(this)} {Value}";
        }
    }
}

// Keyword
namespace Doxlua.Tokenizer
{
    public enum KeywordType
    {
        // Our special keywords
        effect,
        trigger,
        value,
        scope,
        lua,
        // Lua keywords
        and,
        @break,
        @in,
        @not,
        @or,
        @repeat,
        @return,
        @then,
        @until,
        @while,
        local,
        function,
        @if,
        @else,
        @elseif,
        @for,
        @do,
        @end
    }

    /// <summary>
    /// Keyword is a reserved word in Lua that has a special meaning
    /// </summary>
    public class Keyword(KeywordType value, int lineNumber) : LineNumberTracker(lineNumber), IToken<KeywordType>
    {
        readonly KeywordType Value = value;

        public string GetTType()
        {
            return TokenType.Keyword;
        }

        public KeywordType GetValue()
        {
            return Value;
        }

        override public string ToString()
        {
            return $"{Utils.Info(this)} {Value}";
        }
    }
}

// Literal
namespace Doxlua.Tokenizer
{
    public enum LiteralType
    {
        String, // contain a string
        Number, // contain an integer or float
        Boolean, // contain a boolean value
        Nil // contain a null value
    }

    // the literal type is more complex than the other types
    // because it can contain a string, number, boolean, or nil
    // on top of the literaltype
    public class Literal : LineNumberTracker, IToken<LiteralType>
    {
        readonly LiteralType Type;
        readonly LiteralValue Value;

        public enum ValueType
        {
            String,
            Number,
            Boolean,
            Nil
        }
        
        class LiteralValue
        {
            public string? StringValue { get; set; }
            public double? NumberValue { get; set; }
            public bool? BooleanValue { get; set; }
            public object? NilValue { get; set; }

            public override string ToString()
            {
                return $"'{StringValue ?? NumberValue?.ToString() ?? BooleanValue?.ToString() ?? NilValue?.ToString() ?? string.Empty}'";
            }

            public ValueType GetValueType()
            {
                if (StringValue != null)
                {
                    return ValueType.String;
                }
                else if (NumberValue != null)
                {
                    return ValueType.Number;
                }
                else if (BooleanValue != null)
                {
                    return ValueType.Boolean;
                }
                else
                {
                    return ValueType.Nil;
                }
            }
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

        override public string ToString()
        {
            return $"{Utils.Info(this)} {Value.GetValueType()} {Value}";
        }
    }
    

}

// Operator
namespace Doxlua.Tokenizer 
{
    public enum OperatorType
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulus,
        Power,
        Assignment,
        Concatenate,
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual
    }

    public class Operator(OperatorType value, int lineNumber) : LineNumberTracker(lineNumber), IToken<OperatorType>
    {
        readonly OperatorType Value = value;

        public string GetTType()
        {
            return TokenType.Operator;
        }

        public OperatorType GetValue()
        {
            return Value;
        }

        override public string ToString()
        {
            return $"{Utils.Info(this)} {Value}";
        }
    }
}

// Punctuation
namespace Doxlua.Tokenizer
{
    public enum PunctuationType
    {
        ParOpen,
        ParClose,
        BraceOpen,
        BraceClose,
        BracketOpen,
        BracketClose,
        Dot,
        Colon,
        Semicolon,
        Hashtag,
        Comma,
        Comment,
        MultilineComment,
        EOL // end of line
    }

    public class Punctuation(PunctuationType value, int lineNumber) : LineNumberTracker(lineNumber), IToken<PunctuationType>
    {
        readonly PunctuationType Value = value;

        public string GetTType()
        {
            return TokenType.Punctuation;
        }

        public PunctuationType GetValue()
        {
            return Value;
        }

        override public string ToString()
        {
            return $"{Utils.Info(this)} {Value}";
        }
    }
}




