using System;
using System.Collections.Generic;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Doxlua.Tokenizer;
using Pidgin.Comment;

namespace Doxlua.Tokenizer
{
    public static class LuaTokenizer
    {
        public static IEnumerable<IToken> Tokenize(string input) =>
            TokenParsers.TokensParser.ParseOrThrow(input);
    }

    internal static class CommonParsers
    {
        public static Parser<char, int> CurrentLine =>
            CurrentPos.Select(pos => pos.Line).Labelled("current line");

        public static Parser<char, char> AnyCharExcept(char except) =>
            Parser.AnyCharExcept(except).Labelled($"any character except '{except}'");
    }

    internal static class IdentifierParsers
    {
        private static readonly Parser<char, string> IdentifierParser =
            from first in Letter.Or(Char('_'))
            from rest in LetterOrDigit.Or(Char('_')).Many()
            select first + new string(rest.ToArray());

        public static Parser<char, TokenWithLine> IdentifierToken =>
            CommonParsers.CurrentLine.Then(IdentifierParser, (line, name) => new TokenWithLine(new Identifier(name, line), line));
    }

    internal static class KeywordParsers
    {
        private static readonly Parser<char, KeywordType> KeywordParser =
            Try(Parser.Enum<KeywordType>());

        public static Parser<char, TokenWithLine> KeywordToken =>
            CommonParsers.CurrentLine.Then(KeywordParser, (line, kw) => new TokenWithLine(new Keyword(kw, line), line));
    }

    internal static class LiteralParsers
    {
        private static readonly Parser<char, IEnumerable<char>> Integer = Digit.AtLeastOnce();
        private static readonly Parser<char, IEnumerable<char>> Real =
            from whole in Digit.AtLeastOnce()
            from dot in Char('.')
            from fractional in Digit.AtLeastOnce()
            select whole.Concat([dot]).Concat(fractional);

        private static readonly Parser<char, string> RealNum =
            Try(Real).Or(Integer).Select(n => new string(n.ToArray())).Labelled("number");

        public static Parser<char, TokenWithLine> NumberLiteral =>
            CommonParsers.CurrentLine.Then(RealNum, (line, n) => new TokenWithLine(new Literal(double.Parse(n), line), line));

        private static readonly Parser<char, string> QuotedString =
            Token('"')
                .Then(CommonParsers.AnyCharExcept('"').ManyString(), (open, content) => content)
                .Before(Token('"'))
                .Labelled("quoted string");

        public static Parser<char, TokenWithLine> StringLiteral =>
            Try(CommonParsers.CurrentLine.Then(QuotedString, (line, s) => new TokenWithLine(new Literal(s, line), line)));

        private static readonly Parser<char, bool> BooleanParser =
            Try(String("true").ThenReturn(true)).Or(Try(String("false").ThenReturn(false)));

        public static Parser<char, TokenWithLine> BooleanLiteral =>
            Try(CommonParsers.CurrentLine.Then(BooleanParser, (line, b) => new TokenWithLine(new Literal(b, line), line)));

        public static Parser<char, TokenWithLine> NilLiteral =>
            Try(CommonParsers.CurrentLine.Then(String("nil"), (line, _) => new TokenWithLine(new Literal(0, line), line)));

        public static Parser<char, TokenWithLine> LiteralToken =>
            OneOf(NumberLiteral, StringLiteral, BooleanLiteral, NilLiteral);
    }

