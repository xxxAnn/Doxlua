namespace Doxlua.Tests;


using Xunit;
using Doxlua.Doxcode;

public class BytecodeTests
{
    [Fact]
    public void BytecodeType1()
    {
        byte[] byteArray = Bytecode.ToByteArray(BytecodeMode.Execute, BytecodeOp.Call, [0, 1, 5]);

        
        // Check the first byte 
        Assert.Equal(BytecodeOp.Call, Bytecode.GetOp(byteArray));
        // IsExecute should be true
        Assert.True(Bytecode.IsExecute(byteArray));

        Assert.Equal(0, Bytecode.GetArg(byteArray, 0));
        Assert.Equal(1, Bytecode.GetArg(byteArray, 1));
        Assert.Equal(5, Bytecode.GetArg(byteArray, 2));

        // This should throw an ArgumentOutOfRangeException
        Assert.Throws<ArgumentOutOfRangeException>(() => Bytecode.GetArg(byteArray, 3));


    }

        [Fact]
    public void BytecodeType2()
    {
        byte[] byteArray = Bytecode.ToByteArray(BytecodeMode.Write, BytecodeOp.LoadConst, [31, 27]);

        
        // Check the first byte 
        Assert.Equal(BytecodeOp.LoadConst, Bytecode.GetOp(byteArray));
        // IsExecute should be false
        Assert.False(Bytecode.IsExecute(byteArray));

        Assert.Equal(31, Bytecode.GetArg(byteArray, 0));
        Assert.Equal(27, Bytecode.GetArg(byteArray, 1));

        Assert.Equal(2, Bytecode.NumArgs(byteArray));

        // This should throw an ArgumentOutOfRangeException
        Assert.Throws<ArgumentOutOfRangeException>(() => Bytecode.GetArg(byteArray, 2));

    }

    [Fact]
    public void BasicWrite()
    {}
}
