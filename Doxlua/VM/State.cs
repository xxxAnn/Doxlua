using System.Text;

namespace Doxlua.VM
{

    public static class GlobalCodes 
    {
        public static readonly byte Nil = 0;
        public static readonly byte True = 1;
        public static readonly byte False = 2;
        public static readonly byte TableAccess = 3;
    }
    public class DoxState
    {
        
        // Globals
        public IDoxValue[] Globals { get; set; }
        public IDoxValue[] Consts { get; set; }
        public DoxFunction[] Functions { get; set; }

        public int Indent { get; set; } = 0;

        public string? File;

        const int MAXstack_SIZE = 2^8;
        // Stack
        private Stack<IDoxValue> Stack { get; set; }
        private Stack<DoxTable> Envs { get; set; }

        public int GetStackLength()
        {
            return Stack.Count;
        }

        // Stack operations
        public void Push(IDoxValue value)
        {
            if (Stack.Count >= MAXstack_SIZE)
                // Stack overflow
                throw new Exception("Stack overflow");
            Stack.Push(value);
        }
        public IDoxValue[] Pop(int n)
        {
            // Pop n values from the stack
            IDoxValue[] values = new IDoxValue[n];
            for (int i = 0; i < n; i++)
            {
                values[i] = Stack.Pop();
            }
            return values;
        }

        public IDoxValue Pop()
        {
            // Pop one value from the stack
            return Stack.Pop();
        }

        public DoxState(string[] consts)
        {
            Consts = consts.Select(c => new DoxString(c) as IDoxValue).ToArray();
            Functions = [];
            Stack = new Stack<IDoxValue>();
            Envs = new Stack<DoxTable>();
            Envs.Push(DoxTableFactory.CreateTable());
            Globals = new IDoxValue[256];
            InitializeDefaultGlobals();
        }

        public IDoxValue GetConst(int index)
        {
            // Get the constant at index
            if (index < 0 || index >= Consts.Length)
                throw new ArgumentOutOfRangeException(nameof(index), $"index must be between 0 and {Consts.Length - 1}");
            return Consts[index];
        }

        public IDoxValue Peek()
        {
            // Peek the top of the stack
            if (Stack.Count == 0)
                throw new Exception("No value to peek");
            return Stack.Peek();
        }

        public DoxState(IDoxValue[] consts)
        {
            Consts = consts;
            Stack = new Stack<IDoxValue>();
            Envs = new Stack<DoxTable>();
            Envs.Push(DoxTableFactory.CreateTable());
            Globals = new IDoxValue[256];
            InitializeDefaultGlobals();
        }

        public void SetFile(string file)
        {
            File = file;
        }
        public string? GetFile()
        {
            return File;
        }
        public void LayerDown() 
        {
            // Copy the top of the Env stack to the top of the stack
            // This is used to create a new environment for the function
            
            if (Envs.Count == 0)
                throw new Exception("No environment to copy");
            Envs.Push(
                Envs.Peek().DeepCopy()
            );
        }

        public string ShowStack()
        {
            // We show the stack as
            // -------------------
            // | DoxValue(value) |
            // -------------------
            // | DoxValue(value) |
            // -------------------

            var sb = new StringBuilder();
            sb.AppendLine("-------------------");
            foreach (var value in Stack)
            {
                sb.AppendLine($"| {value.GetDoxType()}({value}) |");
                sb.AppendLine("-------------------");
            }
            return sb.ToString();
        }

        private void InitializeDefaultGlobals()
        {
            // Initialize default globals
            Globals[GlobalCodes.Nil] = new DoxNil();
            Globals[GlobalCodes.True] = new DoxBoolean(true);
            Globals[GlobalCodes.False] = new DoxBoolean(false);
            // Dynamic Global

            Globals[GlobalCodes.TableAccess] = new DoxFunction(FunctionMarket.TableAccess);
        }

        public IDoxValue[] GetArgs(int n)
        {
            return Pop(n);
        }

        public void Return(IDoxValue value)
        {
            Push(value);
        }

        public DoxTable PeekEnv()
        {
            // Peek the top of the env stack
            if (Envs.Count == 0)
                throw new Exception("No environment to peek");
            return (DoxTable)Envs.Peek();
        }
    }

    public static class DoxTableFactory
    {
        public static DoxTable CreateTable()
        {
            var table = new DoxTable();
            table.Set("print", new DoxFunction(FunctionMarket.PrintFunction));

            return table;
        }
    }

    public static class FunctionMarket
    {

        // 0 = OK
        // 1 = Expected Table for table access
        // 2 = Expected String or Number for table access
        // Expected on top of the stack: [TABLE, KEY, ...]
        // Top of the stack after the function: [VALUE, ...]

        public static int TableAccess(DoxState state)
        {
            IDoxValue[] arg = state.GetArgs(2);

            if (arg[0].GetDoxType() != DoxValueType.Table)
                return 1;

            if (arg[1].GetDoxType() != DoxValueType.String && arg[1].GetDoxType() != DoxValueType.Number)
                return 2;

            var table = (DoxTable)arg[0];
            var key = ((DoxString)arg[1]).GetValue();

            state.Return(table.Get(key));

            return 0;
        }
        public static int SetTable(DoxState state)
        {
            IDoxValue[] arg = state.GetArgs(3);

            if (arg[0].GetDoxType() != DoxValueType.Table)
                return 1;

            if (arg[1].GetDoxType() != DoxValueType.String && arg[1].GetDoxType() != DoxValueType.Number)
                return 2;

            var table = (DoxTable)arg[0];
            var key = ((DoxString)arg[1]).GetValue();

            table.Set(key, arg[2]);

            state.Return(new DoxNil());

            return 0;
        }
        public static int PrintFunction(DoxState state)
        {
            IDoxValue[] arg = state.GetArgs(1);

            string str = arg[0].GetDoxType() switch
            {
                DoxValueType.Nil => "nil",
                DoxValueType.Boolean => ((DoxBoolean)arg[0]).GetValue().ToString(),
                DoxValueType.Number => ((DoxNumber)arg[0]).GetValue().ToString(),
                DoxValueType.String => ((DoxString)arg[0]).GetValue(),
                _ => "unknown"
            };

            if (str == "unknown")
                return 1;

            Console.WriteLine(str);

            state.Return(new DoxNil());

            return 0;
        }


    }
}