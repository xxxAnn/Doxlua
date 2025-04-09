namespace Doxlua.VM
{
    public interface IDoxValue
    {
        // Get the type of the value
        DoxValueType GetDoxType();

    }

    public enum DoxValueType : byte 
    {
        Nil = 0,
        Boolean = 1,
        Number = 2,
        String = 3,
        Function = 4,
        Table = 5,
        // UserData = 6,
        // Thread = 7,
    }

    public class DoxFunction : IDoxValue
    {

        public delegate int DoxFunctionDelegate(DoxState state);

        private DoxFunctionDelegate _function;

        public DoxFunction(DoxFunctionDelegate function)
        {
            _function = function;
        }

        public DoxValueType GetDoxType()
        {
            return DoxValueType.Function;
        }

        public DoxFunctionDelegate GetValue()
        {
            return _function;
        }
    }

    public class DoxNil : IDoxValue
    {
        public DoxValueType GetDoxType()
        {
            return DoxValueType.Nil;
        }
    }

    public class DoxBoolean : IDoxValue
    {
        private bool _value;

        public DoxBoolean(bool value)
        {
            _value = value;
        }

        public DoxValueType GetDoxType()
        {
            return DoxValueType.Boolean;
        }

        public bool GetValue()
        {
            return _value;
        }
    }

    public class DoxNumber : IDoxValue
    {
        private double _value;

        public DoxNumber(double value)
        {
            _value = value;
        }

        public DoxValueType GetDoxType()
        {
            return DoxValueType.Number;
        }

        public double GetValue()
        {
            return _value;
        }
    }

    public class DoxString : IDoxValue
    {
        private string _value;

        public DoxString(string value)
        {
            _value = value;
        }

        public DoxValueType GetDoxType()
        {
            return DoxValueType.String;
        }

        public string GetValue()
        {
            return _value;
        }
    }

    public class DoxTable : IDoxValue
    {
        private Dictionary<string, IDoxValue> _table;

        public DoxTable()
        {
            _table = new Dictionary<string, IDoxValue>();
        }

        public void Set(string key, IDoxValue value)
        {
            _table[key] = value;
        }

        public IDoxValue Get(string key)
        {
            return _table[key];
        }

        public Dictionary<string, IDoxValue> GetValue()
        {
            return _table;
        }

        public DoxValueType GetDoxType()
        {
            return DoxValueType.Table;
        }
    }
}