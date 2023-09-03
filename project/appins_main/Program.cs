using System.Runtime.Versioning;
using static appins_shell.appins_shell;
namespace appins_main
{
	internal class Program
	{
		[SupportedOSPlatform("windows")]
		[UnsupportedOSPlatform("osx")]
		[UnsupportedOSPlatform("linux")]
		[UnsupportedOSPlatform("freebsd")]
		static void Main(string[] args)
		{
			if (args.Length > 0) { Parse(args); }
			else
			{
				Console.WriteLine("appins (Wrapper + IDE) version 1.0\nType \"help\" to help");
				while (true)
				{
					Console.Write("appins> ");
					string? command = Console.ReadLine();
					if (command != null)
					{
						string[] commands = command.Split(' ');
						if (Parse_(commands) == 1)
						{
							break;
						}
					}
				}
			}
		}
	}
}