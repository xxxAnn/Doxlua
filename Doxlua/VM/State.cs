using System.Text;

namespace Doxlua.VM
{

    public static class GlobalCodes 
    {
        public static readonly byte Nil = 0;
        public static readonly byte True = 1;
        public static readonly byte False = 2;
        public static readonly byte TableAccess = 3;
        public static readonly byte SetTable = 4;

        public static readonly byte SetValue = 5;
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
        private Stack<DoxCell> Stack { get; set; }
        private Stack<DoxTable> Envs { get; set; }

        public DoxCell GetGlobal(int index)
        {
            return new DoxCell(Globals[index]);
        }

        public int GetStackLength()
        {
            return Stack.Count;
        }

        // Stack operations
        public void Push(DoxCell value)
        {
            if (Stack.Count >= MAXstack_SIZE)
                // Stack overflow
                throw new Exception("Stack overflow");
            Stack.Push(value);
        }
        public DoxCell[] Pop(int n)
        {
            // Pop n values from the stack
            DoxCell[] values = new DoxCell[n];
            for (int i = 0; i < n; i++)
            {
                values[i] = Stack.Pop();
            }
            return values;
        }

        public DoxCell Pop()
        {
            // Pop one value from the stack
            return Stack.Pop();
        }

        public DoxState(string[] consts)
        {
            Consts = consts.Select(c => new DoxString(c) as IDoxValue).ToArray();
            Functions = [];
            Stack = new Stack<DoxCell>();
            Envs = new Stack<DoxTable>();
            Envs.Push(DoxTableFactory.CreateTable());
            Globals = new IDoxValue[256];
            InitializeDefaultGlobals();
        }

        public DoxCell GetConst(int index)
        {
            // Get the constant at index
            if (index < 0 || index >= Consts.Length)
                throw new ArgumentOutOfRangeException(nameof(index), $"index must be between 0 and {Consts.Length - 1}");
            return new DoxCell(Consts[index]);
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
            Stack = new Stack<DoxCell>();
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
                Envs.Peek().DeepCopy() as DoxTable ?? throw new Exception("Failed to copy environment")
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
            Globals[GlobalCodes.SetTable] = new DoxFunction(FunctionMarket.SetTable);
            Globals[GlobalCodes.SetValue] = new DoxFunction(FunctionMarket.SetValue);
        }

        public DoxCell[] GetArgs(int n)
        {
            return Pop(n);
        }

        public void Return(DoxCell value)
        {
            Push(value);
        }

        public void Return() 
        {
            Push(new DoxCell(new DoxNil()));
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
            table.Set(new DoxCell(new DoxString("print")), new DoxCell(new DoxFunction(FunctionMarket.PrintFunction)));

            return table;
        }
    }
}