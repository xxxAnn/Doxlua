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
                var index = Doxcode.Bytecode.GetArg(code, 0);
                state.Push(state.Globals[index]);
            }

            public static void LoadConst(DoxState state, byte[] code) 
            {
                // LoadConst FROMINDEX X X
                // Insert Constant at FROMINDEX index onto stack
                var index = Doxcode.Bytecode.GetArg(code, 0);
                state.Push(new DoxString(state.Consts[index]));
            }

            public static void Call(DoxState state, byte[] code) 
            {
                // Call ARGCOUNT RETURNCOUNT LUACSHARP
                // Call function at with ARGCOUNT arguments from the stack 
                var argCount = Doxcode.Bytecode.GetArg(code, 0);
                var returnCount = Doxcode.Bytecode.GetArg(code, 1);
                // If the function is a C# function, we can call it directly
                // If the function is a Lua function, we have to recursively call the VM
                // In that case we can move the Env down
                var luacsharp = Doxcode.Bytecode.GetArg(code, 2);
                var func = state.Pop();
                var length = state.GetStackLength();
                if (func is DoxFunction function) 
                {
                    function.GetValue()(state);
                    // This should pop ARGCOUNT arguments from the stack
                    // and push the return value (if any) onto the stack
                    // So the length should be length - argCount + returnCount
                    // Check if the length is correct
                    if (state.GetStackLength() != length - argCount + returnCount) 
                    {
                        throw new Exception("Stack length mismatch");
                    }

                }
                else 
                {
                    throw new Exception("Expected a function");
                }
            }

            public static void LoadEnv(DoxState state, byte[] code)
            {
                // LoadEnv X X X
                // Load the environment table onto the stack
                // This is a special case, we need to load the environment table
                // from the stack and push it onto the stack
                var env = state.PeekEnv();
                state.Push(env);
            }
        }

        public static void Write(DoxState state, byte[] code) 
        {
            Modder.Get(code)(state, code);
        }

        public static class Modder
        {

            public static Action<DoxState, byte[]> Get(byte[] code) =>
                Doxcode.Bytecode.GetOp(code) switch {
                    Doxcode.BytecodeOp.OpenBlock => OpenBlock,
                    Doxcode.BytecodeOp.CloseBlock => CloseBlock,
                    Doxcode.BytecodeOp.Pair => Pair,
                    Doxcode.BytecodeOp.Element => Element,
                    _ => Null
                };

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