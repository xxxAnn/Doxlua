using Doxlua.Doxcode;
using Doxlua.Tokenizer;
using Doxlua.VM;

using Roll = System.Collections.Generic.IEnumerable<Doxlua.Tokenizer.IToken>;

namespace Doxlua.Lexer
{

    public class Lex 
    {
        // When a statement contains a const
        // specifically:
        // Static primitive (string, number, boolean, int, nil)
        // Function definition or anonymous function assignment
        // it is added to the consts list
        List<IDoxValue> consts;
        List<Statement> statements;
        // This is a state machine
        // It either in EXECUTE or WRITE mode
        // Statements will be parsed differently in either mode
        bool IsExecute { get; set; } = false;

        public Lex(Roll tokens)
        {
            consts = [];
            statements = [];
        }

        void ParseStatements(Roll tokens)
        {
            
        }

        Statement[] GetStatements() => statements.ToArray();

        DoxFunction AsFunction()
        {
            return new DoxFunction(
                Inscriber.Inscribe(GetStatements())
            );
        }
    }

    public struct Statement
    {}
}