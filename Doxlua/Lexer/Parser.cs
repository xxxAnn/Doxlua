using Doxlua.Tokenizer;
using Doxlua.VM;
using Pidgin;

namespace Doxlua.Lexer
{
    public static class StatementParser
    {
        // Returns a parser which parses one token of the specified type

        static Parser<IToken, T> TokenOfType<T>() where T : class, IToken =>
            Parser<Doxlua.Tokenizer.IToken>.Any
                .Where(x => x is T)
                .Select(x => (T)x);
        static Parser<T, U> TokenValue<T, U>(U type) where T : class, IToken<U> where U : struct, System.Enum =>
            Parser<T>.Any
                .Where(x => x.GetValue().Equals(type))
                .Select(x => x.GetValue());

        static Parser<T, string> TokenValue<T>(string type) where T : class, IToken<string> =>
            Parser<T>.Any
                .Where(x => x.GetValue().Equals(type))
                .Select(x => x.GetValue());


        // Usage:
        static class BaseParsers 
        {
            static Parser<IToken, Identifier> Identifier => TokenOfType<Identifier>();
            static Parser<IToken, Keyword> Keyword => TokenOfType<Keyword>();
            static Parser<IToken, Literal> Literal => TokenOfType<Literal>();
            static Parser<IToken, Operator> Operator => TokenOfType<Operator>();
            static Parser<IToken, Punctuation> Punctuation => TokenOfType<Punctuation>();
        }
        
        static class AtomicParsers 
        {
            static Parser<Keyword, KeywordType> Keyword(KeywordType type) => TokenValue<Keyword, KeywordType>(type);

            static Parser<Identifier, string> Identifier(string id) => TokenValue<Identifier>(id);

            static Parser<Operator, OperatorType> Operator(OperatorType type) => TokenValue<Operator, OperatorType>(type);
            
            static Parser<Punctuation, PunctuationType> Punctuation(PunctuationType type) => TokenValue<Punctuation, PunctuationType>(type);
        }

        // Molecular parsers

        static class LiteralParsers
        {
            static Parser<Literal, DoxString> StringLiteral => Parser<Literal>.Any
                .Where(x => x.GetValue() == LiteralType.String)
                .Select(x => new DoxString(x.GetInnerString()));
            static Parser<Literal, DoxNumber> NumberLiteral => Parser<Literal>.Any
                .Where(x => x.GetValue() == LiteralType.Number)
                .Select(x => new DoxNumber(x.GetInnerNumber()));
            static Parser<Literal, DoxBoolean> BooleanLiteral => Parser<Literal>.Any
                .Where(x => x.GetValue() == LiteralType.Boolean)
                .Select(x => new DoxBoolean(x.GetInnerBoolean()));
            static Parser<Literal, DoxNil> NilLiteral => Parser<Literal>.Any
                .Where(x => x.GetValue() == LiteralType.Nil)
                .Select(x => new DoxNil());
        }

    }
}