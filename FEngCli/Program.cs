using CommandLine;

namespace FEngCli;

internal static class Program
{
	private static int Main(string[] args)
	{
		//NFSCO Specific changes
		// FRONTEND\\FECUSTOMIZEPARTS
		// GLOBAL\\WORLDMAPMAIN
		// FRONTEND\\FECHALLENGESERIES
		// GLOBAL\\SCREENPRINTF

		//var path = "D:\\Development\\nfsco\\files\\data";
		//var folder = "FRONTEND";
		//var file = "FECUSTOMIZEPARTS";

		//var @string = $"compile -i {path}\\{folder}\\_FNG\\{file}.json -o {path}\\{folder}\\_FNG\\{file}.BIN";

		//args = @string.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

		return Parser.Default
			.ParseArguments(args, typeof(DecompileCommand), typeof(CompileCommand))
			.MapResult((BaseCommand bc) => bc.Execute(), errs => 1);
	}
}
