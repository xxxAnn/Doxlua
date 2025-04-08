namespace Doxcode
{

    public enum BytecodeMode : byte
    {
        Execute = 0b0000,
        Write = 0b0001,
    }

    public enum BytecodeOp : byte
    {
        GetGlobal = 0b00000000,
        LoadConst = 0b00000001,
        FuncCall = 0b00000010,

    }


    /// <summary>
    /// A bytecode instruction.
    /// Contains eight bytes of data
    /// or 64 bits.
    /// The first byte is the mode of the instruction.
    /// The second byte is the operation of the instruction.
    /// The remaining six bytes are the arguments of the instruction.
    /// </summary>
    public readonly struct Bytecode
    {
        // 1 byte
        public BytecodeMode Mode { get; }
        // 1 byte
        public BytecodeOp Op { get; }

        // maximum of 6 bytes
        public byte[] Args { get; }

        public Bytecode(BytecodeMode mode, BytecodeOp op, byte[] args)
        {
            // Verification
            if (args.Length > 6)
                throw new ArgumentException("Args length cannot be greater than 6 bytes.");
            
            Mode = mode;
            Op = op;
            Args = args;
        }

        public Bytecode(byte mode, byte op, byte[] args)
        {
            Mode = (BytecodeMode)mode;
            Op = (BytecodeOp)op;
            // Verification
            if (args.Length > 6)
                throw new ArgumentException("Args length cannot be greater than 6 bytes.");
            Args = args;
        }


    }
}