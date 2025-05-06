using Doxlua.Doxcode;

namespace Doxlua.VM
{
    public static class DoxMachine
    {
        public static void Run(DoxState state, Doxcode.DoxCode doxcode)
        {
            foreach (byte[] code in doxcode) {
                // Code
                if (Doxcode.Bytecode.IsExecute(code)) 
                {
                    Execute(state, code);
                }
                else
                {
                    // Load
                    Write(state, code);
                }
            }
        }

        public static void Execute(DoxState state, byte[] code) {
           Executor.Get(code)(state, code);
        }
        

        public static class Executor 
        {

            public static Action<DoxState, byte[]> Get(byte[] code) =>
                Doxcode.Bytecode.GetOp(code) switch {
                    Doxcode.BytecodeOp.GetGlobal => GetGlobal,
                    Doxcode.BytecodeOp.LoadConst => LoadConst,
                    Doxcode.BytecodeOp.Call      => Call,
                    _ => Null
                };

            public static void Null(DoxState state, byte[] code) 
            {
                // Null X X X
                // Do nothing
            }
            public static void GetGlobal(DoxState state, byte[] code) 
            {
                // GetGlobal FROMINDEX X X
                // Insert Global at FROMINDEX index onto stack
            }

            public static void LoadConst(DoxState state, byte[] code) 
            {
                // LoadConst FROMINDEX X X
                // Insert Constant at FROMINDEX index onto stack
            }

            public static void Call(DoxState state, byte[] code) 
            {
                // Call STACKINDEX ARGCOUNT X
                // Call function at STACKINDEX index with ARGCOUNT arguments from the stack 
            }
        }

        public static void Write(DoxState state, byte[] code) 
        {

        }

        public static class Modder
        {

            public static void WriteLine(DoxState state, string line) 
            {
                var filename = state.GetFile() ?? "null.txt";
                // Open filename in append mode
                // Write line to file
                File.AppendAllLines(filename, new[] { $"{new string(' ', state.Indent * 4)}{line}" });
            }
            public static string ExpectString(IDoxValue val)
            {
                if (val is DoxString str) 
                {
                    return str.GetValue();
                }
                else 
                {
                    throw new Exception("Expected a string");
                }
            }
            public static void Null(DoxState state, byte[] code) 
            {
                // do nothing
            }
            public static void OpenBlock(DoxState state, byte[] code) 
            {
                // Write X X X
                // Writes the string value atop the stack to the File
                // `STACKINDEX = {`

                Modder.WriteLine(
                    state, 
                    Modder.ExpectString(state.Pop()) + " = {"
                );
                state.Indent++;
            }
            public static void CloseBlock(DoxState state, byte[] code) 
            {
                Modder.WriteLine(state, "}");
                state.Indent--;
            }
            public static void Pair(DoxState state, byte[] code) 
            {
                // Write X X X
                // Writes the pair STACKINDEX1, STACKINDEX2
                // `STACKINDEX1 = `STACKINDEX2`
                var vals = state.Pop(2);
                Modder.WriteLine(
                    state, 
                    $"{Modder.ExpectString(vals[0])} = {Modder.ExpectString(vals[1])}"
                );
            }
            public static void Element(DoxState state, byte[] code) 
            {
                // Write X X X
                // Writes the element at STACKINDEX index
                // `STACKINDEX`
                var val = state.Pop();
                Modder.WriteLine(
                    state, 
                    $"{Modder.ExpectString(val)}"
                );
            }
        }
    }
}