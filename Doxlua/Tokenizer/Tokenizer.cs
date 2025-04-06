using System;
using System.Collections.Generic;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Doxlua.Tokenizer;

namespace Doxlua.Tokenizer
{
    public static class LuaTokenizer
    {
        private class TokenWithLine
        {
            public IToken Token { get; }
            public int Line { get; }

            public TokenWithLine(IToken token, int line)
            {
                Token = token;
                Line = line;
            }
        }

        private static readonly Parser<char, string> IdentifierParser =
            from first in Letter.Or(Char('_'))
            from rest in LetterOrDigit.Or(Char('_')).Many()
            select first + new string(rest.ToArray());

        private static Parser<char, TokenWithLine> IdentifierToken =>
            CurrentLine.Then(IdentifierParser, (line, name) => new TokenWithLine(new Identifier(name, line), line));

        private static Parser<char, KeywordType> KeywordParser =>
            Parser.Enum<KeywordType>();

        private static Parser<char, TokenWithLine> KeywordToken =>
            CurrentLine.Then(KeywordParser, (line, kw) => new TokenWithLine(new Keyword(kw, line), line));

        private static Parser<char, IEnumerable<char>> Integer =>
            Digit.AtLeastOnce();

        private static Parser<char, IEnumerable<char>> Real =>
            from whole in Digit.AtLeastOnce()
            from dot in Char('.')
            from fractional in Digit.AtLeastOnce()
            select whole.Concat(new[] { dot }).Concat(fractional);

        private static Parser<char, IEnumerable<char>> Number =>
            Try(Real).Or(Integer);

        private static readonly Parser<char, string> RealNum =
            Number.Select(n => new string(n.ToArray())).Labelled("number");

        private static Parser<char, TokenWithLine> NumberLiteral =>
            CurrentLine.Then(RealNum, (line, n) => new TokenWithLine(new Literal(double.Parse(n), line), line));

        private static Parser<char, string> QuotedString =>
            Token('"')
                .Then(AnyCharExcept('"').ManyString(), (open, content) => content)
                .Before(Token('"'))
                .Labelled("quoted string");

        private static Parser<char, char> AnyCharExcept(char except) =>
            Parser.AnyCharExcept(except).Labelled($"any character except '{except}'");

        private static Parser<char, TokenWithLine> StringLiteral =>
            CurrentLine.Then(QuotedString, (line, s) => new TokenWithLine(new Literal(s, line), line));

        private static Parser<char, TokenWithLine> BooleanLiteral =>
            CurrentLine.Then(
                String("true").ThenReturn(true).Or(String("false").ThenReturn(false)),
                (line, b) => new TokenWithLine(new Literal(b, line), line)
            );

        private static Parser<char, TokenWithLine> NilLiteral =>
            CurrentLine.Then(
                String("nil"),
                (line, _) => new TokenWithLine(new Literal(0, line), line)
            );

        private static Parser<char, TokenWithLine> LiteralToken =>
            OneOf(NumberLiteral, StringLiteral, BooleanLiteral, NilLiteral);

        private static Parser<char, OperatorType> OperatorParser =>
            String("+").ThenReturn(OperatorType.Plus)
                .Or(String("-").ThenReturn(OperatorType.Minus))
                .Or(String("*").ThenReturn(OperatorType.Multiply))
                .Or(String("/").ThenReturn(OperatorType.Divide))
                .Or(String("%").ThenReturn(OperatorType.Modulus))
                .Or(String("^").ThenReturn(OperatorType.Power))
                .Or(String("..").ThenReturn(OperatorType.Concatenate))
                .Or(String("==").ThenReturn(OperatorType.Equal))
                .Or(String("~=").ThenReturn(OperatorType.NotEqual))
                .Or(String("<=").ThenReturn(OperatorType.LessThanOrEqual))
                .Or(String(">=").ThenReturn(OperatorType.GreaterThanOrEqual))
                .Or(String("<").ThenReturn(OperatorType.LessThan))
                .Or(String(">").ThenReturn(OperatorType.GreaterThan));

        private static Parser<char, TokenWithLine> OperatorToken =>
            CurrentLine.Then(OperatorParser, (line, op) => new TokenWithLine(new Operator(op, line), line));

        private static Parser<char, PunctuationType> PunctuationParser =>
            Char('(').ThenReturn(PunctuationType.ParOpen)
                .Or(Char(')').ThenReturn(PunctuationType.ParClose))
                .Or(Char('{').ThenReturn(PunctuationType.BraceOpen))
                .Or(Char('}').ThenReturn(PunctuationType.BraceClose))
                .Or(Char('[').ThenReturn(PunctuationType.BracketOpen))
                .Or(Char(']').ThenReturn(PunctuationType.BracketClose))
                .Or(Char(',').ThenReturn(PunctuationType.Comma))
                .Or(Char('.').ThenReturn(PunctuationType.Dot))
                .Or(Char(':').ThenReturn(PunctuationType.Colon));

        private static Parser<char, TokenWithLine> PunctuationToken =>
            CurrentLine.Then(PunctuationParser, (line, p) => new TokenWithLine(new Punctuation(p, line), line));

        private static Parser<char, TokenWithLine> CommentToken =>
            Try(String("--").Then(AnyCharExcept('\n').ManyString()))
                .Then(CurrentLine, (txt, line) => new TokenWithLine(new Punctuation(PunctuationType.Comment, line), line));

        private static Parser<char, TokenWithLine> EOLToken =>
            Char('\n').Then(CurrentLine, (_, line) => new TokenWithLine(new Punctuation(PunctuationType.EOL, line), line));

        private static Parser<char, TokenWithLine> TokenParser =>
            OneOf(
                CommentToken,
                KeywordToken,
                LiteralToken,
                IdentifierToken,
                OperatorToken,
                PunctuationToken,
                EOLToken
            ).Before(SkipWhitespaces);

        private static Parser<char, IEnumerable<IToken>> TokensParser =>
            TokenParser.Many().Select(toks => toks.Select(t => t.Token));

        public static IEnumerable<IToken> Tokenize(string input) =>
            TokensParser.ParseOrThrow(input);

        private static Parser<char, int> CurrentLine =>
            Parser<char>.CurrentPos
                .Select(pos => pos.Line)
                .Labelled("current line");

    }
}
