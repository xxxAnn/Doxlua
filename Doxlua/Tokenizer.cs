
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

}


