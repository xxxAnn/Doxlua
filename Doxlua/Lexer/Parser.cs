using Doxlua.Tokenizer;
using Pidgin;
using static Doxlua.Lexer.Atoms;

namespace Doxlua.Lexer
{
    public static class StatementParser
    {
        //  _______  _______  _______  _______  _______  __   __  _______  __    _  _______ 
        // |       ||       ||   _   ||       ||       ||  |_|  ||       ||  |  | ||       |
        // |  _____||_     _||  |_|  ||_     _||    ___||       ||    ___||   |_| ||_     _|
        // | |_____   |   |  |       |  |   |  |   |___ |       ||   |___ |       |  |   |  
        // |_____  |  |   |  |       |  |   |  |    ___||       ||    ___||  _    |  |   |  
        //  _____| |  |   |  |   _   |  |   |  |   |___ | ||_|| ||   |___ | | |   |  |   |  
        // |_______|  |___|  |__| |__|  |___|  |_______||_|   |_||_______||_|  |__|  |___|  
        //  _______  _______  ______    _______  _______  ______    _______                 
        // |       ||   _   ||    _ |  |       ||       ||    _ |  |       |                
        // |    _  ||  |_|  ||   | ||  |  _____||    ___||   | ||  |  _____|                
        // |   |_| ||       ||   |_||_ | |_____ |   |___ |   |_||_ | |_____                 
        // |    ___||       ||    __  ||_____  ||    ___||    __  ||_____  |                
        // |   |    |   _   ||   |  | | _____| ||   |___ |   |  | | _____| |                
        // |___|    |__| |__||___|  |_||_______||_______||___|  |_||_______|        

        // Wahaha I'm Bowser!
        // Welcome to the Statement Parsers
        // What's here you ask? You see.. statements that get parsed!
        // Wahahahaha!!!

        ///// =========================
        ///// || Expression Handling ||
        ///// ==========================
        public static Parser<IToken, IExpression> PrimaryExpression =>
            Parser.OneOf(
                Parser.Try(Atoms.LiteralParsers.AnyLiteral.Select(ExpressionFactory.CreateLiteral)),
                Parser.Try(Atoms.AtomicParsers.Identifier.Select(ExpressionFactory.CreateIdentifier))
            );
        public interface ISuffix {}
        class FieldAccessSuffix(string identifier) : ISuffix
        {
            public string Identifier { get; } = identifier;
        }
        class IndexAccessSuffix(IExpression index) : ISuffix
        {
            public IExpression Index { get; } = index;
        }

        class FuncCallSuffix(IExpression[] args) : ISuffix 
        {
            public IExpression[] Args { get; } = args;
        }
        public static Parser<IToken, ISuffix> FieldAccess =>
            from dot in AtomicParsers.Punctuation(PunctuationType.Dot)
            from id in AtomicParsers.Identifier
            select new FieldAccessSuffix(id) as ISuffix;

        public static Parser<IToken, ISuffix> IndexAccess =>
            from open in AtomicParsers.Punctuation(PunctuationType.BracketOpen)
            from indexExpr in Expression
            from close in AtomicParsers.Punctuation(PunctuationType.BracketClose)
            select new IndexAccessSuffix(indexExpr) as ISuffix;

        public static Parser<IToken, ISuffix> FunctionCallSuffix =>
            from open in AtomicParsers.Punctuation(PunctuationType.ParOpen)
            from exprs in Expression.Many()
            from close in AtomicParsers.Punctuation(PunctuationType.ParClose)
            select new FuncCallSuffix(exprs.ToArray()) as ISuffix;

    public static Parser<IToken, IExpression> Expression =>
        PrimaryExpression.Then(
            Parser.OneOf(
                Parser.Try(FieldAccess),
                Parser.Try(IndexAccess),
                Parser.Try(FunctionCallSuffix)
            ).Labelled("Suffix").Many(),
            (a, b) => {
                IExpression result = a;
                foreach (var suffix in b)
                {
                    switch (suffix)
                    {
                        case FieldAccessSuffix fieldAccess:
                            result = new FieldAccessExpression(result, fieldAccess.Identifier);
                            break;
                        case IndexAccessSuffix indexAccess:
                            result = new IndexAccessExpression(result, indexAccess.Index);
                            break;
                        case FuncCallSuffix funcCall:
                            result = new FunctionCall(result, funcCall.Args);
                            break;
                    }
                }
                return result;
            }
        );

        ///// ===================
        ///// || Table Access1 ||
        ///// ===================
        public static Parser<IToken, FieldAccessExpression> TableAccess => 
            Parser.Map(
                (a, b) => new FieldAccessExpression(a, b),
                Expression.Before(
                    AtomicParsers.Punctuation(
                        PunctuationType.Dot
                    )
                ),
                AtomicParsers.Identifier
            );

        ///// ===================
        ///// || Table Access2 ||
        ///// ===================

        public static Parser<IToken, IndexAccessExpression> TableAccess2 => 
            Parser.Map(
                (a, b) => new IndexAccessExpression(a, b),
                Expression,
                Expression.Between(
                    AtomicParsers.Punctuation(
                        PunctuationType.BracketOpen
                    ),
                    AtomicParsers.Punctuation(
                        PunctuationType.BraceClose
                    )
                )
            );

        ///// =========================
        ///// || Statement Handling ||
        ///// ==========================
        public static Parser<IToken, IStatement> Statement => 
            Parser.OneOf(
                Parser.Try(Assignment.Select(x => x as IStatement)),
                Parser.Try(FunctionCall.Select(x => x as IStatement))
            );

        ///// ================
        ///// || Assignment ||
        ///// ================    
        public static Parser<IToken, Assignment> Assignment =>
            Parser.Map(
                (lhs, rhs) => new Assignment(lhs, rhs),
                Assignable.Before(
                    AtomicParsers.Operator(OperatorType.Assignment)
                ),
                Expression
            );


        public static Parser<IToken, IExpression> Assignable =>
            Expression.Where(expr =>
                expr is IdentifierExpression
                || expr is IndexAccessExpression
                || expr is FieldAccessExpression
            );

        ///// ===================
        ///// || Function Call ||
        ///// ===================
        public static Parser<IToken, FunctionCall> FunctionCall => 
            Expression.OfType<FunctionCall>();

        public static IStatement[] ParseStatements(IEnumerable<IToken> tokens)
        {
            return Statement
                .Many()
                .ParseOrThrow(tokens).ToArray();
        }
    }
}