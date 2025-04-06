namespace Doxlua.Tests;


using Xunit;
using Doxlua.Tokenizer;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // The file is Doxlua.Tests/Testfiles/Basic.lua
        // We want to get its tokenization
        // First, let's read the file
        string filePath = "../../../Testfiles/Basic.lua";
        string fileContent = System.IO.File.ReadAllText(filePath);
        // Now we can tokenize it
        var tokens = LuaTokenizer.Tokenize(fileContent);

        // Now we can print the tokens
        var infos = tokens.Select(t => $"({t.ToString()})").ToList();

        // Print infos separated by ','
        string result = string.Join(", ", infos);
        // Print the result
        System.Console.WriteLine(result);
    }
}
