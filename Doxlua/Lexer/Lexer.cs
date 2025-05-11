using Doxlua.Doxcode;
using Doxlua.Tokenizer;
using Doxlua.VM;
using Microsoft.VisualBasic;
using Roll = System.Collections.Generic.IEnumerable<Doxlua.Tokenizer.IToken>;

namespace Doxlua.Lexer
{
    public class Rootlex
    {
        List<IDoxValue> consts;
        Lex root;

        public Rootlex(Roll tokens)
        {
            consts = new List<IDoxValue>();
            root = new Lex(tokens, this);
        }

        // Add a const to the list
        // Returns the index of the const
        public int AddConst(IDoxValue value)
        {
            consts.Add(value);
            return consts.Count - 1;
        }

        public DoxCode GetCode()
        {
            return root.CodifyOrGetCode();
        }

        public IDoxValue[] GetConsts()
        {
            return consts.ToArray();
        }

    }
    public class Lex 
    {
        Stack<IStatement> Statements;
        DoxCode? code = null;
        Rootlex Root;
        // This is a state machine
        // It either in EXECUTE {0}, WRITE (e) {1}, WRITE (t) {2} or WRITE (v) {3} mode
        // Statements will be parsed differently in either mode
        int context = 0;
        static readonly List<PunctuationType> banned = [PunctuationType.Comment, PunctuationType.MultilineComment, PunctuationType.EOL];


// Root construct
        public Lex(Roll tokens, Rootlex root)
        {
            Statements = new(StatementParser.ParseStatements(
                tokens.Where(x =>
                    x is not Punctuation p || !banned.Contains(p.GetValue())
            )
            
            ).Reverse());
            Root = root;
        }

        public DoxCode CodifyOrGetCode()
        {
            if (code == null)
                Codify();
            return code;
        }

// Sublex construct (this will happen when this is a function call)
        public Lex(Stack<IStatement> statements, Rootlex root)
        {
            Statements = statements;
            Root = root;
        }
        // When a statement contains a const
        // specifically:
        // Static primitive (string, number, boolean, int, nil)
        // Function definition or anonymous function assignment
        // it is added to the consts list

        public int AddConst(IDoxValue value)
        {
            return Root.AddConst(value);
        }

        public void Codify() 
        {
            List<byte[]> _code = [];


            while (Statements.Count > 0)
            {
                // Extend code
                var newCode = Statements.Pop().Codify(this);
                _code.AddRange(newCode);
            }
            code = new DoxCode([.. _code]);
            // extra logic for control flow (if control flow open get a new lex and try to codify it)
            // if THIS IS STATEMENT_CONTROL_FLOW_START or ASSIGN(, FUNCTION_DEFINE_EXPRESSION)
            // then we need to create a new lex and codify it
            // if THIS IS STATEMENT_CONTROL_FLOW_END then we return
        }

        public DoxCode GetCode()
        {
            if (code == null)
                throw new Exception("Attempt to get code from Lex without code");
            return code;
        }

        public DoxFunction AsFunction()
        {
            if (code == null)
                throw new Exception("Attempt to turn Lex into function without code");
            return new DoxFunction(code);
        }

        IStatement[] GetStatements() => Statements.ToArray();

        //DoxFunction AsFunction()
        //{
        //    // TODO
        //    return null;
        //}
    }
}