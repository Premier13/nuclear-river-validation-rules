using System.CommandLine;

namespace NuClear.ValidationRules.Import
{
    public static partial class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                CreateReplicateCommand(),
                CreateInitializeCommand(),
            };

            return rootCommand.Invoke(args);
        }
    }
}
