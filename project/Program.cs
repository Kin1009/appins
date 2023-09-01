using LibGit2Sharp;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
///instructions
///appins upload (your github repo link) (execuable) -> id
///appins download program (id) -> program (admin)
///appins download local (id) -> program (no admin)
///appins download current (id) -> program (current_dir)
namespace appins
{
    [SupportedOSPlatform("windows")]
    [UnsupportedOSPlatform("osx")]
    [UnsupportedOSPlatform("linux")]
    [UnsupportedOSPlatform("freebsd")]
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 3)
                {
                    switch (args[0])
                    {
                        case "upload":
                            Console.WriteLine("ID: " + ToBase64(args[1] + "\n" + args[2]));
                            break;
                        case "download":
                            switch (args[1])
                            {
                                case "program":
                                    if (IsAdministrator())
                                    {
                                        Console.WriteLine("Installing");
                                        var parts = ToPlain(args[2]).Split("\n");
                                        try
                                        {
                                            Repository.Clone(parts[0], "C:\\Program Files\\" + parts[0].Split("/")[^1]);
                                            Console.WriteLine("Installed " + parts[0].Split("/")[^1] + ", program is at " + "C:\\Program Files\\" + parts[0].Split("/")[^1] + "\\" + parts[1]);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Error: " + e.Message);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("The program must be running as admin");
                                    }
                                    break;
                                case "local":
                                    Console.WriteLine("Installing");
                                    var parts_ = ToPlain(args[2]).Split("\n");
                                    try
                                    {
                                        Repository.Clone(parts_[0], "C:\\Users\\" + Environment.UserName + "\\AppData\\" + parts_[0].Split("/")[^1]);
                                        Console.WriteLine("Installed " + parts_[0].Split("/")[^1] + ", program is at " + "C:\\Users\\" + Environment.UserName + "\\AppData\\" + parts_[0].Split("/")[^1] + "\\" + parts_[1]);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Error: " + e.Message);
                                    }
                                    break;
                                case "current":
                                    if (!IsAdministrator())
                                    { Console.WriteLine("WARNING: The program is not running as admin. There may be errors"); }
                                    Console.WriteLine("Installing");
                                    var parts__ = ToPlain(args[2]).Split("\n");
                                    try
                                    {
                                        Repository.Clone(parts__[0], Directory.GetCurrentDirectory() + "\\" + parts__[0].Split("/")[^1]);
                                        Console.WriteLine("Installed " + parts__[0].Split("/")[^1] + ", program is at " + Directory.GetCurrentDirectory() + "\\" + parts__[0].Split("/")[^1] + "\\" + parts__[1]);
                                        Console.WriteLine("Or program is at: " + Directory.GetCurrentDirectory() + parts__[0].Split("/")[^1] + "\\" + parts__[1]);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Error: " + e.Message);
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Invalid option at arg 2: " + args[1]);
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid option at arg 1: " + args[0]);
                            break;
                    }
                }
                else if (args.Length == 0) { Console.WriteLine(@"Help:
appins: help
appins upload <your-public-repo-on-github> <executable-path>: Generate an ID to download, executable-path is a relative path (e.g. C:\a.exe -> a.exe)
appins download program <id>: Download a program (require admin bcz it stores in ""C:\Program Files"")
appins download local <id>: Download a program, store in AppData
appins download current <id>: Download a program, store in current dir (may require admin)
NOTE: You must know about GitHub (https://github.com) (for uploaders) to use this program"); }
                else { Console.WriteLine("Invalid number of args (Expected 3 or 0, got " + args.Length.ToString() + ")"); }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            static string ToBase64(string input)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(plainTextBytes);
            }
            static string ToPlain(string base64)
            {
                var base64EncodedBytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            static bool IsAdministrator()
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
