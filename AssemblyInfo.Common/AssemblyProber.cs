using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AssemblyInfo.Common
{
	public enum ErrorLevel
	{
		Success = 0,
		ReflectionError = 1,
		FileNotFound = 2,
		ArgumentError = 3
	}

	internal static class AppDomainExtensions
	{
		public static T CreateInstanceUsingPrivateConstructor<T>(this AppDomain domain, params object[] args)
		{
			return (T)domain.CreateInstanceFromAndUnwrap(
					typeof(T).Assembly.Location,
					typeof(T).FullName,
					false,
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
					null,
					args,
					null,
					null);
		}
	}

	[Serializable]
	public class AssemblyProber
	{
		private string _displayName;
		private string _clrVersion;
		private string _architecture;
		private string _culture;
		private string _assemblyVersion;
		private string _fileVersion;
		private string _productVersion;
		private string _fileName;
		private string _location;

		private AssemblyDependency[] _dependencies = new AssemblyDependency[0];

		private bool _gac;
		private bool? _debug;

		private readonly ErrorLevel _errorLevel;

		private static readonly Dictionary<Tuple<string, bool>, AssemblyProber> AssemblyCache = new Dictionary<Tuple<string, bool>, AssemblyProber>(); 

		public static AssemblyProber Create(string name, bool isAssemblyName = false)
		{
			var arg = new Tuple<string, bool>(name, isAssemblyName);
			AssemblyProber result;
			if (AssemblyCache.TryGetValue(arg, out result))
			{
				return result;
			}

			AppDomainSetup setup = new AppDomainSetup
			{
				ApplicationBase = isAssemblyName || string.IsNullOrWhiteSpace(name)
					? Directory.GetCurrentDirectory()
					: Path.GetDirectoryName(Path.GetFullPath(name))
			};
			AppDomain tmpDomain = AppDomain.CreateDomain("Temporary AssemblyProber domain", null, setup);
			try
			{
				result = tmpDomain.CreateInstanceUsingPrivateConstructor<AssemblyProber>(name, isAssemblyName);
				AssemblyCache[arg] = result;
				return result;
			}
			finally
			{
				AppDomain.Unload(tmpDomain);
			}
		}

		private AssemblyProber(string name, bool isAssemblyName)
		{
			try
			{
				if (isAssemblyName)
				{
					Assembly assembly;
					try
					{
						assembly = Assembly.ReflectionOnlyLoad(name);
					}
					catch (FileLoadException)
					{
						// Retry ignoring version
						int commaPos = name.IndexOf(',');
						if (commaPos < 0)
						{
							throw;
						}
						var dllPath = Path.Combine(Directory.GetCurrentDirectory(), name.Substring(0, commaPos) + ".dll");
						assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
					}
					LoadAssemblyProperties(assembly);
					LoadFileProperties(assembly.Location);
				}
				else
				{
					LoadFileProperties(name);
					var assembly = Assembly.ReflectionOnlyLoadFrom(name);
					LoadAssemblyProperties(assembly);
				}

				_errorLevel = ErrorLevel.Success;
			}
			catch (ArgumentException)
			{
				_errorLevel = ErrorLevel.ArgumentError;
			}
			catch (FileNotFoundException)
			{
				_errorLevel = ErrorLevel.FileNotFound;
			}
			catch (PathTooLongException)
			{
				_errorLevel = ErrorLevel.FileNotFound;
			}
			catch (FileLoadException)
			{
				_errorLevel = ErrorLevel.ReflectionError;
			}
			catch (BadImageFormatException)
			{
				_errorLevel = ErrorLevel.ReflectionError;
			}
		}

		private static Attribute CreateAttribute(CustomAttributeData data)
		{
			var arguments = data.ConstructorArguments.Select(arg => arg.Value).ToArray();
			var attribute = data.Constructor.Invoke(arguments) as Attribute;
			if (data.NamedArguments == null) return attribute;

			foreach (var namedArgument in data.NamedArguments)
			{
				var propertyInfo = namedArgument.MemberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					propertyInfo.SetValue(attribute, namedArgument.TypedValue.Value, null);
				}
				else
				{
					var fieldInfo = namedArgument.MemberInfo as FieldInfo;
					if (fieldInfo != null)
					{
						fieldInfo.SetValue(attribute, namedArgument.TypedValue.Value);
					}
				}
			}
			return attribute;
		}

		private void LoadAssemblyProperties(Assembly assembly)
		{
			var assemblyName = assembly.GetName();

			_displayName = assemblyName.FullName;
			_architecture = assemblyName.ProcessorArchitecture.ToString();
			_culture = assemblyName.CultureName;
			_assemblyVersion = assemblyName.Version.ToString();

			_clrVersion = assembly.ImageRuntimeVersion;
			_gac = assembly.GlobalAssemblyCache;

			_dependencies = assembly.GetReferencedAssemblies().Select(an => Probe(an.FullName)).OrderBy(a => a.DisplayName).ToArray();

			_debug = assembly.GetCustomAttributesData()
				.Where(attr => attr.AttributeType == typeof(DebuggableAttribute))
				.Select(attr => (DebuggableAttribute)CreateAttribute(attr))
				.Any(d => d.IsJITTrackingEnabled || d.IsJITOptimizerDisabled);
		}

		private void LoadFileProperties(string fileName)
		{
			_location = fileName;
			_fileName = Path.GetFileName(fileName);

			var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
			_fileVersion = fileVersionInfo.FileVersion;
			_productVersion = fileVersionInfo.ProductVersion;
		}

		public ErrorLevel ErrorLevel
		{
			get { return _errorLevel; }
		}

		public string FileName
		{
			get { return _fileName; }
		}

		public string Location
		{
			get { return _location; }
		}

		public string CLRVersion
		{
			get { return _clrVersion; }
		}

		public string Architecture
		{
			get { return _architecture; }
		}

		public string Culture
		{
			get { return _culture; }
		}

		public string AssemblyVersion
		{
			get { return _assemblyVersion; }
		}

		public string FileVersion
		{
			get { return _fileVersion; }
		}

		public string ProductVersion
		{
			get { return _productVersion; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public bool GlobalAssemblyCache
		{
			get { return _gac; }
		}

		public bool? Debug
		{
			get { return _debug; }
		}

		public IEnumerable<AssemblyDependency> Dependencies
		{
			get { return _dependencies; }
		}

		private AssemblyDependency Probe(string assemblyName)
		{
			try
			{
				try
				{
					Assembly assembly = Assembly.ReflectionOnlyLoad(assemblyName);
					return new AssemblyDependency(assemblyName, true, assembly.FullName);
				}
				catch (FileLoadException)
				{
					// Retry ignoring version
					// Helps detect potential app.config redirections
					var locDir = Path.GetDirectoryName(Location);
					int commaPos = assemblyName.IndexOf(',');
					if (commaPos < 0)
					{
						throw;
					}
					var dllPath = Path.Combine(locDir ?? Directory.GetCurrentDirectory(), assemblyName.Substring(0, commaPos) + ".dll");
					Assembly assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
					return new AssemblyDependency(assemblyName, false, assembly.FullName);
				}
			}
			catch (FileNotFoundException)
			{
				return new AssemblyDependency(assemblyName, false, null);
			}
			catch (FileLoadException)
			{
				return new AssemblyDependency(assemblyName, false, null);
			}
			catch (BadImageFormatException)
			{
				return new AssemblyDependency(assemblyName, false, null);
			}
		}
	}
}