    internal static class OperatorParsers
    {
        private static readonly Parser<char, OperatorType> OperatorParser =
            Try(String("+").ThenReturn(OperatorType.Plus)
                .Or(String("-").ThenReturn(OperatorType.Minus))
                .Or(String("*").ThenReturn(OperatorType.Multiply))
                .Or(String("/").ThenReturn(OperatorType.Divide))
                .Or(String("%").ThenReturn(OperatorType.Modulus))
                .Or(String("^").ThenReturn(OperatorType.Power))
                .Or(String("..").ThenReturn(OperatorType.Concatenate))
                .Or(String("=").ThenReturn(OperatorType.Assignment))
                .Or(String("==").ThenReturn(OperatorType.Equal))
                .Or(String("~=").ThenReturn(OperatorType.NotEqual))
                .Or(String("<=").ThenReturn(OperatorType.LessThanOrEqual))
                .Or(String(">=").ThenReturn(OperatorType.GreaterThanOrEqual))
                .Or(String("<").ThenReturn(OperatorType.LessThan))
                .Or(String(">").ThenReturn(OperatorType.GreaterThan)));

        public static Parser<char, TokenWithLine> OperatorToken =>
            CommonParsers.CurrentLine.Then(OperatorParser, (line, op) => new TokenWithLine(new Operator(op, line), line));
    }

    internal static class PunctuationParsers
    {
        private static readonly Parser<char, PunctuationType> PunctuationParser =
            Try(Char('(').ThenReturn(PunctuationType.ParOpen)
                .Or(Char(')').ThenReturn(PunctuationType.ParClose))
                .Or(Char('{').ThenReturn(PunctuationType.BraceOpen))
                .Or(Char('}').ThenReturn(PunctuationType.BraceClose))
                .Or(Char('[').ThenReturn(PunctuationType.BracketOpen))
                .Or(Char(']').ThenReturn(PunctuationType.BracketClose))
                .Or(Char(',').ThenReturn(PunctuationType.Comma))
                .Or(Char('.').ThenReturn(PunctuationType.Dot))
                .Or(Char(':').ThenReturn(PunctuationType.Colon)));

        public static Parser<char, TokenWithLine> PunctuationToken =>
            CommonParsers.CurrentLine.Then(PunctuationParser, (line, p) => new TokenWithLine(new Punctuation(p, line), line));
    }

    internal static class CommentParsers
    {   
        private static readonly Parser<char, PunctuationType> MultiLineComment =
            CommentParser.SkipBlockComment(String("--[["), Try(String("]]")))
                .ThenReturn(PunctuationType.MultilineComment);

        public static Parser<char, TokenWithLine> MultiLineCommentToken =>
            Try(MultiLineComment.Then(CommonParsers.CurrentLine, (type, line) => new TokenWithLine(new Punctuation(type, line), line)));

        public static Parser<char, TokenWithLine> CommentToken =>
            Try(String("--").Then(CommonParsers.AnyCharExcept('\n').ManyString()))
                .Then(CommonParsers.CurrentLine, (txt, line) => new TokenWithLine(new Punctuation(PunctuationType.Comment, line), line))
            .Or(
                Try(String("---[[").Then(CommonParsers.AnyCharExcept('\n').ManyString()))
                    .Then(CommonParsers.CurrentLine, (txt, line) => new TokenWithLine(new Punctuation(PunctuationType.Comment, line), line))
            );
    }

    internal static class EOLParsers
    {
        public static Parser<char, TokenWithLine> EOLToken =>
            Char('\n').Then(CommonParsers.CurrentLine, (_, line) => new TokenWithLine(new Punctuation(PunctuationType.EOL, line), line));
    }

    internal static class TokenParsers
    {
        public static readonly Parser<char, TokenWithLine> TokenParser =
            OneOf(
                CommentParsers.MultiLineCommentToken,
                CommentParsers.CommentToken,
                KeywordParsers.KeywordToken,
                LiteralParsers.LiteralToken,
                IdentifierParsers.IdentifierToken,
                OperatorParsers.OperatorToken,
                PunctuationParsers.PunctuationToken,
                EOLParsers.EOLToken
            ).Before(SkipWhitespaces);

        public static readonly Parser<char, IEnumerable<IToken>> TokensParser =
            TokenParser.Many().Select(toks => toks.Select(t => t.Token));
    }

    internal class TokenWithLine(IToken token, int line)
    {
        public IToken Token { get; } = token;
        public int Line { get; } = line;
    }
}