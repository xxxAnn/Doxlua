using Doxlua.Tokenizer;
using Doxlua.Lexer;
using Doxlua.VM;
using NLog;

namespace Doxlua.Tests
{
    [Collection("BasicSeries")]
    public class LexingTests
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        [Fact]
        public void BasicFile()
        {
            // The file is Doxlua.Tests/Testfiles/Basic.lua
            // We want to get its tokenization
            // First, let's read the file
            string filePath = "../../../Testfiles/Verybasic.lua";
            string fileContent = File.ReadAllText(filePath);
            // Now we can tokenize it
            IEnumerable<IToken> tokens = LuaTokenizer.Tokenize(fileContent) ?? [];
            List<string> infos = tokens.Select(static t => $"({t})").ToList();

            string outputPath = "../../../Testfiles/Verybasic.lua.tokens";
            File.WriteAllLines(outputPath, infos);

            IStatement[] statements = StatementParser.ParseStatements(tokens);

            foreach (IStatement statement in statements)
            {
                Logger.Info(statement);
            }
            Rootlex expr = new Rootlex(tokens);

            Logger.Info(expr.GetCode());
            Logger.Info(string.Join(", ", expr.GetConsts().Select(static c => c.ToString())));

            DoxMachine.Run(expr);
        }
    }
}