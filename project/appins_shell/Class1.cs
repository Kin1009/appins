using LibGit2Sharp;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
///instructions (v0)
///appins upload (your github repo link) (execuable) -> id
///appins download program (id) -> program (admin)
///appins download local (id) -> program (no admin)
///appins download current (id) -> program (current_dir)
///aHR0cHM6Ly9naXRodWIuY29tL
namespace appins_shell
{
	[SupportedOSPlatform("windows")]
	[UnsupportedOSPlatform("osx")]
	[UnsupportedOSPlatform("linux")]
	[UnsupportedOSPlatform("freebsd")]
	public class appins_shell
	{
		public static void Parse(string[] args)
		{
			try
			{
				if ((args.Length == 1) | (args.Length == 2) | (args.Length == 3))
				{
					switch (args[0])
					{
						case "upload":
							Console.WriteLine("ID: " + ToBase64(args[1] + "\n" + args[2]).Substring("aHR0cHM6Ly9naXRodWIuY29tL".Length));
                            break;
						case "program":
							if (IsAdministrator())
							{
								Console.WriteLine("Installing");
								var parts = ToPlain("aHR0cHM6Ly9naXRodWIuY29tL" + args[1]).Split("\n");
								try
								{
									Repository.Clone(parts[0], "C:\\Program Files\\" + parts[0].Split("/")[^1]);
									Console.WriteLine("Installed " + parts[0].Split("/")[^1] + ", program is at " + "C:\\Program Files\\" + parts[0].Split("/")[^1] + "\\" + parts[1]);
								}
								catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
							}
							else { Console.WriteLine("The program must be running as admin"); }
							break;
						case "local":
							Console.WriteLine("Installing");
							var parts_ = ToPlain("aHR0cHM6Ly9naXRodWIuY29tL" + args[1]).Split("\n");
							try
							{
								Repository.Clone(parts_[0], "C:\\Users\\" + Environment.UserName + "\\AppData\\" + parts_[0].Split("/")[^1]);
								Console.WriteLine("Installed " + parts_[0].Split("/")[^1] + ", program is at " + "C:\\Users\\" + Environment.UserName + "\\AppData\\" + parts_[0].Split("/")[^1] + "\\" + parts_[1]);
							}
							catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
							break;
						case "current":
							if (!IsAdministrator()) { Console.WriteLine("WARNING: The program is not running as admin. There may be errors"); }
							Console.WriteLine("Installing");
							var parts__ = ToPlain("aHR0cHM6Ly9naXRodWIuY29tL" + args[1]).Split("\n");
							try
							{
								Repository.Clone(parts__[0], Directory.GetCurrentDirectory() + "\\" + parts__[0].Split("/")[^1]);
								string path = Directory.GetCurrentDirectory() + "\\" + parts__[0].Split("/")[^1] + "\\" + parts__[1];
								if (!(File.Exists(path) | Directory.Exists(path))) { path = Directory.GetCurrentDirectory() + parts__[0].Split("/")[^1] + "\\" + parts__[1]; }
								Console.WriteLine("Installed " + parts__[0].Split("/")[^1] + ", program is at " + path);
							}
							catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
							break;
						case "ids":
							GetProjects(args[1]);
							break;
						case "user":
							GetProjectsFromUser(args[1]);
							break;
						case "users":
							GetUsers(args[1]);
							break;
						case "help":
                            Console.WriteLine(@"Help:
appins: help
appins upload <your-public-repo-on-github> <executable-path>: Generate an ID to download, <executable-path> is a relative path (e.g. C:\a.exe -> a.exe) or """" in command line (e.g. appins upload ... """")
appins program <id>: Download a program (require admin bcz it stores in ""C:\Program Files"")
appins local <id>: Download a program, store in AppData
appins current <id>: Download a program, store in current dir (may require admin)
appins ids <id>: Browse IDs that larger than <id> (not all)
(default) appins ids 0: Browse IDs that larger than 0 (not all)
appins users <id>: Browse user IDs that larger than <id> (not all)
(default) appins users 0: Browse user IDs that larger than 0 (not all)
appins user <user>: Browse IDs from <user>
NOTES:
You must know about GitHub (https://github.com) (for uploaders) to use this program
Use full URL (e.g. https://github.com/.../..., not github.com/.../... and not .../...");
                            break;
						default:
							Console.WriteLine("Invalid option at arg 1: " + args[0]);
							break;
					}
				}
				else if (args.Length == 0) { Console.WriteLine(@"Help:
appins: help
appins upload <your-public-repo-on-github> <executable-path>: Generate an ID to download, <executable-path> is a relative path (e.g. C:\a.exe -> a.exe) or """" in command line (e.g. appins upload ... """")
appins program <id>: Download a program (require admin bcz it stores in ""C:\Program Files"")
appins local <id>: Download a program, store in AppData
appins current <id>: Download a program, store in current dir (may require admin)
appins ids <id>: Browse IDs that larger than <id> (not all)
(default) appins ids 0: Browse IDs that larger than 0 (not all)
appins users <id>: Browse user IDs that larger than <id> (not all)
(default) appins users 0: Browse user IDs that larger than 0 (not all)
appins user <user>: Browse IDs from <user>
NOTES:
You must know about GitHub (https://github.com) (for uploaders) to use this program
Use full URL (e.g. https://github.com/.../..., not github.com/.../... and not .../..."); }
			}
			catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
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
			void GetProjects(string since)
			{
				HttpWebRequest request = WebRequest.Create("https://api.github.com/repositories?since=" + since) as HttpWebRequest;
				request.UserAgent = "appins";
				string content1;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					StreamReader reader = new(response.GetResponseStream());
					content1 = reader.ReadToEnd();
				}
				List<Dictionary<string, object>> content = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content1);
				var defaultColor = Console.ForegroundColor;
				Console.WriteLine("The repositories' data are gotten in https://api.github.com/repositories");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("ID");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("GitHub URL");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Description\n");
				foreach (Dictionary<string, object> i in content)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write(ToBase64((i["html_url"] as string) + "\n").Substring("aHR0cHM6Ly9naXRodWIuY29tL".Length));
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write(i["html_url"]);
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(i["description"] + "\n");
				}
				Console.ForegroundColor = defaultColor;
			}
			void GetUsers(string since)
			{
				HttpWebRequest request = WebRequest.Create("https://api.github.com/users?since=" + since) as HttpWebRequest;
				request.UserAgent = "appins";
				string content1;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					StreamReader reader = new(response.GetResponseStream());
					content1 = reader.ReadToEnd();
				}
				List<Dictionary<string, object>> content = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content1);
				var defaultColor = Console.ForegroundColor;
				Console.WriteLine("The users' data are gotten in https://api.github.com/users");
				Console.WriteLine("Username");
				foreach (Dictionary<string, object> i in content) { Console.WriteLine(i["login"]); }
				Console.ForegroundColor = defaultColor;
			}
			void GetProjectsFromUser(string user)
			{
				HttpWebRequest request = WebRequest.Create("https://api.github.com/users/" + user + "/repos") as HttpWebRequest;
				request.UserAgent = "appins";
				string content1;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					StreamReader reader = new(response.GetResponseStream());
					content1 = reader.ReadToEnd();
				}
				List<Dictionary<string, object>> content = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content1);
				var defaultColor = Console.ForegroundColor;
				Console.WriteLine("The user's repositories' data are gotten in https://api.github.com/users/" + user + "/repos");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("ID");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("GitHub URL");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Description\n");
				foreach (Dictionary<string, object> i in content)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write(ToBase64((i["html_url"] as string) + "\n").Substring("aHR0cHM6Ly9naXRodWIuY29tL".Length));
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write(i["html_url"]);
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(i["description"] + "\n");
				}
				Console.ForegroundColor = defaultColor;
			}
		}
		public static int Parse_(string[] args)
		{
			try
			{
				if ((args.Length > 0) & (args.Length < 4))
				{
					switch (args[0])
					{
						case "upload":
							Console.WriteLine("ID: " + ToBase64(args[1] + "\n" + args[2]).Substring("aHR0cHM6Ly9naXRodWIuY29tL".Length));
							break;
						case "program":
							if (IsAdministrator())
							{
								Console.WriteLine("Installing");
								var parts = ToPlain("aHR0cHM6Ly9naXRodWIuY29tL" + args[1]).Split("\n");
								try
								{
									Repository.Clone(parts[0], "C:\\Program Files\\" + parts[0].Split("/")[^1]);
									Console.WriteLine("Installed " + parts[0].Split("/")[^1] + ", program is at " + "C:\\Program Files\\" + parts[0].Split("/")[^1] + "\\" + parts[1]);
								}
								catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
							}
							else { Console.WriteLine("The program must be running as admin"); }
							break;
						case "local":
							Console.WriteLine("Installing");
							var parts_ = ToPlain("aHR0cHM6Ly9naXRodWIuY29tL" + args[1]).Split("\n");
							try
							{
								Repository.Clone(parts_[0], "C:\\Users\\" + Environment.UserName + "\\AppData\\" + parts_[0].Split("/")[^1]);
								Console.WriteLine("Installed " + parts_[0].Split("/")[^1] + ", program is at " + "C:\\Users\\" + Environment.UserName + "\\AppData\\" + parts_[0].Split("/")[^1] + "\\" + parts_[1]);
							}
							catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
							break;
						case "ids":
							GetProjects(args[1]);
							break;
						case "user":
							GetProjectsFromUser(args[1]);
							break;
						case "users":
							GetUsers(args[1]);
							break;
						case "exit":
							return 1;
						case "help":
							Console.WriteLine(@"IDE help:
help: help
upload <your-public-repo-on-github> <executable-path>: Generate an ID to download, <executable-path> is a relative path (e.g. C:\a.exe -> a.exe) or """" in command line (e.g. appins upload ... """")
program <id>: Download a program (require admin bcz it stores in ""C:\Program Files"")
local <id>: Download a program, store in AppData
ids <id>: Browse IDs that larger than <id> (not all)
(default)  ids 0: Browse IDs that larger than 0 (not all)
users <id>: Browse user IDs that larger than <id> (not all)
(default) users 0: Browse user IDs that larger than 0 (not all)
user <user>: Browse IDs from <user>
NOTES:
You must know about GitHub (https://github.com) (for uploaders) to use this program
Use full URL (e.g. https://github.com/.../..., not github.com/.../... and not .../...");
							break;
						default:
							Console.WriteLine("Invalid option at arg 1: " + args[0]);
							break;
					}
				}
				else if (args.Length > 3) { Console.WriteLine("Invalid number of args (Expected from 1 to 3, got " + args.Length.ToString() + ")"); }
			}
			//catch { throw; }
			catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
			return 0;
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
			void GetProjects(string since)
			{
				HttpWebRequest request = WebRequest.Create("https://api.github.com/repositories?since=" + since) as HttpWebRequest;
				request.UserAgent = "appins";
				string content1;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					StreamReader reader = new(response.GetResponseStream());
					content1 = reader.ReadToEnd();
				}
				List<Dictionary<string, object>> content = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content1);
				var defaultColor = Console.ForegroundColor;
				Console.WriteLine("The repositories' data are gotten in https://api.github.com/repositories");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("ID");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("GitHub URL");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Description\n");
				foreach (Dictionary<string, object> i in content)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write(ToBase64((i["html_url"] as string) + "\n").Substring("aHR0cHM6Ly9naXRodWIuY29tL".Length));
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write(i["html_url"]);
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(i["description"] + "\n");
				}
				Console.ForegroundColor = defaultColor;
			}
			void GetUsers(string since)
			{
				HttpWebRequest request = WebRequest.Create("https://api.github.com/users?since=" + since) as HttpWebRequest;
				request.UserAgent = "appins";
				string content1;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					StreamReader reader = new(response.GetResponseStream());
					content1 = reader.ReadToEnd();
				}
				List<Dictionary<string, object>> content = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content1);
				var defaultColor = Console.ForegroundColor;
				Console.WriteLine("The users' data are gotten in https://api.github.com/users");
				Console.WriteLine("Username");
				foreach (Dictionary<string, object> i in content) { Console.WriteLine(i["login"]); }
				Console.ForegroundColor = defaultColor;
			}
			void GetProjectsFromUser(string user)
			{
				HttpWebRequest request = WebRequest.Create("https://api.github.com/users/" + user + "/repos") as HttpWebRequest;
				request.UserAgent = "appins";
				string content1;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					StreamReader reader = new(response.GetResponseStream());
					content1 = reader.ReadToEnd();
				}
				List<Dictionary<string, object>> content = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content1);
				var defaultColor = Console.ForegroundColor;
				Console.WriteLine("The user's repositories' data are gotten in https://api.github.com/users/" + user + "/repos");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("ID");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("GitHub URL");
				Console.ForegroundColor = defaultColor;
				Console.Write(" | ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Description\n");
				foreach (Dictionary<string, object> i in content)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write(ToBase64((i["html_url"] as string) + "\n").Substring("aHR0cHM6Ly9naXRodWIuY29tL".Length));
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write(i["html_url"]);
					Console.ForegroundColor = defaultColor;
					Console.Write(" | ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(i["description"] + "\n");
				}
				Console.ForegroundColor = defaultColor;
			}
		}
	}
}