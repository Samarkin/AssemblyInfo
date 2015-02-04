using System;
using System.Reflection;
using System.Windows;

namespace AssemblyInfo
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				String resourceName = Assembly.GetExecutingAssembly().GetName().Name + "." + new AssemblyName(args.Name).Name + ".dll";
				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					if (stream == null) return null;
					Byte[] assemblyData = new Byte[stream.Length];
					stream.Read(assemblyData, 0, assemblyData.Length);
					return Assembly.Load(assemblyData);
				}
			};
		}
	}
}
