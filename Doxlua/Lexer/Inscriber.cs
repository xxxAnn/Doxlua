using Doxlua.Doxcode;

namespace Doxlua.Lexer
{
    public static class Inscriber
    {
        public static DoxCode Inscribe(Statement[] statements)
        {
            return statements.Select(Inscribe).ToArray();
        }

        public static byte[] Inscribe(Statement statement)
        {
            // TODO
            return null;
        }
    }
}