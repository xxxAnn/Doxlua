using System.Collections;
using System.Collections.Specialized;

namespace Doxlua.Doxcode
{

    public static class BytecodeMode
    {
        public const byte Execute = 0b0;
        public const byte Write   = 0b1;
    }

    public static class BytecodeOp
    {
        public const byte GetGlobal = 0b0000000;
        public const byte LoadConst = 0b0000001;
        public const byte Call      = 0b0010010; 
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

        public static byte[] ToByteArray(byte mode, byte opcode, byte[] args)
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
        

    }

    /// <summary>
    /// Wrapper around an array of bytes[4]
    /// </summary>
    public class DoxCode(byte[][] code, VM.DoxValue[] consts) : IEnumerable<byte[]>, IEnumerator<byte[]>
    {
        
        private readonly byte[][] _code = code;
        private readonly VM.DoxValue[] _consts = consts;
        private int _index = -1;

        public byte[] Current => _code[_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public VM.DoxValue GetConstant(int index)
        {
            if (index < 0 || index >= _consts.Length)
                throw new ArgumentOutOfRangeException(nameof(index), $"index must be between 0 and {_consts.Length - 1}");
            return _consts[index];
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

    }

    
}