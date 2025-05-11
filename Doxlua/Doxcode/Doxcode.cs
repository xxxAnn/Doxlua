using System.Collections;
using System.Collections.Specialized;
using NLog;

namespace Doxlua.Doxcode
{

    // 1 bit
    public static class BytecodeMode
    {
        public const byte Execute = 0b0;
        public const byte Write   = 0b1;
    }

    // 7 bits
    public static class BytecodeOp
    {
        // Execute Mode

        /// GetGlobal FROMINDEX X X
        /// Insert Global at FROMINDEX index onto stack
        public const byte GetGlobal          = 0b0000000;
        /// LoadConst FROMINDEX X X
        /// Insert Constant at FROMINDEX index onto stack
        public const byte LoadConst          = 0b0000001;
        /// Call ARGCOUNT RETURNCOUNT X
        /// Call function at with ARGCOUNT arguments from the stack 
        public const byte Call               = 0b0000010; 


        /// LoadEnv X X X
        /// Load the environment table onto the stack
        public const byte LoadEnv            = 0b0000011;
        

        /// Write Mode

        /// Write X X X
        /// Writes the string value atop the stack
        /// `STACKINDEX = {`
        public const byte OpenBlock          = 0b1000000;
        /// Write X X X
        /// Writes the end of the block
        /// `}`
        public const byte CloseBlock         = 0b1000001;
        /// Write X X X
        /// Writes the pair of the two elements atop the stack
        /// `STACKINDEX1 = `STACKINDEX2`
        public const byte Pair               = 0b1000010;
        /// Write X X X
        /// Writes the element atop the stack
        /// `STACKINDEX`
        public const byte Element            = 0b1000011;


    }


    /// <summary>
    /// A bytecode instruction.
    /// Contains four bytes of data
    /// or 32 bits.
    /// The first bit is the mode of the instruction.
    /// The next seven bits are the opcode of the instruction.
    /// There are 3 bytes of remaining argument data.
    /// </summary>
    public static class Bytecode {
        public static bool IsExecute(byte[] code)
        {
            return (code[0] & 0b10000000) == BytecodeMode.Execute;
        }

        public static byte GetOp(byte[] code)
        {
            return  (byte)(code[0] & 0b01111111);
        }

        public static byte GetArg(byte[] code, int index)
        {
            if (index < 0 || index > code.Length - 2)
                throw new ArgumentOutOfRangeException(nameof(index), $"index must be between 0 and {code.Length - 2}");
            return code[index + 1];
        }

        public static byte NumArgs(byte[] code)
        {
            return (byte)(code.Length - 1);
        }

        public static byte[] ToByteArray(byte mode, byte opcode, params byte[] args)
        {
            if (args.Length > 3)
                throw new ArgumentException("args must be at most 3 bytes long");

            byte[] code = new byte[args.Length + 1];
            code[0] = (byte)(mode << 7 | opcode);
            for (int i = 0; i < args.Length; i++)
            {
                code[i + 1] = args[i];
            }

            return code;
            
        }

        public static byte[] Execute(byte opcode, params byte[] args)
        {
            if (args.Length < 3)
                // Fill out with 0s
                Array.Resize(ref args, 3);
            return ToByteArray(BytecodeMode.Execute, opcode, args);
        }

        public static byte[] Write(byte opcode, params byte[] args)
        {
            if (args.Length < 3)
                // Fill out with 0s
                Array.Resize(ref args, 3);
            return ToByteArray(BytecodeMode.Write, opcode, args);
        }
        

    }

    /// <summary>
    /// Wrapper around an array of bytes[4]
    /// </summary>
    public class DoxCode(byte[][] code) : IEnumerable<byte[]>, IEnumerator<byte[]>
    {

        public static implicit operator DoxCode(byte[][] code) => new(code);
        
        private readonly byte[][] _code = code;
        private int _index = -1;

        public byte[] Current => _code[_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _code.Length;
        }

        public void Reset()
        {
            _index = -1;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var instr in _code)
            {
                sb.Append(InstructionToString(instr));
            }

            return sb.ToString();
        }

        public static string InstructionToString(byte[] instr)
        {
            var sb = new System.Text.StringBuilder();
            string mode = Bytecode.IsExecute(instr) ? "EXECUTE" : "WRITE";
            byte opcode = Bytecode.GetOp(instr);

            string opname = opcode switch
            {
                BytecodeOp.GetGlobal => nameof(BytecodeOp.GetGlobal),
                BytecodeOp.LoadConst => nameof(BytecodeOp.LoadConst),
                BytecodeOp.Call => nameof(BytecodeOp.Call),
                BytecodeOp.LoadEnv => nameof(BytecodeOp.LoadEnv),
                BytecodeOp.OpenBlock => nameof(BytecodeOp.OpenBlock),
                BytecodeOp.CloseBlock => nameof(BytecodeOp.CloseBlock),
                BytecodeOp.Pair => nameof(BytecodeOp.Pair),
                BytecodeOp.Element => nameof(BytecodeOp.Element),
                _ => "UNKNOWN"
            };

            sb.Append($"[{mode}][{opname}:{opcode:X2}]{{");

            int numArgs = Bytecode.NumArgs(instr);
            for (int i = 0; i < numArgs; i++)
            {
                sb.Append(instr[i + 1]);
                if (i < numArgs - 1)
                    sb.Append(';');
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

    }

    
}