
using static Doxlua.Doxcode.Bytecode;
using static Doxlua.Doxcode.BytecodeOp;
using static Doxlua.VM.GlobalCodes;

namespace Doxlua.Lexer
{
    public interface IStatement {
        public byte[][] Codify(Lex lex);
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

        public byte[][] Codify(Lex lex)
        {
            return [
                ..Rhs.Codify(lex),
                ..Lhs.Codify(lex),
            ];
        }

        public override string ToString() => $"Assign:(LHS={Lhs}, RHS={Rhs})";
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

        public byte[][] Codify(Lex lex)
        {
            // Reverse the Inner list
            // Then add their Codify to the list
            // Then add the Func Codify to the list
            
            return [
                ..Inner.SelectMany(x => x.Codify(lex)).ToArray(),
                ..Func.Codify(lex),
                // All functions return something (or nil)
                Execute(Call, (byte)Inner.Length, (byte)Inner.Length, 1)
            ];
        }

        public override string ToString() => $"Call:(Func={Func}, Args=[{string.Join<IExpression>(", ", Inner)}])";

        byte[][] IExpression.Codify(Lex lex)
        {
            return Codify(lex);
        }
    }

}