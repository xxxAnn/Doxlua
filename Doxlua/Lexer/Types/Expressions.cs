using Doxlua.VM;
using static Doxlua.Doxcode.Bytecode;
using static Doxlua.Doxcode.BytecodeOp;
using static Doxlua.VM.GlobalCodes;

namespace Doxlua.Lexer
{
    public interface IExpression { 

        /// <summary>
        /// An Expression Resolves to an IDoxValue
        /// It will add the resolved value to the stack
        /// with nothing else changed
        /// </summary>
        public byte[][] Codify(Lex lex);
    }

    public class LiteralExpression : IExpression
    {
        IDoxValue inner;

        public LiteralExpression(IDoxValue inp)
        {
            inner = inp;
        }

        public byte[][] Codify(Lex lex)
        {
            return [
                Doxcode.Bytecode.Execute(
                    Doxcode.BytecodeOp.LoadConst,
                    (byte)lex.AddConst(inner)
                )
            ];
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

        public byte[][] Codify(Lex lex)
        {
            // Table Access
            return [
                // Name
                Execute(LoadConst, (byte)lex.AddConst(new DoxString(inner))),
                Execute(LoadEnv),
                Execute(GetGlobal, TableAccess),

                // We now have the ENV table on the stack (it contains global and local variables)
                // Stack should be [TableAccess, ENV, NAME]
                Execute(Call, 2, 1, 0)
                // We now have the value of the identifier on the stack
            ];
        }

        public override string ToString() => $"IdentifierExpression({inner})";
    }

    public class FieldAccessExpression(IExpression lhs, string rhs) : IExpression
    {
        IExpression Lhs = lhs;
        string Rhs = rhs;

        public override string ToString() => $"Access:(LHS={Lhs}, RHS={Rhs})";

        public byte[][] Codify(Lex lex)
        {
            // Extend lists
            return [
                Execute(LoadConst, (byte)lex.AddConst(new DoxString(Rhs))),
                ..Lhs.Codify(lex),
                Execute(GetGlobal, TableAccess),
                // Stack should be [TableAccess, TABLE, NAME]
                Execute(Call, 2, 1, 0)
            ];
        }
    }

    public class IndexAccessExpression(IExpression lhs, IExpression rhs) : IExpression
    {
        IExpression Lhs = lhs;
        IExpression Rhs = rhs;

        public override string ToString() => $"Access:(LHS={Lhs}, RHS={Rhs})";

        public byte[][] Codify(Lex lex)
        {
            // Extend lists
            return [
                ..Rhs.Codify(lex),
                ..Lhs.Codify(lex),
                Execute(GetGlobal, TableAccess),
                // Stack should be [TableAccess, TABLE, NAME]
                Execute(Call, 2, 1, 0)
            ];
        }
    }

    public static class ExpressionFactory
    {
        public static IExpression CreateLiteral(IDoxValue lit) => new LiteralExpression(lit);
        public static IExpression CreateIdentifier(string ind) => new IdentifierExpression(ind);

        public static IExpression CreateFunctionCall(IExpression ind, IExpression[] inner) => new FunctionCall(ind, inner);
    }


}