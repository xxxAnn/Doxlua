using Doxlua.VM;

namespace Doxlua.Lexer

{
    public interface IStatement {
        public byte[] Codify(Lex lex);
    }

    public class Assignment : IStatement
    {
        IExpression Lhs;
        IExpression Rhs;

        public Assignment(IExpression lhs, IExpression rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public byte[] Codify(Lex lex)
        {
            return [];
        }

        public override string ToString() => $"Assign ( {Lhs} = {Rhs})";
    }

    public class FunctionCall : IStatement, IExpression
    {
        IExpression Func;
        IExpression[] Inner;

        public FunctionCall(IExpression func, IExpression[] inner)
        {
            Func = func;
            Inner = inner;
        }

        public byte[] Codify(Lex lex)
        {
            return [];
        }


        public override string ToString() => $"Call ( {Func}({string.Join<IExpression>(", ", Inner)}) )";
    }

    public interface IExpression { }

    public class LiteralExpression : IExpression
    {
        IDoxValue inner;

        public LiteralExpression(IDoxValue inp)
        {
            inner = inp;
        }

        public override string ToString() => $"LiteralExpression({inner})";
    }

    public class IdentifierExpression : IExpression
    {
        string inner;

        public IdentifierExpression(string inp)
        {
            inner = inp;
        }

        public override string ToString() => $"IdentifierExpression({inner})";
    }

    public class FieldAccessExpression(IExpression lhs, string rhs) : IExpression
    {
        IExpression Lhs = lhs;
        string Rhs = rhs;

        public override string ToString() => $"Access ( {Lhs}.{Rhs} )";
    }

    public class IndexAccessExpression(IExpression lhs, IExpression rhs) : IExpression
    {
        IExpression Lhs = lhs;
        IExpression Rhs = rhs;

        public override string ToString() => $"Access ( {Lhs}[{Rhs}] )";
    }

    public static class ExpressionFactory
    {
        public static IExpression CreateLiteral(IDoxValue lit) => new LiteralExpression(lit);
        public static IExpression CreateIdentifier(string ind) => new IdentifierExpression(ind);

        public static IExpression CreateFunctionCall(IExpression ind, IExpression[] inner) => new FunctionCall(ind, inner);
    }


}