

using Xunit;
using Doxlua.Doxcode;
using Doxlua.VM;
using static Doxlua.Doxcode.BytecodeOp;
using static Doxlua.Doxcode.Bytecode;

namespace Doxlua.Tests
{
    [Collection("BasicSeries")]
    public class BytecodeTests
    {
        [Fact]
        public void BytecodeType1()
        {
            byte[] byteArray = ToByteArray(BytecodeMode.Execute, Call, [0, 1, 5]);


            // Check the first byte 
            Assert.Equal(Call, GetOp(byteArray));
            // IsExecute should be true
            Assert.True(IsExecute(byteArray));

            Assert.Equal(0, GetArg(byteArray, 0));
            Assert.Equal(1, GetArg(byteArray, 1));
            Assert.Equal(5, GetArg(byteArray, 2));

            // This should throw an ArgumentOutOfRangeException
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => GetArg(byteArray, 3));


        }

        [Fact]
        public void BytecodeType2()
        {
            byte[] byteArray = ToByteArray(BytecodeMode.Write, LoadConst, [31, 27]);


            // Check the first byte 
            Assert.Equal(LoadConst, GetOp(byteArray));
            // IsExecute should be false
            Assert.False(IsExecute(byteArray));

            Assert.Equal(31, GetArg(byteArray, 0));
            Assert.Equal(27, GetArg(byteArray, 1));

            Assert.Equal(2, NumArgs(byteArray));

            // This should throw an ArgumentOutOfRangeException
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => GetArg(byteArray, 2));

        }

        [Fact]
        public void BasicWrite()
        {

            byte[][] instructions = [
                // "if"
                Execute(LoadConst),
                Write(OpenBlock),
                // limit
                Execute(LoadConst, 1),
                Write(OpenBlock),
                // always, yes
                Execute(LoadConst, 3),
                Execute(LoadConst, 2),
                Write(Pair),
                Write(CloseBlock),
                // do_something, yes
                Execute(LoadConst, 3),
                Execute(LoadConst, 4),
                Write(Pair),
                Write(CloseBlock),
            ];

            string[] consts = [
                "if",
                "limit",
                "always",
                "yes",
                "do_something"
            ];

            DoxState state = new(consts);
            state.SetFile("../../../Testfiles/BytecodeTest-Output.txt");
            // VM
            DoxMachine.Run(
                state,
                instructions
            );

        }
    }
}
