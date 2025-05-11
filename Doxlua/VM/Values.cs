using System.Diagnostics.CodeAnalysis;
using Doxlua.Tokenizer;

namespace Doxlua.VM
{
    public interface IDoxValue
    {
        // Get the type of the value
        DoxValueType GetDoxType();

    }

    // A Cell pointing to a value
    public class DoxCell : IDoxValue
    {
        public IDoxValue Value { get; set; }

        public DoxCell(IDoxValue value)
        {
            if (value is DoxCell cell)
               throw new ArgumentException("DoxCell cannot point to another DoxCell");
            Value = value;
        }

        public DoxValueType GetDoxType()
        {
            return Value.GetDoxType();
        }

        public void SetValue(IDoxValue value)
        {
            if (value is DoxCell cell)
                Value = cell.Value;
            else
                Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is DoxCell cell)
            {
                return Value.Equals(cell.Value);
            }
            return false;
        }

        public DoxCell DeepCopy()
        {
            if (Value is DoxTable table)
            {
                return new DoxCell(table.DeepCopy());
            }
            else
            {
                return new DoxCell(Value);
            }
        }

        // Optional: Transparent casting (e.g., DoxCell → DoxTable)
        public static explicit operator DoxTable(DoxCell cell)
        {
            return (DoxTable)cell.Value;
        }

        public static explicit operator DoxFunction(DoxCell cell)
        {
            return (DoxFunction)cell.Value;
        }

        public static explicit operator DoxString(DoxCell cell)
        {
            return (DoxString)cell.Value;
        }

        public static explicit operator DoxNumber(DoxCell cell)
        {
            return (DoxNumber)cell.Value;
        }

        public static explicit operator DoxBoolean(DoxCell cell)
        {
            return (DoxBoolean)cell.Value;
        }

        public static explicit operator DoxNil(DoxCell cell)
        {
            return (DoxNil)cell.Value;
        }

    }

    public static class IDoxValueFromLiteral
    {
        public static IDoxValue FromLiteral(Literal literal)
        {
            return literal.GetValue() switch
            {
                LiteralType.String => new DoxString(literal.GetInnerString()),
                LiteralType.Number => new DoxNumber(literal.GetInnerNumber()),
                LiteralType.Boolean => new DoxBoolean(literal.GetInnerBoolean()),
                LiteralType.Nil => new DoxNil(),
                _ => throw new ArgumentException("Invalid literal type")
            };
        }
    }

    public interface IDoxPrimitive
    {}

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

    public struct DoxFunction : IDoxValue
    {

        public delegate int DoxFunctionDelegate(DoxState state);
        public Doxcode.DoxCode? _code = null;

        private DoxFunctionDelegate? _function = null;

        public DoxFunction(DoxFunctionDelegate function)
        {
            _function = function;
        }

        public DoxFunction(Doxcode.DoxCode code)
        {
            _function = null;
            _code = code;
        }

        public readonly DoxValueType GetDoxType()
        {
            return DoxValueType.Function;
        }

        public DoxFunctionDelegate GetFunction()
        {
            return _function;
        }

        public Doxcode.DoxCode GetCode()
        {
            return _code;
        }

        public override string ToString()
        {
            return _function != null ? "DoxFunction(Function)" : "DoxFunction(Code)";
        }
    }

    public struct DoxNil : IDoxValue, IDoxPrimitive
    {
        public DoxValueType GetDoxType()
        {
            return DoxValueType.Nil;
        }

        public override string ToString()
        {
            return "DoxNil(nil)";
        }
    }

    public struct DoxBoolean : IDoxValue, IDoxPrimitive
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

        public override string ToString()
        {
            return _value ? "DoxBoolean(true)" : "DoxBoolean(false)";
        }
    }

    public struct DoxNumber : IDoxValue, IDoxPrimitive
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

        public override string ToString()
        {
            return $"DoxNumber({_value})";
        }
    }

    public struct DoxString : IDoxValue, IDoxPrimitive
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

        public override string ToString()
        {
            return $"DoxString({_value})";
        }
    }
    public class DoxTable : IDoxValue
    {

        private readonly Dictionary<DoxCell, DoxCell> _table;

        public DoxTable()
        {
            _table = [];
        }
        public void Set(DoxCell key, DoxCell value)
        {
            _table[key] = value;
        }

        public DoxCell Get(DoxCell key)
        {
            if (_table.TryGetValue(key, out var value))
            {
                return value;
            }
            return new DoxCell(new DoxNil());
        }

        public Dictionary<DoxCell, DoxCell> GetValue()
        {
            return _table;
        }

        public DoxValueType GetDoxType()
        {
            return DoxValueType.Table;
        }

        public DoxTable DeepCopy()
        {
            var newTable = new DoxTable();
            foreach (var kvp in _table)
            {
                var newKey = kvp.Key.DeepCopy();
                var newValue = kvp.Value.DeepCopy();
                newTable.Set(newKey, newValue);
            }
            return newTable;
        }

        public override string ToString()
        {
            return "DoxTable(" + string.Join(", ", _table.Select(kvp => $"{kvp.Key}={kvp.Value}")) + ")";
        }
    }
}