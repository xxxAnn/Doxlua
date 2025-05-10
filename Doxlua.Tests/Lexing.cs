namespace Doxlua.Tests;


using Xunit;
using Doxlua.Tokenizer;
using Doxlua.Lexer;
using Pidgin;

public class LexingTests
{
    [Fact]
    public void BasicFile()
    {
        // The file is Doxlua.Tests/Testfiles/Basic.lua
        // We want to get its tokenization
        // First, let's read the file
        string filePath = "../../../Testfiles/Verybasic.lua";
        string fileContent = System.IO.File.ReadAllText(filePath);
        // Now we can tokenize it
        var tokens = LuaTokenizer.Tokenize(fileContent) ?? [];
        var infos = tokens.Select(t => $"({t})").ToList();

        string outputPath = "../../../Testfiles/Verybasic.lua.tokens";
        System.IO.File.WriteAllLines(outputPath, infos);


        var expr = StatementParser.Statement.ParseOrThrow(tokens);

        Console.WriteLine(expr);
    }
}