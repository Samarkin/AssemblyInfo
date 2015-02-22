using System;
using System.IO;
using Console = System.Console;
using AssemblyInfo.Common;
using System.Linq;

namespace AssemblyInfo.Console
{
	class MainClass
	{
		public static int Main(string[] args)
		{
			var prober = new AssemblyProber(args.FirstOrDefault());
			System.Console.WriteLine("Assembly Info v{0}", typeof(AssemblyProber).Assembly.GetName().Version);
			System.Console.WriteLine();
			if (prober.ErrorLevel == ErrorLevel.ArgumentError)
			{
				System.Console.Error.WriteLine("Usage: {0} <assembly>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
				return (int)prober.ErrorLevel;
			}
			System.Console.WriteLine(prober.FileName);
			System.Console.WriteLine();
			if (prober.ErrorLevel == ErrorLevel.Success)
			{
				System.Console.WriteLine(prober.DisplayName);
				System.Console.WriteLine(prober.CLRVersion);
				System.Console.WriteLine(prober.Architecture);
				System.Console.WriteLine(prober.Culture);
				System.Console.WriteLine("Assembly version: {0}", prober.AssemblyVersion);
				System.Console.WriteLine("File version:     {0}", prober.FileVersion);
				System.Console.WriteLine("Product version:  {0}", prober.ProductVersion);
				System.Console.WriteLine();
				System.Console.WriteLine("Dependencies:");
				foreach (var dependency in prober.Dependencies)
				{
					System.Console.WriteLine("  {0,-2}{1}", dependency.Satisfied ? string.Empty : "?", dependency.DisplayName);
				}
			}
			else if (prober.ErrorLevel == ErrorLevel.FileNotFound)
			{
				System.Console.WriteLine("File not found");
			}
			else
			{
				System.Console.WriteLine("File version:     {0}", prober.FileVersion);
				System.Console.WriteLine("Product version:  {0}", prober.ProductVersion);
			}
			return (int)prober.ErrorLevel;
		}
	}
}
