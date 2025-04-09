namespace Doxlua.VM
{
    public class DoxState
    {
        // Globals
        public Dictionary<string, IDoxValue> Globals { get; set; }
        // Stack
        private Stack<IDoxValue> _stack { get; set; }

        // Stack operations
        public void Push(IDoxValue value)
        {
            _stack.Push(value);
        }
        public IDoxValue[] Pop(int n)
        {
            // Pop n values from the stack
            IDoxValue[] values = new IDoxValue[n];
            for (int i = 0; i < n; i++)
            {
                values[i] = _stack.Pop();
            }
            return values;
        }

        public IDoxValue Pop()
        {
            // Pop one value from the stack
            return _stack.Pop();
        }

        public DoxState()
        {
            Globals = new Dictionary<string, IDoxValue>();
            _stack = new Stack<IDoxValue>();

            InitializeDefaultGlobals();
        }

        private void InitializeDefaultGlobals()
        {
            // Initialize default globals
            Globals["nil"] = new DoxNil();
            Globals["true"] = new DoxBoolean(true);
            Globals["false"] = new DoxBoolean(false);
            Globals["print"] = new DoxFunction(FunctionMarket.PrintFunction);
        }
    }

    public static class FunctionMarket
    {
        public static int PrintFunction(DoxState state)
        {
            IDoxValue[] arg = GetArgs(state, 1);

            string str = arg[0].GetDoxType() switch
            {
                DoxValueType.Nil => "nil",
                DoxValueType.Boolean => ((DoxBoolean)arg[0]).GetValue().ToString(),
                DoxValueType.Number => ((DoxNumber)arg[0]).GetValue().ToString(),
                DoxValueType.String => ((DoxString)arg[0]).GetValue(),
                _ => "unknown"
            };
            return 0;
        }

        public static IDoxValue[] GetArgs(DoxState state, int n)
        {
            return state.Pop(n);
        }
    }
}