

using Doxlua.VM;
using static Doxlua.Lexer.StatementParser;
using Pidgin;
using static Pidgin.Parser;
using Doxlua.Tokenizer;


namespace Doxlua.Lexer
{
    public static class Atoms
    {
            static Parser<IToken, U> TokenValue<T, U>(U type) where T : class, IToken<U> where U : struct, System.Enum =>
                Parser<IToken>.Any
                    .Where(x => x is T)
                    .Where(x => ((T)x).GetValue().Equals(type))
                    .Select(x => ((T)x).GetValue());

            static Parser<IToken, string> TokenValue<T>() where T : class, IToken<string> =>
                Parser<IToken>.Any
                    .Where(x => x is T)
                    .Select(x => ((T)x).GetValue());

            
            public static class AtomicParsers 
            {
                public static Parser<IToken, KeywordType> Keyword(KeywordType type) => TokenValue<Keyword, KeywordType>(type);
    
                public static Parser<IToken, string> Identifier => TokenValue<Identifier>();
    
                public static Parser<IToken, OperatorType> Operator(OperatorType type) => TokenValue<Operator, OperatorType>(type);
                
                public static Parser<IToken, PunctuationType> Punctuation(PunctuationType type) => TokenValue<Punctuation, PunctuationType>(type);
            }

            // Molecular parsers

            public static class LiteralParsers
            {
                static Parser<IToken, DoxString> StringLiteral => Parser<IToken>.Any
                    .Where(x => x is Literal)
                    .Where(x => ((Literal)x).GetValue() == LiteralType.String)
                    .Select(x => new DoxString(((Literal)x).GetInnerString()));
                public static Parser<IToken, DoxNumber> NumberLiteral => Parser<IToken>.Any
                    .Where(x => x is Literal)
                    .Where(x => ((Literal)x).GetValue() == LiteralType.Number)
                    .Select(x => new DoxNumber(((Literal)x).GetInnerNumber()));
                static Parser<IToken, DoxBoolean> BooleanLiteral => Parser<IToken>.Any
                    .Where(x => x is Literal)
                    .Where(x => ((Literal)x).GetValue() == LiteralType.Boolean)
                    .Select(x => new DoxBoolean(((Literal)x).GetInnerBoolean()));
                static Parser<IToken, DoxNil> NilLiteral => Parser<IToken>.Any
                    .Where(x => x is Literal)
                    .Where(x => ((Literal)x).GetValue() == LiteralType.Nil)
                    .Select(x => new DoxNil());

                public static Parser<IToken, IDoxValue> AnyLiteral => 
                    Try(StringLiteral.Select(x => x as IDoxValue))
                        .Or(Try(NumberLiteral.Select(x => x as IDoxValue)))
                        .Or(Try(BooleanLiteral.Select(x => x as IDoxValue)))
                        .Or(Try(NilLiteral.Select(x => x as IDoxValue)));
            }
    }
}